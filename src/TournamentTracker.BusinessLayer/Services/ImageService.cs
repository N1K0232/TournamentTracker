using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MimeMapping;
using OperationResults;
using TinyHelpers.Extensions;
using TournamentTracker.BusinessLayer.Internal;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.DataAccessLayer;
using TournamentTracker.Shared.Models;
using TournamentTracker.StorageProviders;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Services;

public class ImageService(IDataContext dataContext, IStorageProvider storageProvider, IMapper mapper) : IImageService
{
    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var image = await dataContext.GetAsync<Entities.Image>(id);
            if (image is not null)
            {
                dataContext.Delete(image);
                await dataContext.SaveAsync();
                await storageProvider.DeleteAsync(image.Path);

                return Result.Ok();
            }

            return Result.Fail(FailureReasons.ItemNotFound, $"No image found with id {id}");
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
    }

    public async Task<Result<Image>> GetAsync(Guid id)
    {
        var dbImage = await dataContext.GetAsync<Entities.Image>(id);
        if (dbImage is not null)
        {
            var image = mapper.Map<Image>(dbImage);
            return image;
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No image found with id {id}");
    }

    public async Task<Result<IEnumerable<Image>>> GetListAsync()
    {
        var images = await dataContext.GetData<Entities.Image>()
            .ProjectTo<Image>(mapper.ConfigurationProvider)
            .ToListAsync();

        return images;
    }

    public async Task<Result<StreamFileContent>> ReadContentAsync(Guid id)
    {
        var image = await dataContext.GetAsync<Entities.Image>(id);
        if (image is not null)
        {
            var stream = await storageProvider.ReadAsStreamAsync(image.Path);
            var contentType = MimeUtility.GetMimeMapping(image.Path);

            if (stream is not null && contentType.HasValue())
            {
                var content = new StreamFileContent(stream, contentType);
                return content;
            }

            return Result.Fail(FailureReasons.ItemNotFound, "No stream found");
        }

        return Result.Fail(FailureReasons.ItemNotFound, $"No image found with id {id}");
    }

    public async Task<Result<Image>> UploadAsync(Stream stream, string fileName)
    {
        try
        {
            var path = PathGenerator.CreatePath(fileName);
            await storageProvider.SaveAsync(stream, path, true);

            var image = new Entities.Image
            {
                Path = path,
                Length = stream.Length,
                ContentType = MimeUtility.GetMimeMapping(fileName)
            };

            dataContext.Insert(image);
            await dataContext.SaveAsync();

            var createdImage = mapper.Map<Image>(image);
            return createdImage;
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(FailureReasons.DatabaseError, ex);
        }
        catch (IOException ex)
        {
            return Result.Fail(FailureReasons.ClientError, ex);
        }
    }
}
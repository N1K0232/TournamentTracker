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
        var image = await dataContext.GetAsync<Entities.Image>(id);
        if (image is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, $"No image found with id {id}");
        }

        dataContext.Delete(image);
        await dataContext.SaveAsync();

        await storageProvider.DeleteAsync(image.Path);
        return Result.Ok();
    }

    public async Task<Result<Image>> GetAsync(Guid id)
    {
        var dbImage = await dataContext.GetAsync<Entities.Image>(id);
        if (dbImage is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, $"No image found with id {id}");
        }

        var image = mapper.Map<Image>(dbImage);
        return image;
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
        if (image is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, $"No image found");
        }

        var stream = await storageProvider.ReadAsStreamAsync(image.Path);
        if (stream is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, $"No image found");
        }

        return new StreamFileContent(stream, image.ContentType);
    }

    public async Task<Result<Image>> UploadAsync(Stream stream, string fileName)
    {
        var path = PathGenerator.CreatePath(fileName);
        var query = dataContext.GetData<Entities.Image>();

        var databaseExists = await query.AnyAsync(i => i.Path == path);
        var storageExists = await storageProvider.ExistsAsync(path);

        if (databaseExists && storageExists)
        {
            return Result.Fail(FailureReasons.Conflict, "This image already exists");
        }

        await storageProvider.SaveAsync(stream, path);
        var dbImage = new Entities.Image
        {
            Path = path,
            Length = stream.Length,
            ContentType = MimeUtility.GetMimeMapping(fileName)
        };

        dataContext.Insert(dbImage);
        await dataContext.SaveAsync();

        return mapper.Map<Image>(dbImage);
    }
}
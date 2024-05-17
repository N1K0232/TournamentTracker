using OperationResults;
using TournamentTracker.Shared.Models;

namespace TournamentTracker.BusinessLayer.Services.Interfaces;

public interface IImageService
{
    Task<Result> DeleteAsync(Guid id);

    Task<Result<Image>> GetAsync(Guid id);

    Task<Result<IEnumerable<Image>>> GetListAsync();

    Task<Result<StreamFileContent>> ReadContentAsync(Guid id);

    Task<Result<Image>> UploadAsync(Stream stream, string fileName);
}
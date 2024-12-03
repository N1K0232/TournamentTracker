using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Shared.Models;

namespace TournamentTracker.Endpoints;

public class ImagesEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var imagesApiGroup = endpoints.MapGroup("/api/images");

        imagesApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        imagesApiGroup.MapGet("{id:guid}", GetAsync)
            .WithName("GetImage")
            .Produces<Image>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        imagesApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<IEnumerable<Image>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        imagesApiGroup.MapGet("{id:guid}/stream", ReadContentAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        imagesApiGroup.MapPost(string.Empty, UploadAsync)
            .DisableAntiforgery()
            .Produces<Image>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(IImageService imageService, Guid id, HttpContext httpContext)
    {
        var result = await imageService.DeleteAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(IImageService imageService, Guid id, HttpContext httpContext)
    {
        var result = await imageService.GetAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(IImageService imageService, HttpContext httpContext)
    {
        var result = await imageService.GetListAsync();
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> ReadContentAsync(IImageService imageService, Guid id, HttpContext httpContext)
    {
        var result = await imageService.ReadContentAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> UploadAsync(IImageService imageService, IFormFile file, HttpContext httpContext)
    {
        using var stream = file.OpenReadStream();
        var result = await imageService.UploadAsync(stream, file.FileName);

        return httpContext.CreateResponse(result, "GetImage", new { id = result.Content?.Id });
    }
}
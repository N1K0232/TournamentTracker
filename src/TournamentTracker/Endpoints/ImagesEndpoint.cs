using System.Net.Mime;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Models;
using TournamentTracker.Shared.Models;

namespace TournamentTracker.Endpoints;

public class ImagesEndpoint : IEndpointRouteHandlerBuilder
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

        imagesApiGroup.MapGet("{id:guid}/content", ReadContentAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        imagesApiGroup.MapPost(string.Empty, UploadAsync)
            .Accepts<FormFileContent>(MediaTypeNames.Multipart.FormData)
            .Produces<Image>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(IImageService imageService, Guid id, HttpContext context)
    {
        var result = await imageService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(IImageService imageService, Guid id, HttpContext context)
    {
        var result = await imageService.GetAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(IImageService imageService, HttpContext context)
    {
        var result = await imageService.GetListAsync();
        return context.CreateResponse(result);
    }

    private static async Task<IResult> ReadContentAsync(IImageService imageService, Guid id, HttpContext context)
    {
        var result = await imageService.ReadContentAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> UploadAsync(IImageService imageService, FormFileContent content, HttpContext context)
    {
        var result = await imageService.UploadAsync(content.File.OpenReadStream(), content.File.FileName);
        return context.CreateResponse(result, "GetImage", new { id = result.Content?.Id });
    }
}
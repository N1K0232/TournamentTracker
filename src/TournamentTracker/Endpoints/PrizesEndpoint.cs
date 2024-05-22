using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Extensions;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class PrizesEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var prizesApiGroup = endpoints.MapGroup("/api/prizes");

        prizesApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        prizesApiGroup.MapGet("{id:guid}", GetAsync)
            .WithName("GetPrize")
            .Produces<Prize>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        prizesApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<IEnumerable<Prize>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        prizesApiGroup.MapPost(string.Empty, CreateAsync)
            .WithValidation<SavePrizeRequest>()
            .Produces<Prize>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        prizesApiGroup.MapPut("{id:guid}", UpdateAsync)
            .WithValidation<SavePrizeRequest>()
            .Produces<Prize>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(IPrizeService prizeService, Guid id, HttpContext context)
    {
        var result = await prizeService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(IPrizeService prizeService, Guid id, HttpContext context)
    {
        var result = await prizeService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(IPrizeService prizeService, HttpContext context)
    {
        var result = await prizeService.GetListAsync();
        return context.CreateResponse(result);
    }

    private static async Task<IResult> CreateAsync(IPrizeService prizeService, SavePrizeRequest request, HttpContext context)
    {
        var result = await prizeService.CreateAsync(request);
        return context.CreateResponse(result, "GetPrize", new { result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(IPrizeService prizeService, Guid id, SavePrizeRequest request, HttpContext context)
    {
        var result = await prizeService.UpdateAsync(id, request);
        return context.CreateResponse(result);
    }
}
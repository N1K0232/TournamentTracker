using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services;
using TournamentTracker.Extensions;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class TournamentsEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var tournamentsApiGroup = endpoints.MapGroup("/api/tournaments/");

        tournamentsApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        tournamentsApiGroup.MapGet("{id:guid}", GetAsync)
            .WithName("GetTournament")
            .Produces<Tournament>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        tournamentsApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<ListResult<Tournament>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        tournamentsApiGroup.MapPost(string.Empty, CreateAsync)
            .WithValidation<SaveTournamentRequest>()
            .Produces<Tournament>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        tournamentsApiGroup.MapPut("{id:guid}", UpdateAsync)
            .WithValidation<SaveTournamentRequest>()
            .Produces<Tournament>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(ITournamentService tournamentService, Guid id, HttpContext context)
    {
        var result = await tournamentService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(ITournamentService tournamentService, Guid id, HttpContext context)
    {
        var result = await tournamentService.GetAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(ITournamentService tournamentService, HttpContext context, string name = null, int pageIndex = 0, int itemsPerPage = 50)
    {
        var result = await tournamentService.GetListAsync(name, pageIndex, itemsPerPage);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> CreateAsync(ITournamentService tournamentService, SaveTournamentRequest request, HttpContext context)
    {
        var result = await tournamentService.CreateAsync(request);
        return context.CreateResponse(result, "GetTournament", new { result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(ITournamentService tournamentService, Guid id, SaveTournamentRequest request, HttpContext context)
    {
        var result = await tournamentService.UpdateAsync(id, request);
        return context.CreateResponse(result);
    }
}
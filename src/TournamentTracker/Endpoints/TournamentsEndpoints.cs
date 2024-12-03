using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class TournamentsEndpoints : IEndpointRouteHandlerBuilder
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

        tournamentsApiGroup.MapPost(string.Empty, InsertAsync)
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

    private static async Task<IResult> DeleteAsync(ITournamentService tournamentService, Guid id, HttpContext httpContext)
    {
        var result = await tournamentService.DeleteAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(ITournamentService tournamentService, Guid id, HttpContext httpContext)
    {
        var result = await tournamentService.GetAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(ITournamentService tournamentService, HttpContext httpContext, string name = null, int pageIndex = 0, int itemsPerPage = 50)
    {
        var result = await tournamentService.GetListAsync(name, pageIndex, itemsPerPage);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> InsertAsync(ITournamentService tournamentService, SaveTournamentRequest request, HttpContext httpContext)
    {
        var result = await tournamentService.InsertAsync(request);
        return httpContext.CreateResponse(result, "GetTournament", new { result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(ITournamentService tournamentService, Guid id, SaveTournamentRequest request, HttpContext httpContext)
    {
        var result = await tournamentService.UpdateAsync(id, request);
        return httpContext.CreateResponse(result);
    }
}
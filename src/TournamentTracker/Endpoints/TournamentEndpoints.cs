using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class TournamentEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var tournamentsApiGroup = endpoints.MapGroup("/api/tournaments").WithTags("Tournaments");

        tournamentsApiGroup.MapPost(string.Empty, CreateAsync)
            .Produces<Tournament>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<SaveTournamentRequest>()
            .WithName("CreateTournament");

        tournamentsApiGroup.MapGet("{id:guid}", GetAsync)
            .Produces<Tournament>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetTournament");

        tournamentsApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<IEnumerable<Tournament>>()
            .WithName("GetTournaments");

        tournamentsApiGroup.MapPut("{id:guid}", UpdateAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithValidation<SaveTournamentRequest>()
            .WithName("UpdateTournament");

        tournamentsApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteTournament");
    }

    private static async Task<IResult> CreateAsync(SaveTournamentRequest request, ITournamentService tournamentService, HttpContext httpContext)
    {
        var result = await tournamentService.CreateAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetTournament", new { id = result.Content?.Id });
        return response;
    }

    private static async Task<IResult> GetAsync(Guid id, ITournamentService tournamentService, HttpContext httpContext)
    {
        var result = await tournamentService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> GetListAsync(ITournamentService tournamentService, HttpContext httpContext)
    {
        var result = await tournamentService.GetListAsync(httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> UpdateAsync(Guid id, SaveTournamentRequest request, ITournamentService tournamentService, HttpContext httpContext)
    {
        var result = await tournamentService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> DeleteAsync(Guid id, ITournamentService tournamentService, HttpContext httpContext)
    {
        var result = await tournamentService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}
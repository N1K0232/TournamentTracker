using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class TeamEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var tournamentsApiGroup = endpoints.MapGroup("/api/teams").WithTags("Teams");

        tournamentsApiGroup.MapPost(string.Empty, CreateAsync)
            .Produces<Team>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<SaveTeamRequest>()
            .WithName("CreateTeam");

        tournamentsApiGroup.MapGet("{id:guid}", GetAsync)
            .Produces<Team>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetTeam");

        tournamentsApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<IEnumerable<Team>>()
            .WithName("GetTeams");

        tournamentsApiGroup.MapPut("{id:guid}", UpdateAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithValidation<SaveTeamRequest>()
            .WithName("UpdateTeam");

        tournamentsApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteTeam");
    }

    private static async Task<IResult> CreateAsync(SaveTeamRequest request, ITeamService teamService, HttpContext httpContext)
    {
        var result = await teamService.CreateAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetTeam", new { id = result.Content?.Id });
        return response;
    }

    private static async Task<IResult> GetAsync(Guid id, ITeamService teamService, HttpContext httpContext)
    {
        var result = await teamService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> GetListAsync(ITeamService teamService, HttpContext httpContext)
    {
        var result = await teamService.GetListAsync(httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> UpdateAsync(Guid id, SaveTeamRequest request, ITeamService teamService, HttpContext httpContext)
    {
        var result = await teamService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> DeleteAsync(Guid id, ITeamService teamService, HttpContext httpContext)
    {
        var result = await teamService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}
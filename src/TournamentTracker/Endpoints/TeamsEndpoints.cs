using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class TeamsEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var teamsApiGroup = endpoints.MapGroup("/api/teams");

        teamsApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        teamsApiGroup.MapGet("{id:guid}", GetAsync)
            .WithName("GetTeam")
            .Produces<Person>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        teamsApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<ListResult<Team>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        teamsApiGroup.MapPost(string.Empty, InsertAsync)
            .WithValidation<SaveTeamRequest>()
            .Produces<Team>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        teamsApiGroup.MapPut("{id:guid}", UpdateAsync)
            .WithValidation<SaveTeamRequest>()
            .Produces<Team>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(ITeamService teamService, Guid id, HttpContext httpContext)
    {
        var result = await teamService.DeleteAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(ITeamService teamService, Guid id, HttpContext httpContext)
    {
        var result = await teamService.GetAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(ITeamService teamService, HttpContext httpContext, string name = null, int pageIndex = 0, int itemsPerPage = 50)
    {
        var result = await teamService.GetListAsync(name, pageIndex, itemsPerPage);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> InsertAsync(ITeamService teamService, SaveTeamRequest request, HttpContext httpContext)
    {
        var result = await teamService.InsertAsync(request);
        return httpContext.CreateResponse(result, "GetTeam", new { id = result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(ITeamService teamService, Guid id, SaveTeamRequest request, HttpContext httpContext)
    {
        var result = await teamService.UpdateAsync(id, request);
        return httpContext.CreateResponse(result);
    }
}
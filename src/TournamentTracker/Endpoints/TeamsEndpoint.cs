using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Extensions;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class TeamsEndpoint : IEndpointRouteHandlerBuilder
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

        teamsApiGroup.MapPost(string.Empty, CreateAsync)
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

    private static async Task<IResult> DeleteAsync(ITeamService teamService, Guid id, HttpContext context)
    {
        var result = await teamService.DeleteAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(ITeamService teamService, Guid id, HttpContext context)
    {
        var result = await teamService.GetAsync(id);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(ITeamService teamService, HttpContext context, string name = null, int pageIndex = 0, int itemsPerPage = 50)
    {
        var result = await teamService.GetListAsync(name, pageIndex, itemsPerPage);
        return context.CreateResponse(result);
    }

    private static async Task<IResult> CreateAsync(ITeamService teamService, SaveTeamRequest request, HttpContext context)
    {
        var result = await teamService.CreateAsync(request);
        return context.CreateResponse(result, "GetTeam", new { id = result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(ITeamService teamService, Guid id, SaveTeamRequest request, HttpContext context)
    {
        var result = await teamService.UpdateAsync(id, request);
        return context.CreateResponse(result);
    }
}
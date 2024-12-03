using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Collections;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.Endpoints;

public class PeopleEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var peopleApiGroup = endpoints.MapGroup("/api/people");

        peopleApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        peopleApiGroup.MapGet("{id:guid}", GetAsync)
            .WithName("GetPerson")
            .Produces<Person>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        peopleApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<ListResult<Person>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        peopleApiGroup.MapPost(string.Empty, InsertAsync)
            .WithValidation<SavePersonRequest>()
            .Produces<Person>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        peopleApiGroup.MapPut("{id:guid}", UpdateAsync)
            .WithValidation<SavePersonRequest>()
            .Produces<Person>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> DeleteAsync(IPeopleService peopleService, Guid id, HttpContext httpContext)
    {
        var result = await peopleService.DeleteAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetAsync(IPeopleService peopleService, Guid id, HttpContext httpContext)
    {
        var result = await peopleService.GetAsync(id);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(IPeopleService peopleService, HttpContext httpContext, string firstName = null, string lastName = null, string orderBy = "FirstName, LastName", int pageIndex = 0, int itemsPerPage = 50)
    {
        var result = await peopleService.GetListAsync(firstName, lastName, orderBy, pageIndex, itemsPerPage);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> InsertAsync(IPeopleService peopleService, SavePersonRequest request, HttpContext httpContext)
    {
        var result = await peopleService.InsertAsync(request);
        return httpContext.CreateResponse(result, "GetPerson", new { id = result.Content?.Id });
    }

    private static async Task<IResult> UpdateAsync(IPeopleService peopleService, Guid id, SavePersonRequest request, HttpContext httpContext)
    {
        var result = await peopleService.UpdateAsync(id, request);
        return httpContext.CreateResponse(result);
    }
}
using MinimalHelpers.FluentValidation;
using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using TournamentTracker.BusinessLayer.Services.Interfaces;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace PersonTracker.Endpoints;

public class PeopleEndpoints : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var peopleApiGroup = endpoints.MapGroup("/api/people").WithTags("People");

        peopleApiGroup.MapPost(string.Empty, CreateAsync)
            .Produces<Person>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithValidation<SavePersonRequest>()
            .WithName("CreatePerson");

        peopleApiGroup.MapGet("{id:guid}", GetAsync)
            .Produces<Person>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetPerson");

        peopleApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<IEnumerable<Person>>()
            .WithName("GetPeople");

        peopleApiGroup.MapPut("{id:guid}", UpdateAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithValidation<SavePersonRequest>()
            .WithName("UpdatePerson");

        peopleApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeletePerson");
    }

    private static async Task<IResult> CreateAsync(SavePersonRequest request, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.CreateAsync(request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result, "GetPerson", new { id = result.Content?.Id });
        return response;
    }

    private static async Task<IResult> GetAsync(Guid id, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.GetAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> GetListAsync(IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.GetListAsync(httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> UpdateAsync(Guid id, SavePersonRequest request, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.UpdateAsync(id, request, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }

    private static async Task<IResult> DeleteAsync(Guid id, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.DeleteAsync(id, httpContext.RequestAborted);

        var response = httpContext.CreateResponse(result);
        return response;
    }
}
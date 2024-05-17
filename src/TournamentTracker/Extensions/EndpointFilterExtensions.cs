using TournamentTracker.Filters;

namespace TournamentTracker.Extensions;

public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
        => builder.AddEndpointFilter<ValidatorFilter<T>>();
}
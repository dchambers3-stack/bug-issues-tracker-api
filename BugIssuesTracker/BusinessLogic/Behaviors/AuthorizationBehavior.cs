using BugTracker.Api.Services;
using MediatR;

namespace BugTracker.Api.Behaviors;

public interface IAuthorizedRequest
{
    string[] AllowedRoles { get; }
}

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUser;

    public AuthorizationBehavior(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    )
    {
        if (
            request is IAuthorizedRequest authRequest
            && !authRequest.AllowedRoles.Contains(_currentUser.Role)
        )
        {
            throw new UnauthorizedAccessException(
                $"Role '{_currentUser.Role}' is not permitted to perform this action."
            );
        }

        return await next();
    }
}

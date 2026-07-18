// Services/CurrentUserService.cs
using System.Security.Claims;

namespace BugTracker.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public int UserId
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier) ?? User?.FindFirst("sub");

            if (claim == null || !int.TryParse(claim.Value, out var id))
                throw new UnauthorizedAccessException(
                    "No authenticated user found on the request."
                );

            return id;
        }
    }

    public string Role
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.Role);
            return claim?.Value ?? string.Empty;
        }
    }
}

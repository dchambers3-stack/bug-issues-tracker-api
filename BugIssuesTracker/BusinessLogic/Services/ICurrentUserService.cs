// Services/ICurrentUserService.cs
namespace BugTracker.Api.Services;

public interface ICurrentUserService
{
    int UserId { get; }
    string Role { get; }
    bool IsAuthenticated { get; }
}

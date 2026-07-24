using BugIssuesTrackerApi.BugIssuesTracker.Data;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;
using BugTracker.Api.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.Auth;

public record AuthorizeRequest : IRequest<CurrentUserDto>;

// Handler

public class AuthorizeRequestHandler : IRequestHandler<AuthorizeRequest, CurrentUserDto>
{
    private readonly BugIssuesTrackerContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AuthorizeRequestHandler(
        BugIssuesTrackerContext context,
        ICurrentUserService currentUserService
    )
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CurrentUserDto> Handle(
        AuthorizeRequest request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUserService.UserId;
        var role = _currentUserService.Role;
        var query = await _context
            .Users.Where(u => u.Id == userId)
            .Select(u => new
            {
                u.Username,
                u.Role,
                u.FirstName,
                u.LastName,
            })
            .FirstOrDefaultAsync(cancellationToken);

        var response = new CurrentUserDto
        {
            FirstName = query?.FirstName,
            LastName = query?.LastName,
            UserId = userId,
            Username = query?.Username,
            Role = query?.Role?.Name,
        };
        return response;
    }
}

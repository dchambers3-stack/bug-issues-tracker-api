using BugIssuesTracker.Data.Dtos;
using BugIssuesTrackerApi.BugIssuesTracker.Data;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Models;
using BugTracker.Api.DTOs;
using BugTracker.Api.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Queries.Profile.GetProfileInfo;

public record GetProfileInfoRequest(int userId) : IRequest<ProfileInfoDto>;

// Handler

public class GetProfileInfoRequestHandler : IRequestHandler<GetProfileInfoRequest, ProfileInfoDto>
{
    private readonly BugIssuesTrackerContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Users> _passwordHasher = new PasswordHasher<Users>();

    public GetProfileInfoRequestHandler(BugIssuesTrackerContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<ProfileInfoDto> Handle(
        GetProfileInfoRequest request,
        CancellationToken cancellationToken
    )
    {
        var user = await _context
            .Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == request.userId, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        var profileInfo = new ProfileInfoDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Password = null, // Do not expose the password
        };

        return profileInfo;
    }
}

// Validator
public class GetProfileInfoRequestValidator : AbstractValidator<GetProfileInfoRequest>
{
    public GetProfileInfoRequestValidator()
    {
        RuleFor(x => x.userId).GreaterThan(0).WithMessage("User ID is required.");
    }
}

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

namespace BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.Profile.UpdateProfile;

public record UpdateProfileRequest(int userId, ProfileInfoDto profileInfo) : IRequest<string>;

// Handler

public class UpdateProfileRequestHandler : IRequestHandler<UpdateProfileRequest, string>
{
    private readonly BugIssuesTrackerContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Users> _passwordHasher = new PasswordHasher<Users>();

    public UpdateProfileRequestHandler(BugIssuesTrackerContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(
        UpdateProfileRequest request,
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

        user.Username = request.profileInfo.Username!;
        user.Email = request.profileInfo.Email!;
        if (!string.IsNullOrWhiteSpace(request.profileInfo.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.profileInfo.Password);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(user);

        return token;

        // I'm returning the Id here to not expose token and user information.
    }
}

// Validator
public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.profileInfo.Username).NotEmpty().WithMessage("Username is required.");

        RuleFor(x => x.profileInfo.Email).NotEmpty().WithMessage("Email is required.");
    }
}

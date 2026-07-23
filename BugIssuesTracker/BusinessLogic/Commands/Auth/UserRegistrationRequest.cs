using BugIssuesTrackerApi.BugIssuesTracker.Data;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Models;
using BugTracker.Api.DTOs;
using BugTracker.Api.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.Auth;

public record UserRegistrationRequest(string Username, string Email, string Password)
    : IRequest<string>;

// Handler

public class UserRegistrationHandler : IRequestHandler<UserRegistrationRequest, string>
{
    private readonly BugIssuesTrackerContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Users> _passwordHasher = new PasswordHasher<Users>();

    public UserRegistrationHandler(BugIssuesTrackerContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(
        UserRegistrationRequest request,
        CancellationToken cancellationToken
    )
    {
        var defaultRole = await _context.Roles.FindAsync(
            [2], // Assuming 2 is the default role for new users
            cancellationToken
        );

        var user = new Users
        {
            Username = request.Username,
            Email = request.Email,
            RoleId = 2, // Assuming 2 is the default role for new users
            Role = defaultRole,
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateToken(user);

        return token;

        // I'm returning the Id here to not expose token and user information.
    }
}

// Validator
public class UserRegistrationValidator : AbstractValidator<UserRegistrationRequest>
{
    private readonly BugIssuesTrackerContext _context;

    public UserRegistrationValidator(BugIssuesTrackerContext context)
    {
        _context = context;

        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");
        RuleFor(x => x.Password)
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.");
        RuleFor(x => x.Password)
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.");
        RuleFor(x => x.Password)
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one number.");
        RuleFor(x => x.Password)
            .Matches(@"[\W]")
            .WithMessage("Password must contain at least one special character.");

        // check for duplicate username and email
        RuleFor(x => x.Username)
            .MustAsync(
                async (username, cancellation) =>
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(
                        u => u.Username == username,
                        cancellation
                    );
                    return existingUser == null;
                }
            )
            .WithMessage("Username is already taken.");

        RuleFor(x => x.Email)
            .MustAsync(
                async (email, cancellation) =>
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(
                        u => u.Email == email,
                        cancellation
                    );
                    return existingUser == null;
                }
            )
            .WithMessage("Email is already registered.");
    }
}

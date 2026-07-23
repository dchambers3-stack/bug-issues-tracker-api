using BugIssuesTrackerApi.BugIssuesTracker.Data;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Models;
using BugTracker.Api.DTOs;
using BugTracker.Api.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.Auth;

public record LoginRequest(UserLoginDto user) : IRequest<string>;

// Handler

public class LoginRequestHandler : IRequestHandler<LoginRequest, string>
{
    private readonly BugIssuesTrackerContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Users> _passwordHasher = new PasswordHasher<Users>();

    public LoginRequestHandler(BugIssuesTrackerContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _context
            .Users.Include(u => u.Role)
            .FirstOrDefaultAsync(
                u =>
                    u.Username == request.user.UsernameOrEmail
                    || u.Email == request.user.UsernameOrEmail,
                cancellationToken
            );

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid username/email or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.user.Password ?? string.Empty
        );

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            // keeping error message generic for security purposes
            throw new UnauthorizedAccessException("Invalid username/email or password.");
        }

        var token = _tokenService.GenerateToken(user);

        return token;

        // I'm returning the Id here to not expose token and user information.
    }
}

// Validator
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.user.UsernameOrEmail)
            .NotEmpty()
            .WithMessage("Username or email is required.");

        RuleFor(x => x.user.Password).NotEmpty().WithMessage("Password is required.");
    }
}

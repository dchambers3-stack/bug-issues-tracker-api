namespace BugIssuesTracker.Controllers;

using BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.Auth;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpGet("me")]
    public async Task<CurrentUserDto> Me()
    {
        return await _mediator.Send(new AuthorizeRequest());
    }
}

namespace BugIssuesTracker.Controllers;

using BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.Auth;
using BugIssuesTrackerApi.BugIssuesTracker.Data.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.Auth.LoginRequest;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser([FromBody] UserRegistrationRequest request) =>
        Ok(await _mediator.Send(request));

    [HttpPost("login")]
    public async Task<ActionResult> LoginUser([FromBody] UserLoginDto user) =>
        Ok(await _mediator.Send(new LoginRequest(user)));
}

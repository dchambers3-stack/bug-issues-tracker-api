namespace BugIssuesTracker.Controllers;

using BugIssuesTrackerApi.BugIssuesTracker.BusinessLogic.Commands.UserRegistrationRequest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser([FromBody] UserRegistrationRequest request) =>
        Ok(await _mediator.Send(request));
}

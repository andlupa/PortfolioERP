using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioERP.Application.Features.Authentication;
using System.Security.Claims;

namespace PortfolioERP.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserListResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var users =
            await _userService.GetAllAsync(
                cancellationToken);

        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserListResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserListResponse>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user =
                await _userService.CreateAsync(
                    request,
                    cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(
    typeof(UserListResponse),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserListResponse>> UpdateStatus(
    int id,
    UpdateUserStatusRequest request,
    CancellationToken cancellationToken)
    {
        var currentUserIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(currentUserIdValue, out var currentUserId) &&
            currentUserId == id &&
            !request.IsActive)
        {
            return BadRequest(new
            {
                message = "You cannot deactivate your own account."
            });
        }

        var user = await _userService.UpdateStatusAsync(
            id,
            request.IsActive,
            cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPatch("{id:int}/role")]
    [ProducesResponseType(
    typeof(UserListResponse),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserListResponse>> UpdateRole(
    int id,
    UpdateUserRoleRequest request,
    CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateRoleAsync(
            id,
            request.Role,
            cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
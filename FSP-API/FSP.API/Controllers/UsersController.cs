using Microsoft.AspNetCore.Mvc;
using FSP.Domain.Models;
using MediatR;
using FSP.Application.command;
using FSP.Application.Query;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FSP_API.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="model">The object containing the new user's data.</param>
        /// <returns>A confirmation message indicating whether the user was added successfully or if an error occurred.</returns>
        /// <response code="200">User created successfully.</response>
        /// <response code="400">Invalid data.</response>
        [HttpPost("Register")]
        public async Task<ActionResult> RegisterUser([FromBody] UserModelRequest model)
        {

            if (!ModelState.IsValid)
                {
                  return BadRequest(ModelState);
                }
            var command = new AddUserCommand(model);
            var result =  await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Logically deletes a user.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>A confirmation message indicating whether the user was deleted successfully or if an error occurred.</returns>
        /// <response code="200">User deleted successfully.</response>
        /// <response code="400">Invalid data.</response>
        [HttpDelete]
        public async Task<ActionResult> DeleteUser([FromQuery] int userId)
        {
            if (!ModelState.IsValid)
                {
                   return BadRequest(ModelState);
                }
                var command = new DeleteUserCommand(userId);
                var result = await _mediator.Send(command);
                return Ok(result);
        }

        /// <summary>
        /// Retrieves user information.
        /// </summary>
        /// <returns>An object containing the user's information.</returns>
        /// <response code="200">Returns a UserModelDto object.</response>
        /// <response code="400">Invalid data.</response>
        [Authorize]
        [HttpGet("user-information")]
        public async Task<ActionResult> UserById()
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var query = new GetUserByIDQuery(UserId);
            var User = await _mediator.Send(query);

            if (User == null)
            {
                return BadRequest("User Doesn't Exist");
            }
            return Ok(User);
        }

        /// <summary>
        /// Retrieves a list of all registered users.
        /// </summary>
        /// <returns>A list containing information of all users.</returns>
        /// <response code="200">Successfully retrieved the list of users.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden. Only administrators can perform this action.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("all-users")]
        public async Task<ActionResult> AllUsers()
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var query = new GetUserByIDQuery(UserId);
            var User = await _mediator.Send(query);

            if (User == null)
            {
                return BadRequest("User Doesn't Exist");
            }
            return Ok(User);
        }

        /// <summary>
        /// Retrieves the total number of users and records in the database.
        /// </summary>
        /// <returns>An object containing the total counts of users and records.</returns>
        /// <response code="200">Returns the counts of users and records.</response>
        /// <response code="400">Invalid data.</response>
        [Authorize]
        [HttpGet("counts")]
        public async Task<IActionResult> Counts()
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var query = new GetCountQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
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
        /// Create a new user in the system.
        /// </summary>
        /// <param name="model">The object with the new user's data.</param>
        /// <returns>Returns a confirmation message to find out if the user was added or if an error occurred while adding the user.</returns>
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
        /// logically deletes a user.
        /// </summary>
        /// <param name="userId">The object with the UserId.</param>
        /// <returns>Returns a confirmation message to determine if the user was deleted or if an error occurred while deleting the user.</returns>
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
        /// get a user information.
        /// </summary>
        /// <returns>returns an object with the user information.</returns>
        /// <response code="200">UserModelDto</response>
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
        /// get the total number of users and records in the database.
        /// </summary>
        /// <returns>returns an object with the total number of users and records.</returns>
        /// <response code="200">UserModelDto</response>
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
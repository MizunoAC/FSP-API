using Microsoft.AspNetCore.Mvc;
using FSP.Domain.Models;
using MediatR;
using FSP.Application.Query;
using FSP.Domain.Models.DTO;
using FSP.Application.Command;
using Microsoft.AspNetCore.Authorization;
using FSP.Domain.Enums;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

namespace FSP_API.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly ILogger<AuthenticationController> _logger;
        private readonly IMediator _mediator;


        public AuthenticationController(ILogger<AuthenticationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }


        /// <summary>
        /// Method to perform user authentication.
        /// </summary>
        /// <param name="User">The user's authentication data.</param>
        /// <returns>Returns a token with the user's authorization.</returns>
        /// <response code="200">Token.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost("LogIn")]

        public async Task<IActionResult> Login(UserAuthentication User)
        {
            var command = new UserAuthenticationQuery(User);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Method to perform user authentication.
        /// </summary>
        /// <param name="User">The user's authentication data.</param>
        /// <returns>Returns a token with the user's authorization.</returns>
        /// <response code="200">Token.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet("Code")]

        public async Task<IActionResult> GetCode([FromQuery] string email)
        {
            var query = new GetResetCodeQuery(email);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Method to perform user authentication.
        /// </summary>
        /// <param name="User">The user's authentication data.</param>
        /// <returns>Returns a token with the user's authorization.</returns>
        /// <response code="200">Token.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var purpose = this.User.Claims.FirstOrDefault(c => c.Type == "purpose")?.Value;

            if (UserId == null || purpose != "password_reset")
            {
                return Unauthorized();
            }

            var command = new UpdatePasswordCommand(model);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Method to perform user authentication.
        /// </summary>
        /// <param name="User">The user's authentication data.</param>
        /// <returns>Returns a token with the user's authorization.</returns>
        /// <response code="200">Token.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet("verify-code")]
        public async Task<IActionResult> VerifyCode([FromQuery] string email, [FromQuery] int code)
        {
            var query = new VerifyCodeQuery(email, code);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
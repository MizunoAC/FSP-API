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
        /// Authenticates a user and returns an authorization token if the credentials are valid.
        /// </summary>
        /// <param name="User">The user's authentication data, including credentials.</param>
        /// <returns>Returns a token granting access to authorized resources.</returns>
        /// <response code="200">Authentication successful. Returns a token.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">Authentication failed. Invalid username or password.</response>
        [HttpPost("LogIn")]
        public async Task<IActionResult> Login(UserAuthentication User)
        {
            var command = new UserAuthenticationQuery(User);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Generates and sends a password reset code to the specified email address.
        /// </summary>
        /// <param name="email">The email address associated with the user account.</param>
        /// <returns>Returns a confirmation that the reset code has been sent.</returns>
        /// <response code="200">Password reset code sent successfully.</response>
        /// <response code="400">Invalid email address provided.</response>
        /// <response code="401">Unauthorized request.</response>
        [HttpGet("Code")]
        public async Task<IActionResult> GetCode([FromQuery] string email)
        {
            var query = new GetResetCodeQuery(email);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Allows an authenticated user to change their password.
        /// </summary>
        /// <param name="model">The data required to change the password.</param>
        /// <returns>Returns a message indicating the result of the password update.</returns>
        /// <response code="200">Password updated successfully.</response>
        /// <response code="400">Invalid data provided.</response>
        /// <response code="401">Unauthorized request.</response>
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
        /// Verifies whether the provided password reset code is valid for the specified email.
        /// </summary>
        /// <param name="email">The user's email associated with the password reset request.</param>
        /// <param name="code">The verification code sent to the user.</param>
        /// <returns>Returns a confirmation indicating whether the code is valid.</returns>
        /// <response code="200">The verification code is valid.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">The code is incorrect, expired, or unauthorized.</response>
        [HttpGet("verify-code")]
        public async Task<IActionResult> VerifyCode([FromQuery] string email, [FromQuery] int code)
        {
            var query = new VerifyCodeQuery(email, code);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Refreshes the user's authentication token using a valid refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token data sent in the request body.</param>
        /// <returns>Returns a new access token if the refresh token is valid.</returns>
        /// <response code="200">A new token was successfully generated.</response>
        /// <response code="400">The provided refresh token is invalid.</response>
        /// <response code="401">The refresh token is expired, revoked, or unauthorized.</response>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshToken)
        {
            var query = new RefreshTokenCommand(refreshToken);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using FSP.Domain.Models;
using MediatR;
using FSP.Application.Query;

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

        public async Task <IActionResult> Login (UserAuthentication User )
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

        public async Task<IActionResult> GetCode( [FromQuery] string email)
        {
            var query = new GetResetCodeQuery(email);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    
}
}
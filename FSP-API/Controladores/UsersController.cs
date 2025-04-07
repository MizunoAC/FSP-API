using Microsoft.AspNetCore.Mvc;
using FSP_API.Context;
using FSP_API.Excepcionescontroladas;
using FSP_API.ModelosDTO;
using FSP_API.Servicios;
using FSP.Domain.Models;
using MediatR;
using FSP.Application.command;
using FSP.Application.Query;

namespace FSP_API.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUsuariosServicio _usuariosServicio;
        private readonly ContexDb _contextDb;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsuariosServicio usuariosServicio, ContexDb contextDb, ILogger<UsersController> logger, IMediator mediator)
        {
            _usuariosServicio = usuariosServicio;
            _contextDb = contextDb;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> RegisterUser([FromBody] UserModelRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NoContent();
                }
                var result =  await _mediator.Send(new AddUserCommand(model));
                return Ok(result);
            }
            catch( Excepcion ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                throw ex;
            }
        }

        // Obtener todos los Usuarios Registrados en la BD

        [HttpDelete]

        public async Task<ActionResult> DeleteUser([FromQuery] int userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NoContent();
                }
                var result = await _mediator.Send(new DeleteUserCommand(userId));
                return Ok(result);
            }
            catch(Exception ex) { 
                _logger.LogWarning("Error: " + ex.ToString());
                return NotFound("Sin registros"); 
            }
        }

        // Obtener un Usuario usando el Id autogenerado por la BD

        [HttpGet("user-information")]

        public async Task<ActionResult> UserById ([FromQuery] int UserId)
        {

            try
            {

                var User = await _mediator.Send(new GetUserByIDQuery(UserId));

                if (User == null)
                {
                    return BadRequest("User Doesn't Exist");
                }


                return Ok(User);
            }
            catch(Exception ex) {
                _logger.LogWarning("Error: " + ex.ToString()); 
                return NotFound("Sin registros"); 
            
            }


        }






    }


}



        

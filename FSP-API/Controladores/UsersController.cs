using Microsoft.AspNetCore.Mvc;
using FSP_API.Context;
using FSP_API.Excepcionescontroladas;
using FSP_API.ModelosDTO;
using FSP_API.Servicios;
using FSP.Domain.Models;
using FSP.Application.query;
using MediatR;

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
        public async Task<ActionResult> RegisterUser(UserModelRequest model)
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

        [HttpGet("ObtenerUsuarios")]

        public async Task<ActionResult<IEnumerable<ModeloUsuarioDTO>>> Usuarios()
        {

            try
            {

                var User =

               await _usuariosServicio.usuarios();

                return Ok(User);
            }
            catch(Exception ex) { 
                _logger.LogWarning("Error: " + ex.ToString());
                return NotFound("Sin registros"); 
            }
        }

        // Obtener un Usuario usando el Id autogenerado por la BD

        [HttpGet("ObtenerUsuariosconId={ModeloUsuarioId}")]

        public async Task<ActionResult> BuscarUsuarioPorId(int ModeloUsuarioId)
        {

            try
            {

                var usuarios = await _usuariosServicio.BuscarUsuarioPorId(ModeloUsuarioId);

                if (usuarios == null)
                {
                    return BadRequest("Usuario Inexistente en la base de datos");
                }


                return Ok(usuarios);
            }
            catch(Exception ex) {
                _logger.LogWarning("Error: " + ex.ToString()); 
                return NotFound("Sin registros"); 
            
            }


        }






    }


}



        

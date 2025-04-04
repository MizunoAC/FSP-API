using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using NLog.Fluent;
using FSP_API.Context;
using FSP_API.Excepcionescontroladas;
using FSP_API.Modelos;
using FSP_API.ModelosDTO;
using FSP_API.Servicios;

namespace FSP_API.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase

    {
        private readonly IUsuariosServicio _usuariosServicio;
        private readonly ContexDb _contextDb;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuariosServicio usuariosServicio, ContexDb contextDb, ILogger<UsuarioController> logger)
        {
            _usuariosServicio = usuariosServicio;
            _contextDb = contextDb;
            _logger = logger;
        }


        //Agregar Nuevo Usuario

        [HttpPost("AgregarUsuario")]

        public async Task<ActionResult> AgregarUsuario(ModeloUsuarioDTO usuarioDTO)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    NoContent();
                }

                usuarioDTO.Edad = Convert.ToInt32(usuarioDTO.Edad);

                var usuario = await _usuariosServicio.AgregarUsuarioDTO(usuarioDTO);
                return Ok(usuario);


            }

            catch( Excepcion ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());

                return StatusCode(400, "El nombre de usuario ya existe en la base de datos, por favor elija otro");

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



        

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Excepcionescontroladas;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.ModelosDTO;
using Proyecto_faunasilvestre.Servicios;

namespace Proyecto_faunasilvestre.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase

    {
        private readonly IUsuariosServicio _usuariosServicio;
        private readonly ContexDb _contextDb;

        public UsuarioController(IUsuariosServicio usuariosServicio, ContexDb contextDb)
        {
            _usuariosServicio = usuariosServicio;
            _contextDb = contextDb;
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

            catch(Excepcion2)
            {
                return BadRequest("Usuario Existente");

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
            catch
            {
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
            catch
            {
                return NotFound("Sin registros");

            }


        }






    }


}



        
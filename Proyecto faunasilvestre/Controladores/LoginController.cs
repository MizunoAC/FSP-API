using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Excepcionescontroladas;
using Proyecto_faunasilvestre.ModelosDTO;
using Proyecto_faunasilvestre.Servicios;

namespace Proyecto_faunasilvestre.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase 
    {

        private readonly ILoginServicio _loginServicio;



        public LoginController(ILoginServicio loginServicio)
        {
            _loginServicio= loginServicio; 
        }

        //Metodo para generar iniciar sesion

        [HttpPost]

        public async Task <ActionResult> Iniciarsecion(ModeloLoginDTO modeloLoginDTO )
        {

            try
            {
                var Usuario = await _loginServicio.Autentificacion(modeloLoginDTO);

                if (Usuario == null)
                {
                  return NotFound("Usuario no encontrado o contrasena incorrecta");

                }

                var token = _loginServicio.GereneraciondelToken(Usuario);

                return Ok(token);
            }

            catch(Excepcion3)
            {
                return BadRequest("Contrasena Incorrecta");
            }
            catch (Excepcion4)
            {
              return  BadRequest("Usuario no encontrado");
            }

        }


    }
}

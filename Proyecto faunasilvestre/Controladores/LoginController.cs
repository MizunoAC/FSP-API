using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Excepcionescontroladas;
using Proyecto_faunasilvestre.Modelos.ViewModel;
using Proyecto_faunasilvestre.ModelosDTO;
using Proyecto_faunasilvestre.Servicios;
using System.Text.Json.Nodes;

namespace Proyecto_faunasilvestre.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase 
    {

        private readonly ILoginServicio _loginServicio;
        private readonly ILogger<LoginController> _logger;


        public LoginController(ILoginServicio loginServicio, ILogger<LoginController> logger)
        {
            _loginServicio = loginServicio;
            _logger = logger;
        }

        //Metodo para generar iniciar sesion

        [HttpPost("IniciarSesion")]

        public async Task <ActionResult> Iniciarsecion(ModeloLoginDTO modeloLoginDTO )
        {

            try
            {
                var Usuario = await _loginServicio.Autentificacion(modeloLoginDTO);

                if (Usuario == null)
                {
                  return NotFound("Usuario no encontrado o contrasena incorrecta");

                }

                TokenModel tokenModel = new TokenModel()
                {
                    Token = _loginServicio.GereneraciondelToken(Usuario)
            };

                
               
           

                return Ok(tokenModel);
            }

            catch(Excepcion3 ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return BadRequest("Usuario no registrado");
            }
            catch (Excepcion4 ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return  BadRequest("Contraseña incorrecta");
            }

        }


    }
}

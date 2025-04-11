using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FSP_API.Utilidades;
using FSP_API.Context;
using Microsoft.EntityFrameworkCore;
using FSP_API.Modelos.ViewModel;

namespace FSP_API.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestablecerController : ControllerBase
    {
        private readonly IRecuperarcontra _recuperarcontra;
        private readonly ContexDb _contexDb;
        private readonly ILogger<RestablecerController> _logger;




        public RestablecerController(IRecuperarcontra recuperarcontra, ContexDb contexDb, ILogger<RestablecerController> logger)
        {

            _contexDb = contexDb;
            _recuperarcontra = recuperarcontra;
            _logger = logger;
        }

        //Enviar Correo Electronico al Usuario

        [HttpPost("EnviarCorreo{Email}")]
        public async Task<ActionResult> EnviarcorreoDeRecuperacion( string Email)
        {

            try
            {

                var usuario = _contexDb.ModeloUsuarios.Where(e => e.Email == Email).FirstOrDefault();

                if (usuario != null)
                {

                    Random ramdon = new Random();
                    var Codigo = ramdon.Next(10000, 90000).ToString();


                    _contexDb.codigos.Add(new Codigo
                    {
                        fecha = DateTime.Now,
                        Token = Codigo,
                        ModeloUsuarioId = usuario.ModeloUsuarioId,
                        Usado = false,
                        Correo = Email
                    });
                    await _contexDb.SaveChangesAsync();

                    string mensaje = "Hola este es un correo autogenarado para la recuperaci�n " +
                        "o restablecimiento de la contrase�a. <br></br> Por favor copie y pegue el siguiente c�digo en la aplicaci�n" +
                        "<br></br>" + Codigo + "<br></br>" +
                        "Que tenga un buen d�a";



                    await _recuperarcontra.enviarCorreo(new EmailMensaje
                    {
                        Email = Email,
                        Asunto = "Restablecimiento de Contrase�a",
                        Mensaje = mensaje

                    });

                    return Ok();
                }
                return BadRequest("Correo no registrado");
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return BadRequest("Correo no registrado");
            }

        }


        //Restablecer Contrasena

        [HttpPost("RestablecerContrasena")]

        public async Task<ActionResult> RestablecerContrasena(RestablecerModelo datos)
        {
            try
            {
                var usuario = await _contexDb.codigos.Where(t => t.Token == datos.Codigo && t.Usado == false 
                && t.Correo ==datos.Correo).FirstOrDefaultAsync();

               var ExisteUsuario = await _recuperarcontra.CambiarContrasena(usuario, datos.Contrase�a);

                if (ExisteUsuario != null)
                {
                    return Ok();
                }

                else
                {
                    return NotFound("C�digo Invalido");
                }

   
            }
            catch (Exception ex) 
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return NotFound("Error al cambiar la contrase�a");
            }

        }





    }
}

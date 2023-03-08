using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_faunasilvestre.Utilidades;
using Proyecto_faunasilvestre.Context;
using Microsoft.EntityFrameworkCore;
using Proyecto_faunasilvestre.Modelos.ViewModel;

namespace Proyecto_faunasilvestre.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestablecerController : ControllerBase
    {
        private readonly IRecuperarcontra _recuperarcontra;
        private readonly ContexDb _contexDb;




        public RestablecerController(IRecuperarcontra recuperarcontra, ContexDb contexDb) {

            _contexDb = contexDb;
            _recuperarcontra = recuperarcontra;

        }

        //Enviar Correo Electronico al Usuario

        [HttpPost("EnviarCorreo")]
        public async Task<ActionResult> EnviarcorreoDeRecuperacion([FromBody] string Email)
        {

            var usuario = _contexDb.ModeloUsuarios.Where(e => e.Email == Email).FirstOrDefault();

            if (usuario != null)
            {

                Random ramdon = new Random();
                var Codigo = ramdon.Next(10000, 90000).ToString();
                

                _contexDb.codigos.Add(new Codigo
                {
                    fecha = DateTime.Now,
                    Token= Codigo,
                    ModeloUsuarioId=usuario.ModeloUsuarioId,
                    Usado = false,
                    Correo = Email
                });
                await _contexDb.SaveChangesAsync();

                string mensaje = "Hola este es un correo autogenarado para la recuperación " +
                    "o restablecimiento de la contraseña. <br></br> Por favor copie y pegue el siguiente código en la aplicación" +
                    "<br></br>" + Codigo + "<br></br>" +
                    "Que tenga un buen día";



                await _recuperarcontra.enviarCorreo(new EmailMensaje
                {
                    Email = Email,
                    Asunto = "Restablecimiento de Contraseña",
                    Mensaje = mensaje

                });

                return Ok("Correo enviado");
            }

            return BadRequest();

        }


        //Restablecer Contrasena

        [HttpPost("RestablecerContrasena")]

        public async Task<ActionResult> RestablecerContrasena(string Contrasena, string Codigo, string Correo)
        {
            try
            {
                var usuario = await _contexDb.codigos.Where(t => t.Token == Codigo && t.Usado == false 
                && t.Correo ==Correo).FirstOrDefaultAsync();

               var ExisteUsuario = await _recuperarcontra.CambiarContrasena(usuario, Contrasena);

                if (ExisteUsuario != null)
                {
                    return Ok("Contraseña Actualizada");
                }

                else
                {
                    return NotFound("Código Invalido");
                }

   
            }
            catch (Exception ex) 
            {
                return NotFound();
            }

        }





    }
}

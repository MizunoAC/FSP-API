using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.Modelos.ViewModel;
using System.Net;
using System.Net.Mail;

namespace Proyecto_faunasilvestre.Utilidades
{
    public class Recuperarcontra : IRecuperarcontra
    {
        public readonly ContexDb _contexDb;
        public Recuperarcontra( ContexDb contexDb) 
        
       { 
        _contexDb= contexDb;
        }


        public async Task<Task> enviarCorreo(EmailMensaje emailMensaje)
        {


            //Enviar Correo Electronico Autogenerado 


            var credentials = new NetworkCredential("fategoxd@outlook.com", "moneko12");
            var Correo = new MailMessage()
            {
                From = new MailAddress("fategoxd@outlook.com"),
                Subject = emailMensaje.Asunto,
                Body = emailMensaje.Mensaje,
                Priority = MailPriority.Normal,
                IsBodyHtml = true
            };

            Correo.To.Add(new MailAddress(emailMensaje.Email));
            var cliente = new SmtpClient()
            {
                Port = 587,
                Host = "smtp.office365.com",
                EnableSsl = true,
                Credentials = credentials

            };

            await cliente.SendMailAsync(Correo);



            return Task.CompletedTask;
        }




        // Generar URL

       public  string GenerarPath (HttpRequest request)
        {

            string url = string.Format("{0}://{1}", request.Scheme, request.Host);
            if (string.IsNullOrEmpty(request.PathBase))
            {

                url = request.PathBase + "/";

            }

            else
            {
                url = string.Format("{0}//{1}/", request.Scheme, request.Host);
            }

            return url;
        }


        ///Cambiar Contrasena del usuario 


        public  async Task <ModeloUsuario> CambiarContrasena(Codigo codigo, string Contrasena)
        {
            if (codigo != null)
            {
                var ExisteUsuario = await _contexDb.ModeloUsuarios.Where(u => u.ModeloUsuarioId == codigo.ModeloUsuarioId).FirstOrDefaultAsync();
                   

                var CEncriptada = BCrypt.Net.BCrypt.HashPassword(Contrasena);

                codigo.ModeloUsuario.Contraseña = CEncriptada;

                _contexDb.ModeloUsuarios.Update(ExisteUsuario);
                codigo.Usado = true;

                _contexDb.codigos.Update(codigo);

                await _contexDb.SaveChangesAsync();

                return ExisteUsuario;
            }
            return null;

        }



    }
}

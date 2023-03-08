using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.Modelos.ViewModel;

namespace Proyecto_faunasilvestre.Utilidades
{
    public interface IRecuperarcontra
    {


        public Task<Task> enviarCorreo(EmailMensaje emailMensaje);
        public  string GenerarPath(HttpRequest request);

        public Task<ModeloUsuario> CambiarContrasena(Codigo codigo, string Contrasena);

        }
}

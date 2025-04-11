using FSP_API.Modelos;
using FSP_API.Modelos.ViewModel;

namespace FSP_API.Utilidades
{
    public interface IRecuperarcontra
    {


        public Task<Task> enviarCorreo(EmailMensaje emailMensaje);
        public  string GenerarPath(HttpRequest request);

        public Task<ModeloUsuario> CambiarContrasena(Codigo codigo, string Contrasena);

        }
}

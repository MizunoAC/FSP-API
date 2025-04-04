using FSP_API.Modelos;
using FSP_API.ModelosDTO;

namespace FSP_API.Servicios
{
    public interface ILoginServicio
    {
        public Task<ModeloUsuario> Autentificacion(ModeloLoginDTO modeloLoginDTO);
        public string GereneraciondelToken(ModeloUsuario usuario);

    }     
}

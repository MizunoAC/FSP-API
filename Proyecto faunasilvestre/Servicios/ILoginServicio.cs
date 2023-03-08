using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.ModelosDTO;

namespace Proyecto_faunasilvestre.Servicios
{
    public interface ILoginServicio
    {
        public Task<ModeloUsuario> Autentificacion(ModeloLoginDTO modeloLoginDTO);
        public string GereneraciondelToken(ModeloUsuario usuario);

    }     
}

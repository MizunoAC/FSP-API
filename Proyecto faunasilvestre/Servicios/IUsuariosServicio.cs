using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.ModelosDTO;

namespace Proyecto_faunasilvestre.Servicios
{
    public interface IUsuariosServicio
    {

        public Task<ModeloUsuario> AgregarUsuarioDTO(ModeloUsuarioDTO UsuarioDTO);

        public Task<ModeloUsuarioDTO> BuscarUsuarioPorId(int ModeloUsuarioId);

        public Task<IEnumerable<ModeloUsuarioDTO>> usuarios();

        public ModeloUsuario BuscarAnimalPorUsuario(int ModeloUsuarioId);



     }
}

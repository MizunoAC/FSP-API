using FSP_API.Context;
using FSP_API.Modelos;
using FSP_API.ModelosDTO;

namespace FSP_API.Servicios
{
    public interface IUsuariosServicio
    {

        public Task<ModeloUsuario> AgregarUsuarioDTO(ModeloUsuarioDTO UsuarioDTO);

        public Task<ModeloUsuarioDTO> BuscarUsuarioPorId(int ModeloUsuarioId);

        public Task<IEnumerable<ModeloUsuarioDTO>> usuarios();

        public ModeloUsuario BuscarAnimalPorUsuario(int ModeloUsuarioId);



     }
}

using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.ModelosDTO;

namespace Proyecto_faunasilvestre.Servicios
{
    public interface IAnimalServicio
    {
        public Task<ModeloAnimales> AgregarAnimalDTO(ModeloAnimalesDTO animalesDTO);

        public Task<IEnumerable<ModeloAnimalesDTO>> BuscarAnimales();


        public Task<IEnumerable<ModeloAnimalesDTO>> BuscarAnimalesPorUsuario(int ModeloUsuarioId);


        public Task<AnimalesCatalogo> AgregarCatalogo(AnimalesCatalogo animalesCatalogo);


        public  Task<AnimalesCatalogoDTO> BuscarAnimalesCatalogo(string NombreComun);

        public Task<IEnumerable<AnimalesCatalogoDTO>> AnimalesenCatalogo();

        public Task<Contadores> contador();




    }
}

using FSP_API.Modelos;
using FSP_API.Modelos.TablaTemporal;
using FSP_API.ModelosDTO;

namespace FSP_API.Servicios
{
    public interface IAnimalServicio
    {
        public Task<ModeloAnimalTemporal> AgregarAnimalDTO(ModeloAnimalesDTO animalesDTO);

        public Task<IEnumerable<ModeloAnimalesDTO>> BuscarAnimales();

        public Task<IEnumerable<ModeloTemporalDTO>> BuscarAnimalesTemporal();



        public Task<IEnumerable<ModeloAnimalesDTO>> BuscarAnimalesPorUsuario(int ModeloUsuarioId);


        public Task<AnimalesCatalogo> AgregarCatalogo(AnimalesCatalogo animalesCatalogo);


        public  Task<AnimalesCatalogoDTO> BuscarAnimalesCatalogo(string NombreComun);

        public Task<IEnumerable<AnimalesCatalogoDTO>> AnimalesenCatalogo();

        public Task<Contadores> contador();

        public Task<ModeloAnimalTemporal> RegistrosAC(int id);

        public Task<IEnumerable<ModeloTemporalDTO>> BuscarAnimalesAceptados();

        public Task<ModeloAnimalTemporal> RegistrosRE(int id);




    }
}

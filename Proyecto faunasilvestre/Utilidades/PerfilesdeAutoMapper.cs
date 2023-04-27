using AutoMapper;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.Modelos.TablaTemporal;
using Proyecto_faunasilvestre.ModelosDTO;

namespace Proyecto_faunasilvestre.Utilidades
{
    public class PerfilesdeAutoMapper : Profile
    {

        public PerfilesdeAutoMapper()
        {
            CreateMap<ModeloUsuario, ModeloUsuarioDTO>().ReverseMap();
            CreateMap<ModeloAnimales, ModeloAnimalesDTO>().ReverseMap();
            CreateMap<AnimalesCatalogo, AnimalesCatalogoDTO>().ReverseMap();
            CreateMap<ModeloUsuario, ModeloLoginDTO>().ReverseMap();

            CreateMap<ModeloAnimalTemporal, ModeloAnimalesDTO>().ReverseMap();

            CreateMap<ModeloAnimalTemporal, ModeloTemporalDTO>().ReverseMap();

            CreateMap<ModeloAnimalTemporal, ModeloAnimales>().ReverseMap();

        }
    }
}

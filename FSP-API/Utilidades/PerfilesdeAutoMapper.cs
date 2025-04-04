using AutoMapper;
using FSP_API.Modelos;
using FSP_API.Modelos.TablaTemporal;
using FSP_API.ModelosDTO;

namespace FSP_API.Utilidades
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

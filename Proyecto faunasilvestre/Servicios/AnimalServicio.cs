using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Excepcionescontroladas;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.Modelos.TablaTemporal;
using Proyecto_faunasilvestre.ModelosDTO;
using System.Data.SqlTypes;

namespace Proyecto_faunasilvestre.Servicios
{
    public class AnimalServicio : IAnimalServicio
    {


        private readonly ContexDb _contextoDb;
        private readonly IMapper _Mapeo;



           public AnimalServicio(ContexDb contexto, IMapper Mapeo)
        {


            _contextoDb = contexto;
            _Mapeo = Mapeo;
        }


        //Agregar un nuevo registro de un animal

            public async Task<ModeloAnimalTemporal> AgregarAnimalDTO(ModeloAnimalesDTO animalesDTO)
        {

            // Comparar la variable recibida al registrar con la de la tabla estatica y asignar el Id correspondiente

            var Catalogo = from b in _contextoDb.AnimalesCatalogos
                           where b.CNombreComun.Equals(animalesDTO.NombreComun)
                           select b;

            if (Catalogo.Any() == false)
            {

                Console.WriteLine("El catalogo no existe");
                var Animal = _Mapeo.Map<ModeloAnimalTemporal>(animalesDTO);

                Animal.AnimalesCatalogoId = 7;
                Animal.Aceptado = false;
                Animal.rechazado= false;

                await _contextoDb.Temporal.
                    AddAsync(Animal);
                await _contextoDb.SaveChangesAsync();

                return Animal;



            }
            else
            {
                var Animal = _Mapeo.Map<ModeloAnimalTemporal>(animalesDTO);

                Animal.AnimalesCatalogoId = Catalogo.First().AnimalesCatalogoId;

                Animal.Aceptado = false;
                Animal.rechazado = false;

                await _contextoDb.AddAsync(Animal);
                await _contextoDb.SaveChangesAsync();

                return Animal;

            }

        }


        //Agregar Catalogo de Animales

            public async Task<AnimalesCatalogo> AgregarCatalogo(AnimalesCatalogo animalesCatalogo)
        {

                await _contextoDb.Set<AnimalesCatalogo>().AddAsync(animalesCatalogo);
                await _contextoDb.SaveChangesAsync();

                return animalesCatalogo;
        }


        //Obtener todos los animales de todos los usuarios

            public async Task  <IEnumerable<ModeloAnimalesDTO>>  BuscarAnimales()
        {
            return await _contextoDb.ModeloAnimales.ProjectTo<ModeloAnimalesDTO>(_Mapeo.ConfigurationProvider).ToListAsync();

        }


        // Registros en espera de aceptacion del administrador
        public async Task<IEnumerable<ModeloTemporalDTO>> BuscarAnimalesTemporal()
        {
            return await _contextoDb.Temporal.Where(A=> A.Aceptado==false && A.rechazado==false)
                .ProjectTo<ModeloTemporalDTO>(_Mapeo.ConfigurationProvider).ToListAsync();

        }

        // Registros Aceptados por el administrador

        public async Task<IEnumerable<ModeloTemporalDTO>> BuscarAnimalesAceptados()
        {
            return await _contextoDb.Temporal.Where(A => A.Aceptado == true && A.rechazado== false)
                .ProjectTo<ModeloTemporalDTO>(_Mapeo.ConfigurationProvider).ToListAsync();

        }


        // Obtener registros de un usuario

        public async Task <IEnumerable<ModeloAnimalesDTO>> BuscarAnimalesPorUsuario(int ModeloUsuarioId)
        {

           var Animal = await _contextoDb.ModeloAnimales.Where(A => A.ModeloUsuarioId == ModeloUsuarioId).ProjectTo<ModeloAnimalesDTO>
                (_Mapeo.ConfigurationProvider).ToListAsync();





            return Animal;

        }


        // Obtener El catalogo de un animal con su nombre comun

            public async Task<AnimalesCatalogoDTO> BuscarAnimalesCatalogo(string NombreComun)
        {

            var Catalogo = await _contextoDb.AnimalesCatalogos.Where(c => c.CNombreComun == NombreComun).FirstOrDefaultAsync();

            if (Catalogo == null)
            {

                Console.WriteLine("El catalogo no existe");
                throw new Excepcion1();

            }

            else
            {


                var CatalogoDTO = _Mapeo.Map<AnimalesCatalogoDTO>(Catalogo);

                return CatalogoDTO;
            }
        }



        // Obtener el catalogo de animales unicamente si existe un registro hecho por algun usuario// pantalla de inicio de la aplicacion 

            public async Task<IEnumerable<AnimalesCatalogoDTO>> AnimalesenCatalogo()
        {

            List<AnimalesCatalogo> modelos = new List<AnimalesCatalogo>();

            var nombres = await _contextoDb.ModeloAnimales.Select(s => s.NombreComun).ToListAsync();

            var comparacion = await _contextoDb.AnimalesCatalogos.ToListAsync();
            foreach (var item in comparacion)
            {
                if (nombres.IndexOf(item.CNombreComun) != -1)
                {
                    modelos.Add(item);
                }

            }

            List<AnimalesCatalogoDTO> resultado = new List<AnimalesCatalogoDTO>();
            foreach (var animal in modelos) {
                var r = new AnimalesCatalogoDTO();
                r.CNombreComun = animal.CNombreComun;
                r.CDescripcionAnimal = animal.CDescripcionAnimal;
                r.CHabitat=animal.CHabitat;
                r.DistribucionMapa = animal.DistribucionMapa;
                r.CImagenAnimal = animal.CImagenAnimal;


                resultado.Add(r);
            };



            return resultado;
        }




        public  async Task<Contadores> contador()
        {

          var contadores = new Contadores();

            var usuarios =  _contextoDb.ModeloUsuarios.Count();
            var animales =  _contextoDb.ModeloAnimales.Count();

            contadores.Usuarios = usuarios;
            contadores.Registros = animales;


            return contadores;

        }

   
    
    public async Task <ModeloAnimalTemporal> RegistrosAC(int id)
        {

            var registro = await _contextoDb.Temporal.FindAsync(id);

           
            if (registro == null)
            {
                return null;
            }

            registro.Aceptado = true;

            var Animal = _Mapeo.Map<ModeloAnimales>(registro);

            await _contextoDb.ModeloAnimales.AddAsync(Animal);
            await _contextoDb.SaveChangesAsync(); 


            return registro;

        }



        public async Task<ModeloAnimalTemporal> RegistrosRE(int id)
        {

            var registro = await _contextoDb.Temporal.FindAsync(id);


            if (registro == null)
            {
                return null;
            }

            registro.rechazado = true;
            await _contextoDb.SaveChangesAsync();


            return registro;

        }


    }


}

 
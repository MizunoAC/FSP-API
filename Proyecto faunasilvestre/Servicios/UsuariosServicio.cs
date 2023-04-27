using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Excepcionescontroladas;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.ModelosDTO;
using System.Drawing;
using System.Xml.Linq;

namespace Proyecto_faunasilvestre.Servicios
{
    public class UsuariosServicio : IUsuariosServicio
    {
        private readonly ContexDb _contextoDb;
        private readonly IMapper _Mapeo;


        
        public  UsuariosServicio(ContexDb contexto, IMapper Mapeo){


            _contextoDb = contexto;
            _Mapeo = Mapeo;
        }


        //Agregar usuario DTO

        public async Task <ModeloUsuario> AgregarUsuarioDTO(ModeloUsuarioDTO UsuarioDTO) 
        {
            //Comprobar contrasenas iguales

            var ContraseñaCorrecta = UsuarioDTO.Contraseña.Equals(UsuarioDTO.ConfirmarContraseña);

            if (ContraseñaCorrecta == true)
            {

                //Comprobar Nombre de Usuario en la base de datos 

                var Existe = from b in _contextoDb.ModeloUsuarios
                             where b.NombreUsuario.Equals(UsuarioDTO.NombreUsuario)
                             select b;



                if (Existe.Any() == false)
                {

                    var Usuario = _Mapeo.Map<ModeloUsuario>(UsuarioDTO);

                    var CEncriptada = BCrypt.Net.BCrypt.HashPassword(Usuario.Contraseña);

                    Usuario.Contraseña = CEncriptada;

                    await _contextoDb.AddAsync(Usuario);

                    await _contextoDb.SaveChangesAsync();


                    return Usuario;

                }
                else
                {

                    Console.WriteLine("Existe este nombre de usuario en la base de datos");
                    throw new Excepcion();

                }

            }

            else
            {
                throw new Excepcion3();
            }

         
        }



        // Buscar Usuario por id

        public async  Task<ModeloUsuarioDTO>  BuscarUsuarioPorId (int ModeloUsuarioId)
        {


            var usuario = await _contextoDb.ModeloUsuarios.FindAsync(ModeloUsuarioId);

            var usuarioDTO = _Mapeo.Map<ModeloUsuarioDTO>(usuario);              


            //var Usuario = await _contextoDb.ModeloUsuarios.FindAsync(ModeloUsuarioId);
            return usuarioDTO;

        }


        // buscar animales por Id de usuario

        public  ModeloUsuario BuscarAnimalPorUsuario (int ModeloUsuarioId)
        {

            var usuario = _contextoDb.ModeloUsuarios.Find(ModeloUsuarioId);

            return usuario;

        }


        //Obtener todos los usuarios 
        public async Task<IEnumerable<ModeloUsuarioDTO>> usuarios()
            {
             return await _contextoDb.ModeloUsuarios.ProjectTo<ModeloUsuarioDTO>(_Mapeo.ConfigurationProvider).ToListAsync();




        }



    }
}

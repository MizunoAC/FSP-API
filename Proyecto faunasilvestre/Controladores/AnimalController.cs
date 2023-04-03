using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Excepcionescontroladas;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.ModelosDTO;
using Proyecto_faunasilvestre.Servicios;
using System;
using System.Drawing;
using System.Security.Claims;

namespace Proyecto_faunasilvestre.Controladores
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalController : ControllerBase
    {



        private readonly IAnimalServicio _animalServicio;
        private readonly IUsuariosServicio _usuariosServicio;
        private readonly ContexDb _contexDb;
        private readonly IConfiguration _config;

        public AnimalController(IAnimalServicio animalServicio, IUsuariosServicio usuariosServicio, ContexDb contexDb, IConfiguration config)
        {
            _animalServicio = animalServicio;
            _usuariosServicio = usuariosServicio;
            _contexDb = contexDb;
            _config = config;
        }

        // Agregar nuevo animal cuando el usuario inicie sesion
        [Authorize]
        [HttpPost("AgregarAnimal")]


        public async Task<ActionResult>ModeloAnimalDTOAsync(ModeloAnimalesDTO animalDTO)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                   return NoContent();
                }



                var identity = HttpContext.User.Identity as ClaimsIdentity;


                if (identity != null)
                {

                    var Id = HttpContext.User.FindFirstValue("Id");
                    var imagenes = new ImagenesDTO();
                    _config.GetSection(ImagenesDTO.ImagenesSettings).Bind(imagenes);
                    
                    Guid guid= Guid.NewGuid();

                    animalDTO.ModeloUsuarioId = Convert.ToInt32(Id);

                    

                    byte[] img = Convert.FromBase64String(animalDTO.ImagenAnimal);
                    using (MemoryStream ms = new MemoryStream(img))
                    {
                        Image image = Image.FromStream(ms);

                        var imag = ($"{imagenes.Path}" + $"{guid}" + $"{imagenes.extension}");

                        image.Save($"{ imagenes.Path}" + $"{guid}"  + $"{imagenes.extension}"
                            , System.Drawing.Imaging.ImageFormat.Jpeg);

                        
                    }


  

                    animalDTO.ImagenAnimal = guid.ToString();


                    var animal = await _animalServicio.AgregarAnimalDTO(animalDTO);


                    return Ok(//animal);
                        );
                }

                return BadRequest();
            }

            catch (Excepcion1)
            {
                return NotFound("Catalogo de animales inexistente");


            }


        }


        // Generar lista de animales completa, todos los usuarios 
        [Authorize]
        [HttpGet("ObtenerAnimal")]

        public async Task<ActionResult<IEnumerable<ModeloAnimalesDTO>>>BuscarAnimales()
        {

            try
            {

                var animales = await _animalServicio.BuscarAnimales();

                if (animales != null)
                {
                    return Ok(animales);
                }

                return NotFound("Sin registros");
            }
            catch
            {
                return NotFound("Sin registros");

            }
        }

        // Generar la lista de registros del usuario

        [Authorize]
    
        [HttpGet("ObtenerRegistroAnimales")]

        public async Task<ActionResult<IEnumerable<ModeloAnimales>>> BuscarAnimalesPorUsuario()
        {

            try
            {

                var imagenes = new ImagenesDTO();
                _config.GetSection(ImagenesDTO.ImagenesSettings).Bind(imagenes);
                var identity = HttpContext.User.Identity as ClaimsIdentity;


                if (identity != null)
                {

                    var Id = HttpContext.User.FindFirstValue("Id");

                
                var ModeloUsuarioId = Convert.ToInt32(Id);


                var Usuario = _usuariosServicio.BuscarAnimalPorUsuario(ModeloUsuarioId);


                if (Usuario == null)
                {

                    return NotFound("Usuario Inexistente");


                }

                var Animales = await _animalServicio.BuscarAnimalesPorUsuario(ModeloUsuarioId);
                 foreach(var Animal in Animales)
                    {

                        string nombreImg = $"{imagenes.Path}{Animal.ImagenAnimal}{imagenes.extension}";

                        byte[] fileByte = System.IO.File.ReadAllBytes(nombreImg);
                            Animal.ImgBase64 = Convert.ToBase64String(fileByte);
                        
                    }

                return Ok(Animales);

                 }

                return NotFound();
            }


            catch
            {

                throw;



            }

        }

        // Obtener Catalogo de Animal usando nombre Comun


        [Authorize]
       
        [HttpGet("ObtenerCatalogo{NombreComun}")]

        public async Task<ActionResult<AnimalesCatalogoDTO>> ObtenerAnimaldeCatalogo(string NombreComun) 
        {

            try
            {
              var  CatalogoDeAnimal = await _animalServicio.BuscarAnimalesCatalogo
                    (NombreComun);

                if (CatalogoDeAnimal == null)

                {
                    return NotFound("Catalogo inexistente");
                }

                return Ok(CatalogoDeAnimal);
            }
            catch
            {
                return BadRequest("Catalogo de Animales Inexistente en la base de datos");
            }

        }
        
        // pantalla de inicio de la aplicacion 


        [Authorize]
        [HttpGet("Inicio")]

        public async Task<ActionResult<IEnumerable<AnimalesCatalogoDTO>>> AnimalesenBasedeDatos()
        {
            
            try
            {


             var catalogo = await _animalServicio.AnimalesenCatalogo();

                if (catalogo == null)
                {
                    return NotFound("Sin registros");
                }


                return Ok(catalogo);

            }
            catch
            {
                return BadRequest("Catalogo de Animales Inexistente en la base de datos");
            }

        }





        //////////////////////////////////////////////////////
        ///


        // Agregar Catalogo de Animales (Temporal)

        [HttpPost("AgregarAnimalCatalogo")]

        public async Task<ActionResult> AgregarCatalogo(AnimalesCatalogo animalesCatalogo)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    NoContent();
                }

                var animal = await _animalServicio.AgregarCatalogo(animalesCatalogo);

                return Ok(animal);
            }

            catch
            {
                return NoContent();


            }


        }


        /////////////////////////////////////////////////////



        
        [HttpGet("Contador")]

        public async Task<ActionResult<Contadores>> Contador()
        {

            try
            {


                var Total = await _animalServicio.contador();

               return Ok(Total);

            }
            catch
            {
                return BadRequest("Sin Usuarios o registros");
            }

        }











    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using FSP_API.Context;
using FSP_API.Excepcionescontroladas;
using FSP_API.Modelos;
using FSP_API.Modelos.ViewModel;
using FSP_API.ModelosDTO;
using FSP_API.Servicios;
using FSP_API.Utilidades;
using System;
using System.Drawing;
using System.Security.Claims;
using System.Web.Http.Results;

namespace FSP_API.Controladores
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
        private readonly IRecuperarcontra _recuperarcontra;
        private readonly ILogger<AnimalController> _logger;

        public AnimalController(IAnimalServicio animalServicio, IUsuariosServicio usuariosServicio, ContexDb contexDb, IConfiguration config, IRecuperarcontra recuperarcontra, 
            ILogger<AnimalController> logger)
        {
            _animalServicio = animalServicio;
            _usuariosServicio = usuariosServicio;
            _contexDb = contexDb;
            _config = config;
            _recuperarcontra = recuperarcontra;
            _logger = logger;
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
                    _logger.LogWarning("El modelo no es valido");
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

            catch (Excepcion1 ex) 
            {
                _logger.LogWarning("Error: " + ex.ToString());
                return NotFound("Catalogo de animales inexistente");


            }


        }


        // Generar lista de animales completa, todos los usuarios 
        
        [HttpGet("ObtenerAnimal")]

        public async Task<ActionResult<IEnumerable<ModeloAnimalesDTO>>>BuscarAnimales()
        {

            try
            {
                var imagenes = new ImagenesDTO();
                _config.GetSection(ImagenesDTO.ImagenesSettings).Bind(imagenes);
                var animales = await _animalServicio.BuscarAnimales();

                if (animales != null)
                {

                    foreach (var Animal in animales)
                    {

                        string nombreImg = $"{imagenes.Path}{Animal.ImagenAnimal}{imagenes.extension}";

                        byte[] fileByte = System.IO.File.ReadAllBytes(nombreImg);
                        Animal.ImgBase64 = Convert.ToBase64String(fileByte);

                    }
                    return Ok(animales);
                }

                return NotFound("Sin registros");
            }
            catch(Exception ex)
            {
                
                _logger.LogWarning("Error: " + ex.ToString());


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

                        _logger.LogError($"Ocurrio un error al procesar al usuario con Id {Id}");

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

                _logger.LogError($"Ocurrio un error en el servidor");

                return NotFound();
            }


            catch( Exception ex)
            {

                _logger.LogError("ERROR: " + ex.ToString());
                return StatusCode(500);



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


                    _logger.LogError("ERROR: Catalogo no existe");
                    return NotFound("Catalogo inexistente");
                }

                return Ok(CatalogoDeAnimal);
            }
            catch ( Exception ex ) 
            {
                _logger.LogError("ERROR: " + ex.ToString());
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
            catch( Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return StatusCode(500);
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

            catch(Exception ex)
            {

                _logger.LogWarning("Error: " + ex.ToString());
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
            catch(Exception ex)
            {
                _logger.LogWarning("Error: " + ex.ToString());
                return BadRequest("Sin Usuarios o registros");
            }

        }



        [HttpPost("RegistrosAceptado{id}")]

        public async Task<ActionResult<Contadores>> RegistrosAC(int id)
        {

            try
            {

                var Registro = await _animalServicio.RegistrosAC(id);

                return Ok(Registro);

            }
            catch( Exception ex) 
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return StatusCode(500);
            }

        }



        [HttpPost("Enviarcorreo")]

        public async Task<ActionResult<Contadores>> EnviarCorreo(ModeloCorreoRechazado modeloCorreo)
        {

            try
            {
                var usuario = _contexDb.ModeloUsuarios.Where(A=> A.ModeloUsuarioId == modeloCorreo.modeloUsuarioId).FirstOrDefault();

                if (usuario == null){
                    _logger.LogError("ERROR:Correo no registrado");
                    NotFound("Usuario no encontrado");
                }

                
                string mensaje = "Hola este es un correo autogenerado, por favor de no responder " + "<br><br>" +
                  "Lamento informarle que su registro fue rechazado debido a los siguientes criterios:" +
                  "<br></br>" + modeloCorreo.Motivo + "<br></br>" +
                  "Que tenga un buen día y disculpe las molestias";



                await _recuperarcontra.enviarCorreo(new EmailMensaje
                {
                    Email =usuario.Email,
                    Asunto = "Su registro fue rechazado",
                    Mensaje = mensaje

                });


                return Ok();

            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return StatusCode(500, "Ocurrio un error en el servidor, por favor intente de nuevo más tarde");
            }

        }


        [HttpPost("RegistrosRechazado{id}")]

        public async Task<ActionResult<Contadores>> RegistrosRE(int id)
        {

            try
            {


                var Registro = await _animalServicio.RegistrosRE(id);

                return Ok(Registro);

            }
            catch(Exception ex)
            {

                _logger.LogError("ERROR: " + ex.ToString());
                return StatusCode(500, "Ocurrio un error en el servidor, por favor intente más tarde");
            }

        }




        [HttpGet("RegistrosEnespera")]

        public async Task<ActionResult<Contadores>> EnEspera()
        {

            try
            {
                var imagenes = new ImagenesDTO();
                _config.GetSection(ImagenesDTO.ImagenesSettings).Bind(imagenes);
                var animales = await _animalServicio.BuscarAnimalesTemporal();

                if (animales != null)
                {

                    foreach (var Animal in animales)
                    {

                        string nombreImg = $"{imagenes.Path}{Animal.ImagenAnimal}{imagenes.extension}";

                        byte[] fileByte = System.IO.File.ReadAllBytes(nombreImg);
                        Animal.ImgBase64 = Convert.ToBase64String(fileByte);

                    }
                    return Ok(animales);
                }

                return NotFound("Sin registros");
            }
            catch(Exception ex)
            {

                _logger.LogWarning("Error: " + ex.ToString());
                return NotFound("Sin registros");

            }

        }



        [HttpGet("RegistrosAceptados")]

        public async Task<ActionResult<Contadores>> Aceptados()
        {

            try
            {
                var imagenes = new ImagenesDTO();
                _config.GetSection(ImagenesDTO.ImagenesSettings).Bind(imagenes);
                var animales = await _animalServicio.BuscarAnimalesAceptados();

                if (animales != null)
                {

                    foreach (var Animal in animales)
                    {

                        string nombreImg = $"{imagenes.Path}{Animal.ImagenAnimal}{imagenes.extension}";

                        byte[] fileByte = System.IO.File.ReadAllBytes(nombreImg);
                        Animal.ImgBase64 = Convert.ToBase64String(fileByte);

                    }
                    return Ok(animales);
                }

                return NotFound("Sin registros");
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return StatusCode (500);

            }

        }




    }



    
}

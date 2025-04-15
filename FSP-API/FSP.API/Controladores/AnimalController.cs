using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using FSP_API.Context;
using FSP_API.Modelos.ViewModel;
using FSP_API.ModelosDTO;
using FSP_API.Servicios;
using FSP_API.Utilidades;
using System.Security.Claims;
using FSP.Domain.Models;
using FSP.Application.command;
using MediatR;
using FSP.Application.Query;
using FSP.Application.Command;

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
        private readonly IMediator _mediator;

        public AnimalController(IAnimalServicio animalServicio, IUsuariosServicio usuariosServicio, ContexDb contexDb, IConfiguration config, IRecuperarcontra recuperarcontra,
            ILogger<AnimalController> logger, IMediator mediator)
        {
            _animalServicio = animalServicio;
            _usuariosServicio = usuariosServicio;
            _contexDb = contexDb;
            _config = config;
            _recuperarcontra = recuperarcontra;
            _logger = logger;
            _mediator = mediator;
        }

        #region UserAnimalRecord

        /// <summary>
        /// creates a new record for the user.
        /// </summary>
        /// <param name="record">The object with the new record data.</param>
        /// <returns>Returns a confirmation message to let you know if the record was added or if an error occurred while adding it.</returns>
        /// <response code="200">User created successfully.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [Authorize]
        [HttpPost("NewRecord")]
        public async Task<IActionResult> NewRecord( [FromBody] AnimalRecordRequest record)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var command = new AddRecordAnimalCommand(record, UserId);
            var result = await _mediator.Send(command);
            return Ok(result);


            //byte[] img = Convert.FromBase64String(animalDTO.ImagenAnimal);
            //using (MemoryStream ms = new MemoryStream(img))
            //{
            //    //Image image = Image.FromStream(ms);

            //    var imag = ($"{imagenes.Path}" + $"{guid}" + $"{imagenes.extension}");

            //image.Save($"{ imagenes.Path}" + $"{guid}"  + $"{imagenes.extension}"
            //    , System.Drawing.Imaging.ImageFormat.Jpeg);

        }

        /// <summary>
        /// Retrieves a list of records that a user has created.
        /// </summary>
        /// <param name="recordStatus">the status of the records Accepted, Rejected or Pending.</param>
        /// <returns>Returns a list of records that a user has created depending on the status it is in.</returns>
        /// <response code="200">List<AnimalRecordDto>.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet("AnimalRecordByUser/{recordStatus}")]
        public async Task<IActionResult> GetRecordsByUser([FromRoute] string recordStatus)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null || recordStatus == null)
            {
                return Unauthorized();
            }

            var query = new GetAnimalRecordByUserQuery(UserId, recordStatus);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        #endregion

        #region AnimalIndex

        /// <summary>
        /// Retrieves a list of records that a user has created.
        /// </summary>
        /// <param name="AnimalIndexRequest">the object with the data of the new index to add.</param>
        /// <returns>Returns a confirmation message to let you know if the Indexwas added or if an error occurred while adding it.</returns>
        /// <response code="200">MessageResponse.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("new-catalog")]
        public async Task<ActionResult> AddNewAnimalCatalog([FromBody] CatalogRequest model)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var command = new AddNewCatalogCommand(model);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the catalog of animals in the database.
        /// </summary>
        /// <returns>List<CatalogDto></returns>
        /// <response code="200">MessageResponse.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [Authorize]
        [HttpGet("Catalog")]
        public async Task<IActionResult> GetCatalog()
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetCatalogQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a catalog using the animal's common noun as a filter.
        /// </summary>
        /// <returns><CatalogDto></returns>
        /// <response code="200">MessageResponse.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [Authorize]
        [HttpGet("byCommonNoun/{CommonNoun}")]
        public async Task<IActionResult> GetCatalogByCommonNoun([FromRoute] string CommonNoun)
        {

            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetCatalogByCommonNounQuery(CommonNoun);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        #endregion

        [HttpGet("Contador")]

        public async Task<ActionResult<Contadores>> Contador()
        {

            try
            {


                var Total = await _animalServicio.contador();

                return Ok(Total);

            }
            catch (Exception ex)
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
            catch (Exception ex)
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
                var usuario = _contexDb.ModeloUsuarios.Where(A => A.ModeloUsuarioId == modeloCorreo.modeloUsuarioId).FirstOrDefault();

                if (usuario == null)
                {
                    _logger.LogError("ERROR:Correo no registrado");
                    NotFound("Usuario no encontrado");
                }


                string mensaje = "Hola este es un correo autogenerado, por favor de no responder " + "<br><br>" +
                  "Lamento informarle que su registro fue rechazado debido a los siguientes criterios:" +
                  "<br></br>" + modeloCorreo.Motivo + "<br></br>" +
                  "Que tenga un buen día y disculpe las molestias";



                await _recuperarcontra.enviarCorreo(new EmailMensaje
                {
                    Email = usuario.Email,
                    Asunto = "Su registro fue rechazado",
                    Mensaje = mensaje

                });


                return Ok();

            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.LogError("ERROR: " + ex.ToString());
                return StatusCode(500);

            }

        }
    }
}

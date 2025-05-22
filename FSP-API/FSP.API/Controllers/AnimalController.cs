using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IConfiguration _config;
        private readonly ILogger<AnimalController> _logger;
        private readonly IMediator _mediator;
        private readonly IHostEnvironment _env;

        public AnimalController(IConfiguration config,
            ILogger<AnimalController> logger, IMediator mediator, IHostEnvironment env)
        {
            _config = config;
            _logger = logger;
            _mediator = mediator;
            _env = env;
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
        public async Task<IActionResult> NewRecord([FromBody] AnimalRecordRequest record)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var command = new AddRecordAnimalCommand(record, UserId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a list of records that a user has created.
        /// </summary>
        /// <param name="recordStatus">the status of the records Accepted, Rejected or Pending.</param>
        /// <returns>Returns a list of records that a user has created depending on the status it is in.</returns>
        /// <response code="200">List<AnimalRecordDto>.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [Authorize]
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


        /// <summary>
        /// Retrieves a list of records that a user has created.
        /// </summary>
        /// <param name="recordStatus">the status of the records Accepted, Rejected or Pending.</param>
        /// <returns>Returns a list of records that a user has created depending on the status it is in.</returns>
        /// <response code="200">List<AnimalRecordDto>.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="401">Unauthorized.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("all-records/{recordStatus}")]

        public async Task<IActionResult> GetAllRecords([FromRoute] string recordStatus)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null || recordStatus == null)
            {
                return Unauthorized();
            }

            var query = new GetAllAnimalRecordQuery(recordStatus);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPatch("process-record/{recordId}")]

        public async Task<IActionResult> ProcessRecords([FromRoute] int recordId, [FromQuery] string status)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string root = _env.ContentRootPath;

            if (UserId == null || status == null)
            {
                return Unauthorized();
            }

            var command = new ProcessRecordCommand(recordId, status, root);
            var result = await _mediator.Send(command);
            return Ok(result);

        }
    }
}

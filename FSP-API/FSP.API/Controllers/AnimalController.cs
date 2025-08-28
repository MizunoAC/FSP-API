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
        /// Creates a new record for the authenticated user.
        /// </summary>
        /// <param name="record">The object containing the new record data.</param>
        /// <returns>Returns a confirmation message indicating whether the record was successfully added or if an error occurred.</returns>
        /// <response code="200">Record created successfully.</response>
        /// <response code="400">Invalid data provided.</response>
        /// <response code="401">Unauthorized access.</response>
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
        /// Retrieves a paginated list of animal records created by the authenticated user, filtered by status.
        /// </summary>
        /// <param name="recordStatus">The status of the records: Accepted, Rejected, or Pending.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="size">The number of records per page.</param>
        /// <returns>Returns a paginated list of records created by the user, filtered by status.</returns>
        /// <response code="200">Returns a list of AnimalRecordDto objects.</response>
        /// <response code="400">Invalid parameters or request data.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize]
        [HttpGet("AnimalRecordByUser/{recordStatus}")]
        public async Task<IActionResult> GetRecordsByUser([FromRoute] string recordStatus, [FromQuery] int page, [FromQuery] int size)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null || recordStatus == null)
            {
                return Unauthorized();
            }

            var query = new GetAnimalRecordByUserQuery(UserId, recordStatus, page, size);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        #endregion

        #region AnimalIndex

        /// <summary>
        /// Retrieves a paginated list of animals in the catalog.
        /// </summary>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="size">The number of items per page.</param>
        /// <returns>Returns a paginated list of CatalogDto objects.</returns>
        /// <response code="200">Successfully retrieved the catalog. Returns a list of CatalogDto.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize]
        [HttpGet("common-noun")]
        public async Task<IActionResult> GetCatalogCommonNoun()

        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetCatalogCommonNounQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of animals in the catalog.
        /// </summary>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="size">The number of items per page.</param>
        /// <returns>Returns a paginated list of CatalogDto objects.</returns>
        /// <response code="200">Successfully retrieved the catalog. Returns a list of CatalogDto.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize]
        [HttpGet("Catalog")]
       public async Task<IActionResult> GetCatalog([FromQuery] int page, [FromQuery] int size)

        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetCatalogQuery(page, size);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a catalog filtered by the Catalog Id.
        /// </summary>
        /// <returns>A CatalogDto object.</returns>
        /// <response code="200">Returns the requested catalog.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize]
        [HttpGet("by-Id/{CatalogId}")]
        public async Task<IActionResult> GetCatalogById([FromRoute] int CatalogId)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetCatalogById(CatalogId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a catalog map.
        /// </summary>
        /// <returns>A CatalogMapDto object.</returns>
        /// <response code="200">Returns the requested catalog map.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize]
        [HttpGet("map/{CatalogId}")]
        public async Task<IActionResult> GetCatalogMap([FromRoute] int CatalogId)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetCatalogMapQuery(CatalogId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a catalog map.
        /// </summary>
        /// <returns>A CatalogMapDto object.</returns>
        /// <response code="200">Returns the requested catalog map.</response>
        /// <response code="400">Invalid input data.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize]
        [HttpGet("bibliographic/{CatalogId}")]
        public async Task<IActionResult> GenerateBibliographic([FromRoute] int CatalogId)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GenerateBibliographicRecordQuery(CatalogId);
            var result = await _mediator.Send(query);
            return File(result, "application/pdf", "FichaAnimal.pdf");
        }
        #endregion
    }
}

using FSP.Application.Command;
using FSP.Application.Query;
using FSP.Domain.Models;
using FSP.Domain.Models.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FSP_API.Controladores
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        private readonly IHostEnvironment _env;

        public AdminController(ILogger<UsersController> logger, IMediator mediator, IHostEnvironment env)
        {
            _logger = logger;
            _mediator = mediator;
            _env = env;
        }

        /// <summary>
        /// Allows an administrator to add a new entry to the animal catalog.
        /// </summary>
        /// <param name="model">The object containing the data for the new catalog entry.</param>
        /// <returns>Returns a confirmation message indicating whether the catalog entry was added successfully or if an error occurred.</returns>
        /// <response code="200">Successfully added the new catalog entry. Returns a MessageResponse.</response>
        /// <response code="400">The provided data is invalid.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden. Only administrators can perform this action.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("new-catalog")]
        public async Task<ActionResult> AddNewAnimalCatalog([FromBody] CatalogRequest model)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var command = new AddNewCatalogCommand(model, UserId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of all user-submitted records filtered by their status.
        /// </summary>
        /// <param name="recordStatus">The status of the records to filter by: Accepted, Rejected, or Pending.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="size">The number of records to return per page.</param>
        /// <returns>Returns a paginated list of user records with the specified status.</returns>
        /// <response code="200">Successfully retrieved the list of records. Returns List&lt;AnimalRecordDto&gt;.</response>
        /// <response code="400">Invalid status value or pagination parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden. Only administrators can access this resource.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("all-records/{recordStatus}")]
        public async Task<IActionResult> GetAllRecords([FromRoute] string recordStatus, [FromQuery] int page, [FromQuery] int size)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null || recordStatus == null)
            {
                return Unauthorized();
            }

            var query = new GetAllAnimalRecordQuery(recordStatus, page, size);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Allows an administrator to update the status of a user-submitted record.
        /// </summary>
        /// <param name="status">The new status for the record: Accepted, Rejected, or Pending.</param>
        /// <param name="recordId">The ID of the record to be processed.</param>
        /// <returns>Returns a message indicating the result of the operation.</returns>
        /// <response code="200">The record status was successfully updated.</response>
        /// <response code="400">Invalid status value or record ID.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden. Only administrators can process records.</response>
        [Authorize(Roles = "Admin")]
        [HttpPatch("process-record")]
        public async Task<IActionResult> ProcessRecords(ProcessRecordRequest recordRequest)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string root = _env.ContentRootPath;

            if (UserId == null )
            {
                return Unauthorized();
            }

            var command = new ProcessRecordCommand(recordRequest, Convert.ToInt32(UserId));
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        /// <summary>
        /// Allows the administrator to update the details of a catalog entry.
        /// </summary>
        /// <param name="catalog">The catalog object containing the updated data.</param>
        /// <returns>Returns a message indicating the result of the update operation.</returns>
        /// <response code="200">The catalog was successfully updated.</response>
        /// <response code="400">The provided catalog data is invalid.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden. Only administrators can perform this action.</response>
        [Authorize(Roles = "Admin")]
        [HttpPatch("catalog")]
        public async Task<IActionResult> UpdateCatalog([FromBody] CatalogRequestDto catalog)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string root = _env.ContentRootPath;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var command = new UpdateCatalogDetailsCommand(catalog, UserId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Allows the administrator to update the image of a catalog entry.
        /// </summary>
        /// <param name="catalog">The catalog object containing the updated image data.</param>
        /// <returns>Returns a message indicating the result of the update operation.</returns>
        /// <response code="200">The catalog image was successfully updated.</response>
        /// <response code="400">The provided image data is invalid.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden. Only administrators can perform this action.</response>
        /// <response code="404">The catalog entry was not found.</response>
        [Authorize(Roles = "Admin")]
        [HttpPatch("catalog-img")]
        public async Task<IActionResult> UpdateCatalogImg([FromBody] CatalogImgDto catalog)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string root = _env.ContentRootPath;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var command = new UpdateCatalogImgCommand(catalog);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Allows the administrator to get the user list.
        /// </summary>
        /// <param name="catalog">The user object containing the users data.</param>
        /// <returns>Return  a List of users.</returns>
        /// <response code="200">Get Users.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden. Only administrators can perform this action.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("all-users/{active}")]
        public async Task<IActionResult> GetallUsers([FromRoute] bool active, [FromQuery] int page, [FromQuery] int size)
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string root = _env.ContentRootPath;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var command = new GetAllUsersQuery(page, size, active);
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
        [Authorize(Roles = "Admin")]
        [HttpGet("RecordByUser/{userId}")]
        public async Task<IActionResult> GetRecordsByUser([FromRoute] string userId, [FromQuery] string recordStatus, [FromQuery] int page, [FromQuery] int size)
        {
            var AdminId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (AdminId == null || recordStatus == null)
            {
                return Unauthorized();
            }

            var query = new GetAnimalRecordByUserQuery(userId, recordStatus, page, size);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves total statistics of animal records, including counts of Pending, Accepted,
        /// Rejected records, and the overall total.
        /// </summary>
        /// <returns>Returns an object containing aggregated counts of user records per status.</returns>
        /// <response code="200">Returns TotalStatistics object with status counts.</response>
        /// <response code="400">Invalid parameters or request data.</response>
        /// <response code="401">Unauthorized access.</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("statistic")]
        public async Task<IActionResult> GetTotalStatistics()
        {
            var UserId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null)
            {
                return Unauthorized();
            }

            var query = new GetTotalStatisticsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}


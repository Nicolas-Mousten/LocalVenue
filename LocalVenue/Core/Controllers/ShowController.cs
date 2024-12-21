using AutoMapper;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using LocalVenue.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace LocalVenue.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowController : ControllerBase
{
    private readonly IShowService _showService;
    private readonly IMapper _mapper;

    public ShowController(IShowService showService, IMapper mapper)
    {
        _showService = showService;
        _mapper = mapper;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Gets a paginated list of shows", Description = "Retrieves a paginated list of shows with optional search parameters.")]
    [SwaggerResponse(200, "Returns the list of shows", typeof(IEnumerable<ShowDTO_Nested>))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    public async Task<ActionResult<ShowDTO_Nested>> GetShows(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchParameter = null,
        [FromQuery, SwaggerParameter("The property to search by. Defaults to 'Title' if left empty.")] string? searchProperty = null)
    //TODO: can searchProperty be an enum instead of string?
    {
        try
        {
            var shows = await _showService.GetShows(page, pageSize, searchParameter, searchProperty);
            return Ok(shows);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{showId}/tickets")]
    [SwaggerOperation(Summary = "Gets available tickets for a show", Description = "Retrieves a list of available tickets for a specific show by its ID.")]
    [SwaggerResponse(200, "Returns the list of available tickets", typeof(IEnumerable<TicketDTO_Nested>))]
    [SwaggerResponse(404, "If the show is not found or show has no available tickets", typeof(string))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    public async Task<ActionResult<TicketDTO_Nested>> GetAvailableTicketsForShow(long showId)
    {
        try
        {
            var tickets = await _showService.GetAvailableTicketsForShow(showId);
            return Ok(tickets);
        }
        catch (Exception e)
        {
            if (e is KeyNotFoundException || e is ArgumentNullException)
            {
                return NotFound(e.Message);
            }
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Gets a show by ID", Description = "Retrieves a specific show by its ID.")]
    [SwaggerResponse(200, "Returns the show", typeof(ShowDTO_Nested))]
    [SwaggerResponse(404, "If the show is not found", typeof(string))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    public async Task<ActionResult<ShowDTO_Nested>> GetShow(long id)
    {
        try
        {
            var show = await _showService.GetShow(id);
            return Ok(show);
        }
        catch (Exception e)
        {
            if (e is KeyNotFoundException)
            {
                return NotFound($"Show with id {id} not found");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Creates a new show", Description = "Creates a new show with the provided details.")]
    [SwaggerResponse(201, "Returns the created show", typeof(ShowDTO_Nested))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    public async Task<ActionResult<ShowDTO_Nested>> AddShow(ShowDTO showDTO)
    {
        var show = _mapper.Map<Show>(showDTO);
        try
        {
            var createdShow = await _showService.AddShow(show);
            string host = HttpContext.Request.Host.Value;
            string uri = $"{Request.Path}/{createdShow.ShowId}";
            return Created(uri, createdShow);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [SwaggerOperation(Summary = "Updates an existing show", Description = "Updates the details of an existing show.")]
    [SwaggerResponse(200, "Returns the updated show", typeof(ShowDTO_Nested))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    [SwaggerResponse(404, "If the show is not found", typeof(string))]
    public async Task<ActionResult<ShowDTO_Nested>> UpdateShow([FromBody] ShowDTO showDTO)
    {
        var show = _mapper.Map<Show>(showDTO);
        try
        {
            var updatedShow = await _showService.UpdateShow(show);
            return Ok(updatedShow);
        }
        catch (Exception e)
        {
            if (e is KeyNotFoundException)
            {
                return NotFound($"No show found with id {show.ShowId}");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Deletes a show", Description = "Deletes a specific show by its ID.")]
    [SwaggerResponse(200, "Returns the deleted object", typeof(ShowDTO_Nested))]
    [SwaggerResponse(404, "If the show is not found", typeof(string))]
    public async Task<ActionResult<ShowDTO_Nested>> DeleteShow(long id)
    {
        try
        {
            var show = await _showService.DeleteShow(id);
            return Ok(show);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

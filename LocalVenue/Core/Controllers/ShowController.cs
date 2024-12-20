using AutoMapper;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using LocalVenue.Core.Models;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<ShowDTO_Nested>> GetShows([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchParameter = null, [FromQuery] string searchProperty = "Title")
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
    public async Task<ActionResult<TicketDTO_Nested>> GetAvailableTicketsForShow(long showId)
    {
        try
        {
            var tickets = await _showService.GetAvailableTicketsForShow(showId);
            return Ok(tickets);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShowDTO_Nested>> GetShow(long id)
    {
        try
        {
            var show = await _showService.GetShow(id);
            return Ok(show);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {
                return NotFound($"Show with id {id} not found");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Show>> AddShow(ShowDTO showDTO)
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
    public async Task<ActionResult<Show>> UpdateShow([FromBody] ShowDTO showDTO)
    {
        var show = _mapper.Map<Show>(showDTO);
        try
        {
            var updatedShow = await _showService.UpdateShow(show);
            return Ok(updatedShow);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Show>> DeleteShow(long id)
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

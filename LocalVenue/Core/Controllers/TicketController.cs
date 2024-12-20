using AutoMapper;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using LocalVenue.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LocalVenue.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IMapper _mapper;

    public TicketController(ITicketService ticketService, IMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDTO_Nested>> GetTicket(long id)
    {
        try
        {
            var ticket = await _ticketService.GetTicket(id);
            return Ok(ticket);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {
                return NotFound($"Ticket with id {id} not found");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<TicketDTO_Nested>> AddTicket(TicketDTO ticketDTO)
    {
        var ticket = _mapper.Map<Ticket>(ticketDTO);
        try
        {
            var createdTicket = await _ticketService.AddTicket(ticket);
            string host = HttpContext.Request.Host.Value;
            string uri = $"{Request.Path}/{createdTicket.TicketId}";
            return Created(uri, createdTicket);
        }
        catch (Exception e)
        {
            if (e.Message.Contains($"Ticket for show '{ticket.ShowId}' already has seat '{ticket.SeatId}' assigned"))
            {
                return Conflict(e.Message);
            }
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<TicketDTO_Nested>> UpdateTicket([FromBody] TicketDTO ticketDTO)
    {
        var ticket = _mapper.Map<Ticket>(ticketDTO);
        try
        {
            var updatedTicket = await _ticketService.UpdateTicket(ticket);
            return Ok(updatedTicket);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<TicketDTO_Nested>> DeleteTicket(int id)
    {
        try
        {
            var ticket = await _ticketService.DeleteTicket(id);
            return Ok(ticket);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

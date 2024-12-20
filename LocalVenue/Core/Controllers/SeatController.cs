using AutoMapper;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LocalVenue.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeatController : ControllerBase
{
    private readonly ISeatService _seatService;

    public SeatController(ISeatService seatService)
    {
        _seatService = seatService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Seat>> GetSeat(long id)
    {
        try
        {
            var seat = await _seatService.GetSeat(id);
            return Ok(seat);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {
                return NotFound($"Seat with id {id} not found");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpGet("row/{row}")]
    public async Task<ActionResult<List<Seat>>> GetSeatsInRow(int row)
    {
        try
        {
            var seats = await _seatService.GetSeatsInRow(row);
            return Ok(seats);
        }
        catch (Exception e)
        {
            if (e is ArgumentNullException)
            {
                return NotFound($"No seats found in row {row}");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<Seat>> AddSeat(Seat seat)
    {
        try
        {
            var createdSeat = await _seatService.AddSeat(seat);
            string host = HttpContext.Request.Host.Value;
            string uri = $"{Request.Path}/{createdSeat.SeatId}";
            return Created(uri, createdSeat);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<Seat>> UpdateSeat([FromBody] Seat seat)
    {
        try
        {
            var updatedSeat = await _seatService.UpdateSeat(seat);
            return Ok(updatedSeat);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Seat>> DeleteSeat(long id)
    {
        try
        {
            var seat = await _seatService.DeleteSeat(id);
            return Ok(seat);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}

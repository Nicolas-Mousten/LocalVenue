using LocalVenue.Core.Entities;
using LocalVenue.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LocalVenue.Controllers;

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
    [SwaggerOperation(Summary = "Gets a seat by ID", Description = "Retrieves a specific seat by its ID.")]
    [SwaggerResponse(200, "Returns the seat", typeof(Seat))]
    [SwaggerResponse(404, "If the seat is not found", typeof(string))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    public async Task<ActionResult<Seat>> GetSeat(long id)
    {
        try
        {
            var seat = await _seatService.GetSeat(id);
            return Ok(seat);
        }
        catch (Exception e)
        {
            if (e is KeyNotFoundException)
            {
                return NotFound($"Seat with id {id} not found");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpGet("row/{row}")]
    [SwaggerOperation(Summary = "Gets seats in a row", Description = "Retrieves a list of seats in a specific row.")]
    [SwaggerResponse(200, "Returns the list of seats", typeof(List<Seat>))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    [SwaggerResponse(404, "If no seats exist in row", typeof(string))]
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

    [Authorize]
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a new seat", Description = "Creates a new seat with the provided details.")]
    [SwaggerResponse(201, "Returns the created seat", typeof(Seat))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
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
    [SwaggerOperation(Summary = "Updates an existing seat", Description = "Updates the details of an existing seat.")]
    [SwaggerResponse(200, "Returns the updated seat", typeof(Seat))]
    [SwaggerResponse(400, "If there is an error", typeof(string))]
    [SwaggerResponse(404, "If the seat is not found", typeof(string))]
    public async Task<ActionResult<Seat>> UpdateSeat([FromBody] Seat seat)
    {
        try
        {
            var updatedSeat = await _seatService.UpdateSeat(seat);
            return Ok(updatedSeat);
        }
        catch (Exception e)
        {
            if (e is KeyNotFoundException)
            {
                return NotFound($"No seats found with id {seat.SeatId}");
            }
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Deletes a seat", Description = "Deletes a specific seat by its ID.")]
    [SwaggerResponse(200, "Returns the deleted object", typeof(Seat))]
    [SwaggerResponse(404, "If the seat is not found", typeof(string))]
    public async Task<ActionResult<Seat>> DeleteSeat(long id)
    {
        try
        {
            var seat = await _seatService.DeleteSeat(id);
            return Ok(seat);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}
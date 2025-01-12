using AutoMapper;
using LocalVenue.Core;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Services;
using LocalVenue.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace LocalVenue.Tests;

public class MappingProfileTest
{
    private readonly IMapper mapper;

    public MappingProfileTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        mapper = config.CreateMapper();
    }

    [Fact]
    public void TicketRequestToCoreTicket()
    {
        // Arrange
        var ticketRequest = new RequestModels.TicketRequest
        {
            TicketId = 1,
            ShowId = 1,
            SeatId = 1,
            Price = 50.5M,
            Status = Core.Enums.Status.Available,
            CustomerId = "1",
        };

        // Act
        var result = mapper.Map<Core.Entities.Ticket>(ticketRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticketRequest.TicketId, result.TicketId);
        Assert.Equal(ticketRequest.ShowId, result.ShowId);
        Assert.Equal(ticketRequest.SeatId, result.SeatId);
        Assert.Equal(ticketRequest.Price, result.Price);
        Assert.Equal(ticketRequest.Status, result.Status);
        Assert.Equal(ticketRequest.CustomerId, result.CustomerId);
    }

    [Fact]
    public void TicketRequestToCoreTicketNullProperties()
    {
        // Arrange
        var ticketRequest = new RequestModels.TicketRequest
        {
            ShowId = 1,
            SeatId = 1,
            Price = 50.5M,
            Status = Core.Enums.Status.Available,
        };

        // Act
        var result = mapper.Map<Core.Entities.Ticket>(ticketRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TicketId);
        Assert.Equal(ticketRequest.ShowId, result.ShowId);
        Assert.Equal(ticketRequest.SeatId, result.SeatId);
        Assert.Equal(ticketRequest.Price, result.Price);
        Assert.Equal(ticketRequest.Status, result.Status);
        Assert.Null(result.CustomerId);
    }

    [Fact]
    public void ShowRequestToCoreShow()
    {
        // Arrange
        var showRequest = new RequestModels.ShowRequest
        {
            ShowId = 1,
            Title = "Show 1",
            Description = "Lorem ipsum",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(1),
            Genre = Core.Enums.Genre.Comedy,
        };

        // Act
        var result = mapper.Map<Core.Entities.Show>(showRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(showRequest.ShowId, result.ShowId);
        Assert.Equal(showRequest.Title, result.Title);
        Assert.Equal(showRequest.StartTime, result.StartTime);
        Assert.Equal(showRequest.EndTime, result.EndTime);
        Assert.Equal(showRequest.Description, result.Description);
        Assert.Null(result.Tickets);
    }

    [Fact]
    public void ShowRequestToCoreShowNullProperties()
    {
        // Arrange
        var showRequest = new RequestModels.ShowRequest
        {
            Title = "Show 1",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(1),
            Genre = Core.Enums.Genre.Comedy,
        };

        // Act
        var result = mapper.Map<Core.Entities.Show>(showRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.ShowId);
        Assert.Equal(showRequest.Title, result.Title);
        Assert.Equal(showRequest.StartTime, result.StartTime);
        Assert.Equal(showRequest.EndTime, result.EndTime);
        Assert.Null(result.Description);
        Assert.Null(result.Tickets);
    }

    [Fact]
    public void TestCoreShowToWebShowWithTicket()
    {
        // Arrange
        var ticketCore = new Core.Entities.Ticket
        {
            TicketId = 1,
            ShowId = 1,
            SeatId = 1,
            Price = 50.5M,
            Status = Core.Enums.Status.Available,
            CustomerId = "1",
        };
        var showCore = new Core.Entities.Show
        {
            ShowId = 1,
            Title = "Show 1",
            Description = "Lorem ipsum",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(1),
            Genre = Core.Enums.Genre.Comedy,
            OpeningNight = true,
            Tickets = new List<Core.Entities.Ticket> { ticketCore },
        };

        // Act
        var result = mapper.Map<Web.Models.Show>(showCore);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(showCore.ShowId, result.Id);
        Assert.Equal(showCore.Title, result.Title);
        Assert.Equal(showCore.StartTime, result.StartTime);
        Assert.Equal(showCore.EndTime, result.EndTime);
        Assert.Equal(showCore.Description, result.Description);
        Assert.Equal(showCore.Genre, result.Genre);

        Assert.NotEmpty(result.Tickets!);
        Assert.Equal(showCore.Tickets!.Count, result.Tickets!.Count);
        Assert.IsType<Web.Models.Ticket>(result.Tickets!.First());
    }

    [Fact]
    public void TestCoreShowToWebShowNoTicket()
    {
        // Arrange
        var showCore = new Core.Entities.Show
        {
            ShowId = 1,
            Title = "Show 1",
            Description = "Lorem ipsum",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(1),
            Genre = Core.Enums.Genre.Comedy,
            OpeningNight = true,
        };

        // Act
        var result = mapper.Map<Web.Models.Show>(showCore);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(showCore.ShowId, result.Id);
        Assert.Equal(showCore.Title, result.Title);
        Assert.Equal(showCore.StartTime, result.StartTime);
        Assert.Equal(showCore.EndTime, result.EndTime);
        Assert.Equal(showCore.Description, result.Description);
        Assert.Equal(showCore.Genre, result.Genre);

        Assert.Empty(result.Tickets!);
    }

    [Fact]
    public void TestWebShowToCoreShowWithTicket()
    {
        // Arrange
        var seatWeb = new Web.Models.Seat
        {
            SeatId = 1,
            Section = "Front",
            Row = 1,
            Number = 1,
        };
        var ticketWeb = new Web.Models.Ticket
        {
            Id = 1,
            Seat = seatWeb,
            Price = 50.5M,
            Status = Core.Enums.Status.Available,
            SoldToCustomerId = "1",
        };
        var showWeb = new Web.Models.Show
        {
            Id = 1,
            Title = "Show 1",
            Description = "Lorem ipsum",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(1),
            Genre = Core.Enums.Genre.Comedy,
            OpeningNight = true,
            Tickets = new List<Web.Models.Ticket> { ticketWeb },
        };

        // Act
        var result = mapper.Map<Core.Entities.Show>(showWeb);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(showWeb.Id, result.ShowId);
        Assert.Equal(showWeb.Title, result.Title);
        Assert.Equal(showWeb.StartTime, result.StartTime);
        Assert.Equal(showWeb.EndTime, result.EndTime);
        Assert.Equal(showWeb.Description, result.Description);
        Assert.Equal(showWeb.Genre, result.Genre);

        Assert.NotEmpty(result.Tickets!);
        Assert.Equal(showWeb.Tickets!.Count, result.Tickets!.Count);
        Assert.IsType<Core.Entities.Ticket>(result.Tickets!.First());
        Assert.NotNull(result.Tickets!.First().Seat);
        Assert.IsType<Core.Entities.Seat>(result.Tickets!.First().Seat);
    }

    [Fact]
    public void TestWebShowToCoreShowNoTicket()
    {
        // Arrange
        var showWeb = new Web.Models.Show
        {
            Id = 1,
            Title = "Show 1",
            Description = "Lorem ipsum",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(1),
            Genre = Core.Enums.Genre.Comedy,
            OpeningNight = true,
        };

        // Act
        var result = mapper.Map<Core.Entities.Show>(showWeb);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(showWeb.Id, result.ShowId);
        Assert.Equal(showWeb.Title, result.Title);
        Assert.Equal(showWeb.StartTime, result.StartTime);
        Assert.Equal(showWeb.EndTime, result.EndTime);
        Assert.Equal(showWeb.Description, result.Description);
        Assert.Equal(showWeb.Genre, result.Genre);

        Assert.Empty(result.Tickets!);
    }

    [Fact]
    public void TestCoreTicketToWebTicket()
    {
        // Arrange
        var seatCore = new Core.Entities.Seat
        {
            SeatId = 1,
            Section = "Front",
            Row = 1,
            Number = 1,
        };
        var ticketCore = new Core.Entities.Ticket
        {
            TicketId = 1,
            ShowId = 1,
            SeatId = 1,
            Seat = seatCore,
            Price = 50.5M,
            Status = Core.Enums.Status.Available,
            CustomerId = "1",
        };

        // Act
        var result = mapper.Map<Web.Models.Ticket>(ticketCore);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticketCore.TicketId, result.Id);
        Assert.IsType<Web.Models.Seat>(result.Seat);
        Assert.Equal(ticketCore.Price, result.Price);
        Assert.Equal(ticketCore.Status, result.Status);
        Assert.Equal(ticketCore.CustomerId, result.SoldToCustomerId);
    }

    [Fact]
    public void TestWebTicketToCoreTicket()
    {
        // Arrange
        var seatWeb = new Web.Models.Seat
        {
            SeatId = 1,
            Section = "Front",
            Row = 1,
            Number = 1,
        };
        var ticketWeb = new Web.Models.Ticket
        {
            Id = 1,
            Seat = seatWeb,
            Price = 50.5M,
            Status = Core.Enums.Status.Available,
            SoldToCustomerId = "1",
        };

        // Act
        var result = mapper.Map<Core.Entities.Ticket>(ticketWeb);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticketWeb.Id, result.TicketId);
        Assert.IsType<Core.Entities.Seat>(result.Seat);
        Assert.Equal(ticketWeb.Price, result.Price);
        Assert.Equal(ticketWeb.Status, result.Status);
        Assert.Equal(ticketWeb.SoldToCustomerId, result.CustomerId);
    }

    [Fact]
    public void TestCoreSeatToWebSeat()
    {
        // Arrange
        var seatCore = new Core.Entities.Seat
        {
            SeatId = 1,
            Section = "Front",
            Row = 1,
            Number = 1,
        };

        // Act
        var result = mapper.Map<Web.Models.Seat>(seatCore);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(seatCore.SeatId, result.SeatId);
        Assert.Equal(seatCore.Section, result.Section);
        Assert.Equal(seatCore.Row, result.Row);
        Assert.Equal(seatCore.Number, result.Number);
    }

    [Fact]
    public void TestWebSeatToCoreSeat()
    {
        // Arrange
        var seatWeb = new Web.Models.Seat
        {
            SeatId = 1,
            Section = "Front",
            Row = 1,
            Number = 1,
        };

        // Act
        var result = mapper.Map<Core.Entities.Seat>(seatWeb);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(seatWeb.SeatId, result.SeatId);
        Assert.Equal(seatWeb.Section, result.Section);
        Assert.Equal(seatWeb.Row, result.Row);
        Assert.Equal(seatWeb.Number, result.Number);
    }
}

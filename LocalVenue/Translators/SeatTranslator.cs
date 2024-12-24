namespace LocalVenue.Translators;

public static class SeatTranslator
{
    public static Core.Entities.Seat Translate(Web.Models.Seat seat)
    {
        return new Core.Entities.Seat
        {
            SeatId = seat.SeatId,
            Section = seat.Section,
            Row = seat.Row,
            Number = seat.Number
        };
    }

    public static Web.Models.Seat Translate(Core.Entities.Seat seat)
    {
        return new Web.Models.Seat
        {
            SeatId = seat.SeatId,
            Section = seat.Section,
            Row = seat.Row,
            Number = seat.Number
        };
    }
}
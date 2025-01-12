namespace LocalVenue.Web.Models
{
    public class RefundList
    {
        public required int TicketCount { get; set; }
        public required string CustomerName { get; set; }
        public required string? CustomerEmail { get; set; }
        public required decimal TotalAmount { get; set; }
    }
}
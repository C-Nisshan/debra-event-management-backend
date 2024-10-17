namespace DebraSheru.Models
{
    public class TicketBatchResponseDto
    {
        public int TicketBatchID { get; set; }
        public string EventName { get; set; }
        public string TicketTypeName { get; set; }
        public int TotalTickets { get; set; }
        public decimal Price { get; set; }
    }
}

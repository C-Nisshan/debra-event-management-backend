namespace DebraSheru.Models
{
    public class TicketBatchUpdateDto
    {
        public int TicketBatchID { get; set; }
        public int EventID { get; set; }
        public int TicketTypeID { get; set; }
        public int TotalTickets { get; set; }
        public decimal Price { get; set; }
    }
}





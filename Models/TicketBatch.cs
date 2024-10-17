using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DebraSheru.Models
{
    public class TicketBatch
    {
        [Key]
        public int TicketBatchID { get; set; }

        [Required]
        public int EventID { get; set; }

        [ForeignKey("EventID")]
        [JsonIgnore]
        public virtual Event? Event { get; set; } 

        [Required]
        public int TicketTypeID { get; set; }

        [ForeignKey("TicketTypeID")]
        [JsonIgnore]
        public virtual TicketType? TicketType { get; set; } 

        [Required]
        public int TotalTickets { get; set; }

        [Required]
        public decimal Price { get; set; }

        [JsonIgnore]
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>(); 
    }
}

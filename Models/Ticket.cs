using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DebraSheru.Models
{
    public class Ticket
    {
        [Key]
        public int TicketID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int TicketTypeID { get; set; }

        public int TicketBatchID { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } 

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [ForeignKey("EventID")]
        [JsonIgnore]
        public virtual Event? Event { get; set; } 

        [ForeignKey("TicketTypeID")]
        [JsonIgnore]
        public virtual TicketType? TicketType { get; set; } 

        [ForeignKey("TicketBatchID")]
        [JsonIgnore]
        public virtual TicketBatch? TicketBatch { get; set; } 
    }
}

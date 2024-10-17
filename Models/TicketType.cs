using System.ComponentModel.DataAnnotations;

namespace DebraSheru.Models
{
    public class TicketType
    {
        [Key]
        public int TicketTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string TicketTypeName { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebraSheru.Models
{
    public class Sale
    {
        [Key]
        public int SaleID { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual User? User { get; set; } 

        [Required]
        public int TicketID { get; set; }

        [ForeignKey("TicketID")]
        public virtual Ticket? Ticket { get; set; } 

        [StringLength(20)]
        public string NICNumber { get; set; }

        [Required]
        public DateTime SaleDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
    }
}

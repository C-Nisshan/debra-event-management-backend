using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DebraSheru.Models
{
    public class EventCommission
    {
        [Key]
        public int EventCommissionID { get; set; }
        public int EventID { get; set; }

        [Required]
        public decimal CommissionRate { get; set; }

        [JsonIgnore]
        public virtual Event? Event { get; set; } 
    }
}

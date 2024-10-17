using System.ComponentModel.DataAnnotations;

namespace DebraSheru.Models
{
    public class Location
    {
        [Key]
        public int LocationID { get; set; }

        [Required]
        [StringLength(100)]
        public string LocationName { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }
}

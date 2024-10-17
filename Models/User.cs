using System.ComponentModel.DataAnnotations;

namespace DebraSheru.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; }

        [Required]
        [StringLength(50)]
        public string CompanyID { get; set; }

        [Required]
        public string Role { get; set; }

        [StringLength(15)]
        public string ContactNo { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }
}

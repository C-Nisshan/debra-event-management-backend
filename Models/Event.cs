using DebraSheru.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Event
{
    [Key]
    public int EventID { get; set; }

    [Required]
    [StringLength(100)]
    public string EventName { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    public DateTime EventTime { get; set; }

    public int? LocationID { get; set; } 

    [StringLength(50)]
    public string Status { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [JsonIgnore]
    public virtual Location? Location { get; set; } 

    [JsonIgnore]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>(); 
}



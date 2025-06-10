using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace Projekt_zaliczenie.Models;

public class Mission
{
    public long Id { get; set; }
    [Required]
    public string Company { get; set; }
    [Required]
    public string Location { get; set; }
    [Required]
    public DateTime LaunchDate { get; set; }
    [Required]
    public TimeSpan LaunchTime { get; set; }
    [Required]
    public string Rocket { get; set; }
    [Required]
    public string MissionName { get; set; }
    [Required]
    public int RocketStatusId { get; set; }
    public decimal? Price { get; set; }
    [Required]
    public int MissionStatusId { get; set; }
    public int? CreatedByUserId { get; set; }
    
}
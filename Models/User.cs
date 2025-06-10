using System.ComponentModel.DataAnnotations;

namespace Projekt_zaliczenie.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Username { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    [Required]
    [StringLength(20)]
    public string Role { get; set; }
}
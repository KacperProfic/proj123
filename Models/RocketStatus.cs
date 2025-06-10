namespace Projekt_zaliczenie.Models;

public class RocketStatus
{
    public int Id { get; set; }
    public string Status { get; set; } // Przykładowe pole, dostosuj do bazy
    public List<Mission> Missions { get; set; }
}
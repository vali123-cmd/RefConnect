namespace RefConnect.Models;

public class Championship
{
    public int ChampionshipId { get; set; }
    public string Name { get; set; }
    public string Season { get; set; }

    public ICollection<Match> Matches { get; set; }
    
}
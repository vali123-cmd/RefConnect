using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class Championship
{
    [Key]
    public int ChampionshipId { get; set; }
    public string Name { get; set; }
    public string Season { get; set; }

    public virtual ICollection<Match> Matches { get; set; }
    
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RefConnect.Models;

public class Match
{
    [Key]
    public int MatchId { get; set; }
    public DateTime MatchDateTime { get; set; }
    public string Location { get; set; }
    public string Score { get; set; }
    public string Status { get; set; }
    
    public int ChampionshipId { get; set; }
    [ForeignKey("ChampionshipId")]
    public Championship Championship { get; set; }

    public ICollection<MatchAssignment> MatchAssignments { get; set; }
    public Chat GroupChat { get; set; }
}

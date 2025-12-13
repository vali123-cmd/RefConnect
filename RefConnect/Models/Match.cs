using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RefConnect.Models;

public class Match
{
    [Key]
    public string MatchId { get; set; }
    public DateTime MatchDateTime { get; set; }
    public string Location { get; set; }
    public string Score { get; set; }
    public string Status { get; set; }
    
    public string ChampionshipId { get; set; }
    [ForeignKey("ChampionshipId")]
    public Championship Championship { get; set; }

    public virtual ICollection<MatchAssignment> MatchAssignments { get; set; }
    public virtual Chat GroupChat { get; set; }
}

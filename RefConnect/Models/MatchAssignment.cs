using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class MatchAssignment
{
    [Key]
    public int MatchAssignmentId { get; set; }
    public string RoleInMatch { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }

    public int MatchId { get; set; }
    public virtual Match Match { get; set; }
}



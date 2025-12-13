using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;

public class MatchAssignment
{
    [Key]
    public string MatchAssignmentId { get; set; }
    public string RoleInMatch { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public string MatchId { get; set; }
    public virtual Match Match { get; set; }
    
}



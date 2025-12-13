namespace RefConnect.Models;

public class MatchAssignment
{
    public int MatchAssignmentId { get; set; }
    public string RoleInMatch { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int MatchId { get; set; }
    public Match Match { get; set; }
}



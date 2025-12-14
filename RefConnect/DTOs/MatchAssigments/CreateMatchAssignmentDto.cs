using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.MatchAssigments;

public class CreateMatchAssignmentDto
{
    [Required]
    public string RoleInMatch { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string MatchId { get; set; }
}


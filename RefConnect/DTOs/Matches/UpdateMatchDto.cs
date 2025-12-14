using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Matches;

public class UpdateMatchDto
{
    [Required]
    public DateTime MatchDateTime { get; set; }

    [Required]
    public string Location { get; set; }

    public string Score { get; set; }

    [Required]
    public string Status { get; set; }

    [Required]
    public string ChampionshipId { get; set; }
}


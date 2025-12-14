using System.ComponentModel.DataAnnotations;

namespace RefConnect.DTOs.Championships;

public class CreateChampionshipDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Season { get; set; }
}


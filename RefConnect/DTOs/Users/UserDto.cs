namespace RefConnect.DTOs.Users;

public class UserDto
{
    public string Id { get; set; } 
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string UserName { get; set; }
    public string FullName => $"{FirstName} {LastName}"; 
    public DateTime CreatedAt { get; set; }
    
   
    public string? Token { get; set; } 
}


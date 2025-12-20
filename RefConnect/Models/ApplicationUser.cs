using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RefConnect.Models;


public class ApplicationUser : IdentityUser
{
     [Required]
    public override string UserName { get; set; }
    
    [Required] 
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string Description {get; set;}

   
   

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    

   
    public virtual ICollection<MatchAssignment> MatchAssignments { get; set; }
    public virtual ICollection<Post> Posts { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<ChatUser> ChatUsers { get; set; }
}
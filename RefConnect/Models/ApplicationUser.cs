using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace RefConnect.Models;


public class ApplicationUser : IdentityUser
{
    [Required]
    public override string? UserName { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    
    public string? ProfileImageUrl { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }


    public bool IsProfilePublic { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

   
    public virtual ICollection<MatchAssignment> MatchAssignments { get; set; } = new List<MatchAssignment>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();

    public virtual ICollection<FollowRequest> FollowerRequest { get; set; } = new List<FollowRequest>();
    public virtual ICollection<FollowRequest> FollowingRequest { get; set; } = new List<FollowRequest>();
}
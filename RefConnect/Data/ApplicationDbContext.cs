using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RefConnect.Models;

namespace RefConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Championship> Championships { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchAssignment> MatchAssignments { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique(); // folosim email pt lookup in bd
            //championship
            
            modelBuilder.Entity<Championship>()
                .HasMany(c => c.Matches)
                .WithOne(m => m.Championship)
                .HasForeignKey(m => m.ChampionshipId);
            
            //match assignment
            modelBuilder.Entity<MatchAssignment>()
                .HasOne(m => m.Match)
                .WithMany(m=>m.MatchAssignments)
                .HasForeignKey(ma => ma.MatchId);
            
            modelBuilder.Entity<MatchAssignment>()
                .HasOne(m => m.User)
                .WithMany(m => m.MatchAssignments)
                .HasForeignKey(m => m.UserId);
            
            //chat
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Match) 
                .WithOne(m => m.GroupChat) 
                .HasForeignKey<Chat>(c => c.MatchId) 
                .OnDelete(DeleteBehavior.Cascade);
            
            //chatuser
            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.Chat)
                .WithMany(c => c.ChatUsers)
                .HasForeignKey(cu => cu.ChatId);

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.ChatUsers)
                .HasForeignKey(cu => cu.UserId);
            
            
            //message 
            
            modelBuilder.Entity<Message>() 
                .HasOne(m => m.Chat) 
                .WithMany(c => c.Messages) 
                .HasForeignKey(m => m.ChatId);
            
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User) 
                .WithMany(u => u.Messages) 
                .HasForeignKey(m => m.UserId);
            
            //post
            
            modelBuilder.Entity<Post>() 
                .HasOne(p => p.User) 
                .WithMany(u => u.Posts) 
                .HasForeignKey(p => p.UserId);

            //comment
            
            modelBuilder.Entity<Comment>() 
                .HasOne(c => c.Post) 
                .WithMany(p => p.Comments) 
                .HasForeignKey(c => c.PostId);

            modelBuilder.Entity<Comment>() 
                .HasOne(c => c.User) 
                .WithMany() 
                .HasForeignKey(c => c.UserId);
                
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment) 
                .WithMany(c => c.Replies) 
                .HasForeignKey(c => c.ParentCommentId) 
                .OnDelete(DeleteBehavior.Cascade); //cand sterg comentariu stergem si reply-urile
            
            
        }
        
    }
}
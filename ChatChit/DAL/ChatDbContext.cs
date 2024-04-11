using ChatChit.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatChit.DAL
{
    public class ChatDbContext : IdentityDbContext<User>
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRoom>()
                .HasKey(ur => new { ur.UserId, ur.RoomId });

            modelBuilder.Entity<UserRoom>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRooms)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoom>()
                .HasOne(ur => ur.Room)
                .WithMany(r => r.UserRoom)
                .HasForeignKey(ur => ur.RoomId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.FromUser)
                .WithMany(u => u.FromUser)
                .HasForeignKey(m => m.FromUserId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ToUser)
                .WithMany(u => u.ToUser)
                .HasForeignKey(m => m.ToUserId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ToRoom)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RoomId);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Admin)
                .WithMany(u => u.Rooms);

            modelBuilder.Entity<Message>()
                 .HasOne(m => m.Parent)
                 .WithMany()
                 .HasForeignKey(m => m.ParentId)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

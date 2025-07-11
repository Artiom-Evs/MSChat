using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Models;

namespace MSChat.ChatAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MSChat.ChatAPI.Models.Chat> Chat { get; set; } = default!;
    public DbSet<ChatMember> Members { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<ChatMembership> ChatMemberships { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChatMembership>()
            .HasKey(cml => new { cml.ChatId, cml.MemberId });
        modelBuilder.Entity<ChatMember>()
            .HasIndex(cm => cm.UserId)
            .IsUnique();

        modelBuilder.Entity<ChatMembership>()
            .HasOne(cm => cm.Chat)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMembership>()
            .HasOne(cm => cm.Member)
            .WithMany(m => m.Chats)
            .HasForeignKey(cm => cm.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMembership>()
            .Property(cm => cm.RoleInChat)
            .HasDefaultValue(ChatRole.Member);

        modelBuilder.Entity<Message>()
            .HasIndex(m => new { m.ChatId, m.IdInChat })
            .IsUnique();
    }
}

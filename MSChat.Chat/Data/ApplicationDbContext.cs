using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSChat.Chat.Models;

namespace MSChat.Chat.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MSChat.Chat.Models.Chat> Chat { get; set; } = default!;
    public DbSet<ChatMember> Members { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<ChatMemberLink> ChatMemberLinks { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChatMemberLink>()
            .HasKey(cml => new { cml.ChatId, cml.MemberId });
        modelBuilder.Entity<ChatMember>()
            .HasIndex(cm => cm.UserId)
            .IsUnique();

        modelBuilder.Entity<ChatMemberLink>()
            .HasOne(cm => cm.Chat)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMemberLink>()
            .HasOne(cm => cm.Member)
            .WithMany(m => m.Chats)
            .HasForeignKey(cm => cm.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMemberLink>()
            .Property(cm => cm.RoleInChat)
            .HasDefaultValue(ChatRole.Member);
    }
}

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
}

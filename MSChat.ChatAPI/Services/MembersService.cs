using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public class MembersService : IMembersService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<MembersService> _logger;

    public MembersService(ApplicationDbContext dbContext, ILogger<MembersService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync(string? search = null)
    {
        var query = _dbContext.Members.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(m => m.Name.Contains(search));
        }

        var members = await query
            .Select(m => new MemberDto
            {
                Id = m.UserId,
                Name = m.Name,
                PhotoUrl = m.PhotoUrl,
                CreatedAt = m.CreatedAt,
                DeletedAt = m.DeletedAt
            })
            .ToListAsync();

        return members;
    }

    public async Task<MemberDto?> GetMemberByUserIdAsync(string userId)
    {
        var member = await _dbContext.Members
            .Where(m => m.UserId == userId)
            .Select(m => new MemberDto
            {
                Id = m.UserId,
                Name = m.Name,
                PhotoUrl = m.PhotoUrl,
                CreatedAt = m.CreatedAt,
                DeletedAt = m.DeletedAt
            })
            .FirstOrDefaultAsync();

        return member;
    }

    public async Task<MemberDto?> GetCurrentMemberAsync(string userId)
    {
        var member = await _dbContext.Members
            .Where(m => m.UserId == userId)
            .Select(m => new MemberDto
            {
                Id = m.UserId,
                Name = m.Name,
                PhotoUrl = m.PhotoUrl,
                CreatedAt = m.CreatedAt,
                DeletedAt = m.DeletedAt
            })
            .FirstOrDefaultAsync();

        return member;
    }
}
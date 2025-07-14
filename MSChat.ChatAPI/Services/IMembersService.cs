using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public interface IMembersService
{
    Task<IEnumerable<MemberDto>> GetMembersAsync(string? search = null);
    Task<MemberDto?> GetMemberByUserIdAsync(string userId);
    Task<MemberDto?> GetCurrentMemberAsync(string userId);
}

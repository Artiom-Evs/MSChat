using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Services;

public interface IMembersService
{
    Task<IEnumerable<MemberDto>> GetMembersAsync(string? search = null);
    Task<MemberDto?> GetMemberByIdAsync(long memberId);
    Task<MemberDto?> GetCurrentMemberAsync(string userId);
}

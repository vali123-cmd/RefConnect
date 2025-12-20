namespace RefConnect.Services.Interfaces;

using RefConnect.DTOs.Users;

public interface IProfileService
{
    // search by partial/full name or username; anonymous allowed
    Task<IEnumerable<ProfileDto>> SearchUsersAsync(string query, int limit = 20, CancellationToken ct = default);

    // returns profile; sensitive fields only when owner or profile is public
    Task<ProfileDto?> GetProfileAsync(string userId, string? requesterId = null, CancellationToken ct = default);

    // only owner can update
    Task<bool> UpdateProfileAsync(string userId, UpdateProfileDto dto, string requesterId, CancellationToken ct = default);
}
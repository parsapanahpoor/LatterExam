using PollingSystem.Domain.Entities;

namespace PollingSystem.Application.Interfaces;

public interface IPollRepository
{
    Task<Poll?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Poll>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Poll poll, CancellationToken ct = default);
    Task<bool> HasUserVotedAsync(Guid pollId, string encryptedVoterId, CancellationToken ct = default);
    Task AddVoteAsync(Vote vote, Guid optionId, CancellationToken ct = default);
}

using System.Collections.Concurrent;
using PollingSystem.Application.Interfaces;
using PollingSystem.Domain.Entities;

namespace PollingSystem.Infrastructure.Repositories;

public class InMemoryPollRepository : IPollRepository
{
    private readonly ConcurrentDictionary<Guid, Poll> _polls = new();
    private readonly ConcurrentDictionary<Guid, List<Vote>> _votes = new();

    public Task AddAsync(Poll poll, CancellationToken ct = default)
    {
        _polls[poll.Id] = poll;
        _votes[poll.Id] = [];
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Poll>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Poll>>(_polls.Values.ToList());

    public Task<Poll?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_polls.TryGetValue(id, out var poll) ? poll : null);

    public Task<bool> HasUserVotedAsync(Guid pollId, string encryptedVoterId, CancellationToken ct = default)
    {
        if (!_votes.TryGetValue(pollId, out var votes))
            return Task.FromResult(false);

        return Task.FromResult(votes.Any(v => v.EncryptedVoterId == encryptedVoterId));
    }

    public Task AddVoteAsync(Vote vote, Guid optionId, CancellationToken ct = default)
    {
        if (!_polls.TryGetValue(vote.PollId, out var poll))
            throw new KeyNotFoundException("نظرسنجی یافت نشد.");

        var option = poll.Options.FirstOrDefault(o => o.Id == optionId)
            ?? throw new ArgumentException("گزینه معتبر نیست.");

        option.VoteCount++;
        _votes[vote.PollId].Add(vote);
        return Task.CompletedTask;
    }
}

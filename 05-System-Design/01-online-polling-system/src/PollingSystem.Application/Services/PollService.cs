using PollingSystem.Application.DTOs;
using PollingSystem.Application.Interfaces;
using PollingSystem.Domain.Entities;

namespace PollingSystem.Application.Services;

public class PollService(IPollRepository repository, IEncryptionService encryption)
{
    public async Task<PollSummaryDto> CreatePollAsync(CreatePollRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
            throw new ArgumentException("سوال نظرسنجی نمی‌تواند خالی باشد.");

        if (request.Options.Count < 2)
            throw new ArgumentException("حداقل دو گزینه لازم است.");

        var pollId = Guid.NewGuid();
        var poll = new Poll
        {
            Id = pollId,
            Question = request.Question.Trim(),
            Options = request.Options.Select(o => new PollOption
            {
                Id = Guid.NewGuid(),
                PollId = pollId,
                Text = o.Trim()
            }).ToList()
        };

        await repository.AddAsync(poll, ct);

        return new PollSummaryDto(poll.Id, poll.Question, poll.Options.Count, poll.IsActive);
    }

    public async Task VoteAsync(Guid pollId, Guid optionId, string voterId, CancellationToken ct = default)
    {
        var poll = await repository.GetByIdAsync(pollId, ct)
            ?? throw new KeyNotFoundException("نظرسنجی یافت نشد.");

        if (!poll.IsActive)
            throw new InvalidOperationException("این نظرسنجی غیرفعال است.");

        if (!poll.Options.Any(o => o.Id == optionId))
            throw new ArgumentException("گزینه انتخاب‌شده معتبر نیست.");

        var encryptedVoterId = encryption.Encrypt(voterId);

        if (await repository.HasUserVotedAsync(pollId, encryptedVoterId, ct))
            throw new InvalidOperationException("شما قبلاً در این نظرسنجی رأی داده‌اید.");

        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            PollId = pollId,
            OptionId = optionId,
            EncryptedVoterId = encryptedVoterId
        };

        await repository.AddVoteAsync(vote, optionId, ct);
    }

    public async Task<PollResultsDto> GetResultsAsync(Guid pollId, CancellationToken ct = default)
    {
        var poll = await repository.GetByIdAsync(pollId, ct)
            ?? throw new KeyNotFoundException("نظرسنجی یافت نشد.");

        var total = poll.Options.Sum(o => o.VoteCount);

        var options = poll.Options.Select(o => new PollOptionResultDto(
            o.Id,
            o.Text,
            o.VoteCount,
            total == 0 ? 0 : Math.Round(o.VoteCount * 100.0 / total, 2)
        )).ToList();

        return new PollResultsDto(poll.Id, poll.Question, total, options, DateTime.UtcNow);
    }

    public async Task<IReadOnlyList<PollSummaryDto>> ListPollsAsync(CancellationToken ct = default)
    {
        var polls = await repository.GetAllAsync(ct);
        return polls.Select(p => new PollSummaryDto(p.Id, p.Question, p.Options.Count, p.IsActive)).ToList();
    }
}

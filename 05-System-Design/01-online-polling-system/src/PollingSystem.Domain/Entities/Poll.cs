namespace PollingSystem.Domain.Entities;

public class Poll
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public List<PollOption> Options { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class PollOption
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int VoteCount { get; set; }
}

public class Vote
{
    public Guid Id { get; set; }
    public Guid PollId { get; set; }
    public Guid OptionId { get; set; }
    public string EncryptedVoterId { get; set; } = string.Empty;
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}

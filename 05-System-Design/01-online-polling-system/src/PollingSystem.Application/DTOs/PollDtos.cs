namespace PollingSystem.Application.DTOs;

public record CreatePollRequest(string Question, List<string> Options);

public record VoteRequest(Guid OptionId);

public record PollOptionResultDto(Guid OptionId, string Text, int VoteCount, double Percentage);

public record PollResultsDto(
    Guid PollId,
    string Question,
    int TotalVotes,
    List<PollOptionResultDto> Options,
    DateTime UpdatedAt);

public record PollSummaryDto(Guid Id, string Question, int OptionCount, bool IsActive);

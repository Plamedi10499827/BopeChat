namespace CybersecurityAwarenessChatbot;

public sealed class CyberTask
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime? ReminderAt { get; set; }
    public bool IsCompleted { get; set; }
}

public sealed class QuizQuestion
{
    public string Question { get; init; } = "";
    public string[] Options { get; init; } = [];
    public int CorrectIndex { get; init; }
    public string Explanation { get; init; } = "";
}
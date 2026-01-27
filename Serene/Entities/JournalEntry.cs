using System.ComponentModel.DataAnnotations;

namespace Serene.Entities;

public class JournalEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    //storing date without time to enforce one entry per day
    public DateTime EntryDate { get; set; } = DateTime.Today;

    public string Title { get; set; } = string.Empty;
    public string ContentHtml { get; set; } = string.Empty;
    public string ContentMarkdown { get; set; } = string.Empty;

    public string PrimaryMood { get; set; } = string.Empty;

    //storing secondary moods as a comma-separated string
    public string SecondaryMoods { get; set; } = string.Empty;

    public string Category { get; set; } = "General";

    //storing tags as a comma-separated string for local simplicity
    public string Tags { get; set; } = string.Empty;

    public int WordCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
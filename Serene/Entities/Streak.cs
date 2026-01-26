namespace Serene.Entities;
using System.ComponentModel.DataAnnotations;

public class Streak
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.Empty;
    public DateTime ActivityDate { get; set; } = DateTime.Now.Date;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
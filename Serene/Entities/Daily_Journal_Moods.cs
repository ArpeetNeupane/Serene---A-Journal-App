using System.ComponentModel.DataAnnotations;

namespace Serene.Entities
{
    class Daily_Journal_Moods
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid JournalId { get; set; } = Guid.Empty;
        public Guid MoodId { get; set; } = Guid.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

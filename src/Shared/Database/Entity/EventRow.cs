namespace Shared.Database.Entity
{
    public abstract class EventRow
    {
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public string Type { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public string Metadata { get; set; } = "{}";
        public DateTime CreatedAt { get; set; }
        public Guid EventId { get; set; }
    }
}

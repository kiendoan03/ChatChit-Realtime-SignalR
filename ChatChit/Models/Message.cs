namespace ChatChit.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendAt { get; set; } = DateTime.Now;
        public string FromUserId { get; set; }
        public User FromUser { get; set; } = null!;
        public string? ToUserId { get; set; }
        public User ToUser { get; set; } = null!;
        public int? RoomId { get; set; }
        public Room ToRoom { get; set; } = null!;
        public int? ParentId { get; set; }
        public Message Parent { get; set; } = null!;
    }
}

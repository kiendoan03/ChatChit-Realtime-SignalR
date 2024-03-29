﻿namespace ChatChit.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendAt { get; set; } = DateTime.Now;
        public string FromUserId { get; set; }
        public User FromUser { get; set; } = null!;
        public string? ToUserId { get; set; }
        public User? ToUser { get; set; }
        public int? RoomId { get; set; }
        public Room? ToRoom { get; set; }
    }
}

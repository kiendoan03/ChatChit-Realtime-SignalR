namespace ChatChit.ViewModel
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendAt { get; set; } = DateTime.Now;
        public string FromUser { get; set; } = null!;
        public string ToUser { get; set; } = null!;
        public string Room { get; set; } = null!;
        public string Avatar { get; set; } = null!;
    }
}

namespace ChatChit.ViewModel
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SendAt { get; set; } = DateTime.Now;
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public string Avatar { get; set; } = null!;
    }
}

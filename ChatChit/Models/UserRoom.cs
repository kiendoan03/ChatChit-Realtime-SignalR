namespace ChatChit.Models
{
    public class UserRoom
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int Role { get; set; }
    }
}

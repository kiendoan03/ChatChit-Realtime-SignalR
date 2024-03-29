namespace ChatChit.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public User Admin { get; set; }
        //public ICollection<User> Users { get; set; }
        public ICollection<UserRoom> UserRoom { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}

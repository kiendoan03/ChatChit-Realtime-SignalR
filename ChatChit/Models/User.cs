using Microsoft.AspNetCore.Identity;

namespace ChatChit.Models
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Avatar { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<UserRoom> UserRooms { get; set; }
        public ICollection<Message> FromUser { get; set; }
        public ICollection<Message> ToUser { get; set; }
       

    }
}

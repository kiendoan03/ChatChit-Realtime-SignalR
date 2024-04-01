using System.Text.Json.Serialization;

namespace ChatChit.ViewModel
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = null!;
        [JsonIgnore]
        public string? Admin { get; set; } 
    }
}

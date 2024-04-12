using System.ComponentModel.DataAnnotations;

namespace ChatChit.ViewModel
{
    public class UploadViewModel
    {
        public class UploadViewModelToRoom
        {
            [Required]
            public string FromUserId { get; set; }
            [Required]
            public int RoomId { get; set; }
            [Required]
            public IFormFile File { get; set; }
            public int? ParentId { get; set; }

        }

        public class UploadViewModelToUser
        {
            [Required]
            public string FromUserId { get; set; }
            [Required]
            public string ToUserId { get; set; }
            [Required]
            public IFormFile File { get; set; }
        }

        public class UploadViewModelToLobby
        {
            [Required]
            public string FromUserId { get; set; }
            [Required]
            public IFormFile File { get; set; }
            public int? ParentId { get; set; }
        }
    }
}

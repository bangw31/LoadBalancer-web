using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ECommerceSystem.Shared.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Quan hệ với User
        public User? FromUser { get; set; }
        public User? ToUser { get; set; }

        // --- Mở rộng để gửi file/ảnh/video ---
        public string? FileName { get; set; }        // Tên file gốc
        public string? FileUrl { get; set; }         // URL lưu file (S3, Azure, local…)
        public string? FileType { get; set; }        // Loại file: "image", "video", "file"
        public long? FileSize { get; set; }
    }
}


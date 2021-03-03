using System;

namespace AWSS3Introduction.Models
{
    public class UploadJsonRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
    }
}

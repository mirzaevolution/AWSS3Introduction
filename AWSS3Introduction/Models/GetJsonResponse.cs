using System;

namespace AWSS3Introduction.Models
{
    public class GetJsonResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDateUtc { get; set; }
    }
}

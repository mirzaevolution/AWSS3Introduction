using System;

namespace AWSS3Introduction.Models
{
    public class ListFilesResponse
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public long Size { get; set; }
        public string Owner { get; set; }
        public DateTime LastModified { get; set; }
    }
}

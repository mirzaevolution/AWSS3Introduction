using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
namespace AWSS3Introduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        public BucketsController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost("create/{bucketName}")]
        public async Task<IActionResult> CreateIfNotExists(string bucketName)
        {
            await _s3Client.EnsureBucketExistsAsync(bucketName);
            return Ok();
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetBucketList()
        {
            ListBucketsResponse result = await _s3Client.ListBucketsAsync();
            return Ok(result.Buckets);
        }
        [HttpDelete("delete/{bucketName}")]
        public async Task<IActionResult> Delete(string bucketName)
        {
            DeleteBucketResponse result = await _s3Client.DeleteBucketAsync(bucketName);
            return StatusCode((int)result.HttpStatusCode);
        }
    }
}

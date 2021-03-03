using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using System.ComponentModel.DataAnnotations;
using Amazon.S3.Transfer;
using AWSS3Introduction.Models;
using System.IO;

namespace AWSS3Introduction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        public FilesController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }
        [HttpPost("{bucketName}/upload")]
        public async Task<IActionResult> UploadFiles(string bucketName, [FromForm, Required]IEnumerable<IFormFile> files)
        {
            if(files!=null && files.Count() > 0)
            {
                var result = await Upload(bucketName, files);
                return Ok(result);
            }
            return BadRequest(new { error = "File list to be uploaded is empty" });
        }
        [HttpGet("{bucketName}/list")]
        public async Task<IActionResult> ListFiles(string bucketName)
        {
            string continuationToken = null;
            List<ListFilesResponse> result = new List<ListFilesResponse>();
            do
            {
                ListObjectsV2Response response = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    ContinuationToken = continuationToken
                });
                continuationToken = response.ContinuationToken;
                if(response.S3Objects!=null && response.S3Objects.Count > 0)
                {
                    result.AddRange(response.S3Objects.Select(c => new ListFilesResponse
                    {
                        BucketName = c.BucketName,
                        Key = c.Key,
                        Size = c.Size,
                        LastModified = c.LastModified,
                        Owner = c.Owner?.DisplayName
                    }));
                }

            } while (continuationToken != null);
            

            return Ok(result);
            
        }

        [HttpGet("{bucketName}/download/{fileName}")]
        public async Task<IActionResult> DownloadAsStream(string bucketName, string fileName)
        {
            GetObjectResponse response = await _s3Client.GetObjectAsync(bucketName, fileName);
            MemoryStream objectStream = new MemoryStream();
            using(Stream stream = response.ResponseStream)
            {
                await stream.CopyToAsync(objectStream);
            }
            objectStream.Position = 0;
            return File(objectStream,response.Headers.ContentType,fileName);
        }

        [HttpDelete("{bucketName}/delete/{fileName}")]
        public async Task<IActionResult> Delete(string bucketName, string fileName)
        {
           DeleteObjectResponse response = await _s3Client.DeleteObjectAsync(bucketName, fileName);
            return StatusCode((int)response.HttpStatusCode);
        }

        private async Task<IEnumerable<string>> Upload(string bucketName, IEnumerable<IFormFile> files)
        {
            List<string> urls = new List<string>();
            //TransferUtility transferUtility = new TransferUtility(_s3Client);
            foreach (var file in files)
            {
                #region 1st way
                //await transferUtility.UploadAsync(new TransferUtilityUploadRequest
                //{
                //    BucketName = bucketName,
                //    InputStream = file.OpenReadStream(),
                //    Key = file.FileName,
                //    CannedACL = S3CannedACL.PublicRead
                //});
                #endregion

                #region 2nd way
                await _s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    InputStream = file.OpenReadStream(),
                    Key = file.FileName,
                    CannedACL = S3CannedACL.PublicRead
                });
                #endregion

                string url = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = file.FileName,
                    Expires = DateTime.Now.AddDays(1)
                });
                urls.Add(url);
            }
            return urls;
        }
    }
}

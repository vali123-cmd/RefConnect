using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace RefConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly string _bucketName = "refconnect-profile-images";

        public FilesController(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
        }
    
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
          
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var bucketName = _configuration["AWS:BucketName"] ?? _bucketName;
            
           
            var key = $"uploads/{Guid.NewGuid()}_{file.FileName}";

            
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };

            try
            {
               
                await _s3Client.PutObjectAsync(request);

                
                var url = $"https://{bucketName}.s3.amazonaws.com/{key}";

                return Ok(new { Url = url });
            }
            catch (AmazonS3Exception e)
            {
                return StatusCode(500, $"S3 Error: {e.Message}");
            }
        }

        // Upload post media (image/video) and set a Canned ACL (PublicRead) on the object
        [HttpPost("upload-post-media")]
        public async Task<IActionResult> UploadPostMedia(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Basic validation: allow images and videos only
            var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
            if (!contentType.StartsWith("image/") && !contentType.StartsWith("video/"))
                return BadRequest("Only image and video files are allowed.");

            var bucketName = _configuration["AWS:BucketName"] ?? _bucketName;
            var key = $"posts/{Guid.NewGuid()}_{file.FileName}";

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };

            try
            {
                await _s3Client.PutObjectAsync(request);
                var url = $"https://{bucketName}.s3.amazonaws.com/{key}";
                return Ok(new { Url = url, Key = key });
            }
            catch (AmazonS3Exception e)
            {
                return StatusCode(500, $"S3 Error: {e.Message}");
            }
        }

        [HttpGet("presigned-url")]
        [Authorize]
        public ActionResult<string> GetPresignedUrl([FromQuery] string fileName, [FromQuery] string contentType)
        {
            var bucketName = _configuration["AWS:BucketName"] ?? _bucketName;

            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = fileName,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Verb = HttpVerb.PUT,
                ContentType = contentType,
                
            };

            var url = _s3Client.GetPreSignedURL(request);
            return Ok(url);
        }
    }
}
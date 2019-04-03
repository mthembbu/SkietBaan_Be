using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Amazon.DocSamples.S3
{
    public class S3Client
    {
        private const string bucketName = "skietbaan";
        private const string keyName = "g7YPxGaEyElufQuzqc+hBkrC1AuRSyAfaNwSTYRN";
        // Specify your bucket region (an example region is shown).
        private  readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest1;
        private  IAmazonS3 client;
        public S3Client()
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("eu-west-1")
            };

            client = new AmazonS3Client("AKIAZIUDJ5YNDAUBPAOY ", "g7YPxGaEyElufQuzqc+hBkrC1AuRSyAfaNwSTYRN", config);

        }

        public async Task<string> GetObjectString(string key)
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("eu-west-1")
            };

            client = new AmazonS3Client("AKIAZIUDJ5YNDAUBPAOY ", "g7YPxGaEyElufQuzqc+hBkrC1AuRSyAfaNwSTYRN", config);

            string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                {
                    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    string contentType = response.Headers["Content-Type"];
                    Console.WriteLine("Object metadata, Title: {0}", title);
                    Console.WriteLine("Content type: {0}", contentType);

                    responseBody = Convert.ToBase64String(ReadStream(responseStream));
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return responseBody;
        }
        public async Task SaveObjectAsync(Stream streamToReadFrom,string key, string contentType = null)
        {

            try
            {
                await client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    InputStream = streamToReadFrom,
                    AutoCloseStream = false,
                    AutoResetStreamPosition = false,
                    ContentType = contentType ?? "application/octet-stream"
                });
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
        public byte[] ReadStream(Stream responseStream)
        {
            var buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;

                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
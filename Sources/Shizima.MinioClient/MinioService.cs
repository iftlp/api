using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Shizima.ClientAPI.MinioClient
{
    public class MinioService
    {
        private readonly IMinioClient _minioClient;

        public MinioService(IConfiguration configuration)
        {
            var endpoint = configuration["Minio:Endpoint"];
            var accessKey = configuration["Minio:AccessKey"];
            var secretKey = configuration["Minio:SecretKey"];
            var useSSL = bool.Parse(configuration["Minio:UseSSL"] ?? "true");

            _minioClient = new Minio.MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey);

            if (useSSL)
            {
                _minioClient = _minioClient.WithSSL();
            }

            _minioClient = _minioClient.Build();
        }

        public async Task UploadFileAsync(string bucketName, string objectName, IFormFile file)
        {
            try
            {
                var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (!exists)
                {
                    await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                }

                using var stream = file.OpenReadStream();

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType ?? "audio/wav");

                await _minioClient.PutObjectAsync(putObjectArgs);
            }
            catch (MinioException ex)
            {
                throw new ApplicationException("Minio upload failed: " + ex.Message, ex);
            }
        }
    }
}
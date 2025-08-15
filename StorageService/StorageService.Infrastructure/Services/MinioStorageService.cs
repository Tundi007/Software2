using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using StorageService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Infrastructure.Services
{
    public class MinioStorageService : ICloudStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IUserStorageRepository _userStorageRepository;
        private string GetUserBucket(string userStorageId) => $"userStorage-{userStorageId}".ToLowerInvariant();


        public MinioStorageService(IConfiguration config, IUserStorageRepository userStorageRepository)
        {
            var s3Config = new AmazonS3Config
            {
                ServiceURL = config["MinIO:ServiceURL"],
                ForcePathStyle = true
            };

            _s3Client = new AmazonS3Client(
                config["MinIO:AccessKey"],
                config["MinIO:SecretKey"],
                s3Config
            );
            _userStorageRepository = userStorageRepository;
        }

        public async Task EnsureBucketExistsAsync(string userStorageId)
        {
            var response = await _s3Client.ListBucketsAsync();
            if (!response.Buckets.Any(b => b.BucketName == GetUserBucket(userStorageId)))
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = GetUserBucket(userStorageId),
                    UseClientRegion = true
                });
            }
        }

        public async Task<Uri> UploadAsync(IFormFile file, string container, string userStorageId, string? folder)
        {
            var userstprage = await _userStorageRepository.FindUserStorage(int.Parse(userStorageId));
            if (userstprage == null)
            {
                return null;
            }
            int x = userstprage.StorageType.Capacity;
            if(x == 0)
            {
                throw new InvalidOperationException($"sth wnt wrong");
            }
            long maxUserQuota = (long)x * 1024 * 1024 * 1024;
            if (maxUserQuota <= 0)
            {
                throw new InvalidOperationException("kossher");
            }
            var currentUsage = await GetBucketSizeAsync(userStorageId);

            if (currentUsage + file.Length > maxUserQuota)
                throw new InvalidOperationException($"User quota exceeded. Max {x}GB per user.");

            var bucket = GetUserBucket(userStorageId);
            await EnsureUserBucketExistsAsync(userStorageId);

            var key = string.IsNullOrWhiteSpace(folder)
                ? file.FileName
                : $"{folder.TrimEnd('/')}/{file.FileName}";

            //if (folder == "")
            //{
            //    key = file.FileName;
            //}
            //else
            //{
            //    key = $"{folder.TrimEnd('/')}/{file.FileName}";
            //}

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                InputStream = file.OpenReadStream()
            };

            await _s3Client.PutObjectAsync(request);

            return new Uri($"{_s3Client.Config.ServiceURL}/{bucket}/{key}");
        }



        public async Task<Stream> DownloadAsync(string fileName, string container, string userStorageId)
        {
            await EnsureUserBucketExistsAsync(userStorageId);
            var key = $"{fileName}";
            var response = await _s3Client.GetObjectAsync(GetUserBucket(userStorageId), key);
            return response.ResponseStream;
        }

        public async Task<bool> DeleteAsync(string fileName, string container, string userStorageId)
        {
            await EnsureUserBucketExistsAsync(userStorageId);
            var key = $"{fileName}";
            var response = await _s3Client.DeleteObjectAsync(GetUserBucket(userStorageId), key);
            return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
        }

        //public async Task<IEnumerable<string>> ListAsync(string? container, string userStorageId)
        //{
        //    await EnsureUserBucketExistsAsync(userStorageId);
        //    var bucket = GetUserBucket(userStorageId);

        //    var request = new ListObjectsV2Request
        //    {
        //        BucketName = bucket
        //    };

        //    var files = new List<string>();
        //    ListObjectsV2Response response;

        //    do
        //    {
        //        response = await _s3Client.ListObjectsV2Async(request);
        //        files.AddRange(response.S3Objects.Select(obj => obj.Key));
        //        request.ContinuationToken = response.NextContinuationToken;
        //    } while (response.IsTruncated);

        //    return files;
        //}

        public async Task<(IEnumerable<string> Folders, IEnumerable<string> Files)> ListAsync(string? container, string userStorageId)
        {
            await EnsureUserBucketExistsAsync(userStorageId);
            var bucket = GetUserBucket(userStorageId);

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = string.IsNullOrEmpty(container) ? "" : container.TrimEnd('/') + "/", // Folder scope
                Delimiter = "/" // Prevent recursion
            };

            var files = new List<string>();
            var folders = new List<string>();

            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                // Folders (CommonPrefixes gives subfolder "names")
                folders.AddRange(response.CommonPrefixes);

                // Files (skip folder placeholders)
                files.AddRange(response.S3Objects
                    .Where(obj => !obj.Key.EndsWith("/"))
                    .Select(obj => obj.Key));

                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);

            return (folders, files);
        }




        public async Task<long> GetBucketSizeAsync(string userStorageId = null)
        {
            await EnsureUserBucketExistsAsync(userStorageId);

            var bucket = GetUserBucket(userStorageId);

            var request = new ListObjectsV2Request
            {
                BucketName = bucket
            };

            long totalSize = 0;
            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                long a = response.S3Objects.Sum(obj => obj.Size);
                totalSize += a;
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return totalSize;
        }

        public async Task EnsureUserBucketExistsAsync(string userStorageId)
        {
            var bucketName = $"userStorage-{userStorageId}".ToLowerInvariant();
            var buckets = await _s3Client.ListBucketsAsync();

            if (!buckets.Buckets.Any(b => b.BucketName == bucketName))
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                });
            }
        }

        public async Task CreateFolderAsync(string folderName, string userStorageId)
        {
            var bucket = GetUserBucket(userStorageId);
            await EnsureUserBucketExistsAsync(userStorageId);

            // Ensure folder name ends with slash
            if (!folderName.EndsWith("/"))
                folderName += "/";

            // Upload empty object to simulate folder
            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = folderName,
                InputStream = new MemoryStream(new byte[0]),
                ContentType = "application/x-directory"
            };

            await _s3Client.PutObjectAsync(request);
        }

        public async Task<IEnumerable<string>> ListFoldersAsync(string userStorageId)
        {
            var bucket = GetUserBucket(userStorageId);
            await EnsureUserBucketExistsAsync(userStorageId);

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Delimiter = "/", // this groups by folders
                Prefix = ""
            };

            var folders = new List<string>();
            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                folders.AddRange(response.CommonPrefixes);
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return folders;
        }

        public async Task MoveFileAsync(string userStorageId, string oldPath, string newPath)
        {
            var bucket = GetUserBucket(userStorageId);
            await EnsureUserBucketExistsAsync(userStorageId);

            // Copy old to new
            var copyRequest = new CopyObjectRequest
            {
                SourceBucket = bucket,
                SourceKey = oldPath,
                DestinationBucket = bucket,
                DestinationKey = newPath
            };
            await _s3Client.CopyObjectAsync(copyRequest);

            // Delete old
            await _s3Client.DeleteObjectAsync(bucket, oldPath);
        }

        public async Task MoveFolderAsync(string userStorageId, string oldFolder, string newFolder)
        {
            var bucket = GetUserBucket(userStorageId);
            await EnsureUserBucketExistsAsync(userStorageId);

            // Ensure both end with "/"
            oldFolder = oldFolder.TrimEnd('/') + "/";
            newFolder = newFolder.TrimEnd('/') + "/";

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = oldFolder
            };

            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                foreach (var obj in response.S3Objects)
                {
                    var oldKey = obj.Key;
                    var newKey = newFolder + oldKey.Substring(oldFolder.Length);

                    // Copy to new key
                    await _s3Client.CopyObjectAsync(new CopyObjectRequest
                    {
                        SourceBucket = bucket,
                        SourceKey = oldKey,
                        DestinationBucket = bucket,
                        DestinationKey = newKey
                    });

                    // Delete old key
                    await _s3Client.DeleteObjectAsync(bucket, oldKey);
                }

                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);
        }

        public async Task DeleteFolderAsync(string userStorageId, string folderPath)
        {
            var bucket = GetUserBucket(userStorageId);
            await EnsureUserBucketExistsAsync(userStorageId);

            folderPath = folderPath.TrimEnd('/') + "/";

            var request = new ListObjectsV2Request
            {
                BucketName = bucket,
                Prefix = folderPath
            };

            ListObjectsV2Response response;

            do
            {
                response = await _s3Client.ListObjectsV2Async(request);

                foreach (var obj in response.S3Objects)
                {
                    await _s3Client.DeleteObjectAsync(bucket, obj.Key);
                }

                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);
        }

        public async Task DeleteBucket(string userStorageId)
        {
            await EnsureUserBucketExistsAsync(userStorageId);

            try
            {

                var bucket = GetUserBucket(userStorageId);

                Console.WriteLine($"Deleting bucket: {bucket}");

                // 2. List all objects in the bucket (including versions if applicable)
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = bucket
                };

                ListObjectsV2Response listObjectsResponse;
                do
                {
                    listObjectsResponse = await _s3Client.ListObjectsV2Async(listObjectsRequest);

                    foreach (var obj in listObjectsResponse.S3Objects)
                    {
                        Console.WriteLine($"  Removing object: {obj.Key}");
                        await _s3Client.DeleteObjectAsync(bucket, obj.Key);
                    }

                    listObjectsRequest.ContinuationToken = listObjectsResponse.NextContinuationToken;
                }
                while (listObjectsResponse.IsTruncated);

                // 3. Finally delete the bucket
                await _s3Client.DeleteBucketAsync(bucket);


                Console.WriteLine("All buckets deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred: {e.Message}");
            }
        }

        public string Generate(int length)
        {
            const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");

            var result = new StringBuilder(length);
            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[sizeof(uint)];

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    result.Append(_chars[(int)(num % (uint)_chars.Length)]);
                }
            }

            return result.ToString();
        }

        public async Task<string> MakePublicDownloadLink(string userStorageId, string file)
        {
            string fileName = Generate(10) + "." + file.Split('.').Last();
            var res =  await MoveAsync(GetUserBucket(userStorageId), file, Generate(10), fileName);

            return res;
        }

        public async Task<string> MoveAsync(string sourceBucket, string sourceKey, string destBucket, string destKey, CancellationToken ct = default)
        {
            await EnsureUserBucketExistsAsync(destBucket);
            string destBucketId = destBucket;
            destBucket = GetUserBucket(destBucket);
            // 1) Copy
            var copyReq = new CopyObjectRequest
            {
                SourceBucket = sourceBucket,
                SourceKey = sourceKey,
                DestinationBucket = destBucket,
                DestinationKey = destKey,
                // If you need ACLs on MinIO, set it explicitly; many setups default to private:
                CannedACL = S3CannedACL.Private
            };

            var copyResp = await _s3Client.CopyObjectAsync(copyReq, ct).ConfigureAwait(false);

            // Optional: quick integrity check via ETag by re-fetching the object metadata.
            // (MinIO returns ETag as MD5 for non-multipart uploads.)
            var headDest = await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = destBucket,
                Key = destKey
            }, ct).ConfigureAwait(false);

            if (string.IsNullOrEmpty(headDest.ETag))
                throw new InvalidOperationException("Destination copy did not return an ETag; aborting delete of source.");


            return $"/{destBucketId}/{destKey}";
        }
    }

}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Interfaces
{
    public interface ICloudStorageService
    {
        Task<Stream> DownloadAsync(string fileName, string container, string userStorageId);
        Task<bool> DeleteAsync(string fileName, string container, string userStorageId);
        Task<(IEnumerable<string> Folders, IEnumerable<string> Files)> ListAsync(string? container, string userStorageId);
        Task EnsureBucketExistsAsync(string userStorageId);
        Task<long> GetBucketSizeAsync(string userStorageId = null);
        Task EnsureUserBucketExistsAsync(string userStorageId);
        Task CreateFolderAsync(string folderName, string userStorageId);
        Task<IEnumerable<string>> ListFoldersAsync(string userStorageId);

        Task<Uri> UploadAsync(IFormFile file, string container, string userStorageId, string folder = null);
        Task MoveFileAsync(string userStorageId, string oldPath, string newPath);

        Task MoveFolderAsync(string userStorageId, string oldFolder, string newFolder);

        Task DeleteFolderAsync(string userStorageId, string folderPath);
        Task DeleteBucket(string userStorageId);
        Task<string> MakePublicDownloadLink(string userStorageId, string file);

    }

}

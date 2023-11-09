using Entities.DTOs.Flile;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IFileService
    {
        Task<string> UploadFile(IFormFile file);
        Task<string> UploadFile(IFormFile file, string path);
        Task<List<string>> UploadFiles(List<UploadFileDTO> fileDtos);
        Task<bool> DeleteFile(string filePath);
        Task<bool> DeleteFiles(List<string> filePaths);
    }
}

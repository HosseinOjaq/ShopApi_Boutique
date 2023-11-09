using Common;
using Data.Contracts;
using Entities.DTOs.Flile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class FileService : IFileService, IScopedDependency
    {
        private readonly IHostingEnvironment env;

        public FileService(IHostingEnvironment env)
        {
            this.env = env;
        }

        public Task<bool> DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.FromResult(false);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DeleteFiles(List<string> filePaths)
        {
            if (!filePaths.Any())
                return Task.FromResult(false);

            foreach (var filePath in filePaths)
            {
                DeleteFile(filePath);
            }
            return Task.FromResult(true);
        }

        public async Task<string> UploadFile(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
            var uploadFilePath = Path.Combine(env.ContentRootPath, "wwwroot", "Uploads", "Others");
            using (var stream = new FileStream(uploadFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return await Task.FromResult(fileName);
        }

        public async Task<string> UploadFile(IFormFile file, string path)
        {
            if (file == null || file.Length <= 0)
                new BadImageFormatException("image is not valid ...");

            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
            using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }

        public async Task<List<string>> UploadFiles(List<UploadFileDTO> fileDtos)
        {
            var fileNames = new ConcurrentBag<string>();
            foreach (var fileDto in fileDtos)
            {
                var fileName = await UploadFile(fileDto.File, fileDto.Path);
                fileNames.Add(fileName);
            }
            return fileNames.ToList();
        }
    }
}

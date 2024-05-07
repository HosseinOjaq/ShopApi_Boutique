using Microsoft.AspNetCore.Http;

namespace Entities.DTOs.Flile
{
    public class UploadFileDTO
    {
        public IFormFile File { get; set; }
        public string Path { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Flile
{
    public class UploadFileDTO
    {
        public IFormFile File { get; set; }
        public string Path { get; set; }
    }
}

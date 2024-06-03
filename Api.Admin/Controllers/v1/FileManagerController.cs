using System.IO;
using Data.Contracts;
using WebFramework.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Entities.Attributes;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
/*    [AllowAnonymous]*/
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [ApiController]
    [Authorize]
    [Title("مدیریت فایل ها")]
    public class FileManagerController : Controller
    {
        private readonly IFileService fileService;
        private readonly IWebHostEnvironment env;

        public FileManagerController(IFileService fileService, IWebHostEnvironment env)
        {
            this.fileService = fileService;
            this.env = env;
        }
        [Title("حذف عکس محصول")]
        [HttpPost(nameof(DeleteProductFile))]
        public async Task<IActionResult> DeleteProductFile([FromForm] string id)
        {
            var path = Path.Combine(env.ContentRootPath, "wwwroot", "Uploads", "Products", id);
            var result = await fileService.DeleteFile(path);
            return Ok(result);
        }
    }
}

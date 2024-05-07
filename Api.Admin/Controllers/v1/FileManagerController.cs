using Data.Contracts;
using Entities.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using WebFramework.Filters;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class FileManagerController : Controller
    {
        private readonly IFileService fileService;
        private readonly IWebHostEnvironment env;

        public FileManagerController(IFileService fileService, IWebHostEnvironment env)
        {
            this.fileService = fileService;
            this.env = env;
        }

        [Title("مدیریت گروه های کاربران")]
        [Icon("fas fa-users")]
        [HttpPost]
        public async Task<IActionResult> DeleteProductFile([FromForm] string id)
        {
            var path = Path.Combine(env.ContentRootPath, "wwwroot", "Uploads", "Products", id);
            var result = await fileService.DeleteFile(path);
            return Ok(result);
        }
    }
}

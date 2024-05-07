using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using System.Threading;
using WebFramework.Filters;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Entities.Attributes;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [ApiController]
    [IgnorePermissionCheck]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        [Title("مدیریت گروه های کاربران")]
        [Icon("fas fa-users")]
        [HttpGet]
        public async Task<ApiResult<List<Category>>> GetAll(CancellationToken cancellationToken)
        {
            var Result = await categoryRepository.TableNoTracking.ToListAsync(cancellationToken);
            return Result;
        }
    }
}

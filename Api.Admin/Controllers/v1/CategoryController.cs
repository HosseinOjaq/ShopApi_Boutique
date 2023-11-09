using Data.Contracts;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ApiResult<List<Category>>> GetAll(CancellationToken cancellationToken)
        {
            var Result = await categoryRepository.TableNoTracking.ToListAsync(cancellationToken);
            return Result;
        }
    }
}

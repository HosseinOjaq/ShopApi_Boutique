using Entities;
using Data.Contracts;
using WebFramework.Api;
using System.Threading;
using Entities.Attributes;
using WebFramework.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet(nameof(GetAll))]
        public async Task<ApiResult<List<Category>>> GetAll(CancellationToken cancellationToken)
        {
            var Result = await categoryRepository.TableNoTracking.ToListAsync(cancellationToken);
            return Result;
        }

        [HttpPost(nameof(Adds))]
        public async Task<ApiResult<List<Category>>> Adds(Category category, CancellationToken cancellationToken)
        {
            await categoryRepository.Entities.AddAsync(category, cancellationToken);
            return Ok();
        }

        [HttpPost(nameof(Deletes))]
        public async Task<ApiResult<List<Category>>> Deletes(int categoryId, CancellationToken cancellationToken)
        {
            var category = await categoryRepository.Entities.FindAsync(categoryId, cancellationToken);
            categoryRepository.Entities.Remove(category);
            return Ok();
        }

        [HttpPost(nameof(Updates))]
        public async Task<ApiResult<List<Category>>> Updates(CancellationToken cancellationToken)
        {
            var Result = await categoryRepository.TableNoTracking.ToListAsync(cancellationToken);
            return Result;
        }
    }
}

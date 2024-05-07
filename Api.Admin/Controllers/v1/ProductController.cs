using System.Linq;
using Data.Contracts;
using System.Threading;
using WebFramework.Api;
using Data.Repositories;
using WebFramework.Filters;
using Entities.DTOs.Product;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Entities.Attributes;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    [AllowAnonymous]
    [Route("api/Admin/[controller]")]
    [ApiResultFilter]
    [ApiController]
    [EnableCors("AllowCors")]
    public class ProductController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IFileService fileService;
        private readonly IProductLikesRepository productLikesRepository;

        public ProductController(IProductRepository productRepository, IFileService fileService, IProductLikesRepository productLikesRepository)
        {
            this.productRepository = productRepository;
            this.fileService = fileService;
            this.productLikesRepository = productLikesRepository;
        }
        
        [EnableCors("AllowCors")]
        [Title("مدیریت گروه های کاربران")]
        [Icon("fas fa-users")]
        [HttpPost(nameof(AddProduct))]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDTO model, int minindex, CancellationToken cancellationToken)
        {
            if (!model.Files.Any())
                return BadRequest();
            await productRepository.AddProductAsync(model, minindex, cancellationToken);
            return Ok();
        }
        [Title("مدیریت گروه های کاربران")]
        [Icon("fas fa-users")]
        [HttpGet(nameof(GetProducts))]
        public async Task<ApiResult<PaginatedList<ListProductDto>>> GetProducts(CancellationToken cancellationToken, int pageNumber = 1)
        {
            return await productRepository.GetProductsAsync(pageNumber, cancellationToken);
        }
        [Title("مدیریت گروه های کاربران")]
        [Icon("fas fa-users")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var Result = await productRepository.DeleteFilesBayIdProductAsync(id, cancellationToken);
            return Ok(Result);
        }

        //[HttpPut(nameof(Update))]
        //public async Task<ApiResult<Product>> Update([FromForm] CreateProductDTO model, CancellationToken cancellationToken)
        //{
        //    var Result = await productRepository.UpdeteProductAsync(model, cancellationToken);
        //    if (Result is null)
        //    {
        //        return BadRequest();
        //    }
        //    return Ok(Result);
        //}

        [HttpPost(nameof(CategoryProductGetBayId))]
        public async Task<ApiResult<PaginatedList<ProductCategoryById>>> CategoryProductGetBayId(int CategoryId, int? pageNumber, CancellationToken cancellationToken)
        {
            var result = await productRepository.GetProductsByCategoryIdAsync(CategoryId, pageNumber, cancellationToken);
            return result;
        }

        [HttpPost(nameof(Filter))]
        public async Task<ApiResult<PaginatedList<FilterProductDto>>> Filter([FromForm] string filter, int? pageNumber, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(filter))
                throw new System.Exception("جیزی تایپ کنید");

            var result = await productRepository.FilterProductAsync(filter, pageNumber, cancellationToken);
            if (result == null)
            {
                return NotFound("محصولی یافت نشد .");
            }
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFileBayId(int id, CancellationToken cancellationToken)
        {
            var result = await productRepository.DeleteProductFileBayIdAsync(id, cancellationToken);
            if (!result)
            {
                return BadRequest("داده مورد نظر شما پاک نشد");
            }
            return Ok(result);
        }
    }
}
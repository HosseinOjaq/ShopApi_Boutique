using System.Linq;
using Data.Contracts;
using System.Threading;
using WebFramework.Api;
using Data.Repositories;
using Entities.Attributes;
using WebFramework.Filters;
using Entities.DTOs.Product;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Entities;

namespace Api.Admin.Controllers.v1
{
    [ApiVersion("1")]
    /*[AllowAnonymous]*/
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
        [Icon("fas fa-users")]
        [HttpPost(nameof(AddProduct))]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDTO model, int minindex, CancellationToken cancellationToken)
        {
            if (!model.Files.Any())
                return BadRequest();
            await productRepository.AddProductAsync(model, minindex, cancellationToken);
            return Ok();
        }
        [HttpGet(nameof(GetProducts))]
        public async Task<ApiResult<PaginatedList<ListProductDto>>> GetProducts(CancellationToken cancellationToken, int pageNumber = 1)
        {
            return await productRepository.GetProductsAsync(pageNumber, cancellationToken);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var Result = await productRepository.DeleteFilesBayIdProductAsync(id, cancellationToken);
            return Ok(Result);
        }

        [HttpGet(nameof(DeleteFileBayId))]
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
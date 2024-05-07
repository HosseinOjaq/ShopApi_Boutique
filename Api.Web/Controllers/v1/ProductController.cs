using Common;
using Data.Contracts;
using System.Threading;
using WebFramework.Api;
using WebFramework.Filters;
using Entities.DTOs.Product;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ApiResultFilter]
    [EnableCors("AllowCors")]
    public class ProductController : Controller
    {
        private readonly IFileService fileService;
        private readonly IProductRepository productRepository;
        private readonly IProductLikesRepository productLikesRepository;

        public ProductController(IProductRepository productRepository, IFileService fileService, IProductLikesRepository productLikesRepository)
        {
            this.productRepository = productRepository;
            this.fileService = fileService;
            this.productLikesRepository = productLikesRepository;
        }

        [HttpGet(nameof(GetProductDetailById))]
        public async Task<ApiResult<CreateProductInfoDTO>> GetProductDetailById(int id, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.Identity.GetUserId<int>();
            var result = await productRepository.GetProductDetailByIdAsync(id, userId, cancellationToken);
            return result;
        }
    }
}
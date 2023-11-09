using Common;
using Entities.DTOs.Flile;
using Entities.DTOs.Product;
using Entities;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Entities.DTOs.ProductFile;

namespace Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository, IScopedDependency
    {
        private readonly IHostingEnvironment env;
        private readonly IFileService fileService;
        private readonly IProductTagsRepository productTagsRepository;
        private readonly IProductFileRepository productFileRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRatingRepository productRatingRepository;
        private readonly IProductLikesRepository productLikesRepository;

        public ProductRepository(ApplicationDbContext dbContext, IHostingEnvironment env
            , IFileService fileService, IProductTagsRepository productTagsRepository,
                IProductFileRepository productFileRepository, ICategoryRepository categoryRepository,
                IProductRatingRepository productRatingRepository, IProductLikesRepository productLikesRepository) : base(dbContext)
        {
            this.env = env;
            this.fileService = fileService;
            this.productTagsRepository = productTagsRepository;
            this.productFileRepository = productFileRepository;
            this.categoryRepository = categoryRepository;
            this.productRatingRepository = productRatingRepository;
            this.productLikesRepository = productLikesRepository;
        }
        public async Task AddProductAsync(CreateProductDTO model, int mainPictureIndex, CancellationToken cancellationToken)
        {
            var productSaveFilePath = Path.Combine(env.ContentRootPath, "wwwroot", "Uploads", "Products");
            var uploadFileDto = model.Files.Select(x => new UploadFileDTO { File = x, Path = productSaveFilePath }).ToList();
            var fileUploadedFiles = await fileService.UploadFiles(uploadFileDto);
            var productFils = fileUploadedFiles.Select(x => new ProductFile { FileName = x, }).ToList();
            if (productFils is not null && productFils.Any())
            {
                productFils.ElementAt(mainPictureIndex).MainPicture = true;
            }
            var product = new Product
            {
                Title = model.product.Title,
                Discount = model.product.Discount,
                Price = model.product.Price,
                Count = model.product.Count,
                Description = model.product.Description,
                CategoryId = model.product.CategoryId,
                ShippingTime = model.product.ShippingTime,
                ProductFiles = productFils,
            };
            await base.AddAsync(product, cancellationToken);
            await productTagsRepository.AddProductTags(model.product.Tags, product.Id, cancellationToken);

        }
        public async Task<CreateProductInfoDTO> GetProductDetailByIdAsync(int productId, int userId, CancellationToken cancellationToken)
        {
            var product = await TableNoTracking.Where(p => p.Id == productId)
                                .Include(x => x.ProductFiles).SingleOrDefaultAsync(cancellationToken: cancellationToken);
            var result = new CreateProductInfoDTO
            {
                ProductId = product.Id,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Count = product.Count,
                Description = product.Description,
                Discount = product.Discount,
                Price = product.Price,
                ShippingTime = product.ShippingTime,
                Title = product.Title,
                Liked = userId != 0 && await productLikesRepository.CheckLike(productId, userId, cancellationToken),
                ProductFiles = product.ProductFiles.Select(x => new ProductFileDto { Id = x.Id, ImagePath = GetProductFileFullPath(x.FileName, env.WebRootPath) }).ToList(),
                Rating = await productRatingRepository.CalculateRating(product.Id, cancellationToken)
            };
            return result;
        }
        public async Task<PaginatedList<ListProductDto>> GetProductsAsync(int pageNumber, CancellationToken cancellationToken)
        {
            var pageSize = 15;
            var products = TableNoTracking.Include(p => p.ProductFiles).Select(p => new ListProductDto
            {
                CategoryId = p.CategoryId,
                ProductId = p.Id,
                CategoryName = p.Category.Name,
                Title = p.Title,
                FileNames = p.ProductFiles.Select(a => GetProductFileFullPath(a.FileName, env.ContentRootPath)).SingleOrDefault(),
            });
            return await PaginatedList<ListProductDto>.CreateAsync(products, pageNumber, pageSize);
        }
        public async Task<bool> DeleteFilesBayIdProductAsync(int ProductId, CancellationToken cancellationToken)
        {
            var product = GetById(ProductId);
            var productFiles = productFileRepository.TableNoTracking.Where(p => p.ProductId == ProductId).ToList();
            foreach (var productFile in productFiles)
            {
                var pathfile = Path.Combine(env.ContentRootPath, "wwwroot", "Uploads", "Products", productFile.FileName);
                await productFileRepository.DeleteAsync(productFile, cancellationToken, false);
                await fileService.DeleteFile(pathfile);
            }
            await DeleteAsync(product, cancellationToken);
            return true;
        }
        public async Task<Product> UpdeteProductAsync(CreateProductDTO model, CancellationToken cancellationToken)
        {
            var product = GetById(model.product.ProductId);
            product.Title = model.product.Title;
            product.Discount = model.product.Discount;
            product.Price = model.product.Price;
            product.Count = model.product.Count;
            product.Description = model.product.Description;
            product.CategoryId = model.product.CategoryId;
            product.ShippingTime = model.product.ShippingTime;
            var productSaveFilePath = Path.Combine(env.ContentRootPath, "wwwroot", "Uploads", "Products");
            if (model.Files != null)
            {
                var GetfileProduct = productFileRepository.Table.Where(p => p.ProductId == model.product.ProductId).ToList();
                foreach (var item in GetfileProduct)
                {
                    var pathfile = Path.Combine(productSaveFilePath, item.FileName);
                    var result = await fileService.DeleteFile(pathfile);
                }
                await productFileRepository.DeleteRangeAsync(GetfileProduct, cancellationToken);
                var uploadFileDto = model.Files.Select(x => new UploadFileDTO { File = x, Path = productSaveFilePath }).ToList();
                var fileUploadedFiles = await fileService.UploadFiles(uploadFileDto);
                var productFils = fileUploadedFiles.Select(x => new ProductFile { FileName = x, ProductId = model.product.ProductId }).ToList();
                product.ProductFiles = productFils;
            }
            await base.UpdateAsync(product, cancellationToken);
            return product;
        }
        public async Task<PaginatedList<ProductCategoryById>> GetProductsByCategoryIdAsync(int catgoryId, int? pageNumber, CancellationToken cancellationToken)
        {
            var pageSize = 15;
            var products = TableNoTracking.Where(p => p.CategoryId == catgoryId).Include(x => x.ProductFiles).Select(p => new ProductCategoryById
            {
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Description = p.Description,
                Count = p.Count,
                Discount = p.Discount,
                Price = p.Price,
                ProductId = p.Id,
                ShippingTime = p.ShippingTime,
                Title = p.Title,
                ProductFiles = p.ProductFiles.Select(x => GetProductFileFullPath(x.FileName, env.ContentRootPath)).ToList(),
            });
            return await PaginatedList<ProductCategoryById>.CreateAsync(products, pageNumber ?? 1, pageSize);
        }
        private static string GetProductFileFullPath(string fileName, string contentRootPath)
            => Path.Combine(contentRootPath, "wwwroot", "Uploads", "Products", fileName);
        public async Task<PaginatedList<FilterProductDto>> FilterProductAsync(string filter, int? pageNumber, CancellationToken cancellationToken)
        {
            var pageSize = 15;
            var products = TableNoTracking.Include(x => x.ProductFiles).Select(p => new FilterProductDto
            {
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Discount = p.Discount,
                ProductId = p.Id,
                Description = p.Description,
                ProductFiles = p.ProductFiles.Select(a => GetProductFileFullPath(a.FileName, env.ContentRootPath)).ToList(),
                Count = p.Count,
                Price = p.Price,
                ShippingTime = p.ShippingTime,
                Title = p.Title,
            });
            if (!string.IsNullOrEmpty(filter))
            {
                products = products.Where(a => a.Title.Contains(filter, System.StringComparison.OrdinalIgnoreCase));
            }
            return await PaginatedList<FilterProductDto>.CreateAsync(products, pageNumber ?? 1, pageSize);
        }
        public async Task<bool> DeleteProductFileBayIdAsync(int fileId, CancellationToken cancellationToken)
        {
            var ProductFile = await productFileRepository.Table.Where(x => x.Id == fileId).SingleOrDefaultAsync(cancellationToken);
            if (ProductFile != null)
                return false;

            var filepath = GetProductFileFullPath(ProductFile.FileName, env.ContentRootPath);
            var result = await fileService.DeleteFile(filepath);

            if (result == false)
                return false;

            return true;
        }
    }
}
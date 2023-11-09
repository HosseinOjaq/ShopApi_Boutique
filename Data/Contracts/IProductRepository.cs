using Data.Repositories;
using Entities.DTOs.Product;
using Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Data.Contracts
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<CreateProductInfoDTO> GetProductDetailByIdAsync(int productId, int userId, CancellationToken cancellationToken);
        Task AddProductAsync(CreateProductDTO model, int Index, CancellationToken cancellationToken);
        Task<PaginatedList<ListProductDto>> GetProductsAsync(int pageNumber, CancellationToken cancellationToken);
        Task<bool> DeleteFilesBayIdProductAsync(int ProductId, CancellationToken cancellationToken);
        Task<Product> UpdeteProductAsync(CreateProductDTO model, CancellationToken cancellationToken);
        Task<PaginatedList<ProductCategoryById>> GetProductsByCategoryIdAsync(int catgoryId, int? pageNumber, CancellationToken cancellationToken);
        Task<PaginatedList<FilterProductDto>> FilterProductAsync(string filter, int? pageNumber, CancellationToken cancellationToken);
        Task<bool> DeleteProductFileBayIdAsync(int fileId, CancellationToken cancellationToken);
    }
}
using Common;
using Data.Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductTagsRepository : Repository<ProductTags>, IProductTagsRepository, IScopedDependency
    {
        private readonly ITagsRepository tagsRepository;

        public ProductTagsRepository(ApplicationDbContext dbContext, ITagsRepository tagsRepository) : base(dbContext)
        {
            this.tagsRepository = tagsRepository;
        }

        public async Task AddProductTags(List<string> tagItems, int productId, CancellationToken cancellationToken)
        {

            var tags = tagItems.Select(x => new Tags { Title = x });
            foreach (var tag in tags)
            {
                await tagsRepository.AddAsync(tag, cancellationToken);
                var producttag = new ProductTags
                {
                    TagId = tag.Id,
                    ProductId = productId
                };
                await AddAsync(producttag, cancellationToken);
            }
        }
        public async Task DeliteProductTag(int productId, int tagId, CancellationToken cancellationToken)
        {
            var productTag = await Table.SingleOrDefaultAsync(x => x.ProductId == productId && x.TagId == tagId, cancellationToken);
            await DeleteAsync(productTag, cancellationToken);
        }
    }
}

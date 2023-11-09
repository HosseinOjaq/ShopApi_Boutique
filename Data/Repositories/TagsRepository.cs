using Common;
using Data.Contracts;
using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class TagsRepository : Repository<Tags>, ITagsRepository, IScopedDependency
    {
        public TagsRepository(ApplicationDbContext dbContext): base(dbContext) { }

        public Task<List<string>> CheckDuplicateTags(List<string> tags, CancellationToken cancellationToken)
        {
            var duplicateTags = new List<string>();
            foreach (var tag in tags)
            {
                if (Table.Any(t => t.Title == tag))
                {
                    duplicateTags.Add(tag);
                }
            }
            return Task.FromResult(duplicateTags);
        }
    }
}

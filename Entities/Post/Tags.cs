using System.Collections.Generic;

namespace Entities
{
    public class Tags : BaseEntity
    {
        public string Title { get; set; }


        public ICollection<ProductTags> productTags { get; set; }
    }
}

using System.Collections.Generic;

namespace Entities.DTOs.Product
{
    public class ListProductDto
    {
        public string Title { get; set; }
        public string FileNames { get; set; }
        public string CategoryName { get; set; }    
        public int ProductId { get; set; }    
        public int CategoryId { get; set; }    
    }
}

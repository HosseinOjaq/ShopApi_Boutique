using System.Collections.Generic;

namespace Entities.DTOs.Product
{
    public class OrderProduct
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get => Price * Count; }
        public string ProductFiles { get; set; }
    }
}

using Entities.DTOs.Product;
using System;
using System.Collections.Generic;

namespace Entities.DTOs.Order
{
    public class UserOrderDto
    {
        public int OrderId { get; set; }
        public decimal OrderSum { get; set; }
        public DateTime OrderCreatedDate { get; set; }
        public bool IsFinally { get; set; }


        public List<OrderProduct> Products { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.Product
{
    public class FilterProductDto
    {
        public int ProductId { get; set; }
        public string Title { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public string ShippingTime { get; set; }

        [Range(0, 100, ErrorMessage = "discount is not valid. because must between 0 to 100")]
        public int Discount { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<string> ProductFiles { get; set; }
    }
}

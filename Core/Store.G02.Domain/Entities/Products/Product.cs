﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Entities.Products
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int BrandId { get; set; } // FK to ProductBrand
        public ProductBrand Brand { get; set; }

        public int TypeId { get; set; } // FK to ProductType
        public ProductType Type { get; set; }
    }
}

﻿using Store.G02.Domain.Entities;
using Store.G02.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Specifications.Products
{
    public class ProductsWithBrandAndTypeSpecifications : BaseSpecifications<int, Product>
    {
        public ProductsWithBrandAndTypeSpecifications(int id) : base(P => P.Id == id)
        {
            ApplyInclude();
        }

        public ProductsWithBrandAndTypeSpecifications(int? brandId, int? typeId, string? sort, string? search) : base
            (
                P =>
                (!brandId.HasValue || P.BrandId == brandId)
                &&
                (!typeId.HasValue  || P.TypeId == typeId)
                &&
                (string.IsNullOrEmpty(search) ||P.Name.ToLower().Contains(search.ToLower()))
                
            )
        {
            // priceasc
            // pricedesc
            // nameasc
            if (!string.IsNullOrEmpty(sort))
            {
                // Check Value
                switch (sort.ToLower())
                {
                    case "priceasc":
                        // OrderBy = P => P.Price;
                        AddOrderBy(P => P.Price);
                        break;
                    case "pricedesc":
                        // OrderByDescending = P => P.Price;
                        AddOrderByDescending(P => P.Price);
                        break;
                    default:
                        // OrderBy = P => P.Name;
                        AddOrderBy(P => P.Name);
                        break;
                        
                }
            }
            else
            {
                //OrderBy = P => P.Name
                AddOrderBy(P => P.Name);
            }

            ApplyInclude();
        }

        private void ApplyInclude()
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Type);
        }
    }
}

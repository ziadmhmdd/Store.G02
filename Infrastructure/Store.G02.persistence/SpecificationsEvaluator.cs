﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.persistence
{
    public static class SpecificationsEvaluator
    {
        //  _context.Products.Include(P => P.Brand).Include(P => P.Type).Where(P => P.Id == key as int?).FirstOrDefaultAsync() as TEntity;
         
        // Generate Dynamic Query
        public static IQueryable<TEntity> GetQuery<TKey, TEntity>(IQueryable<TEntity> inputQuery ,ISpecifications<TKey, TEntity> spec) where TEntity : BaseEntity<TKey>
        {
            var query = inputQuery; // _context.Products
            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria); // _context.Products.Where(P => P.id == 12);
            }

            // _context.Products.Where(P => P.id == 12).Include(P => P.Brand);
            // _context.Products.Where(P => P.id == 12).Include(P => P.Brand).Include(P => P.Type);
            query = spec.Includes.Aggregate(query, (query, IncludeExpression) => query.Include(IncludeExpression));

            return query;
        }
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace Store.G02.Shared.Dtos.Products
{
    public class BasketItemDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }

        [Range(0,double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1,99)]
        public int Quantity { get; set; }
    }
}
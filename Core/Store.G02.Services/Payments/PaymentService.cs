using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.G02.Domain.Contracts;
using Store.G02.Domain.Entities.Orders;
using Store.G02.Domain.Entities.Products;
using Store.G02.Domain.Exceptions;
using Store.G02.Shared.Dtos.Products;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Store.G02.Domain.Entities.Products.Product;

namespace Store.G02.Services.Payments
{
    public class PaymentService(IBasketRepository _basketRepository, IUnitOfWork _unitOfWork, IConfiguration configuration, IMapper _mapper) : IPaymentService
    {
        public async Task<BasketDto> CreatePaymentIntentAsync(string basketId)
        { 
            // Calculate Amount = SubTotal + Delivery Method Cost

            // get basket by id
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null) throw new BasketNotFoundException(basketId);

            // Check Product and its price
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.GetRepository<int, Product>().GetAsync(item.Id);
                if (product is null) throw new ProductNotFoundExceptions(item.Id);

                item.Price = product.Price;
            }

            // calculate subTotal
            var subTotal = basket.Items.Sum(I => I.Price * I.Quantity);

            // Get Delivery Method By Id

            if (!basket.DeliveryMethodId.HasValue) throw new DeliveryMethodNotFoundException(-1);

            var deliveryMethod = await _unitOfWork.GetRepository<int, DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
            if (deliveryMethod is null) throw new DeliveryMethodNotFoundException(basket.DeliveryMethodId.Value);

            basket.ShipingCost = deliveryMethod.Price;

            var amount = subTotal + deliveryMethod.Price;

            // Send Amount To Stripe

            StripeConfiguration.ApiKey = configuration["StripeOptions:secretKey"];

            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (basket.PaymentintentId is null)
            {
                // Create
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)amount * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card"}
                };

                paymentIntent = await paymentIntentService.CreateAsync(options);
            }
            else
            {
                // Update
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)amount * 100,
                };

                paymentIntent = await paymentIntentService.UpdateAsync(basket.PaymentintentId,options);
            }

            basket.PaymentintentId = paymentIntent.Id;
            basket.ClientSecret = paymentIntent.ClientSecret;

            basket = await _basketRepository.UpdateBasketAsync(basket, TimeSpan.FromDays(1));
            return _mapper.Map<BasketDto>(basket);
        }
    }
}

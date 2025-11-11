using Store.G02.Services.Abstractions.Orders;
using Store.G02.Services.Abstractions.Products;
using Store.G02.Services.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Abstractions
{
    public interface IServiceManager
    {
        IProductService ProductService { get; }
        IBasketService BasketService { get; }
        ICacheService CacheService { get; }
        IAuthService AuthService { get; }
        IOrderService OrderService { get; }
        IPaymentService PaymentService { get; }
    }
}

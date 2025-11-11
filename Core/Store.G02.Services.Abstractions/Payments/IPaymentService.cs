using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Services.Payments
{
    public interface IPaymentService
    {
        Task<BasketDto> CreatePaymentIntentAsync(string basketId);

    }
}

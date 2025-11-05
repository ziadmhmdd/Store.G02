using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Entities.Orders
{
    public class Order : BaseEntity<Guid>
    {
        public string UserEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; }
    }
}

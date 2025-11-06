using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem.Shared.DTOs.PayOrders
{
    public class PayAllOrdersRequest
    {
        public int UserId { get; set; }
        public string PaymentMethod { get; set; }
    }
}

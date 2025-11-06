using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSystem.Shared.DTOs.Product
{
    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public List<OrderItemDTO> Items { get; set; }
        public decimal Total { get; set; }
        public string DeliveryLocation { get; set; }
        public string PaymentMethod { get; set; }
    }
}


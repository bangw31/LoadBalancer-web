using ECommerceSystem.Api.Data;
using ECommerceSystem.Api.Hubs;
using ECommerceSystem.Shared.Constants;
using ECommerceSystem.Shared.DTOs;
using ECommerceSystem.Shared.DTOs.Product;
using ECommerceSystem.Shared.Entities;
using ECommerceSystem.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using ECommerceSystem.Shared.DTOs.PayOrders;

namespace ECommerceSystem.Api.Controllers
{
    [Route("api/user/orders")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [AllowAnonymous] // Cho phép truy cập công khai (GET danh mục)
    public class OrdersController : ControllerBase
    {
        private readonly WebDBContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;

        // Inject DbContext và Hub để đẩy thông báo realtime
        public OrdersController(WebDBContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        /// <summary>
        /// [POST] /api/user/orders/create
        /// Tạo mới một đơn hàng từ người dùng
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            // Kiểm tra tồn kho từng sản phẩm
            foreach (var item in request.Items)
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    return BadRequest("Sản phẩm hết hàng hoặc không tồn tại");
            }

            // Tạo đơn hàng mới
            var order = new Order
            {
                UserId = request.UserId,
                Total = request.Total,
                Status = OrderStatus.Pending, // Mặc định trạng thái "Pending"
                DeliveryLocation = request.DeliveryLocation,
                CreatedAt = DateTime.UtcNow,
                OrderItems = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            // (Tùy chọn) Tích hợp thanh toán
            var paymentUrl = "https://paypal.com/pay/abc123";

            // Gửi thông báo realtime đến tất cả client qua SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", order.Id, order.Status);

            // Trả về thông tin đơn hàng vừa tạo
            return CreatedAtAction(nameof(CreateOrder), new { orderId = order.Id }, new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                Total = order.Total,
                Status = order.Status,
                DeliveryLocation = order.DeliveryLocation,
                //QrCode = order.QrCode,
                Items = order.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            });
        }

        /// <summary>
        /// [POST] /api/user/orders/pay-all
        /// Thanh toán tất cả đơn hàng "Pending" của người dùng
        /// </summary>
        [HttpPost("pay-all")]
        public async Task<IActionResult> PayAllOrders([FromBody] PayAllOrdersRequest request)
        {
            // Lấy danh sách đơn hàng đang chờ của người dùng
            var pendingOrders = await _dbContext.Orders
                .Where(o => o.UserId == request.UserId && o.Status == OrderStatus.Pending)
                .Include(o => o.OrderItems)
                .ToListAsync();

            if (!pendingOrders.Any())
                return BadRequest("Không có đơn hàng đang chờ thanh toán.");

            // Tính tổng số tiền cần thanh toán
            var total = pendingOrders.Sum(o => o.Total);

            // Tạo phiếu thanh toán mới
            var receipt = new PaymentReceipt
            {
                UserId = request.UserId,
                TotalAmount = total,
                PaymentMethod = request.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                Orders = pendingOrders
            };

            _dbContext.PaymentReceipts.Add(receipt);

            // Cập nhật trạng thái từng đơn hàng thành "Paid"
            foreach (var order in pendingOrders)
            {
                order.Status = OrderStatus.Paid;
            }

            await _dbContext.SaveChangesAsync();

            // Trả về kết quả thanh toán
            return Ok(new
            {
                Message = "Thanh toán thành công",
                ReceiptId = receipt.Id,
                Total = receipt.TotalAmount,
                OrderIds = pendingOrders.Select(o => o.Id)
            });
        }

        /// <summary>
        /// [GET] /api/user/orders/user/{userId}
        /// Lấy danh sách đơn hàng theo người dùng
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(int userId)
        {
            // Truy vấn đơn hàng của người dùng, bao gồm cả sản phẩm
            var orders = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Chuyển sang dạng DTO
            var result = orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                Total = o.Total,
                Status = o.Status,
                DeliveryLocation = o.DeliveryLocation,
                QrCode = o.QrCode,
                Items = o.OrderItems.Select(i => new OrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            }).ToList();

            return Ok(result);
        }
    }
}

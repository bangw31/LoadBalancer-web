using ECommerceSystem.Shared.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceSystem.Api.Data;

[Route("api/admin/orders")]
[ApiController]
[Authorize(Roles = "Admin")]
[AllowAnonymous] // Cho phép truy cập công khai (GET danh mục)
public class AdminOrdersController : ControllerBase
{
    private readonly WebDBContext _context;

    // Inject DbContext thông qua constructor
    public AdminOrdersController(WebDBContext context)
    {
        _context = context;
    }

    // [GET] /api/admin/orders
    // Lấy toàn bộ danh sách đơn hàng (bao gồm các mặt hàng)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems) // Gồm cả thông tin sản phẩm trong đơn hàng
            .ToListAsync();

        // Chuyển về DTO để trả về frontend
        var result = orders.Select(o => new OrderDTO
        {
            Id = o.Id,
            UserId = o.UserId,
            Total = o.Total,
            Status = o.Status,
            DeliveryLocation = o.DeliveryLocation,
            Items = o.OrderItems.Select(i => new OrderItemDTO
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        });

        return Ok(result);
    }

    // [GET] /api/admin/orders/{id}
    // Lấy chi tiết một đơn hàng cụ thể theo ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound(); // Trả 404 nếu không tìm thấy

        return Ok(new OrderDTO
        {
            Id = order.Id,
            UserId = order.UserId,
            Total = order.Total,
            Status = order.Status,
            DeliveryLocation = order.DeliveryLocation,
            Items = order.OrderItems.Select(i => new OrderItemDTO
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        });
    }

    // [POST] /api/admin/orders/{id}/update-status
    // Cập nhật trạng thái đơn hàng (Pending, Paid, Cancelled, v.v.)
    [HttpPost("{id}/update-status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound(); // Trả 404 nếu không tìm thấy

        order.Status = status; // Gán trạng thái mới
        await _context.SaveChangesAsync();

        return Ok(); // Trả về 200 OK
    }
}

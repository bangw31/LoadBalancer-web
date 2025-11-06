using ECommerceSystem.Api.Data;
using ECommerceSystem.Shared.DTOs;
using ECommerceSystem.Shared.DTOs.Product;
using ECommerceSystem.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSystem.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly WebDBContext _dbContext;

        public AdminController(WebDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // ======================= 1️⃣ API: Thống kê hệ thống =======================
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(string type, string? period = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(type))
                    return BadRequest(new { Error = "Tham số 'type' là bắt buộc." });

                type = type.ToLowerInvariant();
                period = period?.ToLowerInvariant();
                var result = new StatisticDTO();

                switch (type)
                {
                    case "revenue":
                        var query = _dbContext.Orders
                            .Where(o => o.Status != "Cancelled" && !o.IsDeleted);

                        var grouped = (period ?? "day") switch
                        {
                            "day" => query.GroupBy(o => o.CreatedAt.Date),
                            "month" => query.GroupBy(o => new DateTime(o.CreatedAt.Year, o.CreatedAt.Month, 1)),
                            "year" => query.GroupBy(o => new DateTime(o.CreatedAt.Year, 1, 1)),
                            _ => query.GroupBy(o => o.CreatedAt.Date)
                        };

                        result.Revenue = await grouped
                            .Select(g => new { Date = g.Key, Value = g.Sum(o => o.Total) } as object)
                            .ToListAsync();
                        break;

                    case "orders":
                        result.OrderCount = await _dbContext.Orders
                            .GroupBy(o => o.Status)
                            .Select(g => new { Status = g.Key, Count = g.Count() })
                            .ToDictionaryAsync(g => g.Status, g => g.Count);
                        break;

                    case "top-products":
                        var orderItems = await _dbContext.OrderItems
                            .GroupBy(oi => oi.ProductId)
                            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(oi => oi.Quantity) })
                            .OrderByDescending(g => g.Quantity)
                            .Take(5)
                            .ToListAsync();

                        var productDict = await _dbContext.Products
                            .Where(p => !p.IsDeleted)
                            .Select(p => new { p.Id, p.Name })
                            .ToDictionaryAsync(p => p.Id, p => p.Name);

                        result.TopProducts = orderItems
                            .Where(oi => productDict.ContainsKey(oi.ProductId))
                            .Select(oi => new
                            {
                                Id = oi.ProductId,
                                Name = productDict[oi.ProductId],
                                Quantity = oi.Quantity
                            } as object)
                            .ToList();
                        break;

                    default:
                        return BadRequest(new { Error = "Tham số 'type' không hợp lệ." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Detail = ex.InnerException?.Message,
                    Stack = ex.StackTrace
                });
            }
        }

        // ======================= 2️⃣ API: Kiểm tra tồn kho thấp =======================
        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventory()
        {
            try
            {
                var lowStockProducts = await _dbContext.Products
                    .Where(p => p.Stock > 0 && p.Stock <= 10 && !p.IsDeleted)
                    .Select(p => new { p.Id, p.Name, p.Stock } as object)
                    .ToListAsync();

                return Ok(new { LowStock = lowStockProducts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // ======================= 3️⃣ API: Lấy lịch sử tin nhắn người dùng =======================
        [HttpGet("chat/history")]
        public async Task<IActionResult> GetAllChatHistory()
        {
            try
            {
                var messages = await _dbContext.ChatMessages
                    .Include(m => m.FromUser)
                    .OrderByDescending(m => m.SentAt)
                    .Take(100)
                    .Select(m => new
                    {
                        m.Id,
                        SenderI = m.FromUserId,
                        SenderEmail = m.FromUser.Email,
                        m.Content,
                        m.FileUrl,
                        m.FileType,
                        m.FileName,
                        m.SentAt
                    })
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // ======================= 4️⃣ API: Gửi tin nhắn từ Admin =======================
        [HttpPost("chat/send")]
        public async Task<IActionResult> SendAdminMessage([FromBody] ChatMessage dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Content) && string.IsNullOrWhiteSpace(dto.FileUrl))
                    return BadRequest(new { Error = "Tin nhắn trống." });

                var message = new ChatMessage
                {
                    FromUserId = dto.FromUserId,
                    ToUserId = dto.ToUserId,
                    Content = dto.Content,
                    FileUrl = dto.FileUrl,
                    FileType = dto.FileType,
                    FileName = dto.FileName,
                    SentAt = DateTime.UtcNow
                };

                _dbContext.ChatMessages.Add(message);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Gửi thành công", SentAt = message.SentAt });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}

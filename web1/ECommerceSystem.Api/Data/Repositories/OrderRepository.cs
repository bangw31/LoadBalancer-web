using ECommerceSystem.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace ECommerceSystem.Api.Data.Repositories
{
    public class OrderRepository
    {
        private readonly WebDBContext _context;

        public OrderRepository(WebDBContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
        }
    }
}

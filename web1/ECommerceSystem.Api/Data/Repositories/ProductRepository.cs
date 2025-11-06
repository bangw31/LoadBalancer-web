using ECommerceSystem.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace ECommerceSystem.Api.Data.Repositories
{
    public class ProductRepository
    {
        private readonly WebDBContext _context;

        public ProductRepository(WebDBContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }


        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }
    }
}

using ECommerceSystem.Api.Data;
using ECommerceSystem.Shared.Documents; // Thêm namespace này
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;

namespace ECommerceSystem.Api.Services
{
    public class DataSyncService
    {
        private readonly WebDBContext _dbContext; 

        public DataSyncService(WebDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SyncProductsToMongo()
        {
            var products = await _dbContext.Products
                .Where(p => !p.IsDeleted)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();

            foreach (var product in products)
            {
                var productDoc = new ProductDocument
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    LastSynced = DateTime.UtcNow
                };
            }
        }
    }
}
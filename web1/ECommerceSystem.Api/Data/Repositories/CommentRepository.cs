using ECommerceSystem.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using ECommerceSystem.Api.Data;
    

namespace EcommerceSystem.API.Data.Repositories
{
    public class CommentRepository
    {
        private readonly WebDBContext _context;

        public CommentRepository(WebDBContext context)
        {
            _context = context;
        }

        // Lấy comments theo ProductId
        public async Task<List<Comment>> GetCommentsByProductIdAsync(int productId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.ProductId == productId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // Thay doi phuong thuc nay de so sanh ProductId (int) voi mot gia tri string
        // Vi du: Xunit
        public async Task<List<Comment>> GetCommentsByProductIdAsync(string productIdString)
        {
            if (!int.TryParse(productIdString, out int productId))
                return new List<Comment>(); // hoac xu ly dau vao khong hop le nhu mong muon

            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.ProductId == productId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // Lấy comment theo ID
        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Lấy tất cả comments
        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Product)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // Tìm kiếm comments theo tên Product
        public async Task<List<Comment>> GetCommentsByProductNameAsync(string productName)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.Product.Name.Contains(productName))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // Thêm comment mới
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return await _context.Comments
                .AsNoTracking()
                .Include(c => c.User)
                // Bỏ Include Product nếu response không cần
                .Include(c => c.Product)
                .FirstAsync(c => c.Id == comment.Id);
        }


        // Cập nhật comment
        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        // Xóa comment
        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        // Kiểm tra Product có tồn tại
        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.Id == productId);
        }

        // Kiểm tra User có tồn tại
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}
using ECommerceSystem.Shared.DTOs.Product;
using Refit;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Apis
{
    public interface IProductApi
    {
        [Get("/api/public/products")]
        Task<ProductListResponse> GetProductsAsync(
          [AliasAs("search")] string? search = null,
          [AliasAs("categoryId")] int? categoryId = null,
          [AliasAs("minPrice")] decimal? minPrice = null,
          [AliasAs("maxPrice")] decimal? maxPrice = null,
          [AliasAs("sortBy")] string? sortBy = null,
          [AliasAs("promotion")] bool? promotion = null,
          [AliasAs("page")] int page = 1,
          [AliasAs("pageSize")] int pageSize = 10
        );

        [Get("/api/public/products/{id}")]
        Task<ProductDTO> GetByIdAsync(int id);

        [Post("/api/public/products")]
        Task CreateAsync([Body] ProductDTO dto);

        [Put("/api/public/products/{id}")]
        Task UpdateAsync(int id, [Body] ProductDTO dto);

        [Delete("/api/public/products/{id}")]
        Task DeleteAsync(int id);

        // Gọi API rating (gọi endpoint api/user/products/{id}/rate)
        [Post("/api/user/products/{id}/rate")]
        Task RateProductAsync(int id, [Body] RatingRequest request);

        // Lấy rating của user hiện tại cho sản phẩm (yêu cầu auth)
        [Get("/api/user/products/{id}/rating")]
        Task<RatingResponse> GetUserRatingAsync(int id);
    }
}

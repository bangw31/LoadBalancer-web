namespace ECommerceSystem.Shared.DTOs.Product
{
    public class ProductListResponse
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
    }

}


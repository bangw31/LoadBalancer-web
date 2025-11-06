using ECommerceSystem.Shared.DTOs.Category;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceSystem.GUI.Apis
{
    public interface ICategoryApi
    {
        [Get("/api/public/categories")]
        Task<List<CategoryDTO>> GetAllAsync();

        [Get("/api/public/categories/get/{id}")]
        Task<CategoryDTO> GetByIdAsync(int id);

        [Post("/api/public/categories/Create")]
        Task CreateAsync([Body] CategoryDTO dto);

        [Put("/api/public/categories/edit/{id}")]
        Task UpdateAsync(int id, [Body] CategoryDTO dto);

        [Delete("/api/public/categories/delete/{id}")]
        Task DeleteAsync(int id);
        [Delete("/api/public/categories/delete-all")]
        Task DeleteAllAsync();

    }
}

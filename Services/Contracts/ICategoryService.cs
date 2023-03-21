using Entities.Models;

namespace Services.Contracts;
public interface ICategoryService {
    Task<IEnumerable<Category>> GetAllCategoriesAsync(Boolean trackChanges);
    Task<Category> GetOneCategoryByIdAsync(Guid id, Boolean trackChanges);
}
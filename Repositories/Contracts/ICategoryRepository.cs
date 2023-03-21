using Entities.Models;

namespace Repositories.Contracts;
public interface ICategoryRepository : IRepositoryBase<Category> {
    Task<IEnumerable<Category>> GetAllCategoriesAsync(Boolean trackChanges);
    Task<Category?> GetOneCategoryByIdAsync(Guid id, Boolean trackChanges);
    void CreateOneCategory(Category category);
    void UpdateOneCategory(Category category);
    void DeleteOneCategory(Category category);
}
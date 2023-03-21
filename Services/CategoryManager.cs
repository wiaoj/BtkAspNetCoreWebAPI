using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services;
public class CategoryManager : ICategoryService {
    private readonly IRepositoryManager manager;
    public CategoryManager(IRepositoryManager manager) {
        this.manager = manager;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(Boolean trackChanges) {
        return await this.manager.Category.GetAllCategoriesAsync(trackChanges);
    }

    public async Task<Category> GetOneCategoryByIdAsync(Guid id, Boolean trackChanges) {
        Category? category = await this.manager.Category.GetOneCategoryByIdAsync(id, trackChanges);

        return category is null ? throw new CategoryNotFoundException(id) : category;
    }
}
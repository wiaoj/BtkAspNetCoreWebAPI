using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore;
public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository {
    public CategoryRepository(RepositoryContext context) : base(context) { }

    public void CreateOneCategory(Category category) {
        this.Create(category);
    }

    public void DeleteOneCategory(Category category) {
        this.Delete(category);
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(Boolean trackChanges) {
        return await this.FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category?> GetOneCategoryByIdAsync(Guid id, Boolean trackChanges) {
        return await this.FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
    }

    public void UpdateOneCategory(Category category) {
        this.Update(category);
    }
}
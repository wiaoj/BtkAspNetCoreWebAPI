using Repositories.Contracts;

namespace Repositories.EFCore;
public class RepositoryManager : IRepositoryManager {
    private readonly RepositoryContext context;

    public RepositoryManager(RepositoryContext context,
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository) {
        this.context = context;
        this.Book = bookRepository;
        this.Category = categoryRepository;
    }

    public IBookRepository Book { get; }

    public ICategoryRepository Category { get; }

    public async Task SaveAsync() {
        await this.context.SaveChangesAsync();
    }
}
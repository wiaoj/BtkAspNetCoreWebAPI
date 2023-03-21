using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;

namespace Repositories.EFCore;
public sealed class BookRepository : RepositoryBase<Book>, IBookRepository {
    public BookRepository(RepositoryContext context) : base(context) { }

    public void CreateOneBook(Book book) {
        this.Create(book);
    }

    public void DeleteOneBook(Book book) {
        this.Delete(book);
    }

    public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, Boolean trackChanges) {
        List<Book> books = await this.FindAll(trackChanges)
                              .FilterBooks(bookParameters.MinPrice, bookParameters.MaxPrice)
                              .Search(bookParameters.SearchTerm)
                              .Sort(bookParameters.OrderBy)
                              .ToListAsync();

        return PagedList<Book>.ToPagedList(books, bookParameters.PageNumber, bookParameters.PageSize);
    }

    public async Task<List<Book>> GetAllBooksAsync(Boolean trackChanges) {
        return await this.FindAll(trackChanges).OrderBy(b => b.Id).ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(Boolean trackChanges) {
        return await this.context.Books.Include(b => b.Category).OrderBy(b => b.Id).ToListAsync();
    }

    public async Task<Book?> GetOneBookByIdAsync(Guid id, Boolean trackChanges) {
        return await this.FindByCondition(b => b.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
    }

    public void UpdateOneBook(Book book) {
        this.Update(book);
    }
}

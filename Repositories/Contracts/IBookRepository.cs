using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts;
public interface IBookRepository : IRepositoryBase<Book> {
    Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters, Boolean trackChanges);
    Task<List<Book>> GetAllBooksAsync(Boolean trackChanges);
    Task<Book?> GetOneBookByIdAsync(Guid id, Boolean trackChanges);
    void CreateOneBook(Book book);
    void UpdateOneBook(Book book);
    void DeleteOneBook(Book book);

    Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(Boolean trackChanges);
}
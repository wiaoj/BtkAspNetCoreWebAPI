using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;

namespace Services.Contracts;
public interface IBookService {
    Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksAsync(LinkParameters linkParameters, Boolean trackChanges);
    Task<BookDto> GetOneBookByIdAsync(Guid id, Boolean trackChanges);
    Task<BookDto> CreateOneBookAsync(BookDtoForInsertion book);
    Task UpdateOneBookAsync(Guid id, BookDtoForUpdate bookDto, Boolean trackChanges);
    Task DeleteOneBookAsync(Guid id, Boolean trackChanges);

    Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> GetOneBookForPatchAsync(Guid id, Boolean trackChanges);

    Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book);
    Task<List<Book>> GetAllBooksAsync(Boolean trackChanges);

    Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(Boolean trackChanges);
}
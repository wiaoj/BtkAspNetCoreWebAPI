using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;

namespace Services;
public class BookManager : IBookService {
    private readonly ICategoryService categoryService;
    private readonly IRepositoryManager manager;
    private readonly ILoggerService logger;
    private readonly IMapper mapper;
    private readonly IBookLinks bookLinks;

    public BookManager(IRepositoryManager manager,
                       ILoggerService logger,
                       IMapper mapper,
                       IBookLinks bookLinks,
                       ICategoryService categoryService) {
        this.manager = manager;
        this.logger = logger;
        this.mapper = mapper;
        this.bookLinks = bookLinks;
        this.categoryService = categoryService;
    }

    public async Task<BookDto> CreateOneBookAsync(BookDtoForInsertion bookDto) {
        Category category = await this.categoryService.GetOneCategoryByIdAsync(bookDto.CategoryId, false);

        Book entity = this.mapper.Map<Book>(bookDto);
        this.manager.Book.CreateOneBook(entity);
        await this.manager.SaveAsync();
        return this.mapper.Map<BookDto>(entity);
    }

    public async Task DeleteOneBookAsync(Guid id, Boolean trackChanges) {
        Book entity = await this.GetOneBookByIdAndCheckExists(id, trackChanges);
        this.manager.Book.DeleteOneBook(entity);
        await this.manager.SaveAsync();
    }

    public async Task<(LinkResponse linkResponse, MetaData metaData)> GetAllBooksAsync(LinkParameters linkParameters, Boolean trackChanges) {
        if(linkParameters.BookParameters.ValidPriceRange is false)
            throw new PriceOutofRangeBadRequestException();

        PagedList<Book> booksWithMetaData = await this.manager.Book.GetAllBooksAsync(linkParameters.BookParameters, trackChanges);

        IEnumerable<BookDto> booksDto = this.mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);
        LinkResponse links = this.bookLinks.TryGenerateLinks(booksDto, linkParameters.BookParameters.Fields, linkParameters.HttpContext);

        return (linkResponse: links, metaData: booksWithMetaData.MetaData);
    }

    public async Task<List<Book>> GetAllBooksAsync(Boolean trackChanges) {
        List<Book> books = await this.manager.Book.GetAllBooksAsync(trackChanges);
        return books;
    }

    public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(Boolean trackChanges) {
        return await this.manager
            .Book
            .GetAllBooksWithDetailsAsync(trackChanges);
    }

    public async Task<BookDto> GetOneBookByIdAsync(Guid id, Boolean trackChanges) {
        Book book = await this.GetOneBookByIdAndCheckExists(id, trackChanges);
        return this.mapper.Map<BookDto>(book);
    }

    public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)>
        GetOneBookForPatchAsync(Guid id, Boolean trackChanges) {
        Book book = await this.GetOneBookByIdAndCheckExists(id, trackChanges);
        BookDtoForUpdate bookDtoForUpdate = this.mapper.Map<BookDtoForUpdate>(book);
        return (bookDtoForUpdate, book);
    }

    public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book) {
        this.mapper.Map(bookDtoForUpdate, book);
        await this.manager.SaveAsync();
    }

    public async Task UpdateOneBookAsync(Guid id, BookDtoForUpdate bookDto, Boolean trackChanges) {
        Book entity = await this.GetOneBookByIdAndCheckExists(id, trackChanges);
        entity = this.mapper.Map<Book>(bookDto);
        this.manager.Book.Update(entity);
        await this.manager.SaveAsync();
    }

    private async Task<Book> GetOneBookByIdAndCheckExists(Guid id, Boolean trackChanges) {
        // check entity 
        Book? entity = await this.manager.Book.GetOneBookByIdAsync(id, trackChanges);

        return entity is null ? throw new BookNotFoundException(id) : entity;
    }
}
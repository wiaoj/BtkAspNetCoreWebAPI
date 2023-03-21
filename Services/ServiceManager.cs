using Services.Contracts;

namespace Services;
public class ServiceManager : IServiceManager {
    public IBookService BookService { get; }
    public ICategoryService CategoryService { get; }
    public IAuthenticationService AuthenticationService { get; }

    public ServiceManager(IBookService bookService,
                          ICategoryService categoryService,
                          IAuthenticationService authencationService) {
        this.BookService = bookService;
        this.CategoryService = categoryService;
        this.AuthenticationService = authencationService;
    }
}
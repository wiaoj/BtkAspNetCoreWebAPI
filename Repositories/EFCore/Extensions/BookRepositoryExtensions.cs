using Entities.Models;
using System.Linq.Dynamic.Core;

namespace Repositories.EFCore.Extensions;
public static class BookRepositoryExtensions {
    public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, UInt32 minPrice, UInt32 maxPrice) {
        return books.Where(book => book.Price >= minPrice && book.Price <= maxPrice);
    }

    public static IQueryable<Book> Search(this IQueryable<Book> books, String? searchTerm) {
        if(String.IsNullOrWhiteSpace(searchTerm))
            return books;

        String lowerCaseTerm = searchTerm.Trim().ToLower();
        return books.Where(b => b.Title.ToLower().Contains(searchTerm));
    }

    public static IQueryable<Book> Sort(this IQueryable<Book> books, String? orderByQueryString) {
        if(String.IsNullOrWhiteSpace(orderByQueryString))
            return books.OrderBy(b => b.Id);

        String? orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderByQueryString);

        return orderQuery is null ? books.OrderBy(b => b.Id) : books.OrderBy(orderQuery);
    }
}
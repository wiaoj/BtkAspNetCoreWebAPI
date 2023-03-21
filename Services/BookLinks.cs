using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Services.Contracts;

namespace Services;
public class BookLinks : IBookLinks {
    private readonly LinkGenerator linkGenerator;
    private readonly IDataShaper<BookDto> dataShaper;

    public BookLinks(LinkGenerator linkGenerator, IDataShaper<BookDto> dataShaper) {
        this.linkGenerator = linkGenerator;
        this.dataShaper = dataShaper;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto, String fields, HttpContext httpContext) {
        List<Entity> shapedBooks = this.ShapeData(booksDto, fields);
        return this.ShouldGenerateLinks(httpContext)
            ? this.ReturnLinkedBooks(booksDto, fields, httpContext, shapedBooks)
            : this.ReturnShapedBooks(shapedBooks);
    }

    private LinkResponse ReturnLinkedBooks(IEnumerable<BookDto> booksDto,
        String fields,
        HttpContext httpContext,
        List<Entity> shapedBooks) {
        List<BookDto> bookDtoList = booksDto.ToList();

        for(Int32 index = default; index < bookDtoList.Count(); ++index) {
            List<Link> bookLinks = this.CreateForBook(httpContext, bookDtoList[index], fields);
            shapedBooks[index].Add("Links", bookLinks);
        }

        LinkCollectionWrapper<Entity> bookCollection = new(shapedBooks);
        this.CreateForBooks(httpContext, bookCollection);
        return new() {
            HasLinks = true,
            LinkedEntities = bookCollection
        };
    }

    private LinkCollectionWrapper<Entity> CreateForBooks(HttpContext httpContext, LinkCollectionWrapper<Entity> bookCollectionWrapper) {

        bookCollectionWrapper.Links.Add(new() {
            Href = $"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
            Rel = "self",
            Method = "GET"
        });

        return bookCollectionWrapper;
    }

    private List<Link> CreateForBook(HttpContext httpContext, BookDto bookDto, String fields) {
        List<Link> links = new() {
           new ($"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLowerInvariant()}/{bookDto.Id}",
                "self",
                "GET"),
           new ($"/api/{httpContext.GetRouteData().Values["controller"]}".ToLowerInvariant(), "create", "POST"),
        };
        return links;
    }

    private LinkResponse ReturnShapedBooks(List<Entity> shapedBooks) {
        return new LinkResponse() { ShapedEntities = shapedBooks };
    }

    private Boolean ShouldGenerateLinks(HttpContext httpContext) {
        MediaTypeHeaderValue? mediaType = httpContext.Items["AcceptHeaderMediaType"] as MediaTypeHeaderValue;
        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private List<Entity> ShapeData(IEnumerable<BookDto> booksDto, String fields) {
        return this.dataShaper
            .ShapeData(booksDto, fields)
            .Select(b => b.Entity)
            .ToList();
    }


}

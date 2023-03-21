using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace WebApi.Utilities.Formatters;
public class CsvOutputFormatter : TextOutputFormatter {
    public CsvOutputFormatter() {
        this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        this.SupportedEncodings.Add(Encoding.UTF8);
        this.SupportedEncodings.Add(Encoding.Unicode);
    }

    protected override Boolean CanWriteType(Type? type) {
        return (typeof(BookDto).IsAssignableFrom(type) || typeof(IEnumerable<BookDto>).IsAssignableFrom(type)) && base.CanWriteType(type);
    }
    private static void FormatCsv(StringBuilder buffer, BookDto book) {
        buffer.AppendLine($"{book.Id}, {book.Title}, {book.Price}");
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context,
        Encoding selectedEncoding) {
        HttpResponse response = context.HttpContext.Response;
        StringBuilder buffer = new();

        if(context.Object is IEnumerable<BookDto> bookDtos) {
            foreach(BookDto book in bookDtos) {
                FormatCsv(buffer, book);
            }
        } else {
            FormatCsv(buffer, context.Object as BookDto);
        }
        await response.WriteAsync(buffer.ToString());
    }
}
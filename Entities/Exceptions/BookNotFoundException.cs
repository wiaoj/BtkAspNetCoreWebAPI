using Entities.Exceptions.BaseExceptions;

namespace Entities.Exceptions;

public sealed class BookNotFoundException : NotFoundException {
    public BookNotFoundException(Guid id)
        : base($"The book with id : {id} could not found.") { }
}
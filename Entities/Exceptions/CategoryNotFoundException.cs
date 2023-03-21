using Entities.Exceptions.BaseExceptions;

namespace Entities.Exceptions;

public sealed class CategoryNotFoundException : NotFoundException {
    public CategoryNotFoundException(Guid id)
        : base($"Category with id : {id} could not found.") { }
}
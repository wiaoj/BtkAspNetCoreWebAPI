namespace Entities.Exceptions.BaseExceptions;

public abstract class NotFoundException : Exception {
    protected NotFoundException(String message) : base(message) { }
}
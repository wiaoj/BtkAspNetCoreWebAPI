namespace Entities.Exceptions.BaseExceptions;

public abstract class BadRequestException : Exception {
    protected BadRequestException(String message) : base(message) { }
}
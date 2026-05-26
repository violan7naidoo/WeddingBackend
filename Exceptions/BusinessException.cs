namespace OurBigDay.Api.Exceptions;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string message) : base(message) { }
}

public class ConflictException : BusinessException
{
    public ConflictException(string message) : base(message) { }
}

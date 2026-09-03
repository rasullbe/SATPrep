namespace SATPrep.Api.Exceptions;

public class BadRequestException : ApiException
{
    public string? Field { get; }

    public BadRequestException(string message, string? field = null)
        : base(400, message)
    {
        Field = field;
    }
}

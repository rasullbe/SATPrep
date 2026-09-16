namespace SATPrep.Api.Exceptions;

public class ConflictException : ApiException
{
    public string? Field { get; }

    public ConflictException(string message, string? field = null)
        : base(409, message)
    {
        Field = field;
    }
}
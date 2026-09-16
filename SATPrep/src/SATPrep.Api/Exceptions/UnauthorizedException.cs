namespace SATPrep.Api.Exceptions;

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message)
        : base(401, message)
    {
    }
}
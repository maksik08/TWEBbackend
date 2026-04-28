using Microsoft.AspNetCore.Http;

namespace ProjectBackend.api.Exceptions
{
    public sealed class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, StatusCodes.Status409Conflict)
        {
        }
    }
}

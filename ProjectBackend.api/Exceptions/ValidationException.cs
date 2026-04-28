using Microsoft.AspNetCore.Http;

namespace ProjectBackend.api.Exceptions
{
    public sealed class ValidationException : AppException
    {
        public ValidationException(string message)
            : base(message, StatusCodes.Status400BadRequest)
        {
        }
    }
}

using Microsoft.AspNetCore.Http;

namespace ProjectBackend.BusinessLogic.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound)
        {
        }
    }
}

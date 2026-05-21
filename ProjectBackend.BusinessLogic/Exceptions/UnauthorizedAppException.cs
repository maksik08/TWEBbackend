using Microsoft.AspNetCore.Http;

namespace ProjectBackend.BusinessLogic.Exceptions
{
    public sealed class UnauthorizedAppException : AppException
    {
        public UnauthorizedAppException(string message)
            : base(message, StatusCodes.Status401Unauthorized)
        {
        }
    }
}

using ProjectBackend.api.Exceptions;

namespace ProjectBackend.api.Services
{
    public abstract class ApplicationServiceBase
    {
        protected static T EnsureFound<T>(T? entity, string entityName, int id)
            where T : class
        {
            return entity ?? throw new NotFoundException($"{entityName} with id {id} was not found.");
        }

        protected static string NormalizeRequiredText(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException($"{fieldName} cannot be empty.");
            }

            return value.Trim();
        }

        protected static string? NormalizeOptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        protected static string NormalizeEmail(string? value, string fieldName = "Email")
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException($"{fieldName} cannot be empty.");
            }

            return value.Trim().ToLowerInvariant();
        }

        protected static void EnsureMinimumValue(decimal value, decimal minimumValue, string fieldName)
        {
            if (value < minimumValue)
            {
                throw new ValidationException($"{fieldName} must be greater than or equal to {minimumValue:0.##}.");
            }
        }
    }
}

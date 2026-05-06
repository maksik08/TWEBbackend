using ProjectBackend.api.Exceptions;

namespace ProjectBackend.api.Services
{
    public static class QueryValidationHelper
    {
        private const int MaxPageSize = 100;

        public static int NormalizePage(int page)
        {
            if (page <= 0)
            {
                throw new ValidationException("Page must be greater than zero.");
            }

            return page;
        }

        public static int NormalizePageSize(int pageSize)
        {
            if (pageSize <= 0 || pageSize > MaxPageSize)
            {
                throw new ValidationException($"PageSize must be between 1 and {MaxPageSize}.");
            }

            return pageSize;
        }

        public static string NormalizeSortBy(string? sortBy, string defaultSortBy, params string[] allowedSortFields)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return defaultSortBy;
            }

            var normalized = sortBy.Trim();
            if (!allowedSortFields.Any(field => string.Equals(field, normalized, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException($"Sorting by '{sortBy}' is not supported.");
            }

            return normalized.ToLowerInvariant();
        }

        public static bool NormalizeSortDescending(string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortDirection))
            {
                return false;
            }

            return sortDirection.Trim().Equals("desc", StringComparison.OrdinalIgnoreCase);
        }

        public static string? NormalizeSearch(string? search)
        {
            return string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        }
    }
}

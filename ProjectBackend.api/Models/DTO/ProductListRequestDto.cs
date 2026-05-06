namespace ProjectBackend.api.Models.DTO
{
    /// <summary>
    /// Query parameters for product listing.
    /// </summary>
    public class ProductListRequestDto : ListQueryRequestDto
    {
        /// <summary>
        /// Search text applied to product name, title, category, and supplier.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Filters products by category identifier.
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Filters products by supplier identifier.
        /// </summary>
        public int? SupplierId { get; set; }

        /// <summary>
        /// Minimum product price.
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// Maximum product price.
        /// </summary>
        public decimal? MaxPrice { get; set; }
    }
}

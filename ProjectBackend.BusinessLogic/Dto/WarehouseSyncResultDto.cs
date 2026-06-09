namespace ProjectBackend.BusinessLogic.Dto
{
    public class WarehouseSyncLineDto
    {
        public int ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int PreviousQuantity { get; set; }
        public int NewQuantity { get; set; }
    }

    public class WarehouseSyncResultDto
    {
        /// <summary>Whether the changes were persisted (false for a dry-run preview).</summary>
        public bool Applied { get; set; }

        public int TotalProducts { get; set; }

        /// <summary>Products skipped because they have no SKU.</summary>
        public int WithoutSku { get; set; }

        /// <summary>Products whose SKU the warehouse does not track.</summary>
        public int NotInWarehouse { get; set; }

        /// <summary>Products whose stock level changed.</summary>
        public int Updated { get; set; }

        /// <summary>Products already in sync with the warehouse.</summary>
        public int Unchanged { get; set; }

        public DateTime SyncedAt { get; set; }

        /// <summary>The lines whose quantity changed.</summary>
        public IReadOnlyCollection<WarehouseSyncLineDto> Changes { get; set; } = new List<WarehouseSyncLineDto>();
    }
}

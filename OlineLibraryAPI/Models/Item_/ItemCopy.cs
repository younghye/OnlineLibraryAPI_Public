using OlineLibraryAPI.Models.Loan_;

namespace OlineLibraryAPI.Models.Item_
{
    public enum EnumItemStatus
    {
        NONE,
        AVAILABLE,
        CHECK_OUT,
        MAINTENANCE,
        LOST,
        DAMAGE
    }

    public class ItemCopy
    {
        public ItemCopy()
        {
            this.RentDetails = [];
        }

        public long Barcode { get; set; }
        public int ItemID { get; set; }
        public decimal Price { get; set; }
        public EnumItemStatus Status { get; set; }
        public DateTime? Deleted { get; set; }

        public Item? Item { get; set; }
        public List<LoanDetail> RentDetails { get; set; }
    }
}

namespace OlineLibraryAPI.Models.Item_
{
    public class Book
    {
        public int ItemID { get; set; }
        public required string ISBN { get; set; }
        public required string Publisher { get; set; }

        public Item? Item { get; set; }
    }
}

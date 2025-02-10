namespace OlineLibraryAPI.Models.Item_
{
    public class DVD
    {
        public int ItemID { get; set; }
        public required string Duration { get; set; }
        public required string Director { get; set; }

        public Item? Item { get; set; }
    }
}

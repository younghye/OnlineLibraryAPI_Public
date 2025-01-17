namespace OlineLibraryAPI.Models.Item_
{
    public class Software
    {
        public int ItemID { get; set; }
        public required string Version { get; set; }

        public virtual Item? Item { get; set; }
    }
}

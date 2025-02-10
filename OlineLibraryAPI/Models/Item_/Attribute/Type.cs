namespace OlineLibraryAPI.Models.Item_.Attribute
{
    public class Type
    {
        public Type()
        {
            this.Items = [];
        }

        public int TypeID { get; set; }
        public required string Name { get; set; }

        public List<Item> Items { get; set; }
    }
}

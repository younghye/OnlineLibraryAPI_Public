namespace OlineLibraryAPI.Models.Item_.Attribute
{
    public class Category
    {
        public Category()
        {
            this.Items = [];
        }

        public int CategoryID { get; set; }
        public required string Name { get; set; }

        public List<Item> Items { get; set; }
    }
}

namespace OlineLibraryAPI.Models.Item_.Attribute
{
    public class Genre
    {
        public Genre()
        {
            this.Items = [];
        }

        public int GenreID { get; set; }
        public required string Name { get; set; }

        public List<Item> Items { get; set; }
    }
}

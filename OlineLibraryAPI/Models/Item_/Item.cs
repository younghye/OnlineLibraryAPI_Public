using OlineLibraryAPI.Models.Item_.Attribute;

namespace OlineLibraryAPI.Models.Item_
{
    public enum EnumItemType
    {
        NONE,
        BOOK,
        DVD,
        SOFTWARE
    }

    public class Item
    {
        public Item()
        {
            this.ItemCopies = [];
        }

        public int ItemID { get; set; }
        public int TypeID { get; set; }
        public int CategoryID { get; set; }
        public int GenreID { get; set; }
        public required string Title { get; set; }
        public System.DateTime DateOfPublication { get; set; }
        public required string Producer { get; set; }
        public DateTime? Deleted { get; set; }

        public Book? Book { get; set; }
        public DVD? DVD { get; set; }
        public Software? Software { get; set; }
        public List<ItemCopy> ItemCopies { get; set; }
        public Attribute.Type? Type { get; set; }
        public Category? Category { get; set; }
        public Genre? Genre { get; set; }
    }
}

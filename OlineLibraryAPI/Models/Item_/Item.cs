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

        public virtual Book? Book { get; set; }
        public virtual DVD? DVD { get; set; }
        public virtual Software? Software { get; set; }
        public virtual List<ItemCopy> ItemCopies { get; set; }
        public virtual Attribute.Type? Type { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Genre? Genre { get; set; }
    }
}

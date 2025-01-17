using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Item_;
namespace OlineLibraryAPI.Dao;

public interface IItemDAO : IBaseDAO
{
    Task<int> InsertItem(Item item);

    Task<ItemCopy> InsertItemCopy(ItemCopy itemCopy);

    Task<int> DeleteItem(int id);

    Task<int> DeleteItemCopy(long barcode);

    Task<int> UpdateItem(Item item);

    Task<int> UpdateItemCopy(ItemCopy itemCopy);

    Task<ItemCopy?> FindByBarcode(long barcode);

    Task<Item?> FindByItemID(int id);

    Task<List<Item>> SelectItem(long? barcode, int? typeID, string? title,
   string? producer, DateTime? publishDateFrom, DateTime? ateTo);

}
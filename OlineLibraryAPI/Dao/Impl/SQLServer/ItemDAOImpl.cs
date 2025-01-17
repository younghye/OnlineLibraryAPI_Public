using Dapper;
using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Item_;
using OlineLibraryAPI.Models.Item_.Attribute;
using System.Data;
using Type = OlineLibraryAPI.Models.Item_.Attribute.Type;
namespace OlineLibraryAPI.Dao.Impl.SQLServer;

public class ItemDAOImpl : SQLServerBaseDAO, IItemDAO
{
    private readonly string ItemSearchStr = "SELECT i.*, ic.ItemID, ic.price, ic.Status, ic.Barcode, " +
        "b.*, d.*, s.*, t.*, g.*, c.* FROM Items AS i " +
        "LEFT JOIN ItemCopy AS ic ON i.ItemID = ic.ItemID " +
        "LEFT JOIN Book AS b ON i.ItemID = b.ItemID " +
        "LEFT JOIN DVD AS d ON i.ItemID = d.ItemID " +
        "LEFT JOIN Software AS s ON i.ItemID = s.ItemID " +
        "LEFT JOIN Type AS t ON i.TypeID = t.TypeID " +
        "LEFT JOIN Genre AS g ON i.GenreID = g.GenreID " +
        "LEFT JOIN Category AS c ON i.CategoryID = c.CategoryID ";

    #region Select
    public async Task<ItemCopy?> FindByBarcode(long barcode)
    {
        using var con = GetConnection();
        string query = ItemSearchStr + "WHERE ic.Barcode = @barcode AND ic.Deleted is null";
        var result = await con.QueryAsync(query, [typeof(Item), typeof(ItemCopy), typeof(Book), typeof(DVD), typeof(Software), typeof(Type), typeof(Genre), typeof(Category)],
        objects =>
        {
            if (objects[1] is ItemCopy itemCopy)
            {
                itemCopy.Item = objects[0] as Item;
                SetItem(itemCopy.Item, objects);
                return itemCopy;
            }
            return new ItemCopy();
        },
        param: new { barcode },
        splitOn: "ItemID, ItemID, ItemID, ItemID, ItemID, TypeID, GenreID, CategoryID");

        return result.SingleOrDefault();
    }

    public async Task<Item?> FindByItemID(int id)
    {
        using var con = GetConnection();
        string query = ItemSearchStr + "WHERE i.ItemID = @id AND i.Deleted is null";
        var queryResult = await con.QueryAsync(query, [typeof(Item), typeof(ItemCopy), typeof(Book), typeof(DVD), typeof(Software), typeof(Type), typeof(Genre), typeof(Category)],
        objects =>
        {
            if (objects[0] is Item item)
            {
                if (objects[1] is ItemCopy itemCopy) item.ItemCopies.Add(itemCopy);
                SetItem(item, objects);
                return item;
            }
            return new Item() { Producer = string.Empty, Title = string.Empty }; ;

        },
        param: new { id },
        splitOn: "ItemID, ItemID, ItemID, ItemID, ItemID, TypeID, GenreID, CategoryID");

        return GroupByItems(queryResult).SingleOrDefault();
    }

    public async Task<List<Item>> SelectItem(long? barcode, int? typeID, string? title,
        string? producer, DateTime? publishDateFrom, DateTime? publishDateTo)
    {
        using var con = GetConnection();
        string storedProcedureName = "PROC_SearchItem";
        DynamicParameters parameters = new();
        parameters.Add("@barcode", barcode);
        parameters.Add("@typeID", typeID);
        parameters.Add("@title", title);
        parameters.Add("@producer", producer);
        parameters.Add("@publishDateFrom", publishDateFrom);
        parameters.Add("@publishDateTo", publishDateTo);

        var queryResult = await con.QueryAsync<Item>
            (storedProcedureName, [typeof(Item),
                typeof(ItemCopy),
                typeof(Book),
                typeof(DVD),
                typeof(Software),
                typeof(Type),
                typeof(Genre),
                typeof(Category)],
                objects =>
                {
                    if (objects[0] is Item item)
                    {
                        if (objects[1] is ItemCopy itemCopy) item.ItemCopies.Add(itemCopy);
                        SetItem(item, objects);
                        return item;
                    }
                    return new Item() { Producer = string.Empty, Title = string.Empty }; ;
                },
                parameters,
            splitOn: "ItemID, ItemID, ItemID, ItemID, ItemID, TypeID, GenreID, CategoryID",
            commandType: CommandType.StoredProcedure);

        return GroupByItems(queryResult).AsList();
    }

    private static IEnumerable<Item> GroupByItems(IEnumerable<Item> items)
    {
        return items.GroupBy(i => i.ItemID).Select(g =>
        {
            var groupedItem = g.First();

            List<ItemCopy> copies = g.Where(i => i.ItemCopies.Count > 0).Select(i => i.ItemCopies.First()).ToList();
            groupedItem.ItemCopies.Clear();
            groupedItem.ItemCopies.AddRange(copies);
            return groupedItem;
        });
    }

    #endregion

    #region Update
    public async Task<int> UpdateItem(Item i)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        string storedProcedureName = "PROC_UpdateItem";
        DynamicParameters parameters = SetItemParameters(i);
        parameters.Add("@itemID", i.ItemID);
        int result = await con.ExecuteAsync(storedProcedureName, parameters, tran, commandType: CommandType.StoredProcedure);

        tran.Commit();
        return result;
    }

    public async Task<int> UpdateItemCopy(ItemCopy ic)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        var sql = "UPDATE ItemCopy SET Status=@status, Price=@price WHERE Barcode=@barcode;";
        DynamicParameters parameters = SetItemCopyParameters(ic);
        parameters.Add("@barcode", ic.Barcode);
        int result = await con.ExecuteAsync(sql, parameters, tran);

        tran.Commit();
        return result;
    }
    #endregion

    #region Delete
    public async Task<int> DeleteItem(int id)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        string storedProcedureName = "PROC_DeleteItem";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("ItemID", id);

        int result = await con.ExecuteAsync(storedProcedureName, parameters, tran, commandType: CommandType.StoredProcedure);

        tran.Commit();
        return result;
    }

    public async Task<int> DeleteItemCopy(long barcode)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        var sql = "UPDATE ItemCopy SET Deleted = '" + DateTime.Now + "' WHERE Barcode = @barcode AND Status != '" + EnumItemStatus.CHECK_OUT + "'";
        int result = await con.ExecuteAsync(sql, new { Barcode = barcode }, tran);
        tran.Commit();
        return result;
    }
    #endregion

    #region Insert
    public async Task<int> InsertItem(Item i)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        string storedProcedureName = "PROC_InsertItem";
        DynamicParameters parameters = SetItemParameters(i);
        parameters.Add("@return", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await con.ExecuteAsync(storedProcedureName, parameters, tran, commandType: CommandType.StoredProcedure);
        int newID = parameters.Get<int>("@return");

        tran.Commit();
        return newID;
    }

    public async Task<ItemCopy> InsertItemCopy(ItemCopy ic)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        var sql = "INSERT INTO ItemCopy(ItemID,Price,Status) output inserted.* VALUES(@itemID,@price,@status);";
        DynamicParameters parameters = SetItemCopyParameters(ic);
        parameters.Add("@itemID", ic.ItemID);
        var result = await con.QuerySingleAsync<ItemCopy>(sql, parameters, tran);

        tran.Commit();
        return result;
    }
    #endregion

    #region SetParameters
    private static DynamicParameters SetItemParameters(Item i)
    {
        DynamicParameters parameters = new();
        parameters.Add("@typeID", i.TypeID);
        parameters.Add("@categoryID", i.CategoryID);
        parameters.Add("@genreID", i.GenreID);
        parameters.Add("@title", i.Title);
        parameters.Add("@dateOfPublication", i.DateOfPublication);
        parameters.Add("@producer", i.Producer);
        parameters.Add("@isbn", i.Book?.ISBN);
        parameters.Add("@publisher", i.Book?.Publisher);
        parameters.Add("@duration", i.DVD?.Duration);
        parameters.Add("@director", i.DVD?.Director);
        parameters.Add("@version", i.Software?.Version);
        return parameters;
    }

    private static DynamicParameters SetItemCopyParameters(ItemCopy ic)
    {
        DynamicParameters parameters = new();
        parameters.Add("@barcode", ic.Barcode);
        parameters.Add("@price", ic.Price);
        parameters.Add("@status", ic.Status.ToString());
        return parameters;
    }

    private static void SetItem(Item? item, object[] objects)
    {
        if (item == null) return;
        item.Book = objects[2] as Book;
        item.DVD = objects[3] as DVD;
        item.Software = objects[4] as Software;
        item.Type = objects[5] as Type;
        item.Genre = objects[6] as Genre;
        item.Category = objects[7] as Category;
    }

    #endregion
}
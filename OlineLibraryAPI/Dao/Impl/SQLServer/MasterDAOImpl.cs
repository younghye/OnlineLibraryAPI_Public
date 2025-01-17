using Dapper;
using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Item_.Attribute;
using Type = OlineLibraryAPI.Models.Item_.Attribute.Type;
namespace OlineLibraryAPI.Dao.Impl.SQLServer;
public class MasterDAOImpl : SQLServerBaseDAO, IMasterDAO
{
    public async Task<List<Category>> GetCategory()
    {
        using var con = GetConnection();
        var result = await con.QueryAsync<Category>("SELECT * FROM Category ORDER BY CategoryID");
        return result.AsList<Category>();
    }

    public async Task<List<Genre>> GetGenre()
    {
        using var con = GetConnection();
        var result = await con.QueryAsync<Genre>("SELECT * FROM Genre ORDER BY GenreID");
        return result.AsList<Genre>();
    }

    public async Task<List<Type>> GetTypes()
    {
        using var con = GetConnection();
        var result = await con.QueryAsync<Type>("SELECT * FROM Type ORDER BY TypeID");
        return result.AsList<Type>();
    }
}
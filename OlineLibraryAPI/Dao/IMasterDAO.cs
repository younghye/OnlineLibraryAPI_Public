using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Item_.Attribute;
using Type = OlineLibraryAPI.Models.Item_.Attribute.Type;
namespace OlineLibraryAPI.Dao;
public interface IMasterDAO : IBaseDAO
{
    Task<List<Category>> GetCategory();

    Task<List<Genre>> GetGenre();

    Task<List<Type>> GetTypes();

}
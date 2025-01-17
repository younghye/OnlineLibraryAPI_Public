using OlineLibraryAPI.Models;
using OlineLibraryAPI.Models.Person_;

namespace OlineLibraryAPI.Dao
{
    public interface IAuthDAO
    {
        string GenerateJWTToken(Employee employee);
    }
}

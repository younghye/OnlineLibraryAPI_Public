using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Person_;
namespace OlineLibraryAPI.Dao;

public interface IEmployeeDAO : IBaseDAO
{
    Task<int> Insert(Employee employee);

    Task<int> DeleteByID(int id);

    Task<int> Update(Employee employee);

    Task<int> UpdatePassword(int id, string password);

    Task<Employee?> FindByID(int id);

    Task<Employee?> FindLoginUser(string userName, string password);

    Task<Employee?> FindByEmail(string email);

    Task<List<Employee>> Select(string? firstName, string? lastName, string? role, string? phoneNumber);
}
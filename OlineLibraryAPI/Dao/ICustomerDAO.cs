using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Person_;

namespace OlineLibraryAPI.Dao;
public interface ICustomerDAO : IBaseDAO
{
    Task<Customer> Insert(Customer customer);

    Task<int> DeleteByID(int id);

    Task<int> Update(Customer customer);

    Task<Customer?> FindByID(int id);

    Task<Customer?> FindByLibraryCard(long libraryCardNumber);

    Task<List<Customer>> Select(long? libraryCardNumber, string? firstName, string? lastName, string? phoneNumber);
}
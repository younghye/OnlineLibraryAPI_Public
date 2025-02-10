using Dapper;
using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Person_;
using System.Data;

namespace OlineLibraryAPI.Dao.Impl.SQLServer;

public class CustomerDAOImpl : SQLServerBaseDAO, ICustomerDAO
{
    public async Task<Customer?> FindByID(int id)
    {
        using var con = GetConnection();
        var query = "SELECT * From Customer INNER JOIN Person ON CustomerID = PersonID WHERE CustomerID = @id AND Deleted is null";
        var result = await con.QueryAsync<Customer, Person, Customer>(query, (customer, person) =>
        {
            customer.Person = person;
            return customer;
        },
        splitOn: "PersonID",
        param: new { id });
        return result.SingleOrDefault();
    }

    public async Task<Customer?> FindByLibraryCard(long libraryCardNumber)
    {
        using var con = GetConnection();
        var query = "SELECT * From Customer INNER JOIN Person ON CustomerID = PersonID WHERE LibraryCardNumber = @libraryCardNumber AND Deleted is null";
        var result = await con.QueryAsync<Customer, Person, Customer>(query, (customer, person) =>
        {
            customer.Person = person;
            return customer;
        },
        splitOn: "PersonID",
        param: new { libraryCardNumber });
        return result.SingleOrDefault();
    }

    public async Task<List<Customer>> Select(long? libraryCardNumber, string? firstName, string? lastName, string? phoneNumber)
    {
        using var con = GetConnection();
        string query = "SELECT * From Customer INNER JOIN Person ON CustomerID = PersonID " +
            "WHERE(FirstName = @firstName OR @firstName IS NULL OR @firstName = '') " +
            "AND(LastName = @lastName OR @lastName IS NULL OR @lastName = '') " +
            "AND(PhoneNumber = @phoneNumber OR @phoneNumber IS NULL OR @phoneNumber = '') " +
            "AND(LibraryCardNumber = @libraryCardNumber OR @libraryCardNumber IS NULL OR @libraryCardNumber = '') " +
            "AND Deleted is null ORDER BY CustomerID";

        DynamicParameters parameters = new();
        parameters.Add("@firstName", firstName);
        parameters.Add("@lastName", lastName);
        parameters.Add("@phoneNumber", phoneNumber);
        parameters.Add("@libraryCardNumber", libraryCardNumber);

        var results = await con.QueryAsync<Customer, Person, Customer>(query, (customer, person) =>
        {
            customer.Person = person;
            return customer;
        },
        splitOn: "PersonID",
        param: parameters);
        return results.AsList<Customer>();
    }

    public async Task<int> Update(Customer c)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        var sql = "UPDATE Customer SET DateOfBirth = @dateOfBirth WHERE CustomerID = @customerID";
        DynamicParameters parameters = SetCustomerParameters(c);
        parameters.Add("@customerID", c.CustomerID);
        await con.ExecuteAsync(sql, parameters, tran);

        sql = "UPDATE Person SET FirstName = @firstName, LastName = @lastName, Address = @address, " +
           "PhoneNumber = @phoneNumber, Email = @email WHERE PersonID = @personID";
        parameters = SetPersopnParameters(c.Person);
        parameters.Add("@personID", c.CustomerID);
        int result = await con.ExecuteAsync(sql, parameters, tran);

        tran.Commit();
        return result;
    }

    public async Task<int> DeleteByID(int id)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        string storedProcedureName = "PROC_DeleteCustomer";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("customerID", id);

        int result = await con.ExecuteAsync(storedProcedureName, parameters, tran, commandType: CommandType.StoredProcedure);

        tran.Commit();
        return result;
    }

    public async Task<Customer> Insert(Customer c)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        string storedProcedureName = "PROC_InsertCustomer";
        DynamicParameters parameters = SetPersopnParameters(c.Person);
        parameters.AddDynamicParams(SetCustomerParameters(c));
        var result = await con.QuerySingleAsync<Customer>(storedProcedureName, parameters, tran, commandType: CommandType.StoredProcedure);

        tran.Commit();
        return result;
    }

    private static DynamicParameters SetPersopnParameters(Person p)
    {
        DynamicParameters parameters = new();
        parameters.Add("@firstName", p.FirstName);
        parameters.Add("@lastName", p.LastName);
        parameters.Add("@address", p.Address);
        parameters.Add("@phoneNumber", p.PhoneNumber);
        parameters.Add("@email", p.Email);
        return parameters;
    }

    private static DynamicParameters SetCustomerParameters(Customer c)
    {
        DynamicParameters parameters = new();
        parameters.Add("@dateOfBirth", c.DateOfBirth);
        return parameters;
    }
}
using Dapper;
using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Person_;
using System.Data;
namespace OlineLibraryAPI.Dao.Impl.SQLServer;

public class EmployeeDAOImpl : SQLServerBaseDAO, IEmployeeDAO
{
    public async Task<Employee?> FindLoginUser(string userName, string password)
    {
        using var con = GetConnection();
        var query = "SELECT * From Employee INNER JOIN Person ON EmployeeID = PersonID WHERE UserName =@userName " +
             "AND Password =@password";
        var result = await con.QueryAsync<Employee, Person, Employee>(query, (employee, person) =>
        {
            employee.Person = person;
            return employee;
        },
        splitOn: "PersonID",
        param: new { userName, password });

        return result.SingleOrDefault();
    }

    public async Task<Employee?> FindByID(int id)
    {
        using var con = GetConnection();
        var query = "SELECT * From Employee INNER JOIN Person ON EmployeeID = PersonID WHERE EmployeeID =@id";
        var result = await con.QueryAsync<Employee, Person, Employee>(query, (employee, person) =>
        {
            employee.Person = person;
            return employee;
        },
        splitOn: "PersonID",
        param: new { id });

        return result.SingleOrDefault();
    }

    public async Task<Employee?> FindByEmail(string email)
    {
        using var con = GetConnection();
        var query = "SELECT * FROM Employee INNER JOIN Person ON EmployeeID = PersonID WHERE Email = @email";
        var result = await con.QueryAsync<Employee, Person, Employee>(query, (employee, person) =>
        {
            employee.Person = person;
            return employee;
        },
        splitOn: "PersonID",
        param: new { email });

        return result.SingleOrDefault();
    }

    public async Task<List<Employee>> Select(string? firstName, string? lastName, string? role, string? phoneNumber)
    {
        using var con = GetConnection();
        var query = "SELECT * From Employee INNER JOIN Person ON EmployeeID = PersonID " +
            "WHERE(FirstName = @firstName OR @firstName IS NULL OR @firstName = '') " +
            "AND(LastName = @lastName OR @lastName IS NULL OR @lastName = '') " +
            "AND([Role] = @role OR @role IS NULL OR @role = '') " +
            "AND(PhoneNumber = @phoneNumber OR @phoneNumber IS NULL OR @phoneNumber = '') ORDER BY EmployeeID";

        DynamicParameters parameters = new();
        parameters.Add("@firstName", firstName);
        parameters.Add("@lastName", lastName);
        parameters.Add("@role", role);
        parameters.Add("@phoneNumber", phoneNumber);

        var results = await con.QueryAsync<Employee, Person, Employee>(query, (employee, person) =>
        {
            employee.Person = person;
            return employee;
        },
        splitOn: "PersonID",
        param: parameters);

        return results.AsList<Employee>();
    }

    public async Task<int> Update(Employee e)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        var sql = "UPDATE Employee SET UserName = @userName, Password = @password, Role = @role WHERE EmployeeID = @employeeID";
        DynamicParameters parameters = SetEmployeeParameters(e);
        parameters.Add("@employeeID", e.EmployeeID);
        await con.ExecuteAsync(sql, parameters, tran);

        sql = "UPDATE Person SET FirstName = @firstName, LastName = @lastName, Address = @address, " +
           "PhoneNumber = @phoneNumber, Email = @email WHERE PersonID = @personID";
        parameters = SetPersopnParameters(e.Person);
        parameters.Add("@personID", e.Person.PersonID);
        int result = await con.ExecuteAsync(sql, parameters, tran);

        tran.Commit();
        return result;
    }

    public async Task<int> UpdatePassword(int id, string pass)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        var sql = "UPDATE Employee SET Password = @password WHERE EmployeeID = @employeeId";
        int result = await con.ExecuteAsync(sql, new { employeeId = id, password = pass }, tran);
        tran.Commit();
        return result;
    }
    public async Task<int> DeleteByID(int id)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        var sql = "Delete Person From Person INNER JOIN Employee on PersonID = EmployeeID Where personID = @employeeId";
        int result = await con.ExecuteAsync(sql, new { employeeId = id }, tran);
        tran.Commit();
        return result;
    }

    public async Task<int> Insert(Employee e)
    {
        using var con = GetConnection();
        using var tran = con.BeginTransaction();
        string storedProcedureName = "PROC_InsertEmployee";
        DynamicParameters parameters = SetPersopnParameters(e.Person);
        parameters.AddDynamicParams(SetEmployeeParameters(e));
        parameters.Add("@newID", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await con.ExecuteAsync(storedProcedureName, parameters, tran, commandType: CommandType.StoredProcedure);
        int newID = parameters.Get<int>("@newID");
        tran.Commit();
        return newID;
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

    private static DynamicParameters SetEmployeeParameters(Employee e)
    {
        DynamicParameters parameters = new();
        parameters.Add("@userName", e.UserName);
        parameters.Add("@password", e.Password);
        parameters.Add("@role", e.Role.ToString());
        return parameters;
    }
}
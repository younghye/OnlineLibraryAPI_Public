using OlineLibraryAPI.Dao.Impl;
using OlineLibraryAPI.Dao.Impl.SQLServer;

namespace OlineLibraryAPI.Dao.Factory;
public class SQLServerDAOFactory : DAOFactory
{
    public override IMasterDAO GetMasterDao()
    {
        return (IMasterDAO)new MasterDAOImpl();
    }

    public override IEmployeeDAO GetEmployeeDao()
    {
        return (IEmployeeDAO)new EmployeeDAOImpl();
    }

    public override ICustomerDAO GetCustomerDao()
    {
        return (ICustomerDAO)new CustomerDAOImpl();
    }

    public override IItemDAO GetItemDao()
    {
        return (IItemDAO)new ItemDAOImpl();
    }

    public override ILoanDAO GetRentDao()
    {
        return (ILoanDAO)new LoanDAOImpl();
    }
    public override IAuthDAO GetTokenDao()
    {
        return (IAuthDAO)new AuthDAOImpl();
    }
}
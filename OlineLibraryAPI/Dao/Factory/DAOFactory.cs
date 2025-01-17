namespace OlineLibraryAPI.Dao.Factory;
public abstract class DAOFactory
{
    public enum DAOFactoryType
    {
        SQL_SERVER,
        ORACLE,
        XML
    }

    public abstract IMasterDAO GetMasterDao();
    public abstract IEmployeeDAO GetEmployeeDao();
    public abstract ICustomerDAO GetCustomerDao();
    public abstract IItemDAO GetItemDao();
    public abstract ILoanDAO GetRentDao();
    public abstract IAuthDAO GetTokenDao();

    public static DAOFactory GetDAOFactory(DAOFactoryType type)
    {
        switch (type)
        {
            case DAOFactoryType.SQL_SERVER:
                return new SQLServerDAOFactory();
            case DAOFactoryType.ORACLE:
                return new SQLServerDAOFactory();
            case DAOFactoryType.XML:
                return new SQLServerDAOFactory();
            default:
                return new SQLServerDAOFactory();
        }
    }
}
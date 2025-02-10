
using System.Data;
using System.Data.SqlClient;

namespace OlineLibraryAPI.Dao.Base;


public class SQLServerBaseDAO : IBaseDAO
{
    private readonly SqlConnection connection;
    private readonly string? ConnectionStr = AppSettingsJson.GetAppSettings().GetConnectionString("dbCon");

    protected SQLServerBaseDAO()
    {
        if (connection == null)
            connection = new SqlConnection(ConnectionStr);

        if (connection.State == ConnectionState.Closed || connection.State ==
                    ConnectionState.Broken)
        {
            connection.Open();
        }
    }

    protected SqlConnection GetConnection()
    {
        return connection;
    }

    protected SqlTransaction GetTransaction()
    {
        return connection.BeginTransaction(); ;
    }

    public void Commit()
    {
        GetTransaction().Commit();
    }

    public void Rollback()
    {
        GetTransaction().Rollback();
    }

    public void Close()
    {
        GetConnection().Close();
    }
}
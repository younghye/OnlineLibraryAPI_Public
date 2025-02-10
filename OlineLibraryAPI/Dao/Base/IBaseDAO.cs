namespace OlineLibraryAPI.Dao.Base;

public interface IBaseDAO
{
    void Commit();

    void Rollback();

    void Close();
}
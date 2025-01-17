namespace OlineLibraryAPI.Dao.Base;

public interface IBaseDAO
{
    void Commit();

    void Rollback();

    // void CloseCommand();

    void Close();
}
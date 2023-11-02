namespace Data.Interfaces;

public interface ITransaction
{
    void Commit();
    void Rollback();
}
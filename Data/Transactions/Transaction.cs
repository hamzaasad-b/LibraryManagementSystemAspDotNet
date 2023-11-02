using System.Data;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Transactions;

public class Transaction : ITransaction, IDisposable
{
    private readonly DbContext _dbContext;
    private IDbContextTransaction? _efTransaction;

    public Transaction(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Start(bool withLock = false)
    {
        if (withLock)
        {
            _efTransaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable);
            return;
        }

        _efTransaction = _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        if (_efTransaction is null)
            throw new ArgumentNullException($"efcore", "Transaction not started before commiting");
        _efTransaction.Commit();
    }

    public void Rollback()
    {
        if (_efTransaction is null)
            throw new ArgumentNullException($"efcore", "Transaction not started before rollback");
        _efTransaction.Rollback();
    }

    public void Dispose()
    {
        _efTransaction?.Dispose();
    }
}
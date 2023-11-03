using System.Linq.Expressions;
using Data.Dto;
using Data.Entities;
using Data.Repositories;
using Domain.Common;

namespace Domain.Services;

public class BookService : BaseService<Book>
{
    public BookService(BookRepository bookRepository) : base(bookRepository)
    {
    }

    public async Task<ServiceResult<IEnumerable<Book>>> GetAllBooks()
    {
        return ServiceResult<IEnumerable<Book>>.SuccessfulFactory(
            await BaseRepository.GetAll());
    }

    public async Task<ServiceResult<IEnumerable<Book>>> FindBooksByTerm(string? term = null)
    {
        if (term is null)
        {
            return ServiceResult<IEnumerable<Book>>.SuccessfulFactory(
                await BaseRepository.GetAll());
        }

        var parameter = Expression.Parameter(typeof(Book), "b");

        Expression<Func<Book, bool>> titleSearch = b => b.Title.Contains(term);
        Expression<Func<Book, bool>> ibanSearch = b => b.Iban.Contains(term);

        var titleSearchBody = Expression.Invoke(titleSearch, parameter);
        var ibanSearchBody = Expression.Invoke(ibanSearch, parameter);

        var combinedCondition = Expression.OrElse(titleSearchBody, ibanSearchBody);

        var filter = Expression.Lambda<Func<Book, bool>>(combinedCondition, parameter);


        return ServiceResult<IEnumerable<Book>>.SuccessfulFactory(
            await BaseRepository.GetAllWithFilters(filter));
    }


    public async Task<ServiceResult<PaginationDto<Book>>> GetBooksWithPagination(Expression<Func<Book, bool>>? filter,
        int pageNumber,
        int pageSize)
    {
        if (filter is null)
        {
            return ServiceResult<PaginationDto<Book>>.SuccessfulFactory(
                await BaseRepository.GetAllWithPagination(pageSize, pageNumber));
        }

        return ServiceResult<PaginationDto<Book>>.SuccessfulFactory(
            await BaseRepository.GetFilteredWithPagination(filter, pageSize, pageNumber));
    }

    public async Task<ServiceResult<Book>> AddBook(Book book)
    {
        return ServiceResult<Book>.SuccessfulFactory(
            await BaseRepository.Add(book));
    }

    public async Task<ServiceResult<Book?>> DeleteBook(uint bookId)
    {
        if (await BaseRepository.Delete(bookId))
        {
            return ServiceResult<Book?>.SuccessfulFactory();
        }

        return ServiceResult<Book?>.FailedFactory("Failed to Delete");
    }

    public async Task<ServiceResult<Book?>> IssueBookToUser(uint bookId, uint userId)
    {
        using var transaction = BaseRepository.GetTransaction();

        try
        {
            transaction.Start(true);
            var book = await BaseRepository.GetById(bookId);
            if (book is null)
            {
                transaction.Rollback();
                return ServiceResult<Book?>.FailedFactory("Book not found");
            }

            if (book.IssuedToUserId is not null)
            {
                transaction.Rollback();
                return ServiceResult<Book?>.FailedFactory("Book Already Issued");
            }

            book.IssuedToUserId = userId;
            await BaseRepository.Update(bookId, book);
            transaction.Commit();
            return ServiceResult<Book?>.SuccessfulFactory();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<ServiceResult<Book?>> ReturnBookFromUser(uint bookId, uint userId)
    {
        using var transaction = BaseRepository.GetTransaction();
        try
        {
            transaction.Start(true);
            var book = await BaseRepository.GetById(bookId);
            if (book is null)
            {
                transaction.Rollback();
                return ServiceResult<Book?>.FailedFactory("Book not found");
            }

            if (book.IssuedToUserId != userId)
            {
                transaction.Rollback();
                return ServiceResult<Book?>.FailedFactory("Book not assigned to User");
            }

            book.IssuedToUserId = null;
            await BaseRepository.Update(bookId, book);
            transaction.Commit();
            return ServiceResult<Book?>.SuccessfulFactory();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
    }

    async Task<ServiceResult<IEnumerable<Book>>> GetAllBooksIssuedToUser(uint userId)
    {
        Expression<Func<Book, bool>> queryPredicate = b => b.IssuedToUserId == userId;
        return ServiceResult<IEnumerable<Book>>.SuccessfulFactory(
            await BaseRepository.GetAllWithFilters(queryPredicate));
    }
}
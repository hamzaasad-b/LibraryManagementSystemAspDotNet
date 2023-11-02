using System.Linq.Expressions;
using System.Transactions;
using Data.Dto;
using Data.Entities;
using Data.Repositories;
using Domain.Common;
using Transaction = Data.Transactions.Transaction;

namespace Domain.Services;

public class BookService
{
    private readonly BookRepository _bookRepository;

    public BookService(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<ServiceResult<IEnumerable<Book>>> GetAllBooks()
    {
        return ServiceResult<IEnumerable<Book>>.SuccessfulFactory(
            await _bookRepository.GetAll());
    }

    public async Task<ServiceResult<PaginationDto<Book>>> GetBooksWithPagination(Expression<Func<Book, bool>>? filter,
        int pageNumber,
        int pageSize)
    {
        if (filter is null)
        {
            return ServiceResult<PaginationDto<Book>>.SuccessfulFactory(
                await _bookRepository.GetAllWithPagination(pageSize, pageNumber));
        }

        return ServiceResult<PaginationDto<Book>>.SuccessfulFactory(
            await _bookRepository.GetFilteredWithPagination(filter, pageSize, pageNumber));
    }

    public async Task<ServiceResult<Book>> AddBook(Book book)
    {
        return ServiceResult<Book>.SuccessfulFactory(
            await _bookRepository.Add(book));
    }

    public async Task<ServiceResult<Book?>> DeleteBook(uint bookId)
    {
        if (await _bookRepository.Delete(bookId))
        {
            return ServiceResult<Book?>.SuccessfulFactory();
        }

        return ServiceResult<Book?>.FailedFactory("Failed to Delete");
    }

    public async Task<ServiceResult<Book?>> IssueBookToUser(uint bookId, uint userId)
    {
        using var transaction = _bookRepository.GetTransaction();

        try
        {
            transaction.Start(true);
            var book = await _bookRepository.GetById(bookId);
            if (book is null)
            {
                transaction.Rollback();
                return ServiceResult<Book?>.FailedFactory("Book not found");
            }

            book.IssuedToUserId = userId;
            await _bookRepository.Update(book);
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
        using var transaction = _bookRepository.GetTransaction();
        try
        {
            transaction.Start(true);
            var book = await _bookRepository.GetById(bookId);
            if (book is null)
            {
                transaction.Rollback();
                return ServiceResult<Book?>.FailedFactory("Book not found");
            }

            if (book!.IssuedToUserId != userId)
            {
                transaction.Rollback();
                return ServiceResult<Book?>.FailedFactory("Book not assigned to User");
            }

            book.IssuedToUserId = null;
            await _bookRepository.Update(book);
            transaction.Commit();
            return ServiceResult<Book?>.SuccessfulFactory();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
    }
}
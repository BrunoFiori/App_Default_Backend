﻿
using App_Default_Backend.Data.Models;

namespace App_Default_Backend.Data.Contract
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> ListAllAsync();
        Task<PagedResult<TResult>> ListAllPagedAsync<TResult>(QueryParameters queryParameters);
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> Exists(int id);
        Task Commit();
    }    
}

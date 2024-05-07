using App_Default_Backend.Core.Models;
using App_Default_Backend.Data.Models;

namespace App_Default_Backend.Core.Contract
{
    public interface IServiceTodo
    {
        public Task<List<OutputTodo>> ListAllAsync();
        public Task<PagedResult<OutputTodo>> ListAllPagedAsync(QueryParameters queryParameters);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth01.Application.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object Id);
        Task<IEnumerable<T>> GetAllAsync();
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(object Id);
        Task SaveAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace HMS.Core
{
    public interface ICrudService<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}

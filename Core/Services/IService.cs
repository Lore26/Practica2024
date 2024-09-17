namespace Core.Services;

public interface IService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<List<int>> ExistAllAsync(int[] ids);
    Task<int> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<T>> FilterAsync(object filter);
}

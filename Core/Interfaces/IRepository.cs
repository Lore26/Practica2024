namespace Core.Interfaces;

public interface IRepository<T>
{
    Task<List<T>> GetAll();
    Task<T?> GetById(int id);
    Task<bool> Exists(int id);
    Task<List<int>> ExistAll(int[] ids);
    Task<int> Create(T entity);
    Task<bool> Update(T entity);
    Task<bool> Delete(int id);
}

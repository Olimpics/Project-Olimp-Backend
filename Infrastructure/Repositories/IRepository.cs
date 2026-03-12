public interface IRepository<T> where T : class
{
    IQueryable<T> Query();

    Task<T?> GetByIdAsync(int id);

    Task AddAsync(T entity);

    void Update(T entity);

    void Remove(T entity);

    Task SaveAsync();
}
using Hotel.Generic;

namespace Hotel.Generic
{
    public interface IGenric<T> where T : class
    {
        Task<T> Add(T obj);
        Task<T> Get(int id);
        Task<List<T>> GetAll();
        Task<T> Remove(int id);
        Task<T> Update(T entity);
    }
}
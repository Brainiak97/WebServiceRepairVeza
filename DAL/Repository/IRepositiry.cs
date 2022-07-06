using Core.Models;
using System.Linq.Expressions;

namespace DAL.Repository
{
    public interface IRepository<T>
        where T : class, IEntityBase
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> Get(int id);

        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);

        Task Create(T item);

        Task Update(T item);

        Task Delete(int id);

        IQueryable<T> GetQuery();
    }
}

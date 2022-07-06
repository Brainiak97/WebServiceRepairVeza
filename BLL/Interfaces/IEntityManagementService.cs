using BLL.Models;
using System.Linq.Expressions;

namespace BLL.Interfaces
{
    public interface IEntityManagementService<TDto>
       where TDto : class, IEntityBaseDto
    {
        Task Create(TDto item);

        Task Update(TDto item);

        Task Delete(int id);

        Task<TDto> GetItem(int id);

        Task<IEnumerable<TDto>> GetItems();

        Task<IEnumerable<TDto>> Find(Expression<Func<TDto, bool>> predicate);

    }
}

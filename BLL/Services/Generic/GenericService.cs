using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using Core.Models;
using DAL.Repository;
using FluentValidation;
using System.Linq.Expressions;

namespace BLL.Services.Generic
{
    public class GenericService<TEntity, TDto> : IEntityManagementService<TDto>
        where TDto : class, IEntityBaseDto
        where TEntity : class, IEntityBase
    {
        protected readonly IMapper _mapper;
        protected readonly IRepository<TEntity> _repository;
        protected readonly IValidator<TEntity> _validator;

        public GenericService(IMapper mapper, IRepository<TEntity> repository, IValidator<TEntity> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
        }

        public async Task Create(TDto item)
        {
            if (item != null)
            {
                var entity = _mapper.Map<TEntity>(item);
                await _validator.ValidateAndThrowAsync(entity);

                await _repository.Create(entity);

                item.Id = entity.Id;
            }
            else throw new ArgumentNullException(nameof(item));
        }

        public async Task Delete(int id)
        {
            var item = await _repository.Get(id);
            if (item == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            await _repository.Delete(id);

        }

        public async Task<IEnumerable<TDto>> Find(Expression<Func<TDto, bool>> predicate)
        {
            return _mapper.Map<IEnumerable<TDto>>(await _repository.Find(_mapper.Map<Expression<Func<TEntity, bool>>>(predicate)));
        }

        public async Task<TDto> GetItem(int id)
        {
            var item = await _repository.Get(id);
            if (item == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _mapper.Map<TDto>(item);
        }

        public async Task<IEnumerable<TDto>> GetItems()
        {
            var items = await _repository.GetAll();
            if (items == null)
            {
                throw new InvalidDataException();
            }
            return _mapper.Map<IEnumerable<TDto>>(items);
        }

        public async Task Update(TDto item)
        {
            if (item != null)
            {
                var entity = _mapper.Map<TEntity>(item);
                await _validator.ValidateAndThrowAsync(entity);
                await _repository.Update(entity);
            }
            else throw new ArgumentNullException(nameof(item), "error");
        }
    }
}

using Health.Domain.Entities;
using Health.Domain.Entities.DoctorModule;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IGenericRepository<TEntity , Tkey> where TEntity : BaseEntity<Tkey>
    {

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity , Tkey> specifications);
        Task<TEntity?> GetByIdAsync(Tkey id);
        Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, Tkey> specifications);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entitys);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task<int> CountAsync(ISpecifications<TEntity, Tkey> specifications);

        IQueryable<TEntity> GetAllQuerable();

        IQueryable<TEntity> GetAverageReview(ISpecifications<TEntity , Tkey> spec);

    }
}

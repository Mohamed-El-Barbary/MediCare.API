using Health.Domain.Contracts;
using Health.Domain.Entities;
using Health.Domain.Entities.DoctorModule;
using Health.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Persistence.Repositories
{
    internal class GenericRepository<TEntity, Tkey> : IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        private readonly HealthCareDbContext _healthCareDbContext;

        public GenericRepository(HealthCareDbContext healthCareDbContext)
        {
            _healthCareDbContext = healthCareDbContext;
        }
        public async Task AddAsync(TEntity entity)
        {
            await _healthCareDbContext.AddAsync(entity);
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entitys)
        {
            await _healthCareDbContext.AddRangeAsync(entitys);
        }

        public async Task<int> CountAsync(ISpecifications<TEntity, Tkey> specifications)
        {

            var Query = SpecificationEvaluator.CreateQuery(_healthCareDbContext.Set<TEntity>(), specifications);
            return await Query.CountAsync();
        }

        public void Delete(TEntity entity)
        {
            _healthCareDbContext.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _healthCareDbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, Tkey> specifications)
        {
            var Query = SpecificationEvaluator.CreateQuery(_healthCareDbContext.Set<TEntity>(), specifications);


            return await Query.ToListAsync();
            
        }

        public IQueryable<TEntity> GetAllQuerable()
        {
            return _healthCareDbContext.Set<TEntity>();
        }

        public  IQueryable<TEntity> GetAverageReview(ISpecifications<TEntity, Tkey> spec)
        {
            return SpecificationEvaluator.CreateQuery(_healthCareDbContext.Set<TEntity>(), spec);
          
        }

        public async Task<TEntity?> GetByIdAsync(Tkey id)
        {
            return await _healthCareDbContext.Set<TEntity>().FindAsync(id);
        }


        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, Tkey> specifications)
        {
            var Query = SpecificationEvaluator.CreateQuery(_healthCareDbContext.Set<TEntity>() , specifications);
            
            return await Query.FirstOrDefaultAsync();
        }

        

        public void Update(TEntity entity)
        {
            _healthCareDbContext.Update(entity);
        }
    }
}

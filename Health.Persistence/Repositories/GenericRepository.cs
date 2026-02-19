using Health.Domain.Contracts;
using Health.Domain.Entities;
using Health.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public void Delete(int id)
        {
            _healthCareDbContext.Remove(id);
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

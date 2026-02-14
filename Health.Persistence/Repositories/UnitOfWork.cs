using Health.Domain.Contracts;
using Health.Domain.Entities;
using Health.Persistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HealthCareDbContext _healthCareDbContext;

        private readonly Dictionary<Type, object> _repositories = [];
        public UnitOfWork(HealthCareDbContext healthCareDbContext)
        {
            _healthCareDbContext = healthCareDbContext;
        }
        public IGenericRepository<TEntity, Tkey> GetRepository<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>
        {
            var entityType = typeof(TEntity);
            if(_repositories.TryGetValue(entityType, out var repo))
            {
                return (IGenericRepository<TEntity, Tkey>)repo;
            }

            var newRepo = new GenericRepository<TEntity, Tkey>(_healthCareDbContext);

            _repositories[entityType] = newRepo;    

            return newRepo;          
        }

        public async Task<int> SaveChanges()
        {
            return await _healthCareDbContext.SaveChangesAsync();
        }
    }
}

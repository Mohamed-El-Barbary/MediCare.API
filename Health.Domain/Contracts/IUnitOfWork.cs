using Health.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChanges();

        IGenericRepository<TEntity , Tkey> GetRepository<TEntity , Tkey>() where TEntity : BaseEntity<Tkey>;
    }
}

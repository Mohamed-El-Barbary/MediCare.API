using Health.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface IGenericRepository<TEntity , Tkey> where TEntity : BaseEntity<Tkey>
    {

        Task<IEnumerable<TEntity>> GetAllAsync();


        Task<TEntity?> GetById(int id);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Delete(int id);

    }
}

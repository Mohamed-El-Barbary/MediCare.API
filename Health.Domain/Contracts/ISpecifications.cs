using Health.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Domain.Contracts
{
    public interface ISpecifications<TEntity , Tkey> where TEntity : BaseEntity<Tkey>
    {
        ICollection<Expression<Func<TEntity , object>>> IncludesExpressions { get; }
     
        Expression<Func<TEntity , bool>> Criteria {  get; }

        Expression<Func<TEntity , object>> OrderBy { get; }
        Expression<Func<TEntity , object>> OrderByDescending { get; }
        int Skip { get; }
        int Take { get; }
        bool IsPaginated { get; }
    }
}

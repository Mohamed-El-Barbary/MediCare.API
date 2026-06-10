using Health.Domain.Contracts;
using Health.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Health.Services.Specifications
{
    public abstract class BaseSpecification<TEntity, Tkey> : ISpecifications<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {

        protected BaseSpecification(Expression<Func<TEntity , bool>> criteriaExp)
        {
            Criteria = criteriaExp;
        }


        #region Filteration
        public Expression<Func<TEntity, bool>> Criteria { get; }
        #endregion


        #region Pagination
        public int Skip { private set; get; }

        public int Take { private set; get;}

        public bool IsPaginated { private set; get; }

        protected void ApplyPagination(int PageSize , int PageIndex)
        {
            IsPaginated = true;
            Skip = (PageIndex - 1) * PageSize;
            Take = PageSize;
        }

        #endregion

        #region Includes
        public ICollection<Expression<Func<TEntity, object>>> IncludesExpressions { get; } = [];
        public ICollection<string> IncludeStrings { get; } = [];


        protected void AddInclude(Expression<Func<TEntity, object>> IncludeExp)
        {
            IncludesExpressions.Add(IncludeExp);
        }
        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        #endregion


        #region Ordering
        public Expression<Func<TEntity, object>> OrderBy { private set; get; }

        public Expression<Func<TEntity, object>> OrderByDescending { private set; get; }

        protected void AddOrderBy(Expression<Func<TEntity, object>> OrderByExp)
        {
            OrderBy = OrderByExp;
        }

        protected void AddOrderByDesc(Expression<Func<TEntity, object>> OrderByDescExp)
        {
            OrderByDescending = OrderByDescExp;
        } 
        #endregion

    }
}

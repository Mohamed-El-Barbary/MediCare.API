using Health.Domain.Contracts;
using Health.Domain.Entities;
using Health.Services.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Health.Persistence
{
    internal static class SpecificationEvaluator
    {

        public static IQueryable<TEntity> CreateQuery<TEntity, Tkey>(IQueryable<TEntity> entryPoint, ISpecifications<TEntity, Tkey> specifications) where TEntity : BaseEntity<Tkey>
        {
            var Query = entryPoint;

            if (specifications is not null)
            {
                if (specifications.Criteria is not null)
                {
                    Query = Query.Where(specifications.Criteria);
                }
                if (
                    specifications.IncludesExpressions is not null
                    && specifications.IncludesExpressions.Any()
                )
                {
                    Query = specifications.IncludesExpressions
                                          .Aggregate
                                           (
                                               Query, (currentQuery, includeExp)
                                               => currentQuery.Include(includeExp)
                                           );
                }
                if (
                    specifications.IncludeStrings is not null
                    && specifications.IncludeStrings.Any()
                )
                {
                    Query = specifications.IncludeStrings
                                          .Aggregate(Query,(current, include) => current.Include(include));
                }
                
                if(specifications.OrderBy is not null)
                {
                    Query = Query.OrderBy(specifications.OrderBy);
                }

                if (specifications.OrderByDescending is not null)
                {
                    Query = Query.OrderByDescending(specifications.OrderByDescending);
                }

                if(specifications.IsPaginated == true)
                {
                    Query = Query.Skip(specifications.Skip).Take(specifications.Take);
                }

            }

            return Query;
        }
    }
}

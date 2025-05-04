using E_Government.Core.Domain.RepositoryContracts.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Generic_Repository
{
    public static class SpecificationsEvaluator
    {
        public static IQueryable<T> GetQuery<T>(
            IQueryable<T> inputQuery,
            ISpecification<T> specification) where T : class
        {
            var query = inputQuery;

            // Where clause
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Includes (eager loading)
            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            // Ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            return query;
        }
    }
}

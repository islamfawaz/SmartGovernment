using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Specification.Bills
{
    // CustomerWithMetersSpec.cs
    public class CustomerWithMetersSpec : BaseSpecification<ApplicationUser>
    {
        public CustomerWithMetersSpec(string userId)
            : base(c => c.NID == userId)
        {
            AddInclude(c => c.Meters);
        }
    }
}

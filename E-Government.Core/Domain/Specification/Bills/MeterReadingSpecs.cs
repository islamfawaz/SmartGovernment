using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Specification.Bills
{
    class MeterReadingSpecs :BaseSpecification<MeterReading>
    {
        // For single meter reading by ID
        public MeterReadingSpecs(int readingId)
            : base(r => r.Id == readingId)
        {
            AddInclude(r => r.Meter);
        }

        // For latest reading of a meter
        public MeterReadingSpecs(int meterId, bool getLatest = true)
            : base(r => r.MeterId == meterId)
        {
            if (getLatest)
            {
                AddOrderByDescending(r => r.ReadingDate);
            }
            AddInclude(r => r.Meter);
        }

        // For filtered readings
        public MeterReadingSpecs(
            int meterId,
            DateTime? fromDate,
            DateTime? toDate,
            bool? isEstimated = null)
            : base(r => r.MeterId == meterId &&
                       (!fromDate.HasValue || r.ReadingDate >= fromDate) &&
                       (!toDate.HasValue || r.ReadingDate <= toDate) &&
                       (!isEstimated.HasValue || r.IsEstimated == isEstimated))
        {
            AddOrderByDescending(r => r.ReadingDate);
            AddInclude(r => r.Meter);
        }
    }
}

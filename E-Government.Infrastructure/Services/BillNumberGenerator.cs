using E_Government.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Services
{
    class BillNumberGenerator : IBillNumberGenerator
    {
        private static int _counter = 0;

        public string Generate()
        {
            Interlocked.Increment(ref _counter);
            return $"BL-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}

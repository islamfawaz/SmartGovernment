using E_Government.Core.ServiceContracts;

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

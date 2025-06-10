using E_Government.Domain.RepositoryContracts.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Infrastructure.Generic_Repository
{
  public  class LicenseRepositoryFactory : ILicenseRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public LicenseRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public ILicenseRequestRepository GetRepository(string requestType)
        {
            return requestType.ToLower() switch
            {
                "drivinglicenserenewal" => _serviceProvider.GetRequiredService<DrivingLicenseRenewalRepository>(),
                "licensereplacementrequest" => _serviceProvider.GetRequiredService<LicenseReplacementRequestRepository>(),
                "vehiclelicenserenewal" => _serviceProvider.GetRequiredService<VehicleLicenseRenewalRepository>(),
                _ => throw new ArgumentException($"No repository found for request type: {requestType}"),
            };
        }

    }
}

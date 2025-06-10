using E_Government.Domain.Entities.Liscenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.Liscenses
{
    public static class LicenseEntityTypes
    {
        public const string DrivingLicenseRenewal = "DrivingLicenseRenewal";
        public const string LicenseReplacementRequest = "LicenseReplacementRequest";
        public const string VehicleLicenseRenewal = "VehicleLicenseRenewal";

        // Corrected to use typeof() with actual entity types from E_Government.Core.Domain.Entities
        public static readonly Dictionary<string, Type> TypeNameMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { DrivingLicenseRenewal, typeof(DrivingLicenseRenewal) },
            { LicenseReplacementRequest, typeof(LicenseReplacementRequest) },
            { VehicleLicenseRenewal, typeof(VehicleLicenseRenewal) }
            // Add other license entity types here if they are created and managed by AdminService
        };
    }
}

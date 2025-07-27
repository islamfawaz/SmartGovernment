using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Infrastructure.Infrastructure.Data;

namespace E_Government.Infrastructure.Infrastructure.Services.User
{
    public class GovernorateService : IGovernorateService
    {

        public  string GetGovernorateName(int code)
        {
            return GovernorateData.Governorates.TryGetValue(code, out string? name) ? name : "غير محدد";
        }

        public  bool IsValidGovernorateCode(int code)
        {
            return GovernorateData.Governorates.ContainsKey(code);
        }
    }

}

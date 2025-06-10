using E_Government.Application.ServiceContracts;
using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Application.Services.Admin;
using E_Government.Application.Services.Auth;
using E_Government.Application.Services.License;
using E_Government.Domain.Entities;
using E_Government.Domain.Helper;
using E_Government.Domain.Helper.Hub;
using E_Government.Domain.RepositoryContracts.Persistence;
using E_Government.Domain.ServiceContracts.Common;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.Services.Common
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAdminService> _adminService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly IHubContext<DashboardHub, IHubService> _dashboardHubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILicenseRepositoryFactory _licenseRepositoryFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly IPaymentService paymentService;
        private readonly IBillNumberGenerator billNumberGenerator;
        private readonly ILogger<BillingServices> logg;
        private readonly Lazy<IAuthService> _authService;
        private readonly Lazy<ICivilDocumentsService> _civilDocsService;
        private readonly Lazy<ILicenseService> _licenseService;
        private readonly Lazy<BillingServices> _billingServices;
        public ServiceManager(IUnitOfWork unitOfWork,IMapper mapper,ILogger<AdminService> logger, IHubContext<DashboardHub, IHubService> dashboardHubContext, UserManager<ApplicationUser> userManager, ILicenseRepositoryFactory licenseRepositoryFactory ,SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings ,IPaymentService paymentService ,IBillNumberGenerator billNumberGenerator, ILogger<BillingServices> logg)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _dashboardHubContext = dashboardHubContext;
            _userManager = userManager;
            _licenseRepositoryFactory = licenseRepositoryFactory;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
            this.paymentService = paymentService;
            this.billNumberGenerator = billNumberGenerator;
            this.logg = logg;
            _adminService = new Lazy<IAdminService>(() => new AdminService(_unitOfWork!, _mapper!, _logger!, _dashboardHubContext!, _userManager, _licenseRepositoryFactory));
            _authService = new Lazy<IAuthService>(() => new AuthService(_userManager,_signInManager,_jwtSettings));
            _civilDocsService=new Lazy<ICivilDocumentsService>(()=>new CivilDocumentsService(_unitOfWork));
            _licenseService = new Lazy<ILicenseService>(() =>new LicenseService(_unitOfWork));
            _billingServices = new Lazy<BillingServices>(() => new BillingServices(_unitOfWork, billNumberGenerator, paymentService, logg));

        }






        public IAdminService AdminService => _adminService.Value;

        public IAuthService AuthService => _authService.Value;

        public ICivilDocumentsService CivilDocsService => _civilDocsService.Value;

        public ILicenseService LicenseService =>_licenseService.Value;

        public IBillingService BillingService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

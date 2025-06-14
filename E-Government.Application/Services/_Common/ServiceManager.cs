using E_Government.Application.ServiceContracts;
using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Application.Services.Admin;
using E_Government.Application.Services.Auth;
using E_Government.Application.Services.License;
using E_Government.Application.Services.NIDValidation;
using E_Government.Application.Services.OTP;
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
        private readonly IGovernorateService governorate;
        private readonly INIDValidationService validationService;
        private readonly ILogger<OTPService> loggerOtp;
        private readonly IMailSettings mailSettings;
        private readonly Lazy<ICivilDocumentsService> _civilDocsService;
        private readonly Lazy<ILicenseService> _licenseService;
        private readonly Lazy<BillingServices> _billingServices;
        private readonly Lazy<INIDValidationService> _validationService;
        private readonly Lazy<IOTPService> _OTPService;

        public ServiceManager(IUnitOfWork unitOfWork,IMapper mapper,ILogger<AdminService> logger, IHubContext<DashboardHub, IHubService> dashboardHubContext, UserManager<ApplicationUser> userManager, ILicenseRepositoryFactory licenseRepositoryFactory ,SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings ,IPaymentService paymentService ,IBillNumberGenerator billNumberGenerator, ILogger<BillingServices> logg ,IGovernorateService governorate,INIDValidationService validationService,ILogger<OTPService> loggerOtp,IMailSettings mailSettings)
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
            this.governorate = governorate;
            this.validationService = validationService;
            this.loggerOtp = loggerOtp;
            this.mailSettings = mailSettings;
            _adminService = new Lazy<IAdminService>(() => new AdminService(_unitOfWork!, _mapper!, _logger!, _dashboardHubContext!, _userManager, _licenseRepositoryFactory));
            _civilDocsService=new Lazy<ICivilDocumentsService>(()=>new CivilDocumentsService(_unitOfWork));
            _licenseService = new Lazy<ILicenseService>(() =>new LicenseService(_unitOfWork));
            _billingServices = new Lazy<BillingServices>(() => new BillingServices(_unitOfWork, billNumberGenerator, paymentService, logg));
            _validationService = new Lazy<INIDValidationService>(() => new NIDValidationService(governorate));
            _OTPService = new Lazy<IOTPService>(() => new OTPService(_unitOfWork, _userManager,loggerOtp,mailSettings));

        }






        public IAdminService AdminService => _adminService.Value;


        public ICivilDocumentsService CivilDocsService => _civilDocsService.Value;

        public ILicenseService LicenseService =>_licenseService.Value;

        public IBillingService BillingService => _billingServices.Value;

        public INIDValidationService ValidationService => _validationService.Value;

        public IOTPService OTPService => _OTPService.Value;
    }
}

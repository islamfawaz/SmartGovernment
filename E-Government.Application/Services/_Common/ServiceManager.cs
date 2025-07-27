using E_Government.Application.ServiceContracts;
using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Application.Services.Admin;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly IHubContext<DashboardHub, IHubService> _dashboardHubContext;
        private readonly UserManager<ApplicationUser> _userManager;
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
        private readonly Lazy<BillingServices> _billingServices;
        private readonly Lazy<INIDValidationService> _validationService;
        private readonly Lazy<IOTPService> _OTPService;

        public ServiceManager(IUnitOfWork unitOfWork,IMapper mapper,ILogger<AdminService> logger, IHubContext<DashboardHub, IHubService> dashboardHubContext, UserManager<ApplicationUser> userManager ,SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings ,IPaymentService paymentService ,IBillNumberGenerator billNumberGenerator, ILogger<BillingServices> logg ,IGovernorateService governorate,INIDValidationService validationService,ILogger<OTPService> loggerOtp,IMailSettings mailSettings,IAdminService adminService,ILicenseService licenseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _dashboardHubContext = dashboardHubContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
            this.paymentService = paymentService;
            this.billNumberGenerator = billNumberGenerator;
            this.logg = logg;
            this.governorate = governorate;
            this.validationService = validationService;
            this.loggerOtp = loggerOtp;
            this.mailSettings = mailSettings;
            _civilDocsService=new Lazy<ICivilDocumentsService>(()=>new CivilDocumentsService(_unitOfWork));
            _billingServices = new Lazy<BillingServices>(() => new BillingServices(_unitOfWork, billNumberGenerator, paymentService, logg));
            _validationService = new Lazy<INIDValidationService>(() => new NIDValidationService(governorate));
            _OTPService = new Lazy<IOTPService>(() => new OTPService(_unitOfWork, _userManager,loggerOtp,mailSettings));

        }

        public ICivilDocumentsService CivilDocsService => _civilDocsService.Value;


        public IBillingService BillingService => _billingServices.Value;

        public INIDValidationService ValidationService => _validationService.Value;

        public IOTPService OTPService => _OTPService.Value;
    }
}

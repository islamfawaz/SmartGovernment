using E_Government.APIs.Controllers.Base;
using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace E_Government.APIs.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ApiControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _token;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountController> _logger;
        private readonly IAdminService _adminService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            ITokenService token,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork,
            ILogger<AccountController> logger,
            IAdminService adminService

            
            )
        {
            _userManager = userManager;
            _token = token;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _adminService = adminService;
        }

        [HttpGet("NID")]
        public ActionResult CheckNID(string NID)
        {
            var user = _unitOfWork.GetRepository<ApplicationUser, string>().GetUserByNID(NID);
            return user == null ? NotFound() : Ok();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO registerDTO)
        {
            // التحقق من وجود البريد الإلكتروني مسبقاً
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                return Conflict("البريد الإلكتروني مسجل مسبقاً");
            }

            // إنشاء كائن المستخدم
            var appUser = new ApplicationUser
            {
                UserName = registerDTO.Email, // أو إنشاء اسم مستخدم فريد
                Email = registerDTO.Email,
                NID = registerDTO.NID,
                PhoneNumber = registerDTO.PhoneNumber,
                Address = registerDTO.Address,
               

            };

            try
            {
                // إنشاء المستخدم
                var result = await _userManager.CreateAsync(appUser, registerDTO.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                // توليد التوكن
                var token = await _token.GenerateToken(appUser, _userManager);

                return Ok(new UserDTO
                {
                    Email = appUser.Email,
                    Token = token
                });
            }
            catch (DbUpdateException ex)
            {
                // تسجيل الخطأ للتحليل
                _logger.LogError(ex, "Error saving user to database");

                // التحقق من أسباب أخرى للخطأ
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                {
                    return Conflict("البريد الإلكتروني أو الرقم القومي مسجل مسبقاً");
                }

                return StatusCode(500, "حدث خطأ أثناء تسجيل المستخدم");
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] loginDTO loginDTO)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDTO.Email);
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", loginDTO.Email);
                return Unauthorized("Invalid credentials."); // Provide a generic message
            }

            _logger.LogInformation("User found: {Email}. Checking password.", loginDTO.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Password check failed for user: {Email}", loginDTO.Email);
                return Unauthorized("Invalid credentials.");
            }

            // user.EmailConfirmed = true; // This line might not be necessary here unless specifically intended for login flow.
             user.LastLoginDate = DateTime.UtcNow;
            _logger.LogInformation("Password check succeeded for user: {Email}. Generating token.", loginDTO.Email);
            try
            {
                var tokenString = await _token.GenerateToken(user, _userManager);
                _logger.LogInformation("Token generated successfully for user: {Email}", loginDTO.Email);
                return Ok(new UserDTO
                {
                    Email = user.Email,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token for user: {Email}", loginDTO.Email);
                return StatusCode(500, "An internal error occurred while processing your request.");
            }

           

        }

        [HttpGet]
        [Route("GetUser")]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var result = await _adminService.GetCurrentUser(User);
            return Ok(result);
        }
    }
}
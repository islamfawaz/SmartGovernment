using E_Government.Application.DTO.Auth;
using E_Government.Application.DTO.OTP;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResetPasswordRequest = E_Government.Application.DTO.OTP.ResetPasswordRequest;

namespace E_Government.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly INIDValidationService _validationService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger,INIDValidationService validationService)
        {
            _authService = authService;
            _logger = logger;
            _validationService = validationService;
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginDTO model)
        {
            try
            {
                var result = await _authService.LoginAsync(model);
                return Ok(new
                {
                    success = true,
                    message = "تم تسجيل الدخول بنجاح",
                    data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Direct registration (without OTP)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            try
            {
                var result = await _authService.RegisterAsync(model);
                return Ok(new
                {
                    success = true,
                    message = "تم التسجيل بنجاح",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Send OTP for registration
        /// </summary>
        [HttpPost("send-registration-otp")]
        public async Task<IActionResult> SendRegistrationOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                var result = await _authService.SendRegistrationOtpAsync(request.Email);
                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message
                    });
                }
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send registration OTP error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Register with OTP verification
        /// </summary>
        [HttpPost("register-with-otp")]
        public async Task<IActionResult> RegisterWithOtp([FromBody] RegisterWithOtpRequest request)
        {
            try
            {
                var result = await _authService.RegisterWithOtpAsync(request.RegisterData, request.OtpCode);
                return Ok(new
                {
                    success = true,
                    message = "تم التسجيل بنجاح",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register with OTP error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Send OTP for password reset
        /// </summary>
        [HttpPost("send-password-reset-otp")]
        public async Task<IActionResult> SendPasswordResetOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                var result = await _authService.SendPasswordResetOtpAsync(request.Email);
                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message
                    });
                }
                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send password reset OTP error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Reset password with OTP
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var result = await _authService.ResetPasswordWithOtpAsync(request.Email, request.OtpCode, request.NewPassword);
                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "تم إعادة تعيين كلمة المرور بنجاح"
                    });
                }
                return BadRequest(new { success = false, message = "فشل في إعادة تعيين كلمة المرور" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reset password error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get current user info (requires authentication)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var result = await _authService.GetCurrentUser(User);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        [HttpGet("email-exists")]
        public async Task<IActionResult> EmailExists([FromQuery] string email)
        {
            try
            {
                var exists = await _authService.EmailExist(email);
                return Ok(new
                {
                    success = true,
                    exists = exists
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email exists check error");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Logout (invalidate token - client side)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // In JWT, logout is typically handled on client side by removing the token
            // For server-side token invalidation, you'd need a token blacklist mechanism
            return Ok(new
            {
                success = true,
                message = "تم تسجيل الخروج بنجاح"
            });
        }

        [HttpPost("validate")]
        public IActionResult ValidateNationalId(string NID)
        {
            var result= _validationService.ValidateAndExtractNID(NID);

            return Ok(result);
        }
    }
}
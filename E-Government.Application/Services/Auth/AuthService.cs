using E_Government.Application.DTO.Auth;
using E_Government.Application.DTO.OTP;
using E_Government.Application.DTO.User;
using E_Government.Application.Exceptions;
using E_Government.Application.ServiceContracts;
using E_Government.Application.Services.Common;
using E_Government.Application.Services.NIDValidation;
using E_Government.Domain.DTO;
using E_Government.Domain.Entities;
using E_Government.Domain.Entities.OTP;
using E_Government.Domain.Helper;
using E_Government.Domain.RepositoryContracts.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace E_Government.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly INIDValidationService _validationService;
        private readonly IOTPService _oTPService;
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtOptions,
            INIDValidationService validationService,
            IOTPService otpService,
            ILogger<AuthService> logger,
            IUnitOfWork unitOfWork
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions;
            _validationService = validationService;
            _oTPService = otpService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApplicationUserDto> LoginAsync(loginDTO model)
        {
            try
            {
                // Input validation
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    throw new BadRequestException("البيانات المدخلة غير صحيحة");
                }

                if (!IsValidEmail(model.Email))
                {
                    throw new BadRequestException("صيغة البريد الإلكتروني غير صحيحة");
                }

                ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", model.Email);
                    throw new UnAuthorizedException("بيانات تسجيل الدخول غير صحيحة");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

                if (result.IsNotAllowed)
                {
                    _logger.LogWarning("Login attempt for unconfirmed account: {Email}", model.Email);
                    throw new UnauthorizedAccessException("الحساب غير مؤكد بعد");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Login attempt for locked out account: {Email}", model.Email);
                    throw new UnauthorizedAccessException("الحساب مقفل مؤقتاً");
                }

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed login attempt for: {Email}", model.Email);
                    throw new UnAuthorizedException("بيانات تسجيل الدخول غير صحيحة");
                }

                _logger.LogInformation("Successful login for user: {Email}", model.Email);

                var response = new ApplicationUserDto
                {
                    NID = user.NID!,
                    DisplayName = user.DisplayName!,
                    Email = user.Email!,
                    Token = await GenerateTokenAsync(user),
                    GovernorateName = _validationService.ExtractGovernorateInfo(user.NID),
                    DateOfBirth = _validationService.ExtractDateOfBirth(user.NID),
                    Gender = _validationService.ExtractGender(user.NID),
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    Status = _validationService.ExtractStatus(new GovernorateDto
                    {
                        Name = user.GovernorateName
                    }),
                };

                return response;
            }
            catch (Exception ex) when (!(ex is UnAuthorizedException || ex is UnauthorizedAccessException || ex is BadRequestException))
            {
                _logger.LogError(ex, "Unexpected error during login for email: {Email}", model?.Email);
                throw new Exception("حدث خطأ أثناء تسجيل الدخول");
            }
        }

        public async Task<ApplicationUserDto> RegisterAsync(RegisterDTO model)
        {
            try
            {
                // Comprehensive input validation
                ValidateRegistrationInput(model);

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    throw new BadRequestException("البريد الإلكتروني مسجل مسبقاً");
                }

                // Check if NID already exists
                var existingNIDUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.NID == model.NID);
                if (existingNIDUser != null)
                {
                    throw new BadRequestException("الرقم القومي مسجل مسبقاً");
                }

                // Validate NID
                if (!_validationService.ValidateNID(model.NID))
                {
                    throw new BadRequestException("الرقم القومي غير صحيح");
                }

                // Extract governorate information from NID
                var governorateName = _validationService.ExtractGovernorateInfo(model.NID);

                var user = new ApplicationUser
                {
                    DisplayName = model.DisplayName?.Trim(),
                    NID = model.NID,
                    Email = model.Email.ToLower().Trim(),
                    UserName = model.Email.ToLower().Trim(), // Use email as username
                    Address = model.Address?.Trim(),
                    PhoneNumber = model.PhoneNumber?.Trim(),
                    GovernorateName = _validationService.ExtractGovernorateInfo(model.NID).Name,
                    EmailConfirmed = false, // Require email confirmation
                    PhoneNumberConfirmed = false
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Registration failed for {Email}: {Errors}", model.Email, errors);
                    throw new BadRequestException($"فشل في التسجيل: {errors}");
                }

                _logger.LogInformation("User registered successfully: {Email}", model.Email);
               await  _unitOfWork.CompleteAsync();

                return new ApplicationUserDto
                {
                    NID = user.NID!,
                    DisplayName = user.DisplayName!,
                    Email = user.Email!,
                    Token = await GenerateTokenAsync(user),
                    GovernorateName = governorateName,
                    DateOfBirth = _validationService.ExtractDateOfBirth(user.NID),
                    Gender = _validationService.ExtractGender(user.NID),
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber!
                };
            }
            catch (Exception ex) when (!(ex is BadRequestException))
            {
                _logger.LogError(ex, "Unexpected error during registration for email: {Email}", model?.Email);
                throw new Exception("حدث خطأ أثناء التسجيل");
            }
        }

        public async Task<bool> EmailExist(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                if (!IsValidEmail(email))
                    return false;

                var user = await _userManager.FindByEmailAsync(email.ToLower().Trim());
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Email}", email);
                return false;
            }
        }

        private async Task<ApplicationUser> CreateUserFromTokenClaimsAsync(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirst("uid")?.Value;
            var email = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var displayName = claimsPrincipal.FindFirst("display_name")?.Value;
            var nid = claimsPrincipal.FindFirst("nid")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                throw new BadRequestException("Cannot create user: email not found in token");
            }

            Console.WriteLine($"🔧 Auto-creating user from token claims: {email}");

            var user = new ApplicationUser
            {

                Id = userId, // Use the same ID from the token
                Email = email,
                UserName = email,
                DisplayName = displayName ?? email,
                NID = nid?.Trim(),
                EmailConfirmed = true // Since they had a valid token
            };

            // Create user without password (since they're already authenticated via token)
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"❌ Failed to create user: {errors}");
                throw new BadRequestException($"Failed to recreate user: {errors}");
            }

            Console.WriteLine($"✅ Successfully created user: {user.Email}");
            return user;
        }

        // Then modify your GetCurrentUser method to use this:
        public async Task<ApplicationUserDto> GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity!.IsAuthenticated)
                throw new UnAuthorizedException("User is not authenticated.");

            var userId = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                        claimsPrincipal.FindFirst("uid")?.Value;

            var email = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            Console.WriteLine($"🔍 Looking for user - ID: '{userId}', Email: '{email}'");

            ApplicationUser user = null;

            // Try to find user by ID
            if (!string.IsNullOrEmpty(userId))
            {
                user = await _userManager.FindByIdAsync(userId);
            }

            // Try to find user by email
            if (user == null && !string.IsNullOrEmpty(email))
            {
                user = await _userManager.FindByEmailAsync(email!);
            }

            // If user still not found, auto-create from token claims
            if (user == null)
            {
                Console.WriteLine("⚠️ User not found in database, attempting to recreate from token...");
                try
                {
                    user = await CreateUserFromTokenClaimsAsync(claimsPrincipal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to recreate user: {ex.Message}");
                    throw new NotFoundException($"User not found and could not be recreated: {ex.Message}");
                }
            }

            Console.WriteLine($"✅ Returning user: {user.Email}");

            return new ApplicationUserDto
            {
                DisplayName = user.DisplayName!,
                NID = user.NID,
                Email = user.Email!,
                DateOfBirth=_validationService.ExtractDateOfBirth(user.NID),
                Address=user.Address,
                Gender=user.Gender,
                GovernorateName=_validationService.ExtractGovernorateInfo(user.NID),
                Status=user.Status,
                PhoneNumber =user.PhoneNumber!,     
                Token = await GenerateTokenAsync(user)
            };
        }
        public async Task<OtpResponseDTO> SendRegistrationOtpAsync(string email)
        {
            try
            {
                // Input validation

                if (string.IsNullOrWhiteSpace(email))
                {
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "البريد الإلكتروني مطلوب"
                    };
                }

                if (!IsValidEmail(email))
                {
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "صيغة البريد الإلكتروني غير صحيحة"
                    };
                }

                email = email.ToLower().Trim();

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    _logger.LogWarning("OTP registration attempt for existing email: {Email}", email);
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "هذا البريد الإلكتروني مسجل مسبقاً"
                    };
                }

             
                // Send OTP
                var result = await _oTPService.SendOtpAsync(email, "Register");
                _logger.LogInformation("Registration OTP requested for email: {Email}", email);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending registration OTP for email: {Email}", email);
                return new OtpResponseDTO
                {
                    IsSuccess = false,
                    Message = "حدث خطأ أثناء إرسال رمز التحقق"
                };
            }
        }

        public async Task<ApplicationUserDto> RegisterWithOtpAsync(RegisterDTO model, string otpCode)
        {
            try
            {
                // Input validation
                if (model == null)
                {
                    throw new BadRequestException("البيانات المدخلة غير صحيحة");
                }

                if (string.IsNullOrWhiteSpace(otpCode))
                {
                    throw new BadRequestException("رمز التحقق مطلوب");
                }

                ValidateRegistrationInput(model);

                // Verify OTP
                var otpValidation = await _oTPService.VerifyOtpAsync(new VerifyOtpDTO
                {
                    Email = model.Email.ToLower().Trim(),
                    Code = otpCode.Trim(),
                    Purpose = "Register"
                });

                if (!otpValidation.IsSuccess)
                {
                    _logger.LogWarning("Invalid OTP during registration for email: {Email}", model.Email);
                    throw new BadRequestException(otpValidation.Message);
                }

                // Proceed with registration
                var result = await RegisterAsync(model);

                // Mark email as confirmed since OTP was verified
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }

                _logger.LogInformation("User registered successfully with OTP verification: {Email}", model.Email);
                return result;
            }
            catch (Exception ex) when (!(ex is BadRequestException))
            {
                _logger.LogError(ex, "Error during OTP registration for email: {Email}", model?.Email);
                throw new Exception("حدث خطأ أثناء التسجيل");
            }
        }

        // Password reset methods
        public async Task<OtpResponseDTO> SendPasswordResetOtpAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                {
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "البريد الإلكتروني غير صحيح"
                    };
                }

                email = email.ToLower().Trim();
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Don't reveal that user doesn't exist for security
                    return new OtpResponseDTO
                    {
                        IsSuccess = true,
                        Message = "إذا كان البريد الإلكتروني مسجلاً، سيتم إرسال رمز التحقق"
                    };
                }



                return await _oTPService.SendOtpAsync(email, "PasswordReset");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset OTP for email: {Email}", email);
                return new OtpResponseDTO
                {
                    IsSuccess = false,
                    Message = "حدث خطأ أثناء إرسال رمز التحقق"
                };
            }
        }

        public async Task<bool> ResetPasswordWithOtpAsync(string email, string otpCode, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otpCode) || string.IsNullOrWhiteSpace(newPassword))
                {
                    throw new BadRequestException("جميع الحقول مطلوبة");
                }

                var user = await _userManager.FindByEmailAsync(email.ToLower().Trim());
                if (user == null)
                {
                    throw new BadRequestException("المستخدم غير موجود");
                }

                var otpValidation = await _oTPService.VerifyOtpAsync(new VerifyOtpDTO
                {
                    Email = email.ToLower().Trim(),
                    Code = otpCode.Trim(),
                    Purpose = "PasswordReset"
                });

                if (!otpValidation.IsSuccess)
                {
                    throw new BadRequestException(otpValidation.Message);
                }

                // Reset password
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException($"فشل في إعادة تعيين كلمة المرور: {errors}");
                }

                _logger.LogInformation("Password reset successfully for user: {Email}", email);
                return true;
            }
            catch (Exception ex) when (!(ex is BadRequestException))
            {
                _logger.LogError(ex, "Error resetting password for email: {Email}", email);
                throw new Exception("حدث خطأ أثناء إعادة تعيين كلمة المرور");
            }
        }

        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        // ✅ FIXED: Use consistent claim types
        new Claim(ClaimTypes.NameIdentifier, user.Id), // This maps to nameidentifier
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, user.Email), // This maps to emailaddress
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Iat,
            new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64),
        new Claim("uid", user.Id), // Custom claim for backup
        new Claim("display_name", user.DisplayName ?? ""),
        new Claim("nid", user.NID ?? "")
    };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.DurationInMinutes),
                Issuer = _jwtOptions.Value.Issuer,
                Audience = _jwtOptions.Value.Audience,
                SigningCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Console.WriteLine($"✅ Token created successfully");
            Console.WriteLine($"✅ Token length: {tokenString.Length}");
            Console.WriteLine($"✅ Expires: {tokenDescriptor.Expires}");

            // ✅ DEBUG: Show what claims are in the token
            try
            {
                var decodedToken = tokenHandler.ReadJwtToken(tokenString);
                Console.WriteLine($"✅ Token validation: Header={decodedToken.Header.Count}, Claims={decodedToken.Claims.Count()}");
                Console.WriteLine("✅ Token Claims:");
                foreach (var claim in decodedToken.Claims)
                {
                    Console.WriteLine($"   {claim.Type} = {claim.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Token decode error: {ex.Message}");
            }

            return tokenString;
        }

        private void ValidateRegistrationInput(RegisterDTO model)
        {
            if (model == null)
                throw new BadRequestException("البيانات المدخلة غير صحيحة");

            if (string.IsNullOrWhiteSpace(model.DisplayName))
                throw new BadRequestException("الاسم مطلوب");

            if (string.IsNullOrWhiteSpace(model.Email))
                throw new BadRequestException("البريد الإلكتروني مطلوب");

            if (!IsValidEmail(model.Email))
                throw new BadRequestException("صيغة البريد الإلكتروني غير صحيحة");

            if (string.IsNullOrWhiteSpace(model.NID))
                throw new BadRequestException("الرقم القومي مطلوب");

            if (string.IsNullOrWhiteSpace(model.Password))
                throw new BadRequestException("كلمة المرور مطلوبة");

            if (model.NID.Length != 14 || !model.NID.All(char.IsDigit))
                throw new BadRequestException("الرقم القومي يجب أن يكون 14 رقم");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                var phoneRegex = new Regex(@"^(\+2|002)?01[0-9]{9}$");
                if (!phoneRegex.IsMatch(model.PhoneNumber.Replace(" ", "").Replace("-", "")))
                    throw new BadRequestException("صيغة رقم الهاتف غير صحيحة");
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}

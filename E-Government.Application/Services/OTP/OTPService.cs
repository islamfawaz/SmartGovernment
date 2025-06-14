using E_Government.Application.DTO;
using E_Government.Application.DTO.OTP;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities;
using E_Government.Domain.Entities.OTP;
using E_Government.Domain.RepositoryContracts.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace E_Government.Application.Services.OTP
{
    public class OTPService : IOTPService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OTPService> _logger;
        private readonly IMailSettings _mailSettings;

        public OTPService(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ILogger<OTPService> logger,
            IMailSettings mailSettings
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
            _mailSettings = mailSettings;
        }


        public async Task<OtpResponseDTO> SendOtpAsync(string email, string purpose)
        {
            try
            {
                // Normalize email
                email = email.ToLower().Trim();

                // Check rate limiting
                if (!await CanSendOtpAsync(email))
                {
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "يجب الانتظار دقيقة واحدة قبل طلب رمز جديد"
                    };
                }

                IdentityUser user = null;

                // ✅ فقط تحقق من وجود المستخدم لو الـ purpose مش Register
                if (!string.Equals(purpose, "Register", StringComparison.OrdinalIgnoreCase))
                {
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        _logger.LogWarning("OTP requested for non-existent user: {Email}", email);
                        return new OtpResponseDTO
                        {
                            IsSuccess = false,
                            Message = "المستخدم غير موجود"
                        };
                    }
                }

                await InvalidateOtpAsync(email, purpose);

                var otpCode = await GenerateOtpCodeAsync();

                var otp = new OtpCode
                {
                    Email = email,
                    Code = otpCode,
                    CreatedAt = DateTime.UtcNow,
                    Purpose = purpose,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false
                };

                await _unitOfWork.OtpCodeRepository.AddAsync(otp);

                // إرسال الإيميل: في حالة Register مش هيبقى فيه user أصلاً، فنستخدم email مباشرة
                var emailToSend = new Email
                {
                    To = email,
                    Subject = GetEmailSubject(purpose),
                    Body = GetEmailBody(otpCode, purpose, user?.UserName ?? email)
                };

                try
                {
                    _mailSettings.SendEmail(emailToSend);
                    _logger.LogInformation("OTP {OtpCode} sent successfully to {Email} for purpose {Purpose}",
                        otpCode, email, purpose);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send OTP email to {Email}", email);
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "تم إنشاء رمز التحقق ولكن فشل في إرسال البريد الإلكتروني"
                    };
                }

                var saveResult = await _unitOfWork.CompleteAsync();
                if (saveResult > 0)
                {
                    return new OtpResponseDTO
                    {
                        IsSuccess = true,
                        Message = "تم إرسال رمز التحقق بنجاح"
                    };
                }

                return new OtpResponseDTO
                {
                    IsSuccess = false,
                    Message = "فشل في حفظ رمز التحقق"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {Email}", email);
                return new OtpResponseDTO
                {
                    IsSuccess = false,
                    Message = "حدث خطأ أثناء إرسال رمز التحقق"
                };
            }
        }

        // Helper methods for email content
        private string GetEmailSubject(string purpose)
        {
            return purpose switch
            {
                "PasswordReset" => "إعادة تعيين كلمة المرور - رمز التحقق",
                "EmailVerification" => "تفعيل الحساب - رمز التحقق",
                "TwoFactorAuth" => "رمز التحقق الثنائي",
                _ => "رمز التحقق"
            };
        }

        private string GetEmailBody(string otpCode, string purpose, string userName)
        {
            var greeting = $"مرحباً {userName}،";
            var purposeText = purpose switch
            {
                "PasswordReset" => "لإعادة تعيين كلمة المرور الخاصة بك",
                "EmailVerification" => "لتفعيل حسابك",
                "TwoFactorAuth" => "لتسجيل الدخول إلى حسابك",
                _ => "للمتابعة"
            };

            return $@"{greeting}

رمز التحقق الخاص بك {purposeText} هو: {otpCode}

هذا الرمز صالح لمدة 5 دقائق فقط.
إذا لم تطلب هذا الرمز، يرجى تجاهل هذه الرسالة.

مع تحيات فريق الحكومة الإلكترونية";
        }
        public async Task InvalidateOtpAsync(string email, string purpose)
        {
            try
            {
                await _unitOfWork.OtpCodeRepository.InvalidateOtpAsync(email, purpose);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating OTP for {Email} with purpose {Purpose}",
                    email, purpose);
            }
        }

        public async Task<bool> CanSendOtpAsync(string email)
        {
            try
            {
                var lastOtp = await _unitOfWork.OtpCodeRepository.GetLastOtpAsync(email);
                if (lastOtp == null) return true;
                return DateTime.UtcNow - lastOtp.CreatedAt > TimeSpan.FromSeconds(60);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking OTP rate limit for {Email}", email);
                return false; // Fail safe - don't allow if we can't check
            }
        }

        public async Task CleanupExpiredOtpsAsync()
        {
            try
            {
                _unitOfWork.OtpCodeRepository.DeleteExpiredOtps();
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Expired OTPs cleaned up successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired OTPs");
            }
        }

        public Task<string> GenerateOtpCodeAsync()
        {
            // Use cryptographically secure random number generator
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            var randomNumber = BitConverter.ToUInt32(bytes, 0);
            var otpCode = (randomNumber % 900000 + 100000).ToString(); // Ensures 6-digit number
            return Task.FromResult(otpCode);
        }

        public async Task<bool> IsOtpValidAsync(string email, string code, string purpose)
        {
            try
            {
                return await _unitOfWork.OtpCodeRepository.IsOtpValidAsync(email, code, purpose);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating OTP for {Email}", email);
                return false;
            }
        }

        public async Task<OtpResponseDTO> VerifyOtpAsync(VerifyOtpDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Code) ||
                    string.IsNullOrWhiteSpace(model.Purpose))
                {
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "جميع الحقول مطلوبة"
                    };
                }

                var isValid = await IsOtpValidAsync(model.Email, model.Code, model.Purpose);

                if (!isValid)
                {
                    _logger.LogWarning("Invalid OTP attempt for {Email} with purpose {Purpose}",
                        model.Email, model.Purpose);
                    return new OtpResponseDTO
                    {
                        IsSuccess = false,
                        Message = "كود OTP غير صحيح أو منتهي الصلاحية"
                    };
                }

                await InvalidateOtpAsync(model.Email, model.Purpose);
                _logger.LogInformation("OTP verified successfully for {Email} with purpose {Purpose}",
                    model.Email, model.Purpose);

                return new OtpResponseDTO
                {
                    IsSuccess = true,
                    Message = "تم التحقق بنجاح"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {Email}", model.Email);
                return new OtpResponseDTO
                {
                    IsSuccess = false,
                    Message = "حدث خطأ أثناء التحقق من الرمز"
                };
            }
        }
    }
}


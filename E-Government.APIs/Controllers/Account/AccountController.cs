using E_Government.APIs.Controllers.Base;
using E_Government.Application.DTO.Auth;
using E_Government.Application.DTO.User;
using E_Government.Application.ServiceContracts;
using E_Government.Application.Services.NIDValidation;
using E_Government.Domain.DTO;
using E_Government.Domain.ServiceContracts;
using E_Government.Domain.ServiceContracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E_Government.APIs.Controllers
{
     public class AccountController : ApiControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AccountController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApplicationUserDto>> Register(RegisterDTO model)
        {
            try
            {
                var result = await _serviceManager.AuthService.RegisterAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUserDto>> Login(loginDTO model)
        {
            try
            {
                var result = await _serviceManager.AuthService.LoginAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("validate-nid")]
        public  ActionResult<NIDValidationResultDto> ValidateNID(string nid)
        {
            var result = _serviceManager.ValidationService.ValidateAndExtractNID(nid);

            if (result.IsValid)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("user")]
        [Authorize] // ✅ This endpoint requires authentication
        public async Task<ActionResult<ApplicationUserDto>> GetCurrentUser()
        {
            try
            {
                // Enhanced debugging
                Console.WriteLine("=== REQUEST DEBUG INFO ===");
                Console.WriteLine($"IsAuthenticated: {User.Identity!.IsAuthenticated}");
                Console.WriteLine($"AuthenticationType: {User.Identity.AuthenticationType}");
                Console.WriteLine($"Name: {User.Identity.Name}");
                Console.WriteLine($"Claims Count: {User.Claims.Count()}");

                Console.WriteLine("All Claims:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"  {claim.Type} = {claim.Value}");
                }
                Console.WriteLine("========================");

                var result = await _serviceManager.AuthService.GetCurrentUser(User);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ GetCurrentUser Error: {ex.Message}");
                Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");
                return StatusCode(500, new { message = ex.Message, type = ex.GetType().Name });
            }
        }
       
        
   
    }
}
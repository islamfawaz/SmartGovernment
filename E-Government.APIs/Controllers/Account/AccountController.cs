using E_Government.APIs.Controllers.Base;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Government.APIs.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("login")]//POST/api/account/login
        public async Task<ActionResult<UserDTO>> Login(loginDTO model)
        {
            var response = await _authService.LoginAsync(model);
            return Ok(response);
        }


        [HttpPost("register")]//POST/api/account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            var response = await _authService.RegisterAsync(model);
            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            var result = await _authService.GetCurrentUser(User);
            return Ok(result);
        }
       

        [HttpGet("emailExist")]
        [Authorize]
        public async Task<ActionResult<bool>> CheckEmailExist()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return Ok(await _authService.EmailExist(email!));
        }



    }
}
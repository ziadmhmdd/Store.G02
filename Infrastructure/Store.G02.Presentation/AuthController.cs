using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.G02.Services.Abstractions;
using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IServiceManager serviceManager) : ControllerBase
    {
        // login

        [HttpPost("Login")] // POST: /api/auth/login
        public async Task<IActionResult> Login(LoginDto loginDto)
        { 
            var result = await serviceManager.AuthService.LoginAsync(loginDto);
            return Ok(result);
        }

        // register

        [HttpPost("Register")] // POST: /api/auth/register
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await serviceManager.AuthService.RegisterAsync(registerDto);
            return Ok(result);
        }


        // Check Email Exists
        [HttpGet("EmailExists")]
        public async Task<IActionResult> CheckEmailExists(string email)
        { 
            var result = await serviceManager.AuthService.CheckEmailExistsAsync(email);
            return Ok(result);
        }

        // Get Current User 
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var result = await serviceManager.AuthService.GetCurrentUserAsync(email.Value);
            return Ok(result);
        }

        // Get Current User Address
        [Authorize]
        [HttpGet("Address")]
        public async Task<IActionResult> GetCurrentUserAddress()
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var result = await serviceManager.AuthService.GetCurrentUserAddressAsync(email.Value);
            return Ok(result);
        }

        // Update Current User Address
        [Authorize]
        [HttpPut("Address")]
        public async Task<IActionResult> UpdateCurrentUserAddress(AddressDto request)
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var result = await serviceManager.AuthService.UpdateCurrentUserAddressAsync(request, email.Value);
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Store.G02.Domain.Entities.Identity;
using Store.G02.Domain.Exceptions;
using Store.G02.Services.Abstractions;
using Store.G02.Shared.Dtos.Products;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using static System.Net.WebRequestMethods;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Store.G02.Shared;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Store.G02.Services.Products
{
    public class AuthService(UserManager<AppUser> _userManager, IOptions<JwtOptions> options, IMapper _mapper) : IAuthService
    {
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<UserResultDto> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) throw new UserNotFoundException(email);

            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };
        }

        public async Task<AddressDto?> GetCurrentUserAddressAsync(string email)
        {
            //_userManager.FindByEmailAsync(email); // This Function Dont Load The Navigational Property 
            var user = await _userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.Email.ToLower() == email.ToLower());
            if (user is null) throw new UserNotFoundException(email);
            return _mapper.Map<AddressDto>(user.Address);
        }

        public async Task<AddressDto?> UpdateCurrentUserAddressAsync(AddressDto request, string email)
        {
            var user = await _userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.Email.ToLower() == email.ToLower());
            if (user is null) throw new UserNotFoundException(email);

            if (user.Address is null)
            {
                // Create new Address
                user.Address = _mapper.Map<Address>(request);
            }
            else
            {
                // Update The Old Address
                user.Address.FirstName = request.FirstName;
                user.Address.LastName = request.LastName;
                user.Address.City = request.City;
                user.Address.Street = request.Street;
                user.Address.Country = request.Country;
            }

            await _userManager.UpdateAsync(user);

            return _mapper.Map<AddressDto>(user.Address);
        }

        public async Task<UserResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null) throw new UnAuthorizedException();

            var flag = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!flag) throw new UnAuthorizedException();

            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user),
            };
        }

        public async Task<UserResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new AppUser()
            { 
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(error => error.Description);
                throw new ValidationException(errors);
            }

            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user),
            };
        }

        private async Task<string> GenerateJwtTokenAsync(AppUser user)
        {
            // https://localhost:7085
            // Header
            // PayLoad
            // Signature

            var jwtOptions = options.Value;

            var authClaim = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaim.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: authClaim,
                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
                );

            // TOKEN

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}

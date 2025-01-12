﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using App_Default_Backend.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using App_Default_Backend.Data.Models;
using App_Default_Backend.Core.Contract;
using App_Default_Backend.Core.Models;

namespace App_Default_Backend.Core.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private User? _user;

        private readonly string _loginProvider;
        private readonly string _refreshToken;
        private readonly string _jwtKey;
        public AuthManager(IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _loginProvider = _configuration["RefreshToken:LoginProvider"] ?? throw new ArgumentNullException("RefreshToken:LoginProvider");
            _refreshToken = _configuration["RefreshToken:Name"] ?? throw new ArgumentNullException("RefreshToken:Name");
            _jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
        }

        public async Task<string?> CreateRefreshToken()
        {
            if (_user == null) return null;

            await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);
            return result.Succeeded ? newRefreshToken : null;
        }

        public async Task<AuthResponse?> VerifyRefeshToken(AuthResponse request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);
            var username = tokenContent.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
            _user = await _userManager.FindByEmailAsync(username);

            if (_user == null || _user.Id != request.UserId)
                return null;

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.Token);

            if (isValidRefreshToken)
            {
                var token = await GenerateToken();
                var refreshToken = await CreateRefreshToken();
                return new AuthResponse { UserId = _user.Id, Token = token, RefreshToken = refreshToken };
            }

            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }

        public async Task<AuthResponse?> Login(LoginUser inputUser)
        {
            _user = await _userManager.FindByEmailAsync(inputUser.Email);
            if (_user != null)
            {
                if (await _userManager.CheckPasswordAsync(_user, inputUser.Password))
                {
                    var token = await GenerateToken();
                    return new AuthResponse { UserId = _user.Id, Token = token, RefreshToken = await CreateRefreshToken() };
                }
            }
            return null;
        }

        public async Task<IEnumerable<IdentityError>> Register(InputUser inputUser)
        {
            _user = _mapper.Map<User>(inputUser);
            var result = await _userManager.CreateAsync(_user, inputUser.Password);

            //Defult Role for new user
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(_user, "User");

            return result.Errors;
        }

        private async Task<string> GenerateToken()
        {
            if (_user == null) return string.Empty;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_user);
            var rolesClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);

            var userEmail = _user.Email ?? string.Empty;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userEmail),
                new Claim(JwtRegisteredClaimNames.NameId, _user.Id)
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var token = new JwtSecurityToken(
                               issuer: _configuration["Jwt:Issuer"],
                               audience: _configuration["Jwt:Audience"],
                               claims: claims,
                               expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Jwt:DurationInMinutes"])),
                               signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> GetByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new NotFoundException(nameof(GetByEmail), email); ;
            return true;
        }
    }
}

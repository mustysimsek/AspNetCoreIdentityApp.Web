using System;
using System.Collections.Generic;
using System.Security.Claims;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class HomeService : IHomeService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool, AppUser?)> CheckUserAsync(string email)
        {
            var hasUser = await _userManager.FindByEmailAsync(email);

            if (hasUser == null)
            {
                return (false, null);
            }
            return (true, hasUser);
        }

        public async Task<bool> CheckUserBirthDateHasValue(string userName)
        {
            var getUser = await _userManager.FindByNameAsync(userName);

            if (!getUser.BirthDate.HasValue) return false;
            return true;
        }

        public async Task UserSignInWithClaimsAsync(string userName, bool rememberMe)
        {
            var getUser = await _userManager.FindByNameAsync(userName);

            await _signInManager.SignInWithClaimsAsync(getUser, rememberMe,
            new[] { new Claim("birthdate", getUser.BirthDate.Value.ToString()) });
        }

        public async Task<int> GetUserAccessFailedCountAsync(string userName)
        {
            var getUser = await _userManager.FindByNameAsync(userName);

            return await _userManager.GetAccessFailedCountAsync(getUser);
        }

        public async Task<(bool, SignInResult)> UserPasswordSignInAsync(string userName, string password, bool rememberMe)
        {
            var signInresult = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, true);

            if (signInresult.IsLockedOut)
            {
                return (false, signInresult);
            }
            return (true, signInresult);
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> CreateUserAsync(SignUpViewModel request)
        {
            var identityResult = await _userManager.CreateAsync(new()
            {
                UserName = request.Username,
                PhoneNumber = request.Phone,
                Email = request.Email
            }, request.Password);

            if (!identityResult.Succeeded)
            {
                return (false, identityResult.Errors);
            }
            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());

            var user = await _userManager.FindByNameAsync(request.Username);

            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);

            if (!claimResult.Succeeded)
            {
                return (false, claimResult.Errors);
            }

            return (true, null);
        }

        public async Task<(string, AppUser?)> ForgetUserPassword(string email)
        {
            var hasUser = await _userManager.FindByEmailAsync(email);

            if (hasUser == null)
            {
                return (null, null);
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            return (passwordResetToken, hasUser);
        }

        public async Task<bool> CheckUserByIdAsync(string id)
        {
            var hasUser = await _userManager.FindByIdAsync(id);

            return hasUser != null;
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> ResetUserPasswordAsync(string id, string token, string newPassword)
        {
            var hasUser = await _userManager.FindByIdAsync(id);
            var resetToUserPasswordResult = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, newPassword);

            if (!resetToUserPasswordResult.Succeeded)
            {
                return (false, resetToUserPasswordResult.Errors);
            }
            return (true, null);
        }
    }
}


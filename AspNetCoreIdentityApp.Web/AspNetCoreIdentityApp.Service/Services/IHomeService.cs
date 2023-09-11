using System;
using AspNetCoreIdentityApp.Core.ViewModels;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Service.Services
{
    public interface IHomeService
    {
        Task<(bool, AppUser?)> CheckUserAsync(string email);

        Task<(bool, SignInResult)> UserPasswordSignInAsync(string userName, string password, bool rememberMe);

        Task<int> GetUserAccessFailedCountAsync(string userName);

        Task<bool> CheckUserBirthDateHasValue(string userName);

        Task UserSignInWithClaimsAsync(string userName, bool rememberMe);

        Task<(bool, IEnumerable<IdentityError>?)> CreateUserAsync(SignUpViewModel request);

        Task<(string, AppUser?)> ForgetUserPassword(string email);

        Task<bool> CheckUserByIdAsync(string id);

        Task<(bool, IEnumerable<IdentityError>?)> ResetUserPasswordAsync(string id, string token, string newPassword);
    }
}


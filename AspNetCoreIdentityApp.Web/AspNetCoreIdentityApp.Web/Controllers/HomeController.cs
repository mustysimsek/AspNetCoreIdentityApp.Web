﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Service.Services;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SignUp()
    {
        return View();
    }

    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        returnUrl ??= Url.Action("Index", "Home"); // returnUrl null ise = 'in sağ tarafını ata

        var hasUser = await _userManager.FindByEmailAsync(model.Email);

        if (hasUser == null)
        {
            ModelState.AddModelError(string.Empty, "Email veya Şifre yanlış");
            return View();
        }

        var signInresult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

        if (signInresult.IsLockedOut)
        {
            ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriş yapamazsınız" });
            return View();
        }

        if (!signInresult.Succeeded)
        {
            ModelState.AddModelErrorList(new List<string>() { "Email veya şifre yanlış",
        $"Başarısız giriş sayısı :{await _userManager.GetAccessFailedCountAsync(hasUser)}"});
            return View();
        }

        if (hasUser.BirthDate.HasValue)
        {
            await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe,
            new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
        }
        return Redirect(returnUrl!);
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel request)
    {

        if (!ModelState.IsValid)
        {
            return View();
        }
        var identityResult = await _userManager.CreateAsync(new()
        {
            UserName = request.Username,
            PhoneNumber = request.Phone,
            Email = request.Email
        }, request.Password);

        if (!identityResult.Succeeded)
        {
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            return View();
        }

        var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());

        var user = await _userManager.FindByNameAsync(request.Username);

        var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);

        if (!claimResult.Succeeded)
        {
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            return View();
        }

        TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarılı bir şekilde gerçekleşti";

        return RedirectToAction(nameof(HomeController.SignUp));


    }

    public IActionResult ForgetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
    {
        var hasUser = await _userManager.FindByEmailAsync(request.Email);

        if (hasUser == null)
        {
            ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır");
            return View();
        }

        string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

        var passwordResetLink = Url.Action("ResetPassword", "Home",
            new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);


        //örnek link -> https://localhost:7008?userId=12323&token=asdallkzxjczxc

        await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

        TempData["SuccessMessage"] = "Şifre yenileme linki, e-posta adresinize gönderilmiştir.";

        return RedirectToAction(nameof(ForgetPassword));
    }

    public IActionResult ResetPassword(string userId, string token)
    {
        TempData["userId"] = userId;
        TempData["token"] = token;

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
    {
        var userId = TempData["userId"];
        var token = TempData["token"];

        if (userId == null || token == null)
        {
            throw new Exception("Bir hata meydana geldi");
        }

        var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (hasUser == null)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır.");
            return View();
        }

        var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Şifreniz başarılı bir şekilde yenilenmiştir.";
        }
        else
        {
            ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


using System.Diagnostics;
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
    private readonly IHomeService _homeService;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IHomeService homeService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _homeService = homeService;
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

        var (hasUser, validUser) = await _homeService.CheckUserAsync(model.Email);

        if (!hasUser)
        {
            ModelState.AddModelError(string.Empty, "Email veya Şifre yanlış");
            return View();
        }

        var (isSuccess, signInresult) = await _homeService.UserPasswordSignInAsync(validUser.UserName, model.Password, model.RememberMe);

        if (signInresult.IsLockedOut)
        {
            ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriş yapamazsınız" });
            return View();
        }

        if (!signInresult.Succeeded)
        {
            ModelState.AddModelErrorList(new List<string>() { "Email veya şifre yanlış",
        $"Başarısız giriş sayısı :{await _homeService.GetUserAccessFailedCountAsync(validUser.UserName)}"});
            return View();
        }

        if (await _homeService.CheckUserBirthDateHasValue(validUser.UserName))
        {
            await _homeService.UserSignInWithClaimsAsync(validUser.UserName, model.RememberMe);
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

        var (isSuccess, errors) = await _homeService.CreateUserAsync(request);

        if (!isSuccess)
        {
            ModelState.AddModelErrorList(errors.Select(x => x.Description).ToList());
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
        var (passwordResetToken, hasUser) = await _homeService.ForgetUserPassword(request.Email);

        if (hasUser == null)
        {
            ModelState.AddModelError(string.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır");
            return View();
        }

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

        //var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (!await _homeService.CheckUserByIdAsync(userId.ToString()!))
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı bulunamamıştır.");
            return View();
        }

        //var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);
        var (isSuccess, errors) = await _homeService.ResetUserPasswordAsync(userId.ToString()!, token.ToString()!, request.Password);

        if (isSuccess)
        {
            TempData["SuccessMessage"] = "Şifreniz başarılı bir şekilde yenilenmiştir.";
        }
        else
        {
            ModelState.AddModelErrorList(errors!.Select(x => x.Description).ToList());
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


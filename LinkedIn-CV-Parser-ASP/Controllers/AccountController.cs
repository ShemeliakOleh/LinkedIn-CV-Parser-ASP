using LinkedIn_CV_Parser_ASP.Domain;
using LinkedIn_CV_Parser_ASP.Models;
using LinkedIn_CV_Parser_ASP.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Controllers
{
    public class AccountController : Controller
    {
        private DataManager db;
        public IEmailSender EmailSender { get; set; }
        public AccountController(DataManager context, IEmailSender emailSender)
        {
            db = context;
            EmailSender = emailSender;
        }
        public async Task<IActionResult> SendEmailConfirmation(string toAddress, string callbackUrl) {
            var subject = "Confirm your email";
            var body = $"Confirm registration by clicking on the <a href='{callbackUrl}'>link</a>. <br/><br/>If link doesn't work go to {callbackUrl}";
            await EmailSender.SendEmailAsync(toAddress, subject, body);
            return RedirectToAction("ConfirmEmail", "Account");
        }
        public async Task<IActionResult> SendPasswordRecoverConfirmation(string toAddress, string callbackUrl) {
            var subject = "Password reset confirmation";
            var body = $"Reset password by clicking on the <a href='{callbackUrl}'>link</a>. <br/><br/>If link doesn't work go to {callbackUrl}";
            await EmailSender.SendEmailAsync(toAddress, subject, body);
            return RedirectToAction("ConfirmPasswordReset", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = await db._accountRepository.GetUserbyEmail(model.Email);
                if (user != null && user.Password == HashSHA512.ComputeHash(model.Password) && user.Role != null)
                {
                    await Authenticate(user);

                    if (!db._accountRepository.IsEmailConfirmed(user.Id)) {
                        return RedirectToAction("ConfirmEmail","Account");
                    }
                    string altitudeString = Request.Query.FirstOrDefault(p => p.Key == "ReturnUrl").Value;
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)) {
                        return Redirect(ReturnUrl);
                    } else {
                        if (user.Role == "Admin")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        if (user.Role == "User")
                        {
                            return RedirectToAction("Index", "User");
                        }
                        return RedirectToAction("Index", "Home");
                        
                    }
                }
                ModelState.AddModelError("", "Incorrect login or password");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            User user = db._accountRepository.GetUserbyEmail(email).Result;
            if (user != null) {
                var bytes = SymmetricEncryptor.EncryptString(email + '*' + DateTime.Now.AddHours(2), user.Id.ToString().Replace("-", ""));
                string base64Encoded = Convert.ToBase64String(bytes);
                var callbackUrl = Url.Action(
                                "ResetPassword",
                                "Home",
                                new { userId = user.Id.ToString(), code = base64Encoded },
                                protocol: HttpContext.Request.Scheme);
                return RedirectToAction("SendPasswordRecoverConfirmation", new { toAddress = email, callbackUrl = callbackUrl });
            }
            ViewBag.EmailIsNotRegistered = true;
            return View();
        }
        [HttpGet]
        public IActionResult PasswordChangedSuccessfully() {
            if (User.IsInRole("Admin")) {
                ViewBag.Role = "Admin";
            } else if (User.IsInRole("User")) {
                ViewBag.Role = "User";
            }
            return View();
        }
        [HttpGet]
        public IActionResult ConfirmPasswordReset() {
            return View();
        }
        [HttpGet]
        public IActionResult TemplateChangedSuccessfully() {
            return View();

        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public IActionResult Settings() {
            if (User.IsInRole("Admin")) {
                ViewBag.Role = "Admin";
            } else {
                ViewBag.Role = "User";
            }
            return View();
        }
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> Settings(string oldPassword, string newPassword, string newPasswordConfirm, string operation) {
            
            if ((await db._accountRepository.GetUserbyEmail(User.Identity.Name)) != null)
            {
                if (operation == "Delete my account")
                {
                    if (User.IsInRole("Admin"))
                    {

                       var users =  await db._accountRepository.GetUsersAsync();
                        if (users.Count(x => x.Role == "Admin")<2)
                        {
                            ViewBag.DeleteAccountMessage = "You cannot delete your account as you are the only administrator on the server.";
                            return View();
                        }
                        else
                        {
                           await db._accountRepository.DeleteUsers(true,User.Identity.Name);
                           await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                    }
                    else
                    {
                        await db._accountRepository.DeleteUsers(false,User.Identity.Name);
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                    return RedirectToAction("DeletedUser", "Home");
                }
                if (User.IsInRole("Admin")) {
                    ViewBag.Role = "Admin";
            } else {
                ViewBag.Role = "User";
            }
            User user = db._accountRepository.GetUserbyEmail(User.Identity.Name).Result;
            if (user == null) {
                await Logout();
            }
            if (HashSHA512.ComputeHash(oldPassword) == user.Password) {
                if (newPassword == newPasswordConfirm) {
                    if (newPassword != oldPassword) {
                        db._profileRepository.UpdateRecord<User, string>("Users", user.Id, "Password", HashSHA512.ComputeHash(newPassword));
                        return View("PasswordChangedSuccessfully");
                    } else {
                        ViewBag.OldSameAsNew = true;
                    }
                } else {
                    ViewBag.PasswordsDoesntMatch = true;
                }
            } else {
                ViewBag.IncorrectOldPassword = true;
            }
            return View();
            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("DeletedUser", "Home");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db._accountRepository.GetUserbyEmail(model.Email);

                if ((user == null))
                {
                    RegistrationKey key = db._accountRepository.GetKey(model.Key).Result;
                    if (key != null)
                    {
                        db._accountRepository.RegisterUser(new User { Email = model.Email, Password = HashSHA512.ComputeHash(model.Password), Key = key.Value, Role = key.Role });
                        db._accountRepository.DeleteKey(model.Key);
                        user = await db._accountRepository.GetUserbyEmail(model.Email);
                        var bytes = SymmetricEncryptor.EncryptString(user.Email, user.Id.ToString().Replace("-",""));
                        string base64Encoded = Convert.ToBase64String(bytes);

                        var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Account",
                            new { userId = user.Id.ToString(), code = base64Encoded },
                            protocol: HttpContext.Request.Scheme);
                        return RedirectToAction("SendEmailConfirmation", new { toAddress = user.Email, callbackUrl = callbackUrl });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect registrational key");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login or password");
                }
            }
            return View(model);
        }
        private async Task Authenticate(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index", "Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code) {
            ViewBag.IncorrectEmail = false;
            if (userId == null || code == null) {
                return View();
            }
            var user = db._profileRepository.LoadRecordById<User>("Users", new Guid(userId));
            if (user == null) {
                return RedirectToAction("Index", "Home");
            }
            if (user.Email == SymmetricEncryptor.DecryptToString(Convert.FromBase64String(code), user.Id.ToString().Replace("-", ""))) {
                bool result = db._profileRepository.ConfirmEmail<User>("Users", new Guid(userId));
                if (result) {
                    await Authenticate(user);
                    if (User.IsInRole("Admin"))
                    {
                        ViewBag.UserName = User.Identity.Name;
                        return RedirectToAction("Index", "Admin");
                    }
                    if (User.IsInRole("User"))
                    {
                        ViewBag.UserName = User.Identity.Name;
                        return RedirectToAction("Index", "User");
                    }
                    return RedirectToAction("Index", "Home");
                } else {
                    return RedirectToAction("Index", "Home");
                }
            } else {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email) {
            if (email == null) {
                ViewBag.IncorrectEmail = true;
                return View();
            }
            User user = await db._accountRepository.GetUserbyEmail(email);
            ViewBag.IncorrectEmail = false;
            if (user != null) {
                var bytes = SymmetricEncryptor.EncryptString(user.Email, user.Id.ToString().Replace("-", ""));
                string base64Encoded = Convert.ToBase64String(bytes);

                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = user.Id.ToString(), code = base64Encoded },
                    protocol: HttpContext.Request.Scheme);
                return RedirectToAction("SendEmailConfirmation", new { toAddress = user.Email, callbackUrl = callbackUrl });
            } else {
                ViewBag.IncorrectEmail = true;
                return View();
            }
        }
    }
}

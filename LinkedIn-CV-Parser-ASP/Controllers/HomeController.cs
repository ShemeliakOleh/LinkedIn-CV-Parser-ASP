using LinkedIn_CV_Parser_ASP.Domain;
using LinkedIn_CV_Parser_ASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Controllers {
    public class HomeController : Controller {
        private DataManager db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DataManager context, ILogger<HomeController> logger) {
            _logger = logger;
            db = context;
        }
        public IActionResult Index() {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "User");
            }
            return View();
        }
        public IActionResult DeletedUser()
        {
            return View();
        }
        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string code) {
            var user = db._profileRepository.LoadRecordById<User>("Users", new Guid(userId));
            if (user == null) {
                return RedirectToAction("Index", "Home");
            }
            List<string> emailAndExpiration = SymmetricEncryptor.DecryptToString(Convert.FromBase64String(code), user.Id.ToString().Replace("-", "")).Split('*').ToList();
            if (DateTime.TryParse(emailAndExpiration[1], out _)) {
                
                if (DateTime.Parse(emailAndExpiration[1]) >= DateTime.Now) {
                    HttpContext.Session.SetString("userId", userId);
                    return RedirectToAction("ForgotPassword");
                } else {
                    return RedirectToAction("ResetPasswordExpired","Account");
                }
            } else {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public IActionResult ForgotPassword() {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string newPassword, string newPasswordConfirm) {
            string userId = HttpContext.Session.GetString("userId");
            var user = db._profileRepository.LoadRecordById<User>("Users", new Guid(userId));
            if (user == null) {
                return RedirectToAction("Index", "Home");
            } else {
                if (newPassword == newPasswordConfirm) {
                    db._profileRepository.UpdateRecord<User, string>("Users", user.Id, "Password", HashSHA512.ComputeHash(newPassword));
                    return RedirectToAction("PasswordChangedSuccessfully", "Account");
                } else {
                    ViewBag.PasswordsDoesntMatch = true;
                    return View();
                }
            } 
        }
    }
}

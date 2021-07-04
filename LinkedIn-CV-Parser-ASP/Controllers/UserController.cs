using LinkedIn_CV_Parser_ASP.Domain;
using LinkedIn_CV_Parser_ASP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Controllers
{
    public class UserController : Controller
    {
        private DataManager db;
        public UserController(DataManager context) {
            db = context;
        }
        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            ViewBag.UserName = User.Identity.Name;
            return View();
        }
        [Authorize(Roles = "User")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}

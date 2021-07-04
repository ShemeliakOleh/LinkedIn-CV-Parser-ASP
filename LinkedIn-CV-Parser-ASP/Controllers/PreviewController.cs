using LinkedIn_CV_Parser_ASP.Domain;
using LinkedIn_CV_Parser_ASP.Domain.Entities;
using LinkedIn_CV_Parser_ASP.Domain.Entities.Repository;
using LinkedIn_CV_Parser_ASP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Controllers {
    public class PreviewController : Controller {
        
        private DataManager _dataManager;
        public PreviewController(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> Editor(string token) {
            if( (await _dataManager._accountRepository.GetUserbyEmail(User.Identity.Name)) != null)
            {
                if (User.IsInRole("Admin"))
                {
                    ViewBag.Role = "Admin";
                }
                else
                {
                    ViewBag.Role = "User";
                }
                if (token == null)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
                EditorViewModel editorViewModel;

                try
                {
                    Profile profile = _dataManager._profileRepository.LoadRecordById<Profile>("ProfileInfo", new Guid(token));
                    if (profile == null)
                    {
                        if (User.IsInRole("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index", "User");
                        }
                    }
                    editorViewModel = profile.ToProfileWithCategorizedSkills(_dataManager);
                    _dataManager._profileRepository.DeleteRecord<Profile>("ProfileInfo", new Guid(token));
                }
                catch
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
                return View("Editor", editorViewModel);
            }
            else
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("DeletedUser", "Home");
            }
            
        }
        [HttpPost]
        public IActionResult Editor(Profile profile) {
            _dataManager._profileRepository.InsertRecord("ProfileInfo", profile);
            DeleteUnusedRecordAsync(profile);
            return Content("Post request made successfully!");
        }
        public async void DeleteUnusedRecordAsync(Profile profile) {
            await Task.Delay(1000 * 60 * 2);
            _dataManager._profileRepository.DeleteRecord<Profile>("ProfileInfo", profile.Id);
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public string CoreStackLimit() {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(1).CopyTo(bytes, 0);
            return _dataManager._excelCellsRepository.LoadRecordById<ExcelCellsLocation>("ExcelCellsLocation", new Guid(bytes)).CoreStack.Count.ToString();
        }
        [HttpPost]
        public IActionResult Download(EditorViewModel editorViewModel) {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(1).CopyTo(bytes, 0);
            return editorViewModel.WriteToExcel(_dataManager._excelCellsRepository.LoadRecordById<ExcelCellsLocation>("ExcelCellsLocation", new Guid(bytes)));
        }
    }
}

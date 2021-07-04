using LinkedIn_CV_Parser_ASP.Domain;
using LinkedIn_CV_Parser_ASP.Models;
using LinkedIn_CV_Parser_ASP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Controllers
{
    public class AdminController : Controller {
        private Random random;
        private DataManager db;
        public AdminController(DataManager data) {
            db = data;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index() {
            ViewBag.UserName = User.Identity.Name;
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Privacy() {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            return View(await db._accountRepository.GetUsersAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Users(string email, List<string> usersToDelete)
        {
            if(email != null){usersToDelete.Add(email);}

            if(usersToDelete.Count == 0)
            {
                ViewBag.Message = "Field is empty";
            }
            else
            {
                if ((await db._accountRepository.DeleteUsers(false,usersToDelete.ToArray<string>())))
                 {
                     ViewBag.Message = "Users successfully deleted";
                 }
            }
            return View(await db._accountRepository.GetUsersAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Skills(string category = "ProgrammingLanguages")
        {
            ViewBag.Category = category;
            var skills = await db._skillsRepository.GetListSkillsbyName(category);
            return View(new SkillsViewModel(await db._skillsRepository.GetListSkillsbyName(category)));
        }
        [HttpPost]
        public async Task<IActionResult> SkillsAsync(SkillsViewModel skillsViewModel)
        {
            ViewBag.Category = skillsViewModel.Category;
            var skills = await db._skillsRepository.GetListSkillsbyName(skillsViewModel.Category);
            if (skillsViewModel.Action == "Add")
            {
                if (skillsViewModel.NewValue == "")
                {
                    ViewBag.Message = "Field is empty";
                    return View(new SkillsViewModel(await db._skillsRepository.GetListSkillsbyName(skillsViewModel.Category)));
                }
                if (skills.Contains(skillsViewModel.NewValue))
                {
                    ViewBag.Message = "Item exists";
                    return View(new SkillsViewModel(skills));
                }
                ViewBag.Message = "Item added";
                db._skillsRepository.SetSkill(skillsViewModel.Category, skillsViewModel.NewValue);
                skills.Add(skillsViewModel.NewValue);
                return View(new SkillsViewModel(skills));
            }
            else
            {
                if (skillsViewModel.NewValue != "")
                {
                    if (skills.Contains(skillsViewModel.NewValue))
                    {
                        skillsViewModel.Values.Add(skillsViewModel.NewValue);
                    }
                    else
                    {
                        ViewBag.Message = "Item not found";
                    }
                }
                 if (skillsViewModel.Values.Count != 0)
                {
                    
                    
                    db._skillsRepository.DeleteSkill(skillsViewModel.Category, skillsViewModel.Values);
                    skills.RemoveAll(x=> skillsViewModel.Values.Contains(x));
                    ViewBag.Message = "Items deleted";
                    return View(new SkillsViewModel(skills));
                }
                else
                {
                    ViewBag.Message = "No items selected for deletion";
                    return View(new SkillsViewModel(skills));
                }
            }

        }
        [HttpPost]
        public IActionResult GetKeysByRole(string Role = "User") {
            return RedirectToActionPermanent("GenerateRegistrationKey", new {
                Role = Role
            });
        }
        [HttpGet]
        public IActionResult GenerateRegistrationKey(bool NotAllowedCharacters = false, bool KeyAlreadyExists = false, string Role = "User") {
            RegistrationKeyViewModel registrationKeyViewModel = new RegistrationKeyViewModel();
            registrationKeyViewModel.Role = Role;
            List<RegistrationKey> registrationKeys = db._accountRepository.GetAllUserKeysByRole(Role).Result;
            ViewBag.NotAllowedCharacters = NotAllowedCharacters;
            ViewBag.KeyAlreadyExists = KeyAlreadyExists;
            if (registrationKeys == null) {
                registrationKeyViewModel.RegistrationKeys = new List<Filter>();
            } else {
                registrationKeyViewModel.RegistrationKeys = registrationKeys.Select(x => new Filter() {
                    Value = x.Value,
                    Selected = false
                }).ToList();
            }
            return View("GenerateRegistrationKey",registrationKeyViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> GenerateRegistrationKey(RegistrationKeyViewModel registrationKeyViewModel) {
            if (registrationKeyViewModel.NewKey == null) {
                registrationKeyViewModel.NewKey = "";
            }
            bool NotAllowedCharacters = false;
            bool KeyAlreadyExists = false;
            if (registrationKeyViewModel.Action == "Add") {
                List<RegistrationKey> registrationKeys = await db._accountRepository.GetAllUserKeysByRole(registrationKeyViewModel.Role);
                if (registrationKeys == null) {
                    registrationKeys = new List<RegistrationKey>();
                }
                if (!registrationKeys.Select(x => x.Value).ToList().Contains(registrationKeyViewModel.NewKey)) {
                    if (Regex.IsMatch(registrationKeyViewModel.NewKey, "^[a-zA-Z0-9-_]{8,12}$")) {
                        db._accountRepository.AddKey(registrationKeyViewModel.NewKey, registrationKeyViewModel.Role);
                    } else {
                        NotAllowedCharacters = true;
                    }
                } else {
                    KeyAlreadyExists = true;
                }
            } else if (registrationKeyViewModel.Action == "Generate") {
                registrationKeyViewModel.NewKey = GenerateKey();
                return View("GenerateRegistrationKey", registrationKeyViewModel);
            } else if (registrationKeyViewModel.Action == "Remove selected") {
                registrationKeyViewModel.RegistrationKeys
                    .Where(x => x.Selected == true)
                    .ToList()
                    .ForEach(x => db._accountRepository.DeleteKey(x.Value));
            }
            return RedirectToActionPermanent("GenerateRegistrationKey", new { 
                NotAllowedCharacters = NotAllowedCharacters,
                KeyAlreadyExists = KeyAlreadyExists,
                Role = registrationKeyViewModel.Role
            });
        }
        private string GenerateKey() {
            random = new Random(Convert.ToInt32(DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString() 
                + DateTime.Now.Hour.ToString()) + Convert.ToInt32(DateTime.Now.DayOfYear.ToString() + DateTime.Now.Year.ToString()));
            string key = "";
            int length = random.Next(8, 13);
            for (int i = 0; i < length; ++i) {
                int charType = random.Next(0, 3);
                if (charType == 0) {
                    key += (char)random.Next('a', 'a' + 26);
                } else if (charType == 1) {
                    key += (char)random.Next('A', 'A' + 26);
                } else {
                    char[] specialCharacters = new char[] { '-', '_' };
                    int specialCharIndex = random.Next(0, specialCharacters.Length);
                    key += specialCharacters[specialCharIndex];
                }
            }
            return key;
        }
        [HttpGet]
        public IActionResult UpdateTemplate() {
            ViewBag.IncorrectSheetName = false;
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(1).CopyTo(bytes, 0);
            ExcelCellsLocation cellsLocation = db._excelCellsRepository.LoadRecordById<ExcelCellsLocation>("ExcelCellsLocation", new Guid(bytes));
            return View(new UpdateTemplateViewModel() { 
                CellsLocation = cellsLocation, 
                CoreStack = string.Join(' ', cellsLocation.CoreStack) 
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTemplate(UpdateTemplateViewModel updateTemplateViewModel) {
            ViewBag.IncorrectSheetName = false;
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(1).CopyTo(bytes, 0);
            Guid guid = new Guid(bytes);
            updateTemplateViewModel.CellsLocation.Id = guid;
            updateTemplateViewModel.CellsLocation.CoreStack = updateTemplateViewModel.CoreStack.Split(new char[] { ' ', ',' }).ToList();

            if (updateTemplateViewModel.ExcelFile != null && updateTemplateViewModel.ExcelFile.Count > 0) {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Template.xlsx");
                MemoryStream streamValidation = new MemoryStream();
                await updateTemplateViewModel.ExcelFile[0].CopyToAsync(streamValidation);
                if (!EditorViewModel.IsSheetNameValid(streamValidation, updateTemplateViewModel.CellsLocation.SheetName)) {
                    ViewBag.IncorrectSheetName = true;
                    return View(updateTemplateViewModel);
                }
                db._excelCellsRepository.DeleteRecord<ExcelCellsLocation>("ExcelCellsLocation", guid);
                db._excelCellsRepository.InsertRecord("ExcelCellsLocation", updateTemplateViewModel.CellsLocation);
                using (var stream = System.IO.File.Create(filePath)) {
                    await updateTemplateViewModel.ExcelFile[0].CopyToAsync(stream);
                    return RedirectToAction("TemplateChangedSuccessfully","Account");
                }
            }
            return View(updateTemplateViewModel);
        }
    }
}

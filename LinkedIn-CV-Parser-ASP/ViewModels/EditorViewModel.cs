using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Models {
    public class EditorViewModel {
        public string FirstLastName { get; set; }
        public string About { get; set; }
        public List<List<string>> Education { get; set; }
        public List<Filter> ProgrammingLanguages { get; set; }
        public List<Filter> RDBMS { get; set; }
        public List<Filter> DevelopmentTools { get; set; }
        public List<Filter> LibrariesFrameworksTools { get; set; }
        public List<Filter> OperatingSystems { get; set; }
        public List<Filter> Others { get; set; }

        public List<Filter> ProgrammingLanguagesCoreStack { get; set; }
        public List<Filter> RDBMSCoreStack { get; set; }
        public List<Filter> DevelopmentToolsCoreStack { get; set; }
        public List<Filter> LibrariesFrameworksToolsCoreStack { get; set; }
        public List<Filter> OperatingSystemsCoreStack { get; set; }
        public List<Filter> OthersCoreStack { get; set; }

        public List<string> SkillType { get; set; }

        public EditorViewModel() {
            FirstLastName = String.Empty;
            About = String.Empty;
            Education = new List<List<string>>();
            ProgrammingLanguages = new List<Filter>();
            RDBMS = new List<Filter>();
            DevelopmentTools = new List<Filter>();
            LibrariesFrameworksTools = new List<Filter>();
            OperatingSystems = new List<Filter>();
            Others = new List<Filter>();

            ProgrammingLanguagesCoreStack = new List<Filter>();
            RDBMSCoreStack = new List<Filter>();
            DevelopmentToolsCoreStack = new List<Filter>();
            LibrariesFrameworksToolsCoreStack = new List<Filter>();
            OperatingSystemsCoreStack = new List<Filter>();
            OthersCoreStack = new List<Filter>();

            SkillType = new List<string>();
        }
        public static bool IsSheetNameValid(Stream fileStream, string sheetName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try {
                using (var package = new ExcelPackage(fileStream)) {
                    if (package.Workbook.Worksheets[sheetName] == null) {
                        return false;
                    }
                }
            } catch {
                return false;
            }
            return true;
        }
        public IActionResult WriteToExcel(ExcelCellsLocation excelCellsLocation) {

            for (int i = 0; i < Others.Count; ++i) {
                if (SkillType[i] == "ProgrammingLanguages" && Others[i].Selected) {
                    ProgrammingLanguages.Add(Others[i]);
                } else if (SkillType[i] == "RDBMS") {
                    RDBMS.Add(Others[i]);
                } else if (SkillType[i] == "DevelopmentTools") {
                    DevelopmentTools.Add(Others[i]);
                } else if (SkillType[i] == "LibrariesFrameworksTools") {
                    LibrariesFrameworksTools.Add(Others[i]);
                } else if (SkillType[i] == "OperatingSystems") {
                    OperatingSystems.Add(Others[i]);
                }
            }

            string pathIn = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Template.xlsx");
            var file = new FileInfo(pathIn);
            byte[] bytes;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(file)) {
                string sheet = excelCellsLocation.SheetName;
                package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.FirstLastName].Value = FirstLastName;
                package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.About].Value = About;
                if (Education.Count > 0) {
                    package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.Ed1UN].Value = Education[0][0];
                    package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.Ed1OI].Value = Education[0][1];
                }
                if (Education.Count > 1) {
                    package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.Ed2UN].Value = Education[1][0];
                    package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.Ed2OI].Value = Education[1][1];
                }

                package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.ProgrammingLanguages].Value = string.Join(", ", ProgrammingLanguages.Where(x => x.Selected == true).Select(x => x.Value).ToArray());
                package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.RDBMS].Value = string.Join(", ", RDBMS.Where(x => x.Selected == true).Select(x => x.Value).ToArray());
                package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.DevelopmentTools].Value = string.Join(", ", DevelopmentTools.Where(x => x.Selected == true).Select(x => x.Value).ToArray());
                package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.OperatingSystems].Value = string.Join(", ", OperatingSystems.Where(x => x.Selected == true).Select(x => x.Value).ToArray());
                package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.LibrariesFrameworksTools].Value = string.Join(", ", LibrariesFrameworksTools.Where(x => x.Selected == true).Select(x => x.Value).ToArray());
                
                List<string> OthersSelected = new List<string>();
                for (int i = 0; i < Others.Count; ++i) {
                    if (Others[i].Selected && SkillType[i] == "Uncategorized") {
                        OthersSelected.Add(Others[i].Value);
                    }
                }
                string[] uncategorizedSkillsCells = excelCellsLocation.Others.Split(',');
                Match match = Regex.Match(uncategorizedSkillsCells[0], "^([A-Za-z])([1-9]+[0-9]*)$", RegexOptions.IgnoreCase);
                string startColumn = match.Groups[1].Value;
                int startRow = Int32.Parse(match.Groups[2].Value);
                int height = Int32.Parse(uncategorizedSkillsCells[1]);
                for (int i = 0; i < OthersSelected.Count; ++i) {
                    package.Workbook.Worksheets[sheet].Cells[(char)(startColumn[0] + i / height) + (startRow + i % height).ToString()].Value = OthersSelected[i];
                    if ((char)(startColumn[0] + i / height) == 'Z')
                        break;
                }

                var allCoreStack = new List<string>();
                allCoreStack.AddRange(ProgrammingLanguagesCoreStack.Where(x => x.Selected == true).Select(x => x.Value));
                allCoreStack.AddRange(RDBMSCoreStack.Where(x => x.Selected == true).Select(x => x.Value));
                allCoreStack.AddRange(DevelopmentToolsCoreStack.Where(x => x.Selected == true).Select(x => x.Value));
                allCoreStack.AddRange(LibrariesFrameworksToolsCoreStack.Where(x => x.Selected == true).Select(x => x.Value));
                allCoreStack.AddRange(OperatingSystemsCoreStack.Where(x => x.Selected == true).Select(x => x.Value));
                allCoreStack.AddRange(OthersCoreStack.Where(x => x.Selected == true).Select(x => x.Value));

                for (int i = 0; i < excelCellsLocation.CoreStack.Count && i < allCoreStack.Count; ++i) {
                    package.Workbook.Worksheets[sheet].Cells[excelCellsLocation.CoreStack[i]].Value = allCoreStack[i];
                }

                bytes = package.GetAsByteArray();
            }

            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = $"{FirstLastName}.xlsx";

            var result = new FileContentResult(bytes, contentType);
            result.FileDownloadName = fileName;
            return result;
        }
    }
}

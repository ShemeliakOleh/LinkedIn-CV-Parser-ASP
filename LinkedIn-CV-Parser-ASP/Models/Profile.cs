using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinkedIn_CV_Parser_ASP.Domain;
using LinkedIn_CV_Parser_ASP.Domain.Entities;
using LinkedIn_CV_Parser_ASP.Domain.Entities.Repository;
using MongoDB.Bson.Serialization.Attributes;

namespace LinkedIn_CV_Parser_ASP.Models {
    public class Profile {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstLastName { get; set; }
        public string About { get; set; }
        public List<List<string>> Education { get; set; }
        public List<string> Skills { get; set; }
        public Profile() {
            FirstLastName = String.Empty;
            About = String.Empty;
            Education = new List<List<string>>();
            Skills = new List<string>();
            Id = new Guid();
        }
        public EditorViewModel ToProfileWithCategorizedSkills(DataManager dataManager) {
            EditorViewModel editorViewModel = new EditorViewModel();
            editorViewModel.FirstLastName = FirstLastName;
            editorViewModel.About = About;
            editorViewModel.Education = new List<List<string>>();
            Education.ForEach(x => {
                List<string> temp = new List<string>();
                x.ForEach(y => temp.Add(y));
                editorViewModel.Education.Add(temp);
            });

            List<string> ProgrammingLanguages = dataManager._skillsRepository.GetListSkillsbyName("ProgrammingLanguages").Result;
            ProgrammingLanguages = ProgrammingLanguages.ConvertAll(d => d.ToLower().Trim());
            List<string> RDBMS = dataManager._skillsRepository.GetListSkillsbyName("RDBMS").Result;
            RDBMS = RDBMS.ConvertAll(d => d.ToLower().Trim());
            List<string> DevelopmentTools = dataManager._skillsRepository.GetListSkillsbyName("DevelopmentTools").Result;
            DevelopmentTools = DevelopmentTools.ConvertAll(d => d.ToLower().Trim());
            List<string> LibrariesFrameworksTools = dataManager._skillsRepository.GetListSkillsbyName("LibrariesFrameworksTools").Result;
            LibrariesFrameworksTools = LibrariesFrameworksTools.ConvertAll(d => d.ToLower().Trim());
            List<string> OperatingSystems = dataManager._skillsRepository.GetListSkillsbyName("OperatingSystems").Result;
            OperatingSystems = OperatingSystems.ConvertAll(d => d.ToLower().Trim());

            foreach (string s in Skills) {
                List<string> sVariations = GenerateVariations(s);
                bool IsOther = true;
                foreach (string sVariation in sVariations) {
                    if (ProgrammingLanguages.Contains(sVariation)) {
                        editorViewModel.ProgrammingLanguages.Add(new Filter() { Value = s, Selected = true });
                        editorViewModel.ProgrammingLanguagesCoreStack.Add(new Filter() { Value = s, Selected = false });
                        IsOther = false;
                        break;
                    } else if (RDBMS.Contains(sVariation)) {
                        editorViewModel.RDBMS.Add(new Filter() { Value = s, Selected = true });
                        editorViewModel.RDBMSCoreStack.Add(new Filter() { Value = s, Selected = false });
                        IsOther = false;
                        break;
                    } else if (DevelopmentTools.Contains(sVariation)) {
                        editorViewModel.DevelopmentTools.Add(new Filter() { Value = s, Selected = true });
                        editorViewModel.DevelopmentToolsCoreStack.Add(new Filter() { Value = s, Selected = false });
                        IsOther = false;
                        break;
                    } else if (LibrariesFrameworksTools.Contains(sVariation)) {
                        editorViewModel.LibrariesFrameworksTools.Add(new Filter() { Value = s, Selected = true });
                        editorViewModel.LibrariesFrameworksToolsCoreStack.Add(new Filter() { Value = s, Selected = false });
                        IsOther = false;
                        break;
                    } else if (OperatingSystems.Contains(sVariation)) {
                        editorViewModel.OperatingSystems.Add(new Filter() { Value = s, Selected = true });
                        editorViewModel.OperatingSystemsCoreStack.Add(new Filter() { Value = s, Selected = false });
                        IsOther = false;
                        break;
                    }
                }
                if(IsOther) {
                    editorViewModel.Others.Add(new Filter() { Value = s, Selected = true });
                    editorViewModel.OthersCoreStack.Add(new Filter() { Value = s, Selected = false });
                    editorViewModel.SkillType.Add("");
                }
            }
            return editorViewModel;
        }

        private List<string> GenerateVariations(string s) {
            List<string> res = new List<string>();
            s = s.Trim();
            res.Add(s);
            res.Add(s.ToLower());
            res.Add(Regex.Replace(s, "[\\.]", ""));
            string normalized = Regex.Replace(s, "[\\.]", "").ToLower();
            res.Add(normalized + ".js");
            res.Add(normalized + "js");
            res.Add(normalized);
            if (normalized.EndsWith("js") && normalized.Length > 2) {
                res.Add(normalized.Substring(0, normalized.Length - 2));
            }
            return res;
        }
    }
}

using LinkedIn_CV_Parser_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.ViewModels
{
    public class SkillsViewModel
    {
        public string Category { get; set; }
        public List<string> Values { get; set; }
        public string NewValue { get; set; }
        public string Action { get; set; }
        public List<string> FirstSkillsColumn { get; set; }
        public List<string> SecondSkillsColumn { get; set; }
        public List<string> ThirdSkillsColumn { get; set; }
        public List<string> FourthSkillsColumn { get; set; }


        public SkillsViewModel(List<string> skills)
        {
            FirstSkillsColumn = new List<string>();
            SecondSkillsColumn = new List<string>();
            ThirdSkillsColumn = new List<string>();
            FourthSkillsColumn = new List<string>();
            if (skills.Count != 0)
            {
                int countInCoolumn = (int)(skills.Count()/4);
                int startIndex = 0;
                int lastIndex = countInCoolumn;
                var remainder = skills.Count % 4;
                if (remainder >= 1)
                {
                    lastIndex++;
                }
                for (int i = startIndex; i<lastIndex; i++)
                {
                    FirstSkillsColumn.Add(skills[i]);
                }
                startIndex = lastIndex;
                lastIndex += countInCoolumn;
                if (remainder >=2)
                {
                    lastIndex++;
                }
                for (int i = startIndex; i < lastIndex; i++)
                {
                    SecondSkillsColumn.Add(skills[i]);
                }
                startIndex = lastIndex;
                lastIndex += countInCoolumn;
                if (remainder >= 3)
                {
                    lastIndex++;
                }
                for (int i = startIndex; i < lastIndex; i++)
                {
                    ThirdSkillsColumn.Add(skills[i]);
                }
                startIndex = lastIndex;
                lastIndex += countInCoolumn;
                for (int i = startIndex; i<skills.Count; i++)
                {
                    FourthSkillsColumn.Add(skills[i]);
                }
                
            }
            Category = "";
            Values = new List<string>();
            NewValue = "";
            Action = "";

        }
        public SkillsViewModel()
        {
            Category = "";
            Values = new List<string>();
            NewValue = "";
            Action = "";
        }
        
    }
}

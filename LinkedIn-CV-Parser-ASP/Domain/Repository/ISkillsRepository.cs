using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Entities.Repository
{
    public interface ISkillsRepository
    {
        public Task<List<string>> GetListSkillsbyName(string nameOfSkill);
        public void SetSkill(string category, string value);
        public void DeleteSkill(string category, List<string> value);
        public void DeleteSkillAsync(string category, List<string> value);
    }
}

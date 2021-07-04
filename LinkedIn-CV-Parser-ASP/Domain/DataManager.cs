using LinkedIn_CV_Parser_ASP.Domain.Entities.Repository;
using LinkedIn_CV_Parser_ASP.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain
{
    public class DataManager
    {
        public ISkillsRepository _skillsRepository { get; set; }
        public IProfileRepository _profileRepository { get; set; }
        public IAccountRepository _accountRepository { get; set; }
        public IExcelCellsRepository _excelCellsRepository { get; set; }
        public DataManager(ISkillsRepository skillsRepository, IProfileRepository profileRepository, IAccountRepository accountRepository, IExcelCellsRepository excelCellsRepository)
        {
            _skillsRepository = skillsRepository;
            _profileRepository = profileRepository;
            _accountRepository = accountRepository;
            _excelCellsRepository = excelCellsRepository;
        }

    }
}

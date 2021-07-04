using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Models {
    public class ExcelCellsLocation {
        public Guid Id { get; set; }
        public string SheetName { get; set; }
        public string FirstLastName { get; set; }
        public string About { get; set; }
        public string Ed1UN { get; set; }
        public string Ed1OI { get; set; }
        public string Ed2UN { get; set; }
        public string Ed2OI { get; set; }
        public string ProgrammingLanguages { get; set; }
        public string RDBMS { get; set; }
        public string DevelopmentTools { get; set; }
        public string OperatingSystems { get; set; }
        public string LibrariesFrameworksTools { get; set; }
        public string Others { get; set; }
        public List<string> CoreStack { get; set; } = new List<string>();

    }
}

using LinkedIn_CV_Parser_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Models {
    public class RegistrationKeyViewModel {
        public List<Filter> RegistrationKeys { get; set; }
        public string NewKey { get; set; }
        public string Action { get; set; }
        public string Role { get; set; }
        public RegistrationKeyViewModel() {
            RegistrationKeys = new List<Filter>();
            NewKey = "";
            Action = "";
            Role = "";
        }
    }
}

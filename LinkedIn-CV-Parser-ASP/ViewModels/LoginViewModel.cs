using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не вказано Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не вказано Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

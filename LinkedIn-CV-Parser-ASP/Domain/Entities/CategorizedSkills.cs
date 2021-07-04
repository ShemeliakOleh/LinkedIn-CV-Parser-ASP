using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Entities
{
    public class CategorizedSkills
    {
        [Required]
        public Guid Id { get; set; }
        
        public String Name { get; set; }

        public List<string> Values { get; set; }
    }
}

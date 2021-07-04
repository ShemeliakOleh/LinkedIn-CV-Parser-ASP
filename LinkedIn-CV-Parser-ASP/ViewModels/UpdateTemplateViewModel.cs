using LinkedIn_CV_Parser_ASP.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LinkedIn_CV_Parser_ASP.ViewModels {
    public class UpdateTemplateViewModel {
        public List<IFormFile> ExcelFile { get; set; }
        public ExcelCellsLocation CellsLocation { get; set; }
        public string CoreStack { get; set; }
        public UpdateTemplateViewModel() {
            CellsLocation = new ExcelCellsLocation();
        }
    }
}

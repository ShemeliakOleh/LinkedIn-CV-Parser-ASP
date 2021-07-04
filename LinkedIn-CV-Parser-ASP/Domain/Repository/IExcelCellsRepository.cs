using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Repository {
    public interface IExcelCellsRepository {
        public void InsertRecord<T>(string table, T record);
        public T LoadRecordById<T>(string table, Guid id);
        public void DeleteRecord<T>(string table, Guid id);

    }
}

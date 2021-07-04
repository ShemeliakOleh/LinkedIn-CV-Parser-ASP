using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Repository
{
    public interface IProfileRepository
    {
        public void InsertRecord<T>(string table, T record);
        public T LoadRecordById<T>(string table, Guid id);
        public void DeleteRecord<T>(string table, Guid id);
        public bool UpdateRecord<T, V>(string table, Guid id, string fieldName, V newValue);
        public bool ConfirmEmail<T>(string table, Guid id);
        
    }
}

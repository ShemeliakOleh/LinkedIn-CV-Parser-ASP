using LinkedIn_CV_Parser_ASP.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Repository {
    public class ExcelCellsRepository : IExcelCellsRepository {
        private MongoDbService _context;
        public ExcelCellsRepository(MongoDbService context) {
            this._context = context;
        }
        public void InsertRecord<T>(string table, T record) {
            var collection = _context.db.GetCollection<T>(table);
            collection.InsertOne(record);
        }
        public T LoadRecordById<T>(string table, Guid id) {
            var collection = _context.db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            return collection.Find(filter).FirstOrDefault();
        }
        public void DeleteRecord<T>(string table, Guid id) {
            var collection = _context.db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    }
}

using LinkedIn_CV_Parser_ASP.Domain.Entities;
using LinkedIn_CV_Parser_ASP.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Repository
{
    public class ProfileMongoRepository : IProfileRepository
    {
        private MongoDbService _context;
        public ProfileMongoRepository(MongoDbService context) {
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
        public bool UpdateRecord<T, V>(string table, Guid id, string fieldName, V newValue) {
            try {
                var collection = _context.db.GetCollection<T>(table);
                var filter = Builders<T>.Filter.Eq("Id", id);
                var update = Builders<T>.Update.Set(fieldName, newValue);
                collection.UpdateOne(filter, update);
            } catch {
                return false;
            }
            return true;
        }
        public void DeleteRecord<T>(string table, Guid id) {
            var collection = _context.db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
        public bool ConfirmEmail<T>(string table, Guid id) {
            return UpdateRecord<T, bool>(table, id, "IsEmailConfirmed", true);
        }
    }
}

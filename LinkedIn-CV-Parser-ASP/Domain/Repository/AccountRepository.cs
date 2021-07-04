using LinkedIn_CV_Parser_ASP.Domain.Entities;
using LinkedIn_CV_Parser_ASP.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private MongoDbService _context;
        public AccountRepository(MongoDbService context)
        {
            _context = context;
        }

        public void DeleteKey(string key)
        {
            var collection = _context.db.GetCollection<BsonDocument>("Registration_keys");
            collection.DeleteOne(p => p["Value"] == key);
        }
        public void DeleteKeyAsync(string key) {
            var collection = _context.db.GetCollection<BsonDocument>("Registration_keys");
            collection.DeleteOneAsync(p => p["Value"] == key);
        }
        public void AddKeyAsync(string key) {
            var collection = _context.db.GetCollection<RegistrationKey>("Registration_keys");
            collection.InsertOneAsync(new RegistrationKey() { Value = key, Role = "User" });
        }
        public void AddKey(string key, string role) {
            var collection = _context.db.GetCollection<RegistrationKey>("Registration_keys");
            collection.InsertOne(new RegistrationKey() { Value = key, Role = role });
        }

        public async Task<RegistrationKey> GetKey(string KeyValue)
        {
            var collection = _context.db.GetCollection<BsonDocument>("Registration_keys");
            var filter = new BsonDocument("Value", KeyValue);
            var key = await collection.Find(filter).ToListAsync();
            if (key.FirstOrDefault() == null)
            {
                return null;
            }
            else
            {
                return BsonSerializer.Deserialize<RegistrationKey>(key.FirstOrDefault());
            }
        }
        public async Task<List<RegistrationKey>> GetAllUserKeysByRole(string role) {
            var collection = _context.db.GetCollection<BsonDocument>("Registration_keys");
            var filter = new BsonDocument("Role", role);
            var key = await collection.Find(filter).ToListAsync();
            if (key.FirstOrDefault() == null) {
                return null;
            } else {
                return key.Select(x => BsonSerializer.Deserialize<RegistrationKey>(x)).ToList();
            }
        }
        public async Task<User> GetUserbyEmail(string email)
        {
            var collection = _context.db.GetCollection<BsonDocument>("Users");
            var filter = new BsonDocument("Email", email);
            var user = await collection.Find(filter).ToListAsync();
            if( user.FirstOrDefault() == null)
            {
                return null;
            }
            else
            {
                return BsonSerializer.Deserialize<User>(user.FirstOrDefault());
            }

        }
        public async Task<List<User>> GetUsersAsync()
        {
            var collection = _context.db.GetCollection<User>("Users");
            var filter = new BsonDocument();
            var users = await collection.Find(filter).ToListAsync();
            return users;
        }

        public void RegisterUser(User user)
        {
            var collection = _context.db.GetCollection<BsonDocument>("Users");
            collection.InsertOneAsync(user.ToBsonDocument());
        }
        public bool IsEmailConfirmed(Guid id) {
            var collection = _context.db.GetCollection<User>("Users");
            var filter = Builders<User>.Filter.Eq("Id", id);
            return collection.Find(filter).First().IsEmailConfirmed;
        }

        public async Task<bool> DeleteUsers(bool deleteAdmin ,params string[] emails)
        {
            var collection = _context.db.GetCollection<User>("Users");
            if (deleteAdmin)
            {
                return ((await collection.DeleteManyAsync(x => emails.Contains(x.Email))).DeletedCount == 1);
            }
            else
            {
                return ((await collection.DeleteManyAsync(x => emails.Contains(x.Email) && x.Role == "User")).DeletedCount == 1);
            }
            
        }

        public async Task<List<RegistrationKey>> GetAllUserKeys()
        {
            var collection = _context.db.GetCollection<BsonDocument>("Registration_keys");
            var filter = new BsonDocument();
            var key = await collection.Find(filter).ToListAsync();
            if (key.FirstOrDefault() == null)
            {
                return null;
            }
            else
            {
                return key.Select(x => BsonSerializer.Deserialize<RegistrationKey>(x)).ToList();
            }
        }
    }
}

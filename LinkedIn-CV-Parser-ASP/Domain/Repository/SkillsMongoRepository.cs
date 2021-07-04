using LinkedIn_CV_Parser_ASP.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Entities.Repository
{
    public class SkillsMongoRepository : ISkillsRepository
    {
        private MongoDbService _context;
        public SkillsMongoRepository(MongoDbService context)
        {
            this._context = context;
        }
        public async Task<List<string>> GetListSkillsbyName(string nameOfSkill)
        {
            var collection = _context.db.GetCollection<CategorizedSkills>("CategorizedSkills");
            var filter = new BsonDocument("Name", nameOfSkill);
            var categorizedSkills = await collection.Find(filter).ToListAsync();
            var progLangSkills = categorizedSkills.FirstOrDefault();
            return progLangSkills.Values;
        }
        public async void SetSkill(string category, string value)
        {

            var collection = _context.db.GetCollection<CategorizedSkills>("CategorizedSkills");
            await collection.UpdateOneAsync(new BsonDocument("Name",category),new BsonDocument("$addToSet",new BsonDocument("Values",value)));
        }
        public async void DeleteSkillAsync(string category, List<string> values)
        {
            var collection = _context.db.GetCollection<CategorizedSkills>("CategorizedSkills");
            BsonArray bsonArray = new BsonArray();
            bsonArray.AddRange(values);
            await collection.UpdateOneAsync(new BsonDocument("Name", category), new BsonDocument("$pull", new BsonDocument("Values", new BsonDocument("$in", bsonArray))));
        }
        public void DeleteSkill(string category, List<string> values)
        {
            var collection = _context.db.GetCollection<CategorizedSkills>("CategorizedSkills");
            BsonArray bsonArray = new BsonArray();
            bsonArray.AddRange(values);
            collection.UpdateOne(new BsonDocument("Name", category), new BsonDocument("$pull", new BsonDocument("Values", new BsonDocument("$in", bsonArray))));
        }
    }
}

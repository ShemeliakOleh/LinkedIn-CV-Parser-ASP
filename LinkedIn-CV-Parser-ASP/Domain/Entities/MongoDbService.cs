using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Entities {
    public class MongoDbService {
        public IMongoDatabase db;
        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            db = client.GetDatabase("LinkedIn-CV-Parser_DB");
        }
        
    }
}

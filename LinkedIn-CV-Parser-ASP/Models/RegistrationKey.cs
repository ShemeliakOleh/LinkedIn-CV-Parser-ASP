using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Models
{
    public class RegistrationKey
    {
        public ObjectId Id { get; set; }
        public string Value { get; set; }
        public string Role { get; set; }
    }
}

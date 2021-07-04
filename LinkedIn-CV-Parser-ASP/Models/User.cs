using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Key { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public User()
        {
            Email = "";
            Password = "";
            IsEmailConfirmed = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Models {
    public static class HashSHA512 {
        public static string ComputeHash(string input) {
            SHA512 hashSvc = SHA512.Create();
            byte[] hash = hashSvc.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}

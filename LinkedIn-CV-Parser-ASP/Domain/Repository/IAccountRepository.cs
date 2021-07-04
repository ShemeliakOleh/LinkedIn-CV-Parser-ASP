using LinkedIn_CV_Parser_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedIn_CV_Parser_ASP.Domain.Repository {
    public interface IAccountRepository {
        public Task<User> GetUserbyEmail(string email);
        public void RegisterUser(User user);
        public void DeleteKey(string KeyValue);
        public void DeleteKeyAsync(string key);
        public void AddKeyAsync(string key);
        public void AddKey(string key, string role);
        public Task<RegistrationKey> GetKey(string KeyValue);
        public Task<List<User>> GetUsersAsync();
        public bool IsEmailConfirmed(Guid id);
        public Task<List<RegistrationKey>> GetAllUserKeysByRole(string role);
        public Task<List<RegistrationKey>> GetAllUserKeys();
        public Task<bool> DeleteUsers(bool deleteAdmin, params string[] emails);
    }
}

using System.Security.Cryptography;
using System.Text;
using System;
using ClassLib;
using RPC;
namespace DataControl
{
    public static class Authentication
    {
        public static RemoteUserRepository userRepository;
        public static void SetRepo(RemoteUserRepository userRepo)
        {
            userRepository = userRepo;
        }
        public static User Registration(User user)
        {
            User isExists = userRepository.GetByUsername(user.username);
            if(isExists != null)
            {
                throw new System.Exception("User with that username is already exists");
            }
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, user.password);
            user.password = hash;
            user.registrationDate = DateTime.Now;
            user.role = "user";
            user.id = userRepository.Insert(user);
            return user;
        }
        public static User LogIn(string username, string password)
        {
            User user = userRepository.GetByUsername(username);
            if(user == null)
            {
                throw new System.Exception("User with that username is not exists");
            }
            SHA256 sha256Hash = SHA256.Create();
            if(!VerifyHash(sha256Hash, password, user.password))
            {
                throw new Exception("Wrong password! Try again");
            }
            return user;
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            var hashOfInput = GetHash(hashAlgorithm, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;  
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
using System;
namespace ClassLib
{
    public class User
    {
        public int id;
        public string username;
        public string password;
        public string fullname;
        public string role;
        public DateTime registrationDate;
        public Review[] reviews;
    }
}
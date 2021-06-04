using System;
using System.Collections.Generic;
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
        public List<Review> reviews;
        public string UserCon()
        {
            string sep = "!";
            string con = id+sep+username+sep+password+sep+fullname+sep+role+sep+registrationDate+sep;
            return con;
        }
        public static User Parse(string userToParse)
        {
            User user = new User();
            string[] arr = userToParse.Split("!");
            user.id = int.Parse(arr[0]);
            user.username = arr[1];
            user.password = arr[2];
            user.fullname = arr[3];
            user.role = arr[4];
            user.registrationDate = DateTime.Parse(arr[5]);
            return user;
        }
    }
}
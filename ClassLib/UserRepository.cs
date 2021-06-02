using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
namespace ClassLib
{
    public class UserRepository
    {
        private SqliteConnection connection;
        public UserRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        private static User GetUser(SqliteDataReader reader)
        {
            User user = new User();
            user.id = int.Parse(reader.GetString(0));
            user.username = reader.GetString(1);
            user.password = reader.GetString(2);
            user.fullname = reader.GetString(3);
            user.role = reader.GetString(4);
            user.registrationDate = DateTime.Parse(reader.GetString(5));
            return user;
        }
        public User GetById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();
            if(reader.Read())
                user= GetUser(reader);
            else
                user = null;
            reader.Close();
            return user;
        }
        public int DeleteById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(User user)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText =@"INSERT INTO users (username, password, fullname, role, registrationDate)
            VALUES ($username, $password, $fullname, $role, $registrationDate);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$password",user.password);
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$role",user.role);
            command.Parameters.AddWithValue("$registrationDate",user.registrationDate);
            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public int GetTotalPages()
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(this.GetCount() / (double)pageSize);
        }
        public long GetCount()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public List<User> GetPage(int page)
        {
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users LIMIT $pagesize OFFSET $offset";
            command.Parameters.AddWithValue("$pagesize", pageSize);
            command.Parameters.AddWithValue("$offset", pageSize*(page-1));

            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while(reader.Read())
            {
                User user = new User();
                
                users.Add(user);
            }
            reader.Close();
            return users;
        }
        public List<User> GetExport(string valueX)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE user LIKE $valueX";
            command.Parameters.AddWithValue("$valueX", valueX);

            SqliteDataReader reader = command.ExecuteReader();
            List<User> usersToExport = new List<User>();
            while(reader.Read())
            {
                User user = GetUser(reader);
                
                usersToExport.Add(user);
            }
            reader.Close();
            return usersToExport;
        }
        public User[] GetAll()
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";
            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while(reader.Read())
            {
                User user = GetUser(reader);
                
                users.Add(user);
            }
            reader.Close();
            User[] array = new User[users.Count];
            users.CopyTo(array);
            return array;
        }
        public int GetUserForReview(Review review)
        {
            User[] users = GetAll();
            Random rand = new Random();
            int randId = rand.Next(0,users.Length);
            for(int i = randId; i<=users.Length; i++)
            {
                if(i == users.Length)
                    i = 0;
                if(users[i].registrationDate < review.createdAt)
                    return users[i].id;
            }
            return users[0].id;
        }
        public DateTime GetMinRegDate()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT registrationDate FROM users";
            SqliteDataReader reader = command.ExecuteReader();
            DateTime min = DateTime.Now;
            while(reader.Read())
            {
                DateTime current = DateTime.Parse(reader.GetString(0));
                if(current < min)
                    min = current;
            }
            reader.Close();
            return min;
        }
        public User GetByUsername(string username)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username = $u";
            command.Parameters.AddWithValue("$u", username);

            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();
            if(reader.Read())
                user= GetUser(reader);
            else
                user = null;
            reader.Close();
            return user;
        }
        public bool Update(User user)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE users SET username = $u, password = $p, fullname = $f,
            role = $r, registrationDate = $rd WHERE id = $id";
            command.Parameters.AddWithValue("$id", user.id);
            command.Parameters.AddWithValue("$u", user.username);
            command.Parameters.AddWithValue("$p", user.password);
            command.Parameters.AddWithValue("$f", user.fullname);
            command.Parameters.AddWithValue("$r", user.role);
            command.Parameters.AddWithValue("$rd", user.registrationDate.ToString("o"));
            int nChanged = command.ExecuteNonQuery();
            return nChanged == 1;
        }
    }
}
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
namespace ClassLib
{
    public class ActorRepository
    {
        private SqliteConnection connection;
        public ActorRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        private static Actor GetActor(SqliteDataReader reader)
        {
            Actor actor = new Actor();
            actor.id = int.Parse(reader.GetString(0));
            actor.fullname = reader.GetString(1);
            actor.country = reader.GetString(2);
            actor.age = int.Parse(reader.GetString(3));
            return actor;
        }
        public Actor GetById(int id) //---------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            Actor actor = new Actor();
            if(reader.Read())
                actor = GetActor(reader);
            else
                actor = null;
            reader.Close();
            return actor;
        }
        public int DeleteById(int id) //-----------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Actor actor)//--------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText =@"INSERT INTO actors (fullname, country, age)
            VALUES ($fullname, $country, $age);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$fullname", actor.fullname);
            command.Parameters.AddWithValue("$country",actor.country);
            command.Parameters.AddWithValue("$age", actor.age);
            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public long GetCount()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public int GetSearchCount(string valueX)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors WHERE fullname LIKE '%' || $value || '%'";
            command.Parameters.AddWithValue("$value", valueX);

            long count = (long)command.ExecuteScalar();
            return (int)count;
        }
        public bool Update(Actor actor)//------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE actors SET fullname = $fullname, country = $country, age = $age WHERE id = $id";
            command.Parameters.AddWithValue("$id", actor.id);
            command.Parameters.AddWithValue("$fullname", actor.fullname);
            command.Parameters.AddWithValue("$country", actor.country);
            command.Parameters.AddWithValue("$age", actor.age);
            int nChanged = command.ExecuteNonQuery();
            return nChanged == 1;
        }
        public int[] GetAllIds()
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT id FROM actors";
            SqliteDataReader reader = command.ExecuteReader();
            List<int> ids = new List<int>();
            while(reader.Read())
            {   
                ids.Add(int.Parse(reader.GetString(0)));
            }
            reader.Close();
            int[] array = new int[ids.Count];
            ids.CopyTo(array);
            return array;
        }

        public int GetSearchPagesCount(string searchTitle)//----
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(GetSearchCount(searchTitle) / (double)pageSize);
        }
        public List<Actor> GetSearchPage(string searchTitle, int page)//-------
        {
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE fullname LIKE '%' || $value || '%' LIMIT $pageSize OFFSET $offset";
            command.Parameters.AddWithValue("$pageSize",pageSize);
            command.Parameters.AddWithValue("offset",pageSize*(page-1));
            command.Parameters.AddWithValue("$value", searchTitle);
            List<Actor> actors = new List<Actor>();
            SqliteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Actor actor = GetActor(reader);
                actors.Add(actor);
            }
            reader.Close();
            return actors;
        }

    }
}
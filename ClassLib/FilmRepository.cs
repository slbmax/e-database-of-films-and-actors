using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
namespace ClassLib
{
    public class FilmRepository
    {
        private SqliteConnection connection;
        public FilmRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        private static Film GetFilm(SqliteDataReader reader)//------
        {
            Film film = new Film();
            film.id = int.Parse(reader.GetString(0));
            film.title = reader.GetString(1);
            film.genre = reader.GetString(2);
            film.releaseYear = int.Parse(reader.GetString(3));
            return film;
        }
        public bool Update(Film film)//-------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE films SET title = $title, genre = $genre, releaseYear = $ry WHERE id = $id";
            command.Parameters.AddWithValue("$id", film.id);
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$genre", film.genre);
            command.Parameters.AddWithValue("$ry", film.releaseYear);
            int nChanged = command.ExecuteNonQuery();
            return nChanged == 1;
        }
        public Film GetById(int id) //-------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            Film film = new Film();
            if(reader.Read())
                film= GetFilm(reader);
            else
                film = null;
            reader.Close();
            return film;
        }
        public int DeleteById(int id)//----------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Film film)//----------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText =@"INSERT INTO films (title, genre, releaseYear)
            VALUES ($title, $genre, $releaseYear);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$genre",film.genre);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public long GetCount()//----------
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public int GetSearchCount(string valueX)//----------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films WHERE title LIKE '%' || $value || '%'";
            command.Parameters.AddWithValue("$value", valueX);

            long count = (long)command.ExecuteScalar();
            return (int)count;
        }
        public List<Film> GetAll()//-------
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films";
            SqliteDataReader reader = command.ExecuteReader();
            List<Film> films = new List<Film>();
            while(reader.Read())
            {
                Film film = GetFilm(reader);
                
                films.Add(film);
            }
            reader.Close();
            return films;
        }
        public int[] GetAllIds()//-----
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT id FROM films";
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
        public List<Film> GetSearchPage(string searchTitle, int page)//--
        {
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films WHERE title LIKE '%' || $value || '%' LIMIT $pageSize OFFSET $offset";
            command.Parameters.AddWithValue("$pageSize",pageSize);
            command.Parameters.AddWithValue("offset",pageSize*(page-1));
            command.Parameters.AddWithValue("$value", searchTitle);
            List<Film> films = new List<Film>();
            SqliteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Film film = GetFilm(reader);
                films.Add(film);
            }
            reader.Close();
            return films;
        }
    }
}
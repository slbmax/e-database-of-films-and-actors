using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
namespace ClassLib
{
    public class RoleRepository
    {
        private SqliteConnection connection;
        public RoleRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        private static Role GetRole(SqliteDataReader reader)
        {
            Role role = new Role();
            role.id = int.Parse(reader.GetString(0));
            role.actor_id = int.Parse(reader.GetString(2));
            role.film_id = int.Parse(reader.GetString(1));
            return role;
        }
        public Role GetById(int id)/////
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM roles WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            Role role = new Role();
            if(reader.Read())
                role= GetRole(reader);
            else
                role = null;
            reader.Close();
            return role;
        }
        public int DeleteFilmById(int id)///////
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = $"DELETE FROM roles WHERE film_id = $id";
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            return result;
        }
        public int DeleteActorById(int id)/////
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = $"DELETE FROM roles WHERE actor_id = $id";
            command.Parameters.AddWithValue("$id", id);
            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Role role)/////
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText =@"INSERT INTO roles (film_id, actor_id)
            VALUES ($film_id, $actor_id);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$film_id", role.film_id);
            command.Parameters.AddWithValue("$actor_id",role.actor_id);
            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        private static Actor GetActor(SqliteDataReader reader)//
        {
            Actor actor = new Actor();
            actor.id = int.Parse(reader.GetString(0));
            actor.fullname = reader.GetString(1);
            actor.country = reader.GetString(2);
            actor.age = int.Parse(reader.GetString(3));
            return actor;
        }
        public Film[] GetAllFilms(int id)////////
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText=@"SELECT films.id, title, genre, releaseYear
                                FROM films, roles WHERE roles.actor_id=$id AND roles.film_id = films.id";
            command.Parameters.AddWithValue("$id",id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Film> allFilms = new List<Film>();
            while(reader.Read()) 
            {
                Film film = GetFilm(reader);
                allFilms.Add(film);
            }
            reader.Close();
            Film[] films = new Film[allFilms.Count];
            allFilms.CopyTo(films);
            return films;
        }
        public Actor[] GetCast(int id)/////
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText=@"SELECT actors.id, fullname, country, age
                                FROM actors, roles WHERE roles.film_id=$id AND roles.actor_id = actors.id";
            command.Parameters.AddWithValue("$id",id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Actor> cast = new List<Actor>();
            while(reader.Read()) 
            {
                Actor actor = GetActor(reader);
                cast.Add(actor);
            }
            reader.Close();
            Actor[] allActors = new Actor[cast.Count];
            cast.CopyTo(allActors);
            return allActors;
        }
        private static Film GetFilm(SqliteDataReader reader)
        {
            Film film = new Film();
            film.id = int.Parse(reader.GetString(0));
            film.title = reader.GetString(1);
            film.genre = reader.GetString(2);
            film.releaseYear = int.Parse(reader.GetString(3));
            return film;
        }
        public bool IfExists(int filmId, int actorId)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText=@"SELECT * FROM roles WHERE film_id = $film_id AND actor_id = $actor_id";
            command.Parameters.AddWithValue("$film_id",filmId);
            command.Parameters.AddWithValue("$actor_id",actorId);
            SqliteDataReader reader = command.ExecuteReader();
            bool unique = false;
            if(reader.Read())
                unique = true;
            else
                unique = false;
            reader.Close();
            return unique;
        }
    }
}
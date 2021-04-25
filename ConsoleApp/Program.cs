using System;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp
{
    class Program
    {
        struct DatasetsFilePathes
        {
            public string fullnames;
            public string countries;
            public string revievs;
            public string titles;
            public string genres;
        }
        static void Main(string[] args)
        {
            DatasetsFilePathes dataFiles = new DatasetsFilePathes()
            {
                fullnames = @"C:\Users\Макс\myprojects\progbase3\data\generator\names.txt",
                countries = @"C:\Users\Макс\myprojects\progbase3\data\generator\countries.txt",
                titles = @"C:\Users\Макс\myprojects\progbase3\data\generator\titles.txt",
                genres = @"C:\Users\Макс\myprojects\progbase3\data\generator\genres.txt"
            };

            string databaseFileName = @"C:\Users\Макс\myprojects\progbase3\data\data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
            connection.Open();
            ConnectionState state = connection.State;
            if(state != ConnectionState.Open)
            {
                Console.WriteLine("Connection isn`t opened");
                Environment.Exit(1);
            }
            bool exit = false;
            while(!exit)
            {
                Console.WriteLine(@"Enter the entity that you want to generate:
                1.Actor
                2.Film
                3.Review
                4.Exit");
                string input = Console.ReadLine(); 
                switch (input)
                {
                    case "1":
                        ActorRepository actorRepository = new ActorRepository(connection);
                        ProcessActorsGenerator(actorRepository, dataFiles);
                        Console.WriteLine("Successfully");
                        break;
                    case "2":
                        FilmRepository filmRepository = new FilmRepository(connection);
                        ProcessFilmsGenerator(filmRepository, dataFiles);
                        break;
                    case "3":

                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.Error.WriteLine("Error: invalid input");
                        break;
                }
            }
            connection.Close();
        }
        static void ProcessActorsGenerator(ActorRepository repository,DatasetsFilePathes dataFiles)
        {
            int amount, ageL, ageH;
            amount = GetAmountOfEntities();
            while(true)
            {
                Console.WriteLine("Enter the range of actors` age (only in range from 4 to 95)\n1 num:");
                if(!int.TryParse(Console.ReadLine(), out ageL) || ageL<=3 || ageL >94)
                {
                    Console.Error.WriteLine("Error: invalid minimum age of actors");
                    continue;
                }
                Console.WriteLine("2 num:");
                if(!int.TryParse(Console.ReadLine(), out ageH) || ageH<ageL || ageH>95)
                {
                    Console.Error.WriteLine("Error: invalid maximum age of actors");
                    continue;
                }
                break;
            }
            string[] fullnames = File.ReadAllText(dataFiles.fullnames).Split("\n");
            string[] countries = File.ReadAllText(dataFiles.countries).Split("\n");
            Random rand = new Random();
            for(int i = 0; i < amount; i++)
            {
                Actor actor = new Actor();
                actor.fullname = fullnames[rand.Next(0,fullnames.Length)];
                actor.country = countries[rand.Next(0,countries.Length)];
                actor.age = rand.Next(ageL,ageH+1);
                repository.Insert(actor);
            }
        }
        static void ProcessFilmsGenerator(FilmRepository repository, DatasetsFilePathes dataFiles)
        {
            int amount, releaseYearsL, releaseYearsH;
            amount = GetAmountOfEntities();
            while(true)
            {
                Console.WriteLine("Enter the range of films` release year (only in range from 1910 to 2021)\n1 num:");
                if(!int.TryParse(Console.ReadLine(), out releaseYearsL) || releaseYearsL<1910 || releaseYearsL>2021)
                {
                    Console.Error.WriteLine("Error: invalid minimum year parameter");
                    continue;
                }
                Console.WriteLine("2 num:");
                if(!int.TryParse(Console.ReadLine(), out releaseYearsH) || releaseYearsH<releaseYearsL || releaseYearsH>2021)
                {
                    Console.Error.WriteLine("Error: invalid maximum year parameter");
                    continue;
                }
                break;
            }
            string[] titles = File.ReadAllText(dataFiles.titles).Split("\n");
            string[] genres = File.ReadAllText(dataFiles.genres).Split("\n");
            Random rand = new Random();
            for(int i = 0; i < amount; i++)
            {
                Film film = new Film();
                film.title = titles[rand.Next(0,titles.Length)];
                film.genre = genres[rand.Next(0,genres.Length)];
                film.releaseYear = rand.Next(releaseYearsL,releaseYearsH+1);
                repository.Insert(film);
            }
        }
        static int GetAmountOfEntities()
        {
            while(true)
            {
                Console.WriteLine("Enter the amount of the entities:");
                int amount;
                if(!int.TryParse(Console.ReadLine(), out amount))
                {
                    Console.Error.WriteLine("Error: invalid input");
                    continue;
                }
                return amount;
            }
        }

    }
    class User
    {
        public int id;
        public string username;
        public string password;
        public string fullname;
        public string role;
        public DateTime registrationDate;
        
    }
    class Actor
    {
        public int id;
        public string fullname;
        public string country;
        public int age;
        public Actor()
        {
            this.id = 0;
            this.fullname = "";
            this.country = "";
            this.age = 0;
        }
        public Actor(string fullName, string country, int age)
        {
            this.fullname = fullName;
            this.country = country;
            this.age = age;
        }
        public override string ToString()
        {
            return $"[{id}] {fullname} : {country}; [{age}]";
        }
    }
    class Film
    {
        public int id;
        public string title;
        public string genre;
        public int releaseYear;
        public Film()
        {
            this.id = 0;
            this.title = "";
            this.genre = "";
            this.releaseYear = 0;
        }
        public Film(string title, string genre, int releaseYear)
        {
            this.title = title;
            this.genre = genre;
            this.releaseYear = releaseYear;
        }
    }
    class Review
    {
        public int id;
        public string content;
        public int rating;
        public DateTime createdAt;
        public Review()
        {
            this.id = 0;
            this.content = "";
            this.rating = 0;
            this.createdAt = DateTime.Now;
        }
        public Review(string content, int rating, DateTime createdAt)
        {
            this.content = content;
            this.rating = rating;
            this.createdAt = createdAt;
        }
    }
    class ActorRepository
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
        public Actor GetById(int id)
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
        public int DeleteById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Actor actor)
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
        public int GetTotalPages()
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(this.GetCount() / (double)pageSize);
        }
        private long GetCount()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public List<Actor> GetPage(int page)
        {
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors LIMIT $pagesize OFFSET $offset";
            command.Parameters.AddWithValue("$pagesize", pageSize);
            command.Parameters.AddWithValue("$offset", pageSize*(page-1));

            SqliteDataReader reader = command.ExecuteReader();
            List<Actor> actors = new List<Actor>();
            while(reader.Read())
            {
                Actor actor = GetActor(reader);
                
                actors.Add(actor);
            }
            reader.Close();
            return actors;
        }
        public List<Actor> GetExport(string valueX)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE user LIKE $valueX";
            command.Parameters.AddWithValue("$valueX", valueX);

            SqliteDataReader reader = command.ExecuteReader();
            List<Actor> actorsToExport = new List<Actor>(); 
            while(reader.Read())
            {
                Actor actor = GetActor(reader);
                
                actorsToExport.Add(actor);
            }
            reader.Close();
            return actorsToExport;
        }
    }
    class FilmRepository
    {
        private SqliteConnection connection;
        public FilmRepository(SqliteConnection connection)
        {
            this.connection = connection;
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
        public Film GetById(int id)
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
        public int DeleteById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Film film)
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
        public int GetTotalPages()
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(this.GetCount() / (double)pageSize);
        }
        private long GetCount()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public List<Film> GetPage(int page)
        {
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films LIMIT $pagesize OFFSET $offset";
            command.Parameters.AddWithValue("$pagesize", pageSize);
            command.Parameters.AddWithValue("$offset", pageSize*(page-1));

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
        public List<Film> GetExport(string valueX)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films WHERE user LIKE $valueX";
            command.Parameters.AddWithValue("$valueX", valueX);

            SqliteDataReader reader = command.ExecuteReader();
            List<Film> filmsToExport = new List<Film>();
            while(reader.Read())
            {
                Film film = GetFilm(reader);
                
                filmsToExport.Add(film);
            }
            reader.Close();
            return filmsToExport;
        }
    }
     class ReviewRepository
    {
        private SqliteConnection connection;
        public ReviewRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        private static Review GetReview(SqliteDataReader reader)
        {
            Review review = new Review();
            review.id = int.Parse(reader.GetString(0));
            review.content = reader.GetString(1);
            review.rating = int.Parse(reader.GetString(2));
            review.createdAt= DateTime.Parse(reader.GetString(3));
            return review;
        }
        public Review GetById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            Review review = new Review();
            if(reader.Read())
                review= GetReview(reader);
            else
                review = null;
            reader.Close();
            return review;
        }
        public int DeleteById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Review review)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText =@"INSERT INTO reviews (content, rating, createdAt)
            VALUES ($content, $rating, $createdAt);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$content", review.content);
            command.Parameters.AddWithValue("$rating",review.rating);
            command.Parameters.AddWithValue("$createdAt", review.createdAt.ToString("o"));
            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public int GetTotalPages()
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(this.GetCount() / (double)pageSize);
        }
        private long GetCount()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public List<Review> GetPage(int page)
        {
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews LIMIT $pagesize OFFSET $offset";
            command.Parameters.AddWithValue("$pagesize", pageSize);
            command.Parameters.AddWithValue("$offset", pageSize*(page-1));

            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while(reader.Read())
            {
                Review review = new Review();
                
                reviews.Add(review);
            }
            reader.Close();
            return reviews;
        }
        public List<Review> GetExport(string valueX)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE user LIKE $valueX";
            command.Parameters.AddWithValue("$valueX", valueX);

            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviewsToExport = new List<Review>();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                
                reviewsToExport.Add(review);
            }
            reader.Close();
            return reviewsToExport;
        }
    }
}

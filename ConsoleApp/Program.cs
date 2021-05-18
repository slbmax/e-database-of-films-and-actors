using System;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleApp
{
    class Program
    {
        class CurrentInsertedEntities
        {
            public List<int> actorIds;
            public List<int> filmIds;
            public CurrentInsertedEntities()
            {
                this.actorIds = new List<int>();
                this.filmIds = new List<int>();
            }
        }
        struct DatasetsFilePathes
        {
            public string fullnames;
            public string countries;
            public string reviews;
            public string titles;
            public string genres;
            public string usernames;
            public string passwords;
        }
        static void Main(string[] args)
        {





            DatasetsFilePathes dataFiles = new DatasetsFilePathes()
            {
                fullnames = @"C:\Users\Макс\myprojects\progbase3\data\generator\names.txt",
                countries = @"C:\Users\Макс\myprojects\progbase3\data\generator\countries.txt",
                titles = @"C:\Users\Макс\myprojects\progbase3\data\generator\titles.txt",
                genres = @"C:\Users\Макс\myprojects\progbase3\data\generator\genres.txt",
                reviews = @"C:\Users\Макс\myprojects\progbase3\data\generator\reviews.txt",
                usernames = @"C:\Users\Макс\myprojects\progbase3\data\generator\usernames.txt",
                passwords = @"C:\Users\Макс\myprojects\progbase3\data\generator\passwords.txt"
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

            Service repositories = new Service(connection);


            CurrentInsertedEntities currInsEnt = new CurrentInsertedEntities();



            /* List<Review> reviews = repositories.reviewRepository.GetAllFilmReviews(1);
            XML xml = new XML();
            xml.Export("./opa.xml", reviews);
            connection.Close();
            Environment.Exit(0); */
            
            /* List<Review> reviews =XML.Import("a");
            foreach(Review re in reviews)
                repositories.reviewRepository.Insert(re);
            connection.Close();
            Environment.Exit(0);  */
            


            bool exit = false;
            while(!exit)
            {
                Console.WriteLine(@"Enter the entity that you want to generate:
                1.Actor
                2.Film
                3.Review
                4.User
                5.Exit");
                string input = Console.ReadLine(); 
                switch (input)
                {
                    case "1":
                        ProcessActorsGenerator(repositories.actorRepository, dataFiles, currInsEnt);
                        Console.WriteLine("Actors were generated successfully");
                        break;
                    case "2":
                        ProcessFilmsGenerator(repositories.filmRepository, dataFiles, currInsEnt);
                        Console.WriteLine("Films were generated successfully");
                        break;
                    case "3":
                        ProcessReviewsGenerator(repositories, dataFiles);
                        break;
                    case "4":
                        ProcessUsersGenerator(repositories.userRepository, dataFiles);
                        Console.WriteLine("Users were generated successfully");
                        break;
                    case "5":
                        if(repositories.actorRepository.GetCount()==0 && repositories.filmRepository.GetCount()!=0)
                            Console.WriteLine("Error: Films can`t be without actors: please, generate them too");
                        else if((repositories.actorRepository.GetCount()!=0 && repositories.filmRepository.GetCount()==0))
                            Console.WriteLine("Error: There aren`t any films for actors: please, generate them too");
                        else
                            exit = true;
                        break;
                    default:
                        Console.Error.WriteLine("Error: invalid input");
                        break;
                }
            }
            int[] filmIds = new int[1];  
            int[] actorIds = new int[1];
            bool actorLinks = false;
            bool filmLinks = false;
            if(currInsEnt.actorIds.Count!=0)
            {
                actorLinks = true;
                actorIds = new int[currInsEnt.actorIds.Count];
                currInsEnt.actorIds.CopyTo(actorIds);
            }
            else
                actorIds = repositories.actorRepository.GetAllIds();
            if(currInsEnt.filmIds.Count!=0)
            {
                filmLinks=true;
                filmIds = new int[currInsEnt.filmIds.Count];
                currInsEnt.filmIds.CopyTo(filmIds);
            }
            else
                filmIds = repositories.filmRepository.GetAllIds();
            if(actorLinks || filmLinks)
                Console.WriteLine("Linking films and actors..");
            ProcessLinking(repositories, actorIds, filmIds, actorLinks, filmLinks);
            if(actorLinks || filmLinks)
                Console.WriteLine("Successfull");
///
            GUI.RunInterface(repositories);
            Environment.Exit(0);
///
            connection.Close();
        }
        static void ProcessActorsGenerator(ActorRepository repository,DatasetsFilePathes dataFiles, CurrentInsertedEntities currInsEnt)
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
            string[] fullnames = File.ReadAllText(dataFiles.fullnames).Split("\r\n");
            string[] countries = File.ReadAllText(dataFiles.countries).Split("\r\n");
            Random rand = new Random();
            for(int i = 0; i < amount; i++)
            {
                Actor actor = new Actor();
                actor.fullname = fullnames[rand.Next(0,fullnames.Length)];
                actor.country = countries[rand.Next(0,countries.Length)];
                actor.age = rand.Next(ageL,ageH+1);
                int id = repository.Insert(actor);
                currInsEnt.actorIds.Add(id);
            }
        }
        static void ProcessFilmsGenerator(FilmRepository repository, DatasetsFilePathes dataFiles, CurrentInsertedEntities currInsEnt)
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
            string[] titles = File.ReadAllText(dataFiles.titles).Split("\r\n");
            string[] genres = File.ReadAllText(dataFiles.genres).Split("\r\n");
            Random rand = new Random();
            for(int i = 0; i < amount; i++)
            {
                Film film = new Film();
                film.title = titles[rand.Next(0,titles.Length)];
                film.genre = genres[rand.Next(0,genres.Length)];
                film.releaseYear = rand.Next(releaseYearsL,releaseYearsH+1);
                int id = repository.Insert(film);
                currInsEnt.filmIds.Add(id);
            }
        }
        static void ProcessReviewsGenerator(Service repo, DatasetsFilePathes dataFiles)
        {
            if(repo.userRepository.GetCount() == 0)
            {
                Console.WriteLine("Error: there aren`t any users to generate reviews");
                return;
            }
            if(repo.filmRepository.GetCount() == 0)
            {
                Console.WriteLine("Error: there aren`t any films to generate reviews");
                return;
            }
            int amount = GetAmountOfEntities();
            DateTime createdAtL, createdAtH;
            DateTime minDateUser = repo.userRepository.GetMinRegDate();
            DateTime minDateFilm = repo.filmRepository.GetMinRegDate();
            DateTime minDate = minDateUser>minDateFilm ? minDateUser : minDateFilm;
            while(true)
            {
                Console.WriteLine($"Enter the range of reviews` creation date (only in range from {minDate.ToString("o")} to 2020)\n1 num:");
                if(!DateTime.TryParse(Console.ReadLine(), out createdAtL) || createdAtL<=minDate || createdAtL.Year>2020)
                {
                    Console.Error.WriteLine("Error: invalid first date");
                    continue;
                }
                Console.WriteLine("2 num:");
                if(!DateTime.TryParse(Console.ReadLine(), out createdAtH) || createdAtH<createdAtL || createdAtH.Year>2020)
                {
                    Console.Error.WriteLine("Error: invalid second date");
                    continue;
                }
                break;
            }
            string[] reviews = File.ReadAllText(dataFiles.reviews).Split("\r\n");
            Random rand = new Random();
            TimeSpan range = createdAtH - createdAtL;
            for(int i = 0; i<amount; i++)
            {
                Review review = new Review();
                int reviewPos = rand.Next(0,reviews.Length);
                review.content = reviews[reviewPos];
                if(reviewPos<reviews.Length/2.0)
                    review.rating=rand.Next(5,11);
                else
                    review.rating = rand.Next(1,6);
                TimeSpan randDate = new TimeSpan((long)(rand.NextDouble() * range.Ticks));
                review.createdAt = createdAtL + randDate;
                review.user_id = repo.userRepository.GetUserForReview(review);
                review.film_id = repo.filmRepository.GetFilmForReview(review);
                repo.reviewRepository.Insert(review);
            }
            Console.WriteLine("Reviews were generated successfully");
        }
        static void ProcessUsersGenerator(UserRepository repository, DatasetsFilePathes dataFiles)
        {
            int amount;
            DateTime regL, regH;
            amount = GetAmountOfEntities();
            while(true)
            {
                Console.WriteLine("Enter the range of users` registration date (only in range from 2000 to 2020)\n1 num:");
                if(!DateTime.TryParse(Console.ReadLine(), out regL) || regL.Year<2000 || regL.Year>2020)
                {
                    Console.Error.WriteLine("Error: invalid first date");
                    continue;
                }
                Console.WriteLine("2 num:");
                if(!DateTime.TryParse(Console.ReadLine(), out regH) || regH<regL || regH.Year>2020)
                {
                    Console.Error.WriteLine("Error: invalid second date");
                    continue;
                }
                break;
            }
            string[] usernames = File.ReadAllText(dataFiles.usernames).Split("\r\n");
            string[] passwords = File.ReadAllText(dataFiles.passwords).Split("\r\n");
            string[] fullnames = File.ReadAllText(dataFiles.fullnames).Split("\r\n");
            int[] roles = new int[]{0,0,0,0,1,0,0,0,0};
            Random rand = new Random();
            TimeSpan range = regH - regL;
            for(int i = 0; i<amount; i++)
            {
                User user = new User();
                user.username=usernames[rand.Next(0,usernames.Length)]+i;
                user.password=passwords[rand.Next(0,passwords.Length)];
                user.fullname=fullnames[rand.Next(0,fullnames.Length)];
                int role = roles[rand.Next(0,roles.Length)];
                if(role == 1)
                    user.role = "moderator";
                else
                    user.role = "user";
                TimeSpan randDate = new TimeSpan((long)(rand.NextDouble() * range.Ticks));
                user.registrationDate = regL + randDate;
                repository.Insert(user);
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
                if(amount <= 0)
                {
                    Console.Error.WriteLine("Error: invalid amount");
                    continue;
                }
                return amount;
            }
        }
        static void ProcessLinking(Service repo, int[] actors, int[] films, bool actorLinks, bool filmLinks)
        {
            Random rand = new Random();
            if(actorLinks)
            {
                for(int i =0; i< actors.Length; i++)
                {
                    int amount = rand.Next(2,11);
                    if(amount>films.Length)
                        amount=films.Length;
                    for(int j = 0;j< amount; j++)
                    {
                        int randFilm = rand.Next(0,films.Length);
                        if(repo.roleRepository.IfExists(films[randFilm],actors[i]))
                        {
                            j--;
                            continue;
                        }
                        repo.roleRepository.Insert(new Role{actor_id = actors[i], film_id = films[randFilm]});       
                    }
                }
            }
            if(filmLinks)
            {
                for(int i = 0; i<films.Length; i++)
                {
                    int amount = rand.Next(4,13);
                    if(amount>actors.Length)
                        amount = actors.Length;
                    for(int j = 0; j<amount; j++)
                    {
                        int randActor = rand.Next(0, actors.Length);
                        if(repo.roleRepository.IfExists(films[i],actors[randActor]))
                        {
                            if(amount == actors.Length && actorLinks)
                                break;
                            j--;
                            continue;
                        }
                        repo.roleRepository.Insert(new Role{actor_id = actors[randActor],film_id=films[i]});
                    }
                }
            }
        }

    }
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
    public class Actor
    {
        public int id;
        public string fullname;
        public string country;
        public int age;
        public Film[] films;
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
            return $"[{id}] {fullname}, {age} -- {country}";
        }
    }
    public class Film
    {
        public int id;
        public string title;
        public string genre;
        public int releaseYear;
        public Review[] reviews;
        public Actor[] actors;
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
        public override string ToString()
        {
            return $"[{id}] {title} [{releaseYear}] -- {genre}";
        }
    }
    [XmlType(TypeName="review")]
    public class Review
    {
        public int id;
        public string content;
        public int rating;
        public DateTime createdAt;
        public int user_id;
        public int film_id;
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
        public override string ToString()
        {
            return $"[{id}] {content} [{rating}] -- {createdAt}";
        }
    }
    public class Role
    {
        public int id;
        public int actor_id;
        public int film_id;
        public Role()
        {
            this.id = 0;
            this.actor_id =0;
            this.film_id =0;
        }
    }
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
        public List<Actor> GetAll()
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors";
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
        public long GetCount()
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
            command.CommandText = @"SELECT * FROM actors WHERE user LIKE $valueX";
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
        public bool Update(Actor actor)
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
    }
    public class FilmRepository
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
        public bool Update(Film film)
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
        public long GetCount()
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
        public List<Film> GetAll()
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
        public int GetFilmForReview(Review review)
        {
            List<Film> filmsList = GetAll();
            Film[] films = new Film[filmsList.Count];
            filmsList.CopyTo(films);
            Random rand = new Random();
            int randId = rand.Next(0,films.Length);
            for(int i = randId+1; i<=films.Length; i++)
            {
                if(i == films.Length)
                    i = 0;
                if(films[i].releaseYear <= review.createdAt.Year)
                    return films[i].id;
            }
            return films[0].id;
        }
        public DateTime GetMinRegDate()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT releaseYear FROM films";
            SqliteDataReader reader = command.ExecuteReader();
            DateTime min = DateTime.Now;
            while(reader.Read())
            {
                int year = int.Parse(reader.GetString(0));
                DateTime current = new DateTime(year,1,1);
                if(current < min)
                    min = current;
            }
            reader.Close();
            return min;
        }
        public int[] GetAllIds()
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
    }
    public class ReviewRepository
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
        public bool Update(Review review)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE reviews SET content = $content, rating = $r, createdAt = $cr WHERE id = $id";
            command.Parameters.AddWithValue("$id", review.id);
            command.Parameters.AddWithValue("$content", review.content);
            command.Parameters.AddWithValue("$r", review.rating);
            command.Parameters.AddWithValue("$cr", review.createdAt.ToString("o"));
            int nChanged = command.ExecuteNonQuery();
            return nChanged == 1;
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
            command.CommandText =@"INSERT INTO reviews (content, rating, createdAt, user_id, film_id)
            VALUES ($content, $rating, $createdAt, $user_id, $film_id);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$content", review.content);
            command.Parameters.AddWithValue("$rating",review.rating);
            command.Parameters.AddWithValue("$createdAt", review.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$user_id",review.user_id);
            command.Parameters.AddWithValue("$film_id",review.film_id);
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
                Review review = GetReview(reader);
                
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
        public Review[] GetAllAuthorReviews(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE user_id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> userReviews = new List<Review>();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                
                userReviews.Add(review);
            }
            reader.Close();
            Review[] allUserReviews = new Review[userReviews.Count];
            userReviews.CopyTo(allUserReviews);
            return allUserReviews;
        }
        public List<Review> GetAllFilmReviews(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE film_id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> filmReviews = new List<Review>();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                review.film_id = int.Parse(reader.GetString(5)); 
                review.user_id = int.Parse(reader.GetString(4));
                
                filmReviews.Add(review);
            }
            reader.Close();
            
            return filmReviews;
        }
    }
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
    }
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
        public Role GetById(int id)
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
        public int DeleteById(int id, string role)
        {
            string entity_id = "film_id";
            if(role == "actor") entity_id = "actor_id";
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = $"DELETE FROM roles WHERE {entity_id} = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Role role)
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
        private static Actor GetActor(SqliteDataReader reader)
        {
            Actor actor = new Actor();
            actor.id = int.Parse(reader.GetString(0));
            actor.fullname = reader.GetString(1);
            actor.country = reader.GetString(2);
            actor.age = int.Parse(reader.GetString(3));
            return actor;
        }
        public Film[] GetAllFilms(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText=@"SELECT films.id, title, genre, releaseYear
                                FROM films, roles WHERE roles.actor_id=$id AND roles.film_id = films.id";
            command.Parameters.AddWithValue("$film_id",id);
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
        public Actor[] GetCast(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText=@"SELECT actors.id, fullname, country, age
                                FROM actors, roles WHERE roles.film_id=2 AND roles.actor_id = actors.id";
            command.Parameters.AddWithValue("$film_id",id);
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
    public class Service
    {
        public ActorRepository actorRepository;
        public FilmRepository filmRepository;
        public ReviewRepository reviewRepository;
        public UserRepository userRepository;
        public RoleRepository roleRepository;
        public Service(SqliteConnection connection)
        {
            this.actorRepository = new ActorRepository(connection);
            this.filmRepository = new FilmRepository(connection);
            this.reviewRepository = new ReviewRepository(connection);
            this.userRepository = new UserRepository(connection);
            this.roleRepository = new RoleRepository(connection);
        }
    }
}
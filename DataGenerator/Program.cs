using System;
using ClassLib;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Data;
using System.Collections.Generic;

namespace DataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
             DatasetsFilePathes dataFiles = new DatasetsFilePathes()
            {
                fullnames = @".\..\data\generator\names.txt",
                countries = @".\..\data\generator\countries.txt",
                titles = @".\..\data\generator\titles.txt",
                genres = @".\..\data\generator\genres.txt",
                reviews = @".\..\data\generator\reviews.txt",
                usernames = @".\..\data\generator\usernames.txt",
                passwords = @".\..\data\generator\passwords.txt"
            };

            string databaseFileName = @".\..\data\data.db";
            if(!File.Exists(databaseFileName))
            {
                Console.WriteLine("Error: invalid database filepath");
                Environment.Exit(1);
            }
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
}

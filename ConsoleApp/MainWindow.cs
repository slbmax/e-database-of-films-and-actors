using Terminal.Gui;
using System.IO;
using ClassLib;
using System.Collections.Generic;
namespace ConsoleApp
{
    public class MainWindow : Window
    {
        private static Service repo;
        private User user;
        private Label labUser; 
        private Button promote;
        private Button createActorBut;
        private Button createFilmBut;
        public MainWindow()
        {
            this.Title = "FilMax";this.X = 0; this.Y =1;

            Label labWelcome = new Label("Hello! Glad to see you!\nThis is a FilMax - an e-database of films and actors");
            labWelcome.X = Pos.Percent(28);
            labWelcome.Y = 4;
            labWelcome.TextAlignment = TextAlignment.Centered;
            
            labUser = new Label();
            labUser.X = 1;
            labUser.Y = Pos.Percent(98);

            Button browseFilmsBut = new Button("Browse films");
            browseFilmsBut.X = Pos.Percent(15);
            browseFilmsBut.Y = Pos.Top(labWelcome)+7;
            browseFilmsBut.Clicked+=OnPageFilmButton;

            Button browseActorsBut = new Button("Browse actors");
            browseActorsBut.X = Pos.Percent(15);
            browseActorsBut.Y = Pos.Bottom(browseFilmsBut)+2;
            browseActorsBut.Clicked += OnPageActorButton;


            Button browseReviewsBut = new Button("Browse reviews");
            browseReviewsBut.X = Pos.Percent(15);
            browseReviewsBut.Y = Pos.Bottom(browseActorsBut)+2;
            browseReviewsBut.Clicked += OnPageReviewButton;

            Button profileBut = new Button("My profile");
            profileBut.X = Pos.Percent(70);
            profileBut.Y = Pos.Top(browseFilmsBut);
            profileBut.Clicked+= OnProfileButton;

            Button createRewBut = new Button("Write a review");
            createRewBut.X = Pos.Percent(70);
            createRewBut.Y = Pos.Top(browseActorsBut);
            createRewBut.Clicked += OnCreateReviewButton;

            promote = new Button("Promote to\nmoderator");
            promote.TextAlignment = TextAlignment.Centered;
            promote.X = Pos.Percent(15);
            promote.Y = Pos.Bottom(browseReviewsBut)+2;
            promote.Clicked += OnPromote;

            createActorBut = new Button("Create actor");
            createActorBut.X = Pos.Percent(70);
            createActorBut.Y = Pos.Bottom(createRewBut) + 2;
            createActorBut.Clicked+= OnCreateActorButton;

            createFilmBut = new Button("Create film");
            createFilmBut.X = Pos.Percent(70);
            createFilmBut.Y = Pos.Bottom(createActorBut) + 2;
            createFilmBut.Clicked+= OnCreateFilmButton;



            Button logOutBut = new Button("Log out");
            logOutBut.X = Pos.Percent(90);
            logOutBut.Y = Pos.Percent(98);
            logOutBut.Clicked+= OnLogOutButton;

            
            this.Add(labWelcome, browseFilmsBut,browseActorsBut,browseReviewsBut, createRewBut, profileBut, labUser,promote, logOutBut);
            this.Add(createActorBut, createFilmBut);
        }
        private void OnLogOutButton()
        {
            Application.Top.RemoveAll();
            GUI.OnRegistration();
        }
        public void SetService(Service service)
        {
           repo = service;
        }
        public void SetUser(User currUser)
        {
            this.user = currUser;
            user.reviews = repo.reviewRepository.GetAllAuthorReviews(user.id);
            labUser.Text = $"Logged in as: {user.username} ({user.fullname})"; 
            if(user.role == "user")
            {
                promote.Visible = false;
                createFilmBut.Visible = false;
                createActorBut.Visible = false;
            }
        }
        private void OnPromote()
        {
            PromoteDialog dialog = new PromoteDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                User userToUpdate = repo.userRepository.GetByUsername(dialog.GetInput());
                if(userToUpdate == null)
                {
                    MessageBox.ErrorQuery("Promote to moderator","Error: user not found", "OK");
                }
                else
                {
                    userToUpdate.role = "moderator";
                    repo.userRepository.Update(userToUpdate);
                    MessageBox.Query("Promote to moderator",$"User {userToUpdate.username} now is a moderator", "OK");
                }
            }
        }
        private static void OnQuit()
        {
            Application.RequestStop();
        }       
        private void OnCreateReviewButton()
        {
            CreateReviewDialog dialog = new CreateReviewDialog();
            dialog.SetRepositories(repo.reviewRepository, repo.filmRepository);
            dialog.SetUser(user);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
                review.id = repo.reviewRepository.Insert(review);
                user.reviews.Add(review);
                MessageBox.Query("Create review", "Review was created succesfully", "OK");
                OnReview(review);
            }
        }
        private void OnReview(Review review)
        {
            OpenReviewDialog dialog = new OpenReviewDialog();
            dialog.X = 2; dialog.Y = 2;
            dialog.Width = Dim.Fill() - 2; dialog.Height = 25;
            dialog.SetService(repo);
            dialog.SetReview(review);
            dialog.SetUser(user);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                repo.reviewRepository.DeleteById(review.id);
                MessageBox.Query("Delete review","Review was deleted succesfully","OK");
            }
            if(dialog.edited)
            {
                Review newReview = dialog.GetReview();
                newReview.id = review.id;
                newReview.createdAt = review.createdAt;
                repo.reviewRepository.Update(newReview);
            }
        }
        private void OnPageFilmButton()
        {
            ShowFilmsWind wind = new ShowFilmsWind();
            wind.SetService(repo);
            wind.SetUser(user);
            Application.Run(wind);
        }
        private void OnPageActorButton()
        {
            ShowActorsWind wind = new ShowActorsWind();
            wind.SetService(repo);
            wind.SetUser(user);
            Application.Run(wind);
        }
        private void OnPageReviewButton()
        {
            ShowReviewsWind wind = new ShowReviewsWind();
            wind.SetService(repo);
            wind.SetUser(user);
            Application.Run(wind);
        }
        private void OnProfileButton()
        {
            ProfileWindow wind = new ProfileWindow();
            wind.SetService(repo);
            wind.SetUser(user);
            Application.Run(wind);
        }
        private void OnCreateActorButton()
        {
            CreateActorDialog dialog = new CreateActorDialog();
            dialog.SetRepository(repo.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Actor actor = dialog.GetActor();
                int actorID = repo.actorRepository.Insert(actor);
                int[] filmsId = dialog.GetFilmsId();
                if(filmsId != null)
                {
                    foreach(int id in filmsId)
                    {
                        Role role = new Role(){actor_id = actorID, film_id = id};
                        repo.roleRepository.Insert(role);
                    }
                }
                actor.id = actorID;
                MessageBox.Query("Create actor", "Actor was created succesfully", "OK");
                OnActor(actor);
            }
        }
        private void OnActor(Actor actor)
        {
            OpenActorDialog dialog = new OpenActorDialog();
            actor.films = repo.roleRepository.GetAllFilms(actor.id);
            dialog.SetService(repo);
            dialog.SetActor(actor);
            dialog.SetUser(user);

            Application.Run(dialog);
            if(dialog.canceled)
            {
                return;
            }
            if(dialog.deleted)
            {
                repo.actorRepository.DeleteById(actor.id);
                repo.roleRepository.DeleteActorById(actor.id);
                MessageBox.Query("Delete actor","Actor was deleted succesfully","OK");          
            }
        }
        private void OnCreateFilmButton()
        {
            CreateFilmDialog dialog = new CreateFilmDialog();
            dialog.SetRepository(repo.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Film film = dialog.GetFilm();
                int filmID = repo.filmRepository.Insert(film);
                int[] actorsId = dialog.GetActorsId();
                if(actorsId != null)
                {
                    foreach(int id in actorsId)
                    {
                        Role role = new Role(){actor_id = id, film_id = filmID};
                        repo.roleRepository.Insert(role);
                    }
                }
                film.id = filmID;    
                MessageBox.Query("Create film", "Film was created succesfully", "OK"); 
                OnFilm(film);
            }
        }
        private void OnFilm(Film film)
        {
            film.reviews = repo.reviewRepository.GetAllFilmReviews(film.id);
            OpenFilmDialog dialog = new OpenFilmDialog();
            film.actors = repo.roleRepository.GetCast(film.id);
            dialog.SetService(repo);
            dialog.SetFilm(film);
            dialog.SetUser(user);

            Application.Run(dialog);

            if(dialog.canceled)
            {
                return;
            }
            if(dialog.deleted)
            {
                repo.filmRepository.DeleteById(film.id);
                repo.roleRepository.DeleteFilmById(film.id);
                repo.reviewRepository.DeleteByFilmId(film.id);
                MessageBox.Query("Delete film","Film was deleted succesfully","OK");
            }
            
        }
    }
}
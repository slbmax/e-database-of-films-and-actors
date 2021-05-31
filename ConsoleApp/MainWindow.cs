using Terminal.Gui;
namespace ConsoleApp
{
    public class MainWindow : Window
    {
        private static Service repo;
        public MainWindow()
        {
            this.Title = "FilMax";

            Label labWelcome = new Label("Hello! Glad to see you!\nThis is a FilMax - an e-database of films and actors");
            labWelcome.X = Pos.Percent(28);
            labWelcome.Y = 4;
            labWelcome.TextAlignment = TextAlignment.Centered;
            
            Label labUser = new Label("Logged in as:");
            labUser.X = 1;
            labUser.Y = Pos.Percent(98);

            Button browseFilmsBut = new Button("Browse films");
            browseFilmsBut.X = Pos.Percent(15);
            browseFilmsBut.Y = Pos.Top(labWelcome)+7;
            browseFilmsBut.Clicked+=OnPageFilmButton;

            Button browseActorsBut = new Button("Browse Actors");
            browseActorsBut.X = Pos.Percent(15);
            browseActorsBut.Y = Pos.Bottom(browseFilmsBut)+2;

            Button profileBut = new Button("My profile");
            profileBut.X = Pos.Percent(70);
            profileBut.Y = Pos.Top(browseFilmsBut);

            Button createRewBut = new Button("Write a review");
            createRewBut.X = Pos.Percent(70);
            createRewBut.Y = Pos.Top(browseActorsBut);
            createRewBut.Clicked += OnCreateReviewButton;

            Button logOutBut = new Button("Log out");
            logOutBut.X = Pos.Percent(90);
            logOutBut.Y = Pos.Percent(98);

            
            this.Add(labWelcome, browseFilmsBut,browseActorsBut,createRewBut, profileBut, labUser, logOutBut);
        }
        public void SetRepositories(Service service)
        {
           repo = service;
        }
        private static void OnQuit()
        {
            Application.RequestStop();
        }
        
        private static void OnCreateFilmButton()
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
            }
        }
        private static void OnCreateActorButton()
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
            }
        }
        private static void OnCreateReviewButton()
        {
            CreateReviewDialog dialog = new CreateReviewDialog();
            dialog.SetRepositories(repo.reviewRepository, repo.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
                review.id = repo.reviewRepository.Insert(review);
            }
        }
        private static void OnPageFilmButton()
        {
            ShowFilmsWind wind = new ShowFilmsWind();
            wind.SetRepositories(repo.filmRepository, repo.roleRepository, repo.actorRepository, repo.reviewRepository);
            Application.Run(wind);
        }
        private static void OnPageActorButton()
        {
            ShowActorsWind wind = new ShowActorsWind();
            wind.SetRepositories(repo.actorRepository, repo.roleRepository, repo.filmRepository);
            Application.Run(wind);
        }
        private static void OnPageReviewButton()
        {
            ShowReviewsWind wind = new ShowReviewsWind();
            wind.SetRepository(repo.reviewRepository);
            Application.Run(wind);
        }
    }
}
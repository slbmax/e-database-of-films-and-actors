using Terminal.Gui;
using System.Collections.Generic;
using System.IO;
namespace ConsoleApp
{
    public static class GUI
    {
        private static FrameView frameListEnt;
        private static ListView allListView;
        private static Service repo;
        public static void RunInterface(Service repositories)
        {
            repo = repositories;
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("File", new MenuItem [] {
                    new MenuItem ("Export","export reviews",null),
                    new MenuItem ("Import","import reviews",null),
                    new MenuItem ("Exit","",OnQuit)
                }),
                new MenuBarItem ("Help",new MenuItem [] {
                    new MenuItem ("Help","",OnQuit)
                })
            });
            Window win = new Window("e-database of films and actors")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(menu, win);

            int mainButtonsX = 2;
            Button createEntBut = new Button(mainButtonsX, 1, "Create new entity");
            createEntBut.Clicked += OnCreateButtonClicked;

            Button viewPageBut = new Button(mainButtonsX, 3, "View all entities");
            viewPageBut.Clicked += OnPageButtonClicked;

            Button viewEntity = new Button(mainButtonsX, 5, "Select entity");
            viewEntity.Clicked += OnViewEntityButtonClicked;

            
            win.Add(createEntBut, viewPageBut, viewEntity);
            Application.Run();

        }
        static void OnQuit()
        {
            Application.RequestStop();
        }
        static void OnCreateButtonClicked()
        {
            Window window = new Window("")
            {
                X = 2,
                Y = 14,
                Width = 27,
                Height = 14
            };
            Label label = new Label(0, 0, "Select entity to create:");
            Button cancelBut = new Button("Cancel");
            cancelBut.X = 1;
            cancelBut.Y = 10;
            cancelBut.Clicked += OnQuit;

            Button filmBut = new Button(1,2,"Film");
            filmBut.Clicked += OnCreateFilmButton;

            Button actorBut = new Button(1,4,"Actor");
            actorBut.Clicked += OnCreateActorButton;

            Button reviewBut = new Button(1,6,"Review");
            reviewBut.Clicked += OnCreateReviewButton;
            

            window.Add(label, cancelBut, filmBut, actorBut, reviewBut);

            Application.Run(window);
        }
        static void OnPageButtonClicked()
        {
            Window window = new Window("")
            {
                X = 2,
                Y = 14,
                Width = 27,
                Height = 14
            };
            Label label = new Label(0, 0, "Select entity to view:");
            Button cancelBut = new Button("Cancel");
            cancelBut.X = 1;
            cancelBut.Y = 10;
            cancelBut.Clicked += OnQuit;

            window.Add(label, cancelBut);

            Button filmBut = new Button(1,2,"Film");
            filmBut.Clicked += OnPageFilmButton;

            Button actorBut = new Button(1,4,"Actor");
            actorBut.Clicked += OnPageActorButton;

            Button reviewBut = new Button(1,6,"Review");
            reviewBut.Clicked += OnPageReviewButton;
            

            window.Add(label, cancelBut, filmBut, actorBut, reviewBut);

            Application.Run(window);
        }
        static void OnViewEntityButtonClicked()
        {

        }
        static void OnCreateFilmButton()
        {
            CreateFilmDialog dialog = new CreateFilmDialog();
            dialog.SetRepositories(repo.actorRepository, repo.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Film film = dialog.GetFilm();
                int filmID = repo.filmRepository.Insert(film);
                try{
                    int[] actorsId = dialog.GetActorsId();
                    foreach(int id in actorsId)
                    {
                        Role role = new Role(){actor_id = id, film_id = filmID};
                        repo.roleRepository.Insert(role);
                    }
                }
                catch{}
            }
        }
        static void OnCreateActorButton()
        {
            CreateActorDialog dialog = new CreateActorDialog();
            dialog.SetRepositories(repo.actorRepository, repo.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Actor actor = dialog.GetActor();
                int actorID = repo.actorRepository.Insert(actor);
                try{
                    int[] filmsId = dialog.GetFilmsId();
                    foreach(int id in filmsId)
                    {
                        Role role = new Role(){actor_id = actorID, film_id = id};
                        repo.roleRepository.Insert(role);
                    }
                }
                catch{}
            }
        }
        static void OnCreateReviewButton()
        {
            CreateReviewDialog dialog = new CreateReviewDialog();
            dialog.SetRepository(repo.reviewRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Review review = dialog.GetReview();
            }
        }
        static void OnPageFilmButton()
        {
            ShowFilmsWind wind = new ShowFilmsWind();
            wind.SetRepository(repo.filmRepository);
            Application.Run(wind);
        }
        static void OnPageActorButton()
        {
            ShowActorsWind wind = new ShowActorsWind();
            wind.SetRepositories(repo.actorRepository, repo.roleRepository);
            
            Application.Run(wind);
        }
        static void OnPageReviewButton()
        {
            ShowReviewsWind wind = new ShowReviewsWind();
            wind.SetRepository(repo.reviewRepository);
            Application.Run(wind);
        }
    }
}
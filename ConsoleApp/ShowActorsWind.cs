using Terminal.Gui;
using System.Collections.Generic;
namespace ConsoleApp
{
    public class ShowActorsWind : Window
    {
        public bool canceled;
        private ListView allActorsListView;
        private int pageSize = 10;
        private int page = 1;
        private ActorRepository actorRepo;
        private FilmRepository filmRepo;
        private RoleRepository roleRepo;
        public Label pagesLabelCur;
        public Label pagesLabelAll;
        private Button nextPageButton;
        private Button prevPageButton;
        public ShowActorsWind()
        {
            this.Title = "List of actors"; X = 30; Y = 3; Width = 87; Height = 25;
            Button cancelBut = new Button("Cancel"){X = Pos.Percent(85),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnQuit;
            allActorsListView = new ListView(new List<Actor>())
            {
                X = 1, Y = 0, Width = Dim.Fill(), Height = pageSize
            };
            allActorsListView.OpenSelectedItem += OnOpenActor;
            this.Add(cancelBut, allActorsListView);
            Label page = new Label("Page: ")
            {
                X = 1, Y = pageSize+2
            };
            pagesLabelCur = new Label("?"){X = Pos.Right(page), Y = Pos.Top(page), Width = 5};
            Label of = new Label(" of ")
            {
                X = Pos.Right(pagesLabelCur),Y = Pos.Top(page)
            };
            pagesLabelAll = new Label("?"){X = Pos.Right(of), Y = Pos.Top(page), Width = 5};
            prevPageButton = new Button(1,pageSize+5,"Previous page");
            prevPageButton.Clicked += OnPrevPageButClicked;
            nextPageButton = new Button("Next page"){X = Pos.Right(prevPageButton)+3, Y=Pos.Top(prevPageButton)};
            nextPageButton.Clicked += OnNextPageButClicked;
            this.Add(page,pagesLabelCur,of,pagesLabelAll,prevPageButton, nextPageButton);
        }
        public void SetRepositories(ActorRepository repository, RoleRepository roleRepo, FilmRepository filmRepo)
        {
            this.actorRepo = repository;
            this.roleRepo = roleRepo;
            this.filmRepo = filmRepo;
            ShowCurrPage();
        }
        private void ShowCurrPage()
        {
            this.pagesLabelCur.Text = page.ToString();
            int total = actorRepo.GetTotalPages();
            this.pagesLabelAll.Text = total.ToString();
            this.allActorsListView.SetSource(actorRepo.GetPage(page));
            if(total==0)
            {
                this.page = 0;
                this.pagesLabelCur.Text = page.ToString();
            }
        }
        private void OnQuit()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnNextPageButClicked()
        {
            int totalPages = actorRepo.GetTotalPages();
            if(page>=totalPages)
                return;
            this.page += 1;
            ShowCurrPage();
        }
        private void OnPrevPageButClicked()
        {
            if(this.page==1)
                return;
            this.page -=1;
            ShowCurrPage();
        }
        private void OnOpenActor(ListViewItemEventArgs args)
        {
            Actor actor = (Actor)args.Value;
            OpenActorDialog dialog = new OpenActorDialog();
            actor.films = roleRepo.GetAllFilms(actor.id);
            dialog.SetActor(actor);
            dialog.SetRepositories(actorRepo, filmRepo);

            Application.Run(dialog);

            if(dialog.deleted)
            {
                actorRepo.DeleteById(actor.id);
                roleRepo.DeleteActorById(actor.id);
                ShowCurrPage();
            }
            if(dialog.edited)
            {
                Actor newAc = dialog.GetActor();
                newAc.id = actor.id;
                actorRepo.Update(newAc);
                roleRepo.DeleteActorById(actor.id);
                int[] filmsId = dialog.GetFilmsId();

                if(filmsId != null)
                {
                    foreach(int id in filmsId)
                    {
                        Role role = new Role(){actor_id = newAc.id, film_id = id};
                        roleRepo.Insert(role);
                    }
                }
                ShowCurrPage();
            }
        }
    }
}
using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ConsoleApp
{
    public class ShowActorsWind : Window
    {
        public bool canceled;
        private ListView allActorsListView;
        private int pageSize = 10;
        private int page = 1;
        private Service service;
        public Label pagesLabelCur;
        public Label pagesLabelAll;
        private Button nextPageButton;
        private Button prevPageButton;
        public ShowActorsWind()
        {
            this.Title = "List of actors"; X = 10; Y = 4; Width = Dim.Fill()-10; Height = Dim.Fill()-4;
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
        public void SetService(Service service)
        {
            this.service = service;
            ShowCurrPage();
        }
        private void ShowCurrPage()
        {
            this.pagesLabelCur.Text = page.ToString();
            int total = service.actorRepository.GetTotalPages();
            this.pagesLabelAll.Text = total.ToString();
            this.allActorsListView.SetSource(service.actorRepository.GetPage(page));
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
            int totalPages = service.actorRepository.GetTotalPages();
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
            actor.films = service.roleRepository.GetAllFilms(actor.id);
            dialog.SetActor(actor);
            dialog.SetRepository(service.filmRepository);

            Application.Run(dialog);

            if(dialog.deleted)
            {
                service.actorRepository.DeleteById(actor.id);
                service.roleRepository.DeleteActorById(actor.id);
                MessageBox.Query("Delete actor","Actor was deleted succesfully","OK");
                int pages = service.actorRepository.GetTotalPages();
                if(page>pages && page >1) page += -1;
                ShowCurrPage();
            }
            if(dialog.edited)
            {
                Actor newAc = dialog.GetActor();
                newAc.id = actor.id;
                service.actorRepository.Update(newAc);
                service.roleRepository.DeleteActorById(actor.id);
                int[] filmsId = dialog.GetFilmsId();

                if(filmsId != null)
                {
                    foreach(int id in filmsId)
                    {   
                        if(id ==0 ) continue;
                        Role role = new Role(){actor_id = newAc.id, film_id = id};
                        service.roleRepository.Insert(role);
                    }
                }
                MessageBox.Query("Edit actor","Actor was edited succesfully","OK");
                ShowCurrPage();
            }
        }
    }
}
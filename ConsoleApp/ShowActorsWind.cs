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
        private TextField searchField;
        private string filtervalue ="";
        private User user;
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

            searchField = new TextField()
            {
                X = Pos.Right(nextPageButton)+2, Y = Pos.Top(nextPageButton), Width = Dim.Fill()-5
            };
            searchField.TextChanging += OnSearchEnter;
            this.Add(searchField);
        }
        private void OnSearchEnter(TextChangingEventArgs obj)
        {
            this.filtervalue = obj.NewText.ToString();
            page = 1;
            ShowCurrPage();
        }
        public void SetService(Service service)
        {
            this.service = service;
            ShowCurrPage();
        }
        public void SetUser(User currUser)
        {
            this.user = currUser; 
        }
        private void ShowCurrPage()
        {
            this.pagesLabelCur.Text = page.ToString();
            int total = service.actorRepository.GetSearchPagesCount(filtervalue);
            this.pagesLabelAll.Text = total.ToString();
            this.allActorsListView.SetSource(service.actorRepository.GetSearchPage(filtervalue, page));
            this.nextPageButton.Visible = true;
            this.prevPageButton.Visible = true;
            if(total==0)
            {
                this.page = 0;
                this.pagesLabelAll.Text ="x";
                this.pagesLabelCur.Text ="x";
                this.nextPageButton.Visible = false;
                this.prevPageButton.Visible = false;
                this.allActorsListView.SetSource(new List<string>(){"No results"});
            }
            Application.Refresh();
        }
        private void OnQuit()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnNextPageButClicked()
        {
            int totalPages = service.actorRepository.GetSearchPagesCount(filtervalue);
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
            Actor actor = new Actor();
            try{
            actor = (Actor)args.Value;}catch{return;}
            OpenActorDialog dialog = new OpenActorDialog();
            actor.films = service.roleRepository.GetAllFilms(actor.id);
            dialog.SetActor(actor);
            dialog.SetUser(user);
            dialog.SetService(service);

            Application.Run(dialog);
            if(dialog.canceled)
            {
                return;
            }
            if(dialog.deleted)
            {
                service.actorRepository.DeleteById(actor.id);
                service.roleRepository.DeleteActorById(actor.id);
                MessageBox.Query("Delete actor","Actor was deleted succesfully","OK");
                int pages = service.actorRepository.GetSearchPagesCount(filtervalue);
                if(page>pages && page >1) page += -1;
                ShowCurrPage();
            }
            if(dialog.edited)
            {
                ShowCurrPage();
            }
        }
    }
}
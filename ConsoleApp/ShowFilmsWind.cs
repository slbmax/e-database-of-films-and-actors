using Terminal.Gui;
using System.Collections.Generic;
namespace ConsoleApp
{
    public class ShowFilmsWind : Window
    {
        public bool canceled;
        private ListView allFilmsListView;
        private int pageSize = 10;
        private int page = 1;
        private FilmRepository repository;
        public Label pagesLabelCur;
        public Label pagesLabelAll;
        private Button nextPageButton;
        private Button prevPageButton;
        public ShowFilmsWind()
        {
            this.Title = "List of films"; X = 30; Y = 3; Width = 87; Height = 25;
            Button cancelBut = new Button("Cancel"){X = Pos.Percent(85),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnQuit;
            allFilmsListView = new ListView(new List<Film>())
            {
                X = 1, Y = 0, Width = Dim.Fill(), Height = pageSize
            };
            allFilmsListView.OpenSelectedItem += OnOpenActor;
            this.Add(cancelBut, allFilmsListView);
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
        public void SetRepository(FilmRepository repository)
        {
            this.repository = repository;
            ShowCurrPage();
        }
        private void ShowCurrPage()
        {
            this.pagesLabelCur.Text = page.ToString();
            int total = repository.GetTotalPages();
            this.pagesLabelAll.Text = total.ToString();
            this.allFilmsListView.SetSource(repository.GetPage(page));
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
            int totalPages = repository.GetTotalPages();
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
            Film film = (Film)args.Value;
            OpenFilmDialog dialog = new OpenFilmDialog();
            dialog.SetFilm(film);

            Application.Run(dialog);

            if(dialog.deleted)
            {
                repository.DeleteById(film.id);
                ShowCurrPage();
            }
            if(dialog.edited)
            {
                Film newFilm = dialog.GetFilm();
                newFilm.id = film.id;
                repository.Update(newFilm);
                ShowCurrPage();
            }
        }
    }
}
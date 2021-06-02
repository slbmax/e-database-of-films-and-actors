using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
namespace ConsoleApp
{
    public class ShowFilmsWind : Window
    {
        public bool canceled;
        private ListView allFilmsListView;
        private int pageSize = 10;
        private int page = 1;
        private Service service;
        public Label pagesLabelCur;
        public Label pagesLabelAll;
        private Button nextPageButton;
        private Button prevPageButton;
        public ShowFilmsWind()
        {
            this.Title = "List of films"; X = 10; Y = 4; Width = Dim.Fill()-10; Height = Dim.Fill()-4;
            Button cancelBut = new Button("Cancel"){X = Pos.Percent(87),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnQuit;
            allFilmsListView = new ListView(new List<Film>())
            {
                X = 1, Y = 1, Width = Dim.Fill(), Height = pageSize
            };
            allFilmsListView.OpenSelectedItem += OnOpenFilm;
            this.Add(cancelBut, allFilmsListView);
            Label page = new Label("Page: ")
            {
                X = 1, Y = pageSize+3
            };
            pagesLabelCur = new Label("?"){X = Pos.Right(page), Y = Pos.Top(page), Width = 5};
            Label of = new Label(" of ")
            {
                X = Pos.Right(pagesLabelCur),Y = Pos.Top(page)
            };
            pagesLabelAll = new Label("?"){X = Pos.Right(of), Y = Pos.Top(page), Width = 5};
            prevPageButton = new Button(1,pageSize+6,"Previous page");
            prevPageButton.Clicked += OnPrevPageButClicked;
            nextPageButton = new Button("Next page"){X = Pos.Right(prevPageButton)+3, Y=Pos.Top(prevPageButton)};
            nextPageButton.Clicked += OnNextPageButClicked;
            this.Add(page,pagesLabelCur,of,pagesLabelAll,prevPageButton, nextPageButton);
        }
        public void SetService(Service repo)
        {
            this.service = repo;
            ShowCurrPage();
        }
        private void ShowCurrPage()
        {
            this.pagesLabelCur.Text = page.ToString();
            int total = service.filmRepository.GetTotalPages();
            this.pagesLabelAll.Text = total.ToString();
            this.allFilmsListView.SetSource(service.filmRepository.GetPage(page));
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
            int totalPages = service.filmRepository.GetTotalPages();
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
        private void OnOpenFilm(ListViewItemEventArgs args)
        {
            Film film = (Film)args.Value;
            film.reviews = service.reviewRepository.GetAllFilmReviews(film.id);
            OpenFilmDialog dialog = new OpenFilmDialog();
            film.actors = service.roleRepository.GetCast(film.id);
            dialog.SetFilm(film);
            dialog.SetService(service);

            Application.Run(dialog);

            if(dialog.deleted)
            {
                service.filmRepository.DeleteById(film.id);
                service.roleRepository.DeleteFilmById(film.id);
                MessageBox.Query("Delete film","Film was deleted succesfully","OK");
                int pages = service.filmRepository.GetTotalPages();
                if(page>pages && page >1) page += -1;
                ShowCurrPage();
            }
            if(dialog.edited)
            {
                Film newFilm = dialog.GetFilm();
                newFilm.id = film.id;
                service.filmRepository.Update(newFilm);
                service.roleRepository.DeleteFilmById(film.id);
                int[] actorsId = dialog.GetActorsId();
                if(actorsId != null)
                {
                    foreach(int id in actorsId)
                    {
                        if(id ==0 ) continue;
                        Role role = new Role(){actor_id = id, film_id = newFilm.id};
                        service.roleRepository.Insert(role);
                    }
                }
                MessageBox.Query("Edit film","Film was edited succesfully","OK");
                ShowCurrPage();
            }
        }
    }
}
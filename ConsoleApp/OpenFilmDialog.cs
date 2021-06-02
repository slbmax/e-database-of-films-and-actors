using Terminal.Gui;
using ClassLib;
using System.Collections.Generic;
namespace ConsoleApp
{
    public class OpenFilmDialog : Window
    {
        public bool canceled;
        public bool deleted = false;
        public bool edited = false;
        private Service service;
        private Label filmTitle;
        private Label filmGenre;
        private Label filmYear;
        private ListView allActorsListView;
        private ListView allReviewsListView;
        private Film film;
        private int[] actorIntIds;
        public OpenFilmDialog()
        {
            this.Title = "Open Film"; this.Width = Dim.Fill(); this.Height = Dim.Fill();
            Button cancelBut = new Button("Cancel"){X = Pos.Percent(87),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.Add(cancelBut);

            int posX = 20;
            int width = 40;
            Label filmTitleLab = new Label(2,2,"Title:");
            filmTitle = new Label("")
            {
                X = posX, Y = Pos.Top(filmTitleLab), Width =width
            };
            /* filmTitle.ReadOnly = true; */
            this.Add(filmTitleLab,filmTitle);
            Label filmGenreLab = new Label(2,4,"Genre:");
            filmGenre = new Label("")
            {
                X = posX, Y = Pos.Top(filmGenreLab), Width =width
            };
            /* filmGenre.ReadOnly = true; */
            this.Add(filmGenreLab,filmGenre);
            Label filmYearLab = new Label(2,6,"Release year:");
            filmYear = new Label("")
            {
                X = posX, Y = Pos.Top(filmYearLab), Width =width
            };
            /* filmYear.ReadOnly = true; */
            this.Add(filmYearLab,filmYear);


            Label filmCastLab = new Label(2,8,"Cast:");
            this.Add(filmCastLab);


            allActorsListView = new ListView(new List<Actor>())
            {
                X = posX, Y = Pos.Top(filmCastLab), Width = Dim.Fill()-5, Height = 5
            };
            this.Add(allActorsListView);

            Label reviewLab = new Label(2, 15, "Reviews:");
            this.Add(reviewLab);

            allReviewsListView = new ListView(new List<Review>())
            {
                X = posX, Y = Pos.Top(reviewLab), Width = Dim.Fill()-5, Height = 5
            };
            allReviewsListView.OpenSelectedItem += OnOpenReview;
            this.Add(allReviewsListView);
            

            Button deleteFilm = new Button("Delete"){X = 2, Y = Pos.Bottom(reviewLab)+6};
            deleteFilm.Clicked += OnDeleteFilm;
            Button editFilm = new Button("Edit"){X = Pos.Right(deleteFilm)+2, Y = Pos.Top(deleteFilm)};
            editFilm.Clicked += OnEditFilm;
            this.Add(deleteFilm, editFilm);
        }
        private void OnOpenDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnOpenReview(ListViewItemEventArgs args)
        {
            Review review = new Review();
            try{
            review = (Review)args.Value;}
            catch{return;}
            OpenReviewDialog dialog = new OpenReviewDialog();
            dialog.deleteReview.Visible = false;
            dialog.editReview.Visible = false;
            dialog.X = 2; dialog.Y = 2;
            dialog.Width = Dim.Fill() - 2; dialog.Height = 25;
            dialog.SetRepositories(service.filmRepository, service.reviewRepository);
            dialog.SetReview(review);
            Application.Run(dialog);
        }
        private List<Actor> GetListOfActors()
        {
            List<Actor> actors = new List<Actor>();
            for(int i = 0; i<film.actors.Length; i ++)
            {
                actors.Add(film.actors[i]);
            }
            return actors;
        }
        public void SetFilm(Film film)
        {
            this.film = film;
            this.filmGenre.Text = film.genre;
            this.filmTitle.Text = film.title;
            this.filmYear.Text = film.releaseYear.ToString();
            allActorsListView.SetSource(GetListOfActors().Count !=0 ? GetListOfActors() : new List<string>(){"There aren`t any actors in this film"});
            allReviewsListView.SetSource(film.reviews.Count !=0 ? film.reviews : new List<string>(){"There aren`t any reviews in this film yet"});
        }
        private void OnDeleteFilm()
        {
            int index =MessageBox.Query("Delete film","Are you sure?","Yes","No");
            if(index == 0)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditFilm()
        {
            EditFilm dialog = new EditFilm();
            dialog.SetFilm(film);
            dialog.SetRepository(service.actorRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.edited = true;
                film = dialog.GetFilm();
                actorIntIds = dialog.GetActorsId();
                OnOpenDialogCanceled();
            }
        }
        public Film GetFilm()
        {
            return film;
        }
        public int[] GetActorsId()
        {
            return actorIntIds;
        }
        public void SetService(Service service)
        {
            this.service = service;
        }
    }
}
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
        private Label filmRating;
        private ListView allActorsListView;
        private ListView allReviewsListView;
        public Button deleteFilm;
        public Button editFilm;
        private Film film;
        private int[] actorIntIds;
        private User user;
        public OpenFilmDialog()
        {
            this.Title = "Open Film"; this.Width = Dim.Fill(); this.Height = Dim.Fill();
            Button cancelBut = new Button("Cancel"){X = Pos.Percent(87),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.Add(cancelBut);

            int posX = 20;
            int width = 40;
            Label filmTitleLab = new Label(2,2,"Title:");
            filmTitle = new Label(" ")
            {
                X = posX, Y = Pos.Top(filmTitleLab), Width =width
            };
            
            this.Add(filmTitleLab,filmTitle);
            Label filmGenreLab = new Label(2,4,"Genre:");
            filmGenre = new Label(" ")
            {
                X = posX, Y = Pos.Top(filmGenreLab), Width =width
            };
            
            this.Add(filmGenreLab,filmGenre);
            Label filmYearLab = new Label(2,6,"Release year:");
            filmYear = new Label(" ")
            {
                X = posX, Y = Pos.Top(filmYearLab), Width =width
            };
            
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

            Label filmRatingLab = new Label(2,22,"Rating:");
            this.Add(filmRatingLab);
            filmRating = new Label(" ")
            {
                X = Pos.Right(filmRatingLab)+1, Y = Pos.Top(filmRatingLab), Width =width
            };
            this.Add(filmRating);
            

            deleteFilm = new Button("Delete"){X = 2, Y = Pos.Bottom(reviewLab)+8};
            deleteFilm.Clicked += OnDeleteFilm;
            editFilm = new Button("Edit"){X = Pos.Right(deleteFilm)+2, Y = Pos.Top(deleteFilm)};
            editFilm.Clicked += OnEditFilm;
            this.Add(deleteFilm, editFilm);
        }
        private void OnOpenDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetUser(User user)
        {
            this.user = user;
            if(user.role == "user")
            {
                deleteFilm.Visible = false;
                editFilm.Visible = false;
            }
        }
        private void OnOpenReview(ListViewItemEventArgs args)
        {
            Review review = new Review();
            try{
            review = (Review)args.Value;}
            catch{return;}
            OnReview(review);
        }
        private void OnReview(Review review)
        {
            OpenReviewDialog dialog = new OpenReviewDialog();
            dialog.deleteReview.Visible = false;
            dialog.editReview.Visible = false;
            if(user.id == review.user_id)
            {
                dialog.deleteReview.Visible = true;
                dialog.editReview.Visible = true;
            }
            dialog.X = 2; dialog.Y = 2;
            dialog.Width = Dim.Fill() - 2; dialog.Height = 25;
            dialog.SetService(service);
            dialog.SetUser(user);
            dialog.SetReview(review);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                service.reviewRepository.DeleteById(review.id);
                MessageBox.Query("Delete review","Review was deleted succesfully","OK");
                film.reviews.Remove(review);
                allReviewsListView.SetSource(film.reviews.Count !=0 ? film.reviews : new List<string>(){"There aren`t any reviews in this film yet"});
                SetFilm(film);
            }
            if(dialog.edited)
            {
                Review newReview = dialog.GetReview();
                newReview.id = review.id;
                service.reviewRepository.Update(newReview);
                film.reviews.Remove(review);
                film.reviews.Add(newReview);
                allReviewsListView.SetSource(film.reviews);
                SetFilm(film);
            }
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
            allReviewsListView.SetSource(service.reviewRepository.GetAllFilmReviews(film.id).Count !=0 
            ? service.reviewRepository.GetAllFilmReviews(film.id)
            : new List<string>(){"There aren`t any reviews in this film yet"});
            this.filmRating.Text = service.reviewRepository.GetAllFilmReviews(film.id).Count !=0
            ? service.reviewRepository.GetFilmRating(film.id).ToString()
            : "no rating yet";
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
                Film newFilm = dialog.GetFilm();
                newFilm.id = film.id;
                service.filmRepository.Update(newFilm);
                service.roleRepository.DeleteFilmById(film.id);
                actorIntIds = dialog.GetActorsId();
                if(actorIntIds != null)
                {
                    foreach(int id in actorIntIds)
                    {
                        if(id ==0 ) continue;
                        Role role = new Role(){actor_id = id, film_id = newFilm.id};
                        service.roleRepository.Insert(role);
                    }
                }

                newFilm.actors = service.roleRepository.GetCast(film.id);
                newFilm.reviews = service.reviewRepository.GetAllFilmReviews(film.id);
                film = newFilm;
                SetFilm(film);
                MessageBox.Query("Edit film","Film was edited succesfully","OK");
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
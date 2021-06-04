using Terminal.Gui;
using System;
using System.IO;
using ClassLib;
using RPC;
namespace ConsoleApp
{
    public class CreateReviewDialog : Dialog
    {
        public bool canceled;
        protected RemoteReviewRepository reviewRepo;
        protected RemoteFilmRepository filmRepo;
        protected TextView reviewContentInp;
        protected TextField reviewRatingInp;
        protected TextField reviewFilmInp;
        protected User user;
        public CreateReviewDialog()
        {
            this.Title = "Create Review";
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.AddButton(cancelBut);
            this.AddButton(okBut);

            int posX = 20;

            Label reviewContentLab = new Label(4,2,"Your review:");
            reviewContentInp = new TextView()
            {
                X = posX, Y = Pos.Top(reviewContentLab), Width =Dim.Fill()-5, Height = 3
            };
            this.Add(reviewContentLab,reviewContentInp);


            Label reviewFilmLab = new Label(4,7,"For film(id):");
            reviewFilmInp = new TextField()
            {
                X = posX, Y = Pos.Top(reviewFilmLab), Width = 10
            };
            this.Add(reviewFilmLab, reviewFilmInp);

            Label reviewRatingLab = new Label(4,9,"Rating:");
            reviewRatingInp = new TextField()
            {
                X = posX, Y = Pos.Top(reviewRatingLab), Width =10
            };
            this.Add(reviewRatingLab,reviewRatingInp);

            

            Label remarkLbl = new Label(4,13,"Remark: \n-rating should be in range from 1 to 10");

            this.Add(remarkLbl);
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetUser(User currUser)
        {
            this.user = currUser; 
        }
        private void OnCreateDialogSubmit()
        {
            string error = "noerrors";
            int rating = 0;
            if(reviewContentInp.Text.ToString() == "")
                error = "Empty review content";
            else if(!int.TryParse(reviewRatingInp.Text.ToString(), out rating))
                error = "Invalid review rating value";
            else if(rating <1 || rating >10)
                error = "Review rating value is in invalid range";
            else{
                int filmId =0;
                if(!int.TryParse(reviewFilmInp.Text.ToString(), out filmId))
                {
                    error = "Invalid film id input";
                }
                else
                {
                    Film film = filmRepo.GetById(filmId);
                    if(film == null)
                    {
                        error = $"Non-existing film with id '{filmId}'";
                    }
                }
            }
            if(error == "noerrors")
            {
                this.canceled = false;
                Application.RequestStop();
            }
            else
                MessageBox.ErrorQuery("Error",$"{error}", "OK");
        }
        public Review GetReview()
        {
            return new Review()
            {
                content = reviewContentInp.Text.ToString(),
                createdAt = DateTime.Now,
                rating = int.Parse(reviewRatingInp.Text.ToString()),
                film_id = int.Parse(reviewFilmInp.Text.ToString()),
                user_id = user.id
            };
        }
        public void SetRepositories(RemoteReviewRepository repository, RemoteFilmRepository filmRepository)
        {
            this.reviewRepo = repository;
            this.filmRepo = filmRepository;
        }
    }
}
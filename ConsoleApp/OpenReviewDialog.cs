using Terminal.Gui;
using ClassLib;
namespace ConsoleApp
{
    public class OpenReviewDialog : Window
    {
        public bool canceled;
        public bool deleted = false;
        public bool edited = false;
        private Service service;
        private TextView reviewContent;
        private Label reviewRating;
        private Label reviewDate;
        private Label filmLab;
        private Label byUser;
        public Button deleteReview;
        public Button editReview;
        private Review review;
        private User user;
        public OpenReviewDialog()
        {
            this.Title = "Open review";
            Button cancelBut = new Button("Cancel") {X = Pos.Percent(87),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.Add(cancelBut);

            int posX = 15;
            Label reviewContentLab = new Label(2,1,"Content:");
            reviewContent = new TextView()
            {
                X = posX, Y = Pos.Top(reviewContentLab), Width =Dim.Fill()-posX, Height = 5
            };
            this.Add(reviewContentLab,reviewContent);

            Label filmLabel = new Label(2,8,"For film:");
            filmLab = new Label()
            {
                X = posX, Y = Pos.Top(filmLabel), Width = 10
            };
            this.Add(filmLabel, filmLab);

            Label reviewRatingLab = new Label(2,10,"Rating:");
            reviewRating = new Label(" ")
            {
                X = posX, Y = Pos.Top(reviewRatingLab), Width =10
            };
            this.Add(reviewRatingLab,reviewRating);
            Label reviewDateLab = new Label(2,12,"cr. Date:");
            reviewDate = new Label("")
            {
                X = posX, Y = Pos.Top(reviewDateLab), Width = 30
            };
            this.Add(reviewDateLab,reviewDate);

            Label userLab = new Label(2,14,"By user:");
            byUser = new Label()
            {
                X = posX, Y = Pos.Top(userLab)
            };
            this.Add(userLab,byUser);
            deleteReview = new Button("Delete"){X = 2, Y = Pos.Bottom(reviewDate)+6};
            deleteReview.Clicked += OnDeleteReview;
            editReview = new Button("Edit"){X = Pos.Right(deleteReview)+2, Y = Pos.Bottom(reviewDate)+6};
            editReview.Clicked += OnEditReview;
            this.Add(deleteReview, editReview);
        }
        private void OnOpenDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetReview(Review review)
        {
            this.review = review;
            this.reviewContent.Text = review.content;
            this.reviewRating.Text = review.rating.ToString();
            this.reviewDate.Text = review.createdAt.ToString();
            this.filmLab.Text = service.filmRepository.GetById(review.film_id).title.ToString();
            this.byUser.Text = service.userRepository.GetById(review.user_id).username.ToString();
        }
        public void SetUser(User user)
        {
            this.user = user;
        }
        private void OnDeleteReview()
        {
            int index =MessageBox.Query("Delete review","Are you sure?","Yes","No");
            if(index == 0)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditReview()
        {
            EditReview dialog = new EditReview();
            dialog.SetReview(review);
            dialog.SetUser(user);
            dialog.SetRepositories(service.reviewRepository, service.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.edited = true;
                Review newReview = dialog.GetReview();
                newReview.id = review.id;
                newReview.createdAt = review.createdAt;
                service.reviewRepository.Update(newReview);
                review = newReview;
                MessageBox.Query("Edit review","Review was edited succesfully","OK");
                SetReview(review);
            }
        }
        public Review GetReview()
        {
            return review;
        }
        public void SetService(Service service)
        {
            this.service= service;
        }
    }
}
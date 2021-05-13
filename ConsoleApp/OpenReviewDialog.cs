using Terminal.Gui;
namespace ConsoleApp
{
    public class OpenReviewDialog : Dialog
    {
        public bool canceled;
        public bool deleted = false;
        public bool edited = false;
        private TextField reviewContent;
        private TextField reviewRating;
        private TextField reviewDate;
        private Button deleteReview;
        private Review review;
        public OpenReviewDialog()
        {
            this.Title = "Open review";
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.AddButton(cancelBut);

            int posX = 20;
            Label reviewContentLab = new Label(2,2,"Content:");
            reviewContent = new TextField("")
            {
                X = posX, Y = Pos.Top(reviewContentLab), Width =40
            };
            reviewContent.ReadOnly = true;
            this.Add(reviewContentLab,reviewContent);
            Label reviewRatingLab = new Label(2,4,"Rating:");
            reviewRating = new TextField("")
            {
                X = posX, Y = Pos.Top(reviewRatingLab), Width =40
            };
            reviewRating.ReadOnly = true;
            this.Add(reviewRatingLab,reviewRating);
            Label reviewDateLab = new Label(2,6,"Date:");
            reviewDate = new TextField("")
            {
                X = posX, Y = Pos.Top(reviewDateLab), Width =40
            };
            this.Add(reviewDateLab,reviewDate);
            reviewDate.ReadOnly = true;

            deleteReview = new Button("Delete"){X = 2, Y = Pos.Bottom(reviewDate)+6};
            deleteReview.Clicked += OnDeleteReview;
            Button editReview = new Button("Edit"){X = Pos.Right(deleteReview)+2, Y = Pos.Bottom(reviewDate)+6};
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
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.edited = true;
                review = dialog.GetReview();
                OnOpenDialogCanceled();
            }
        }
        public Review GetReview()
        {
            return review;
        }
    }
}
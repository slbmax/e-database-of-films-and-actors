using Terminal.Gui;
using System;
namespace ConsoleApp
{
    public class CreateReviewDialog : Dialog
    {
        public bool canceled;
        public TextField reviewContentInp;
        public TextField reviewRatingInp;
        public TextField reviewCreatedAtInp;
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

            Label reviewContentLab = new Label(2,2,"Content:");
            reviewContentInp = new TextField()
            {
                X = posX, Y = Pos.Top(reviewContentLab), Width =40
            };
            this.Add(reviewContentLab,reviewContentInp);

            Label reviewRatingLab = new Label(2,4,"Rating:");
            reviewRatingInp = new TextField()
            {
                X = posX, Y = Pos.Top(reviewRatingLab), Width =40
            };
            this.Add(reviewRatingLab,reviewRatingInp);

            Label reviewCreatedAtLab = new Label(2,6,"Created At:");
            reviewCreatedAtInp = new TextField()
            {
                X = posX, Y = Pos.Top(reviewCreatedAtLab), Width =40
            };
            this.Add(reviewCreatedAtLab,reviewCreatedAtInp);

            Label remarkLbl = new Label(2,10,"Remark: \n-rating should be in range from 1 to 10;\n"+
            "-date should be in range from 2000 to 2021");

            this.Add(remarkLbl);
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnCreateDialogSubmit()
        {
            string error = "noerrors";
            int rating = 0;
            DateTime date = DateTime.Now;
            if(reviewContentInp.Text.ToString() == "")
                error = "Empty review content";
            else if(!int.TryParse(reviewRatingInp.Text.ToString(), out rating))
                error = "Invalid review rating value";
            else if(rating <1 || rating >10)
                error = "Review rating value is in invalid range";
            else if(!DateTime.TryParse(reviewCreatedAtInp.Text.ToString(), out date) ||date.Year <2000 || date>DateTime.Now)
                error = "Review created at value is in invalid range";
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
                createdAt = DateTime.Parse(reviewCreatedAtInp.Text.ToString()),
                rating = int.Parse(reviewRatingInp.Text.ToString())
            };
        }
    }
}
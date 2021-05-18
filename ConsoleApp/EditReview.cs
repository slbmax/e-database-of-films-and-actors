using Terminal.Gui;
namespace ConsoleApp
{
    public class EditReview : CreateReviewDialog
    {
        public EditReview()
        {
            this.Title = "Edit review";      
        }
        public void SetReview(Review review)
        {
            this.reviewContentInp.Text = review.content;
            //this.reviewCreatedAtInp.Text = review.createdAt.ToString();
            this.reviewRatingInp.Text = review.rating.ToString();
        }
    }
}
using Terminal.Gui;
using ClassLib;
namespace ConsoleApp
{
    public class EditReview : CreateReviewDialog
    {
        public EditReview()
        {
            this.Title = "Edit review"; 
            this.reviewFilmInp.ReadOnly = true;     
        }
        public void SetReview(Review review)
        {
            this.reviewContentInp.Text = review.content;
            this.reviewFilmInp.Text = review.film_id.ToString();
            this.reviewRatingInp.Text = review.rating.ToString();
        }
    }
}
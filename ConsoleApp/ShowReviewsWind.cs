using Terminal.Gui;
using System.Collections.Generic;
using System.IO;
using ClassLib;
namespace ConsoleApp
{
    public class ShowReviewsWind : Window
    {
        public bool canceled;
        private ListView allReviewsListView;
        private int pageSize = 10;
        private int page = 1;
        private ReviewRepository repository;
        public Label pagesLabelCur;
        public Label pagesLabelAll;
        private Button nextPageButton;
        private Button prevPageButton;
        public ShowReviewsWind()
        {
            this.Title = "List of reviews"; X = 30; Y = 3; Width = 87; Height = 25;
            Button cancelBut = new Button("Cancel"){X = Pos.Percent(85),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnQuit;
            allReviewsListView = new ListView(new List<Review>())
            {
                X = 1, Y = 0, Width = Dim.Fill(), Height = pageSize
            };
            allReviewsListView.OpenSelectedItem += OnOpenReview;
            this.Add(cancelBut, allReviewsListView);
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
        public void SetRepository(ReviewRepository repository)
        {
            this.repository = repository;
            ShowCurrPage();
        }
        private void ShowCurrPage()
        {
            this.pagesLabelCur.Text = page.ToString();
            int total = repository.GetTotalPages();
            this.pagesLabelAll.Text = total.ToString();
            this.allReviewsListView.SetSource(repository.GetPage(page));
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
        private void OnOpenReview(ListViewItemEventArgs args)
        {
            Review review = (Review)args.Value;
            OpenReviewDialog dialog = new OpenReviewDialog();
            dialog.SetReview(review);

            Application.Run(dialog);

            if(dialog.deleted)
            {
                repository.DeleteById(review.id);
                ShowCurrPage();
            }
            if(dialog.edited)
            {
                Review newReview = dialog.GetReview();
                newReview.id = review.id;
                repository.Update(newReview);
                ShowCurrPage();
            }
        }
    }
}
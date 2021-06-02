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
        private Service repository;
        public Label pagesLabelCur;
        public Label pagesLabelAll;
        private Button nextPageButton;
        private Button prevPageButton;
        private TextField searchField;
        private string filtervalue ="";
        private User user;
        public ShowReviewsWind()
        {
            this.Title = "List of reviews"; X = 10; Y = 4; Width = Dim.Fill()-10; Height = Dim.Fill()-4;
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

            searchField = new TextField()
            {
                X = Pos.Right(nextPageButton)+2, Y = Pos.Top(nextPageButton), Width = Dim.Fill()-5
            };
            searchField.TextChanging += OnSearchEnter;
            this.Add(searchField);
        }
        private void OnSearchEnter(TextChangingEventArgs obj)
        {
            this.filtervalue = obj.NewText.ToString();
            page = 1;
            ShowCurrPage();
        }

        public void SetService(Service service)
        {
            this.repository = service;
            ShowCurrPage();
        }
        public void SetUser(User currUser)
        {
            this.user = currUser;
        }
        private void ShowCurrPage()
        {
            this.pagesLabelCur.Text = page.ToString();
            int total = repository.reviewRepository.GetSearchPagesCount(filtervalue);
            this.pagesLabelAll.Text = total.ToString();
            this.allReviewsListView.SetSource(repository.reviewRepository.GetSearchPage(filtervalue, page));
            this.nextPageButton.Visible = true;
            this.prevPageButton.Visible = true;
            if(total==0)
            {
                this.page = 0;
                this.pagesLabelAll.Text ="x";
                this.pagesLabelCur.Text ="x";
                this.nextPageButton.Visible = false;
                this.prevPageButton.Visible = false;
                this.allReviewsListView.SetSource(new List<string>(){"No results"});
            }
            Application.Refresh();
        }
        private void OnQuit()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnNextPageButClicked()
        {
            int totalPages = repository.reviewRepository.GetSearchPagesCount(filtervalue);
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
            Review review = new Review();
            try{
            review = (Review)args.Value;}catch{return;}
            OpenReviewDialog dialog = new OpenReviewDialog();
            dialog.SetService(repository);
            if(user.id != review.user_id)
            {
                dialog.deleteReview.Visible = false;
                dialog.editReview.Visible = false;
            }
            dialog.SetUser(user);
            dialog.SetReview(review);

            Application.Run(dialog);
            if(canceled)
            {
                return;
            }
            if(dialog.deleted)
            {
                repository.reviewRepository.DeleteById(review.id);
                MessageBox.Query("Delete review","Review was deleted succesfully","OK");
                int pages = repository.actorRepository.GetSearchPagesCount(filtervalue);
                if(page>pages && page >1) page += -1;
                ShowCurrPage();
            }
            if(dialog.edited)
            {
                ShowCurrPage();
            }
        }
    }
}
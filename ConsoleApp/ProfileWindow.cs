using ClassLib;
using Terminal.Gui;
using System;
using System.Collections.Generic;
using RPC;
namespace ConsoleApp
{
    public class ProfileWindow : Window 
    {
        public bool canceled = false;
        private ListView allReviewsListView;
        private RemoteService service;
        private Label username;
        private Label fullname;
        private Label role;
        private Label regDate;
        private User user;
        public ProfileWindow()
        {
            this.Title = "My profile";X = 10; Y = 3; Width = Dim.Fill()-10; Height = Dim.Fill()-3;
            Button cancelBut = new Button("Cancel"){X = Pos.Percent(87),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnQuit;
            this.Add(cancelBut);
            Label usernameLbl = new Label("Username:"){
                X = 1, Y = 1
            };
            username = new Label()
            {
                X = 13, Y = Pos.Top(usernameLbl)
            };
            this.Add(usernameLbl, username);
            Label fullnameLbl = new Label("Fullname:")
            {
                X = 1, Y = Pos.Top(usernameLbl)+2
            };
            fullname = new Label()
            {
                X = 13, Y = Pos.Top(fullnameLbl)
            };
            this.Add(fullnameLbl, fullname);
            Label regDateLbl = new Label("Registration date:")
            {
                X = 1, Y = Pos.Top(fullnameLbl)+2
            };
            regDate = new Label()
            {
                X = 20, Y = Pos.Top(regDateLbl)
            };
            this.Add(regDateLbl, regDate);

            Label roleLbl = new Label("Role:")
            {
                X = 1, Y = Pos.Top(regDateLbl)+2
            };
            role = new Label()
            {
                X = 7, Y = Pos.Top(roleLbl)
            };
            this.Add(roleLbl, role);

            Label reviewsLbl = new Label("My Reviews:")
            {
                X = Pos.Center(), Y = Pos.Top(roleLbl)+2
            };
            allReviewsListView = new ListView(new List<Review>())
            {
                Width = Dim.Fill(), Height = Dim.Fill()
            };
            allReviewsListView.OpenSelectedItem += OnOpenReview;
            FrameView frame = new FrameView()
            {
                X = Pos.Center(), Y = Pos.Bottom(reviewsLbl), Width = Dim.Fill() - 4, Height = 9
            };
            frame.Add(allReviewsListView);
            this.Add(reviewsLbl, frame);

        }
        private void OnQuit()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetUser(User user)
        {
            this.user = user;
            this.username.Text = user.username;
            this.fullname.Text = user.fullname;
            this.regDate.Text = user.registrationDate.ToString();
            this.role.Text = user.role;
            this.allReviewsListView.SetSource(service.reviewRepository.GetAllAuthorReviews(user.id).Count == 0 
            ? new List<string>(){"There aren`t any reviews yet"}
            : service.reviewRepository.GetAllAuthorReviews(user.id));
        }
        private void OnOpenReview(ListViewItemEventArgs args)
        {
            Review review = new Review();
            try{
            review = (Review)args.Value;}
            catch{return;}
            OpenReviewDialog dialog = new OpenReviewDialog();
            dialog.X = 2; dialog.Y = 2;
            dialog.Width = Dim.Fill() - 2; dialog.Height = 25;
            dialog.SetService(service);
            dialog.SetReview(review);
            dialog.SetUser(user);
            Application.Run(dialog);
            if(dialog.deleted)
            {
                service.reviewRepository.DeleteById(review.id);
                MessageBox.Query("Delete review","Review was deleted succesfully","OK");
                allReviewsListView.SetSource(service.reviewRepository.GetAllAuthorReviews(user.id).Count !=0 
                ? service.reviewRepository.GetAllAuthorReviews(user.id) : new List<string>(){"There aren`t any reviews in this film yet"});
            }
            else if(dialog.edited)
            {
                Review newReview = dialog.GetReview();
                newReview.id = review.id;
                newReview.createdAt = review.createdAt;
                service.reviewRepository.Update(newReview);
                /* MessageBox.Query("Edit review","Review was edited succesfully","OK"); */
                allReviewsListView.SetSource(service.reviewRepository.GetAllAuthorReviews(user.id));
            }
        }
        public void SetService(RemoteService service)
        {
           this.service = service;
        }
    }
}
using Terminal.Gui;
using ClassLib;
using System;
namespace ConsoleApp
{
    public class PromoteDialog : Dialog
    {
        private TextField inp;
        public bool canceled;
        public PromoteDialog()
        {
            this.Title = "Promote to moderator";
            Width = 70; Height = 7;
            Label label = new Label(1,1,"User:");
            inp = new TextField()
            {
                X = Pos.Right(label)+1, Y= 1, Width = Dim.Fill()-3
            };
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.Add(label,inp);
            this.AddButton(cancelBut);
            this.AddButton(okBut);
        }
        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public string GetInput()
        {
            return inp.Text.ToString();
        }
    }
}
using Terminal.Gui;
using System;
using ClassLib;
namespace ConsoleApp
{
    public class SingInDialog : Dialog
    {
        public User user;
        private UserRepository userRepository;
        public bool canceled = false;
        private TextField usernameInp;
        private TextField passwordInp;
        public SingInDialog()
        {
            this.Title = "";
            this.Width = 40; this.Height = 25;
            Label label = new Label("Sing  in")
            {
                TextAlignment = TextAlignment.Centered,
                X = 15, Y = 1
            };
            Label usernameLab = new Label("Username:")
            {
                X = 1, Y = Pos.Center()
            };
            usernameInp = new TextField("")
            {
                X = 12, Y = Pos.Top(usernameLab), Width = Dim.Fill() -3
            };
            Label passwordLab = new Label("Password:")
            {
                X =1, Y = Pos.Bottom(usernameLab) + 1
            };
            passwordInp = new TextField("")
            {
                X = 12, Y = Pos.Top(passwordLab), Width = Dim.Fill() -3
            };
            this.Add(label, usernameLab, usernameInp, passwordLab, passwordInp);
            /* Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit; */
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.AddButton(cancelBut);
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetRepository(UserRepository userRepo)
        {
            userRepository = userRepo;
        }

        
    }
}
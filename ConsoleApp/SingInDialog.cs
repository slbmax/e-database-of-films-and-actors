using Terminal.Gui;
using System;
using ClassLib;
namespace ConsoleApp
{
    public class SingInDialog : Dialog
    {
        private UserRepository userRepository;
        public bool canceled = false;
        private TextField usernameInp;
        private TextField passwordInp;
        private User user;
        public SingInDialog()
        {
            this.Title = "";
            this.Width = 40; this.Height = 25;
            Label label = new Label("Sing in")
            {
                X = Pos.Center(), Y = 1
            };
            Label usernameLab = new Label("Username:")
            {
                X = Pos.Center(), Y = Pos.Center()-3
            };
            usernameInp = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(usernameLab)+1, Width = 30
            };
            Label passwordLab = new Label("Password:")
            {
                X =Pos.Center(), Y = Pos.Bottom(usernameInp) + 1
            };
            passwordInp = new TextField("")
            {
                X = Pos.Center(), Y = Pos.Bottom(passwordLab) + 1, Width = 30, Secret = true
            };

            this.Add(label, usernameLab, usernameInp, passwordLab, passwordInp);
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.AddButton(cancelBut);
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            this.AddButton(okBut);
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
        private void OnCreateDialogSubmit()
        {
            user = new User();
            string error = "noerrors";
            while(true)
            {
                if(usernameInp.Text.ToString() == "")
                {
                    error = "null username value";
                    break;
                }
                if(passwordInp.Text.ToString() == "")
                {
                    error = "null password value";
                    break;
                }
                try{
                    Authentication.SetRepo(userRepository);
                    user = Authentication.LogIn(usernameInp.Text.ToString(), passwordInp.Text.ToString());
                }
                catch (Exception ex){
                    error = ex.Message.ToString();
                }
                break;
            }
            if(error == "noerrors")
            {
                MessageBox.Query("Log in",$"Welcome to FilMax, {user.username}!","Ok");
                Application.RequestStop();
            }
            else
            {
                MessageBox.ErrorQuery("Log in",$"Error: {error}","Ok");
            } 
        }
        public User GetUser()
        {
            return user;
        }
        
    }
}
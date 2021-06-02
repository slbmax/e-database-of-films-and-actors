using Terminal.Gui;
using ClassLib;
using System;
using System.IO;
namespace ConsoleApp
{
    public class RegistrationDialog : Dialog
    {
        public bool canceled = false;
        private User user;
        private UserRepository userRepository;
        private TextField nameInp;
        private TextField surnameInp;
        private TextField usernameInp;
        private TextField passwordInp;
        private TextField confirmPasswordInp;
        public RegistrationDialog()
        {
            this.Title = "";
            this.Width = 40; this.Height = 25;
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Exit FilMax");
            cancelBut.Clicked += OnCreateDialogCanceled;

            Label singInLabel = new Label("New to FilMax?\nRegister to continue")
            {
                TextAlignment = TextAlignment.Centered,
                X = 9, Y = 1
            };
            this.AddButton(okBut); this.AddButton(cancelBut); this.Add(singInLabel);

            Label nameLab = new Label("Name:")
            {
                X =1, Y = Pos.Bottom(singInLabel)+3
            };
            nameInp = new TextField("")
            {
                X = 12, Y = Pos.Top(nameLab), Width = Dim.Fill() -3
            };

            Label surnameLab = new Label("Surname:")
            {
                X = 1, Y = Pos.Bottom(nameLab) + 1
            };
            surnameInp = new TextField("")
            {
                X = 12, Y = Pos.Top(surnameLab), Width = Dim.Fill() -3
            };
            Label usernameLab = new Label("Username:")
            {
                X = 1, Y = Pos.Bottom(surnameLab)+1
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
                X = 12, Y = Pos.Top(passwordLab), Width = Dim.Fill() -3, Secret = true
            };

            this.Add(nameLab, nameInp, surnameLab,surnameInp, usernameLab,usernameInp, passwordLab, passwordInp);

            Label confirmPasswordLab = new Label("Confirm:\n(password)")
            {
                X =1, Y = Pos.Bottom(passwordLab) + 1
            };
            confirmPasswordInp = new TextField("")
            {
                X = 12, Y = Pos.Top(confirmPasswordLab), Width = Dim.Fill() -3, Secret = true
            };
            this.Add(confirmPasswordLab, confirmPasswordInp);


            Label toSingIn = new Label("Already with us?")
            {
                X = 5, Y = Pos.Top(confirmPasswordLab) +4
            };
            Button singInBut = new Button("Sing in")
            {
                X = Pos.Right(toSingIn)+1, Y = Pos.Top(toSingIn)
            };
            singInBut.Clicked+= OnSingInButton;

            this.Add(toSingIn); this.Add(singInBut);
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnSingInButton()
        {
            SingInDialog dialog = new SingInDialog();
            dialog.SetRepository(userRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                user = dialog.GetUser();
                Application.RequestStop();
            }
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
                error = ValidateNamesInput(nameInp.Text.ToString());
                if(error != "noerrors")
                {
                    break;
                }
                error = ValidateNamesInput(surnameInp.Text.ToString());
                if(error != "noerrors")
                {
                    break;
                }
                error = ValidateUsernameInput(usernameInp.Text.ToString());
                if(error != "noerrors")
                    break;

                if(passwordInp.Text.ToString() == "")
                {
                    error = "null password value";
                    break;
                }
                if(passwordInp.Text.ToString().Length<=6)
                {
                    error = "short password";
                    break;
                }
                if(passwordInp.Text.ToString() != confirmPasswordInp.Text.ToString())
                {
                    error = "different passwords";
                    break;
                }
                user.fullname = nameInp.Text.ToString() + " " + surnameInp.Text.ToString();
                user.username = usernameInp.Text.ToString();
                user.password = passwordInp.Text.ToString();
                try{
                    Authentication.SetRepo(userRepository);
                    user = Authentication.Registration(user);
                }
                catch (Exception ex){
                    error = ex.Message.ToString();
                }
                break;
            }
            if(error == "noerrors")
            {
                MessageBox.Query("Registration",$"Welcome to FilMax, {user.username}!","Ok");
                Application.RequestStop();
            }
            else
            {
                MessageBox.ErrorQuery("Registration",$"Error: {error}","Ok");
            } 
        }
        private string ValidateNamesInput(string input)
        {
            if(input.Length == 0)
                return "please, enter your fullname correctly";
            if(input.Length > 16)
                return "value must be shorter than 16 symbols";    
            for(int i = 0; i < input.Length; i ++)
            {
                if(!char.IsLetter(input[i]))
                    return $"wrong character '{input[i]}'";
            }
            return "noerrors";
        }
        private string ValidateUsernameInput(string input)
        {
            if(input.Length == 0)
                return "null username input";
            if(input.Length > 16)
                return "value must be shorter than 16 symbols";  
            for(int i = 0; i < input.Length; i ++)
            {
                if(!char.IsLetterOrDigit(input[i]))
                    return $"wrong character '{input[i]}'";
            }
            return "noerrors";
        }
        public User GetUser()
        {
            return user;
        }
    }
}
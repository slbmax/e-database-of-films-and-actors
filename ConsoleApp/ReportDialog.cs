using Terminal.Gui;
using ClassLib;
using System.IO;

namespace ConsoleApp
{
    public class ReportDialog : Dialog
    {
        private Service service;
        private TextField inpId;
        private TextField inpPath;
        private Film film;
        public bool canceled;
        public ReportDialog()
        {
            this.Title = "Report";
            Width = 70; Height = 8;
            Label label = new Label(1,3,"Select film [id]:");
            inpId = new TextField()
            {
                X = Pos.Right(label)+1, Y= 3, Width = 6
            };
            Button selectDirectory = new Button(1,1,"Select directory");
            inpPath = new TextField()
            {
                X = Pos.Right(selectDirectory)+1, Y =1, Width = Dim.Fill() - 5
            };

            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            selectDirectory.Clicked+=OnSelectDirectory;
            this.Add(label,inpId);
            this.AddButton(selectDirectory);
            this.AddButton(cancelBut);
            this.AddButton(okBut);
            this.Add(inpPath);
        }
        public void SetService(Service service)
        {
            this.service = service;
        }
        private void OnCreateDialogSubmit()
        {
            string error = "noerrors";
            while(true)
            {
                if(inpId.Text == null)
                {
                    error = "null film id";
                    break;
                }
                int id = 0;
                if(!int.TryParse(inpId.Text.ToString(), out id))
                {
                    error = "non-integer film id";
                    break;
                }
                if(!Directory.Exists(inpPath.Text.ToString()))
                {
                    error = "non-existing directory";
                    break;
                }
                film = service.filmRepository.GetById(id);
                if(film == null)
                {
                    error = $"non-existing film with id '{id}'";
                    break;
                }
                film.reviews = service.reviewRepository.GetAllFilmReviews(film.id);
                film.actors = service.roleRepository.GetCast(film.id);
                break;
            }
            if(error == "noerrors")
            {
                Application.RequestStop();
            }
            else
            {
                MessageBox.ErrorQuery("Report",$"Error: {error}","OK");
            }
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public Film GetFilm()
        {
            return film;
        }
        public string GetFilePath()
        {
            return inpPath.Text.ToString();
        }
        private void OnSelectDirectory()
        {
            OpenDialog dialog = new OpenDialog("Open directory", "Open?");
            dialog.CanChooseDirectories = true;
            dialog.CanChooseFiles = false;
            
            Application.Run(dialog);
        
            if (!dialog.Canceled)
            {
                NStack.ustring filePath = dialog.FilePath;
                inpPath.Text = filePath;
            }
            else
            {
                inpPath.Text = "not selected.";
            }

        }
    }
}
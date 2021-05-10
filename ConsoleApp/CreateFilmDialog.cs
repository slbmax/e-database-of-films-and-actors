using Terminal.Gui;
namespace ConsoleApp
{
    public class CreateFilmDialog : Dialog
    {
        public bool canceled;
        public TextField filmTitleInp;
        public TextField filmGenreInp;
        public TextField filmYearInp;
        public CreateFilmDialog()
        {
            this.Title = "Create film";
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.AddButton(cancelBut);
            this.AddButton(okBut);

            int posX = 20;

            Label filmTitleLab = new Label(2,2,"Title:");
            filmTitleInp = new TextField("")
            {
                X = posX, Y = Pos.Top(filmTitleLab), Width =40
            };
            this.Add(filmTitleLab,filmTitleInp);

            Label filmGenreLab = new Label(2,4,"Genre:");
            filmGenreInp = new TextField("")
            {
                X = posX, Y = Pos.Top(filmGenreLab), Width =40
            };
            this.Add(filmGenreLab,filmGenreInp);

            Label filmYearLab = new Label(2,6,"Year:");
            filmYearInp = new TextField("")
            {
                X = posX, Y = Pos.Top(filmYearLab), Width =40
            };
            this.Add(filmYearLab,filmYearInp);

            Label remarkLbl = new Label(2,10,"Remark: Year value should be in range from 2000 to 2021");

            this.Add(remarkLbl);

        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnCreateDialogSubmit()
        {
            string error = "noerrors";
            int year = 0;
            if(filmTitleInp.Text.ToString() == "")
                error = "Empty film title";
            else if(filmGenreInp.Text.ToString() == "")
                error = "Empty film genre";
            else if(filmYearInp.Text.ToString()=="" || !int.TryParse(filmYearInp.Text.ToString(), out year))
                error = "Invalid film year value";
            else if(year <2000 || year >2021)
                error = "Film year value is in invalid range";
            if(error == "noerrors")
            {
                MessageBox.Query("Create film", "Successfully", "OK");
                this.canceled = false;
                Application.RequestStop();
            }
            else
                MessageBox.ErrorQuery("Error",$"{error}", "OK");
        }
        public Film GetFilm()
        {
            return new Film()
            {
                title = filmTitleInp.Text.ToString(),
                genre = filmGenreInp.Text.ToString(),
                releaseYear = int.Parse(filmYearInp.Text.ToString())
            };
        }
    }
}
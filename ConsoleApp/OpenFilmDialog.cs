using Terminal.Gui;
namespace ConsoleApp
{
    public class OpenFilmDialog : Dialog
    {
        public bool canceled;
        public bool deleted = false;
        public bool edited = false;
        private TextField filmTitle;
        private TextField filmGenre;
        private TextField filmYear;
        private Button deleteFilm;
        private Film film;
        public OpenFilmDialog()
        {
            this.Title = "Open Film";
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.AddButton(cancelBut);

            int posX = 20;
            Label filmTitleLab = new Label(2,2,"Title:");
            filmTitle = new TextField("")
            {
                X = posX, Y = Pos.Top(filmTitleLab), Width =40
            };
            filmTitle.ReadOnly = true;
            this.Add(filmTitleLab,filmTitle);
            Label filmGenreLab = new Label(2,4,"Genre:");
            filmGenre = new TextField("")
            {
                X = posX, Y = Pos.Top(filmGenreLab), Width =40
            };
            filmGenre.ReadOnly = true;
            this.Add(filmGenreLab,filmGenre);
            Label filmYearLab = new Label(2,6,"Release year:");
            filmYear = new TextField("")
            {
                X = posX, Y = Pos.Top(filmYearLab), Width =40
            };
            filmYear.ReadOnly = true;
            this.Add(filmYearLab,filmYear);

            deleteFilm = new Button("Delete"){X = 2, Y = Pos.Bottom(filmYear)+6};
            deleteFilm.Clicked += OnDeleteFilm;
            Button editFilm = new Button("Edit"){X = Pos.Right(deleteFilm)+2, Y = Pos.Bottom(filmYear)+6};
            editFilm.Clicked += OnEditFilm;
            this.Add(deleteFilm, editFilm);
        }
        private void OnOpenDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetFilm(Film film)
        {
            this.film = film;
            this.filmGenre.Text = film.genre;
            this.filmTitle.Text = film.title;
            this.filmYear.Text = film.releaseYear.ToString();
        }
        private void OnDeleteFilm()
        {
            int index =MessageBox.Query("Delete film","Are you sure?","Yes","No");
            if(index == 0)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditFilm()
        {
            EditFilm dialog = new EditFilm();
            dialog.SetFilm(film);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.edited = true;
                film = dialog.GetFilm();
                OnOpenDialogCanceled();
            }
        }
        public Film GetFilm()
        {
            return film;
        }
    }
}
using Terminal.Gui;
using System.Collections.Generic;
namespace ConsoleApp
{
    public class OpenFilmDialog : Dialog
    {
        public bool canceled;
        public bool deleted = false;
        public bool edited = false;
        private ActorRepository actorRepo;
        private FilmRepository filmRepo;
        private TextField filmTitle;
        private TextField filmGenre;
        private TextField filmYear;
        private ListView allActorsListView;
        private Button deleteFilm;
        private Film film;
        private int[] actorIntIds;
        public OpenFilmDialog()
        {
            this.Title = "Open Film";
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.AddButton(cancelBut);

            int posX = 20;
            int width = 40;
            Label filmTitleLab = new Label(2,2,"Title:");
            filmTitle = new TextField("")
            {
                X = posX, Y = Pos.Top(filmTitleLab), Width =width
            };
            filmTitle.ReadOnly = true;
            this.Add(filmTitleLab,filmTitle);
            Label filmGenreLab = new Label(2,4,"Genre:");
            filmGenre = new TextField("")
            {
                X = posX, Y = Pos.Top(filmGenreLab), Width =width
            };
            filmGenre.ReadOnly = true;
            this.Add(filmGenreLab,filmGenre);
            Label filmYearLab = new Label(2,6,"Release year:");
            filmYear = new TextField("")
            {
                X = posX, Y = Pos.Top(filmYearLab), Width =width
            };
            filmYear.ReadOnly = true;
            this.Add(filmYearLab,filmYear);
            Label filmCastLab = new Label(2,8,"Cast:");
            this.Add(filmCastLab);


            allActorsListView = new ListView(new List<Film>())
            {
                X = posX, Y = Pos.Top(filmCastLab), Width = Dim.Fill()-5, Height = 5
            };
            this.Add(allActorsListView);
            

            deleteFilm = new Button("Delete"){X = 2, Y = Pos.Bottom(filmCastLab)+6};
            deleteFilm.Clicked += OnDeleteFilm;
            Button editFilm = new Button("Edit"){X = Pos.Right(deleteFilm)+2, Y = Pos.Top(deleteFilm)};
            editFilm.Clicked += OnEditFilm;
            this.Add(deleteFilm, editFilm);
        }
        private void OnOpenDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private List<Actor> GetListOfActors()
        {
            List<Actor> actors = new List<Actor>();
            for(int i = 0; i<film.actors.Length; i ++)
            {
                actors.Add(film.actors[i]);
            }
            return actors;
        }
        public void SetFilm(Film film)
        {
            this.film = film;
            this.filmGenre.Text = film.genre;
            this.filmTitle.Text = film.title;
            this.filmYear.Text = film.releaseYear.ToString();
            allActorsListView.SetSource(GetListOfActors());
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
            dialog.SetRepositories(actorRepo, filmRepo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.edited = true;
                film = dialog.GetFilm();
                actorIntIds = dialog.GetActorsId();
                OnOpenDialogCanceled();
            }
        }
        public Film GetFilm()
        {
            return film;
        }
        public int[] GetActorsId()
        {
            return actorIntIds;
        }
        public void SetRepositories(ActorRepository repository, FilmRepository filmRepo)
        {
            this.actorRepo = repository;
            this.filmRepo = filmRepo;
        }
    }
}
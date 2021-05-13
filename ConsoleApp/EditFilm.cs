using Terminal.Gui;
namespace ConsoleApp
{
    public class EditFilm : CreateFilmDialog
    {
        public EditFilm()
        {
            this.Title = "Edit film";
        }
        public void SetFilm(Film film)
        {
            this.filmGenreInp.Text = film.genre;
            this.filmTitleInp.Text = film.title;
            this.filmYearInp.Text = film.releaseYear.ToString();
        }
    }
}
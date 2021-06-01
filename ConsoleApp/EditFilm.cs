using Terminal.Gui;
using ClassLib;
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
            string ids = "";
            for(int i =0; i<film.actors.Length; i++)
            {
                ids = ids + $"{film.actors[i].id}";
                if(i+1 != film.actors.Length)
                    ids = ids +","; 
            }
            this.filmCast.Text = ids;
        }
    }
}
using System.Collections.Generic;
namespace ClassLib
{
    public class Film
    {
        public int id;
        public string title;
        public string genre;
        public int releaseYear;
        public List<Review> reviews;
        public Actor[] actors = null;
        public Film()
        {
            this.id = 0;
            this.title = "";
            this.genre = "";
            this.releaseYear = 0;
        }
        public Film(string title, string genre, int releaseYear)
        {
            this.title = title;
            this.genre = genre;
            this.releaseYear = releaseYear;
        }
        public override string ToString()
        {
            string newCont = title;
            if(newCont.Length > 55)
            {
                newCont = title.Substring(0, 55) + "...";
            }
            return $"[{id}] {newCont} [{releaseYear}] -- {genre}";
        }
        public string FilmCon()
        {
            string sep = "!-!-!-!-!";
            string con = id+sep+title+sep+genre+sep+releaseYear;
            return con;
        }
        public static Film Parse(string userToParse)
        {
            Film film = new Film();
            string sep = "!-!-!-!-!";
            string[] arr = userToParse.Split(sep);
            film.id = int.Parse(arr[0]);
            film.title = arr[1];
            film.genre = arr[2];
            film.releaseYear = int.Parse(arr[3]);
            return film;
        }
    }
}
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
            return $"[{id}] {title} [{releaseYear}] -- {genre}";
        }
    }
}
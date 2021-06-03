using System.Xml.Serialization;
using System;
namespace ClassLib
{
    [XmlType(TypeName="review")]
    public class Review
    {
        public int id;
        public string content;
        public int rating;
        public DateTime createdAt;
        public int user_id;
        public int film_id;
        public Review()
        {
            this.id = 0;
            this.content = "";
            this.rating = 0;
            this.createdAt = DateTime.Now;
        }
        public Review(string content, int rating, DateTime createdAt)
        {
            this.content = content;
            this.rating = rating;
            this.createdAt = createdAt;
        }
        public override string ToString()
        {
            string newCont = content;
            if(newCont.Length > 55)
            {
                newCont = content.Substring(0, 55) + "...";
            }
            return $"[{id}] {newCont} [{rating}] -- {createdAt}";
        }
    }
}
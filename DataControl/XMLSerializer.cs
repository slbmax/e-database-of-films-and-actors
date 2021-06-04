using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using ClassLib;
using RPC;

namespace ClassLib
{
    public static class XMLSerializer
    {
        private static RemoteService service;
        public static void Export(string filePath, List<Review> reviews)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Review>));
            StreamWriter writer = new StreamWriter(filePath);
            ser.Serialize(writer, reviews);
            writer.Close();
        }
        public static void SetService(RemoteService ser)
        {
            service = ser;
        }
        public static void Import(string filePath, int user_id)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Review>));
            string xmlData = File.ReadAllText(filePath);
            StringReader reader = new StringReader(xmlData);
            List<Review> reviews = (List<Review>)ser.Deserialize(reader);
            reader.Close();
            foreach(Review review in reviews)
            {
                Review reviewToChek = service.reviewRepository.GetById(review.id);
                if(reviewToChek != null && EqualsR(reviewToChek, review))
                    continue;
                review.user_id = user_id;
                Film film = service.filmRepository.GetById(review.film_id);
                if(film != null)
                {
                    service.reviewRepository.Insert(review);
                }
                else
                {
                    if(service.filmRepository.GetCount() == 0)
                        continue;
                    else
                    {
                        review.film_id = GetFilmForReview();
                        service.reviewRepository.Insert(review);
                    }
                }
            }
        }
        private static bool EqualsR(Review f, Review s)
        {
            return f.content == s.content && f.createdAt == s.createdAt &&
            f.rating == s.rating && f.user_id == s.user_id && f.film_id == s.film_id;
        }
        public static int GetFilmForReview()
        {
            List<Film> filmsList = service.filmRepository.GetAll();
            Film[] films = new Film[filmsList.Count];
            filmsList.CopyTo(films);
            Random rand = new Random();
            int randId = rand.Next(0,films.Length);
            return films[randId].id;
        }
        
    }
}
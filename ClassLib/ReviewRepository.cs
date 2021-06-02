using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
namespace ClassLib
{
    public class ReviewRepository
    {
        private SqliteConnection connection;
        public ReviewRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }
        private static Review GetReview(SqliteDataReader reader)
        {
            Review review = new Review();
            review.id = int.Parse(reader.GetString(0));
            review.content = reader.GetString(1);
            review.rating = int.Parse(reader.GetString(2));
            review.createdAt= DateTime.Parse(reader.GetString(3));
            review.user_id = int.Parse(reader.GetString(4));
            review.film_id = int.Parse(reader.GetString(5));
            return review;
        }
        public Review GetById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            Review review = new Review();
            if(reader.Read())
                review= GetReview(reader);
            else
                review = null;
            reader.Close();
            return review;
        }
        public bool Update(Review review)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"UPDATE reviews SET content = $content, rating = $r, createdAt = $cr WHERE id = $id";
            command.Parameters.AddWithValue("$id", review.id);
            command.Parameters.AddWithValue("$content", review.content);
            command.Parameters.AddWithValue("$r", review.rating);
            command.Parameters.AddWithValue("$cr", review.createdAt.ToString("o"));
            int nChanged = command.ExecuteNonQuery();
            return nChanged == 1;
        }
        public int DeleteById(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int result = command.ExecuteNonQuery();
            return result;
        }
        public int Insert(Review review)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText =@"INSERT INTO reviews (content, rating, createdAt, user_id, film_id)
            VALUES ($content, $rating, $createdAt, $user_id, $film_id);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$content", review.content);
            command.Parameters.AddWithValue("$rating",review.rating);
            command.Parameters.AddWithValue("$createdAt", review.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$user_id",review.user_id);
            command.Parameters.AddWithValue("$film_id",review.film_id);
            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public int GetTotalPages()
        {
            const int pageSize = 10;
            return (int)Math.Ceiling(this.GetCount() / (double)pageSize);
        }
        private long GetCount()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews";
            long count = (long)command.ExecuteScalar();
            return count;
        }
        public List<Review> GetPage(int page)
        {
            const int pageSize = 10;
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews LIMIT $pagesize OFFSET $offset";
            command.Parameters.AddWithValue("$pagesize", pageSize);
            command.Parameters.AddWithValue("$offset", pageSize*(page-1));

            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                
                reviews.Add(review);
            }
            reader.Close();
            return reviews;
        }
        public List<Review> GetExport(string valueX)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE user LIKE $valueX";
            command.Parameters.AddWithValue("$valueX", valueX);

            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviewsToExport = new List<Review>();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                
                reviewsToExport.Add(review);
            }
            reader.Close();
            return reviewsToExport;
        }
        public List<Review> GetAllAuthorReviews(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE user_id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> userReviews = new List<Review>();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                
                userReviews.Add(review);
            }
            reader.Close();
            return userReviews;
        }
        public List<Review> GetAllFilmReviews(int id)
        {
            SqliteCommand command = this.connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE film_id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Review> filmReviews = new List<Review>();
            while(reader.Read())
            {
                Review review = GetReview(reader);
                review.film_id = int.Parse(reader.GetString(5)); 
                review.user_id = int.Parse(reader.GetString(4));
                
                filmReviews.Add(review);
            }
            reader.Close();
            
            return filmReviews;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ClassLib;
namespace RPC
{
    public class RemoteReviewRepository
    {
        public Socket sender;
        public RemoteReviewRepository(Socket sender)
        {
            this.sender = sender;
        }
        public Review GetById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "reviewRepository.GetById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<Review> response = GetResponse<Review>();
            return response.value;
        }
        public bool Update(Review review)
        {
            string[] parametrs = new string[] { review.ReviewCon() };
            Request request = new Request()
            {
                methodName = "reviewRepository.Update",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<bool> response = GetResponse<bool>();
            return response.value;
        }
        public int DeleteById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "reviewRepository.DeleteById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int DeleteByFilmId(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "reviewRepository.DeleteByFilmId",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int Insert(Review review)
        {
            string[] parametrs = new string[] { review.ReviewCon() };
            Request request = new Request()
            {
                methodName = "reviewRepository.Insert",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public List<Review> GetAllAuthorReviews(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "reviewRepository.GetAllAuthorReviews",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<List<Review>> response = GetResponse<List<Review>>();
            return response.value;
        }
        public List<Review> GetAllFilmReviews(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "reviewRepository.GetAllFilmReviews",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<List<Review>> response = GetResponse<List<Review>>();
            return response.value;
        }
        public int GetSearchPagesCount(string searchTitle)
        {
            string[] parametrs = new string[] { searchTitle };
            Request request = new Request()
            {
                methodName = "reviewRepository.GetSearchPagesCount",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public List<Review> GetSearchPage(string searchTitle, int page)
        {
            string[] parametrs = new string[] { searchTitle, page.ToString() };
            Request request = new Request()
            {
                methodName = "reviewRepository.GetSearchPage",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<List<Review>> response = GetResponse<List<Review>>();
            return response.value;
        }
        public double GetFilmRating(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "reviewRepository.GetFilmRating",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<double> response = GetResponse<double>();
            return response.value;
        }
        private void SendRequest(Request request)
        {
            string xmlRequest = ServerSerializer.SerializeRequest(request);
            byte[] msg = Encoding.UTF8.GetBytes(xmlRequest); sender.Send(msg);
        }
        private Response<T> GetResponse<T>()
        {
            byte[] bytes = new byte[1024];
            string xmlResponse = "";
            while (true)
            {
                int bytesRec = sender.Receive(bytes);
                xmlResponse += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                if (xmlResponse.IndexOf("</response>") > -1)
                    break;
            }
            Response<T> response = ServerSerializer.DeserializeResponse<T>(xmlResponse);
            return response;
        }
    }
}
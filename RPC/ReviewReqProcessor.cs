using System.Net;
using System.Net.Sockets;
using System.Text;
using ClassLib;
using System;
using System.Collections.Generic;
namespace RPC
{
    public class ReviewReqProcessor
    {
        private Socket handler;
        private Service service;
        public ReviewReqProcessor(Socket handler, Service service)
        {
            this.handler = handler;
            this.service = service;
        }
        public void ProcessRequest(Request request)
        {
            switch(request.method)
            {
                case "reviewRepository.GetById":
                    ProcessGetById(request);
                    break;
                case "reviewRepository.Update":
                    ProcessUpdate(request);
                    break;
                case "reviewRepository.DeleteById":
                    ProcessDeleteById(request);
                    break;
                case "reviewRepository.DeleteByFilmId":
                    ProcessDeleteByFilmId(request);
                    break;
                case "reviewRepository.Insert":
                    ProcessInsert(request);
                    break;
                case "reviewRepository.GetAllAuthorReviews":
                    ProcessGetAllAuthorReviews(request);
                    break;
                case "reviewRepository.GetAllFilmReviews":
                    ProcessGetAllFilmReviews(request);
                    break;
                case "reviewRepository.GetSearchPagesCount":
                    ProcessGetSearchPagesCount(request);
                    break;
                case "reviewRepository.GetSearchPage":
                    ProcessGetSearchPage(request);
                    break;
                case "reviewRepository.GetFilmRating":
                    ProcessGetFilmRating(request);
                    break;
            }
        }
        private void ProcessGetFilmRating(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            double toReturn = service.reviewRepository.GetFilmRating(id);
            Response<double> response = new Response<double>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetSearchPage(Request request)
        {
            string valueX = request.methodParametrs[0];
            int page = int.Parse(request.methodParametrs[1]);
            List<Review> toReturn = service.reviewRepository.GetSearchPage(valueX, page);
            Response<List<Review>> response = new Response<List<Review>>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetSearchPagesCount(Request request)
        {
            string valueX = request.methodParametrs[0];
            int toReturn = service.reviewRepository.GetSearchPagesCount(valueX);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetAllAuthorReviews(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            List<Review> toReturn = service.reviewRepository.GetAllAuthorReviews(id);
            Response<List<Review>> response = new Response<List<Review>>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetAllFilmReviews(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            List<Review> toReturn = service.reviewRepository.GetAllFilmReviews(id);
            Response<List<Review>> response = new Response<List<Review>>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            Review toReturn = service.reviewRepository.GetById(id);
            Response<Review> response = new Response<Review>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessUpdate(Request request)
        {
            Review review = Review.Parse(request.methodParametrs[0]);
            bool toReturn = service.reviewRepository.Update(review);
            Response<bool> response = new Response<bool>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessDeleteById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            int toReturn = service.reviewRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessDeleteByFilmId(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            int toReturn = service.reviewRepository.DeleteByFilmId(id);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessInsert(Request request)
        {
            Review review = Review.Parse(request.methodParametrs[0]);
            int toReturn = service.reviewRepository.Insert(review);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void SendResponse<T>(Response<T> response)
        {
            string xmlResponse = ServerSerializer.SerializeResponse(response);
            byte[] message = Encoding.UTF8.GetBytes(xmlResponse);
            handler.Send(message);
            Console.WriteLine($"Response to {handler.RemoteEndPoint} was sent succesfully");
        }
    }
}
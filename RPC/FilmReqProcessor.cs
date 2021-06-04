using System.Net;
using System.Net.Sockets;
using System.Text;
using ClassLib;
using System;
using System.Collections.Generic;
namespace RPC
{
    public class FilmReqProcessor
    {
        private Socket handler;
        private Service service;
        public FilmReqProcessor(Socket handler, Service service)
        {
            this.handler = handler;
            this.service = service;
        }
        public void ProcessRequest(Request request)
        {
            switch(request.methodName)
            {
                case "filmRepository.Update":
                    ProcessUpdate(request);
                    break;
                case "filmRepository.GetById":
                    ProcessGetById(request);
                    break;
                case "filmRepository.DeleteById":
                    ProcessDeleteById(request);
                    break;
                case "filmRepository.Insert":
                    ProcessInsert(request);
                    break;
                case "filmRepository.GetCount":
                    ProcessGetCount(request);
                    break;
                case "filmRepository.GetSearchCount":
                    ProcessGetSearchCount(request);
                    break;
                case "filmRepository.GetAllIds":
                    ProcessGetAllIds(request);
                    break;
                case "filmRepository.GetSearchPagesCount":
                    ProcessGetSearchPagesCount(request);
                    break;
                case "filmRepository.GetSearchPage":
                    ProcessGetSearchPage(request);
                    break;
                case "filmRepository.GetAll":
                    ProcessGetAll(request);
                    break;
            }
        }
        private void ProcessUpdate(Request request)
        {
            Film film = Film.Parse(request.methodParametrs[0]);
            bool toReturn = service.filmRepository.Update(film);
            Response<bool> response = new Response<bool>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            Film toReturn = service.filmRepository.GetById(id);
            Response<Film> response = new Response<Film>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessDeleteById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            int toReturn = service.filmRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessInsert(Request request)
        {
            Film film = Film.Parse(request.methodParametrs[0]);
            int toReturn = service.filmRepository.Insert(film);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetCount(Request request)
        {
            long toReturn = service.filmRepository.GetCount();
            Response<long> response = new Response<long>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetSearchCount(Request request)
        {
            string valueX = request.methodParametrs[0];
            int toReturn = service.filmRepository.GetSearchCount(valueX);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetAllIds(Request request)
        {
            int[] toReturn = service.filmRepository.GetAllIds();
            Response<int[]> response = new Response<int[]>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetSearchPagesCount(Request request)
        {
            string valueX = request.methodParametrs[0];
            int toReturn = service.filmRepository.GetSearchPagesCount(valueX);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetSearchPage(Request request)
        {
            string valueX = request.methodParametrs[0];
            int page = int.Parse(request.methodParametrs[1]);
            List<Film> toReturn = service.filmRepository.GetSearchPage(valueX, page);
            Response<List<Film>> response = new Response<List<Film>>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetAll(Request request)
        {
            List<Film> toReturn = service.filmRepository.GetAll();
            Response<List<Film>> response = new Response<List<Film>>(){value = toReturn};
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
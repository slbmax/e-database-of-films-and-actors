using System.Net;
using System.Net.Sockets;
using System.Text;
using ClassLib;
using System;
using System.Collections.Generic;
namespace RPC
{
    public class ActorReqProcessor
    {
        private Socket handler;
        private Service service;
        public ActorReqProcessor(Socket handler, Service service)
        {
            this.handler = handler;
            this.service = service;
        }
        public void ProcessRequest(Request request)
        {
            switch(request.method)
            {
                case "actorRepository.GetById":
                    ProcessGetById(request);
                    break;
                case "actorRepository.DeleteById":
                    ProcessDeleteById(request);
                    break;
                case "actorRepository.Insert":
                    ProcessInsert(request);
                    break;
                case "actorRepository.Update":
                    ProcessUpdate(request);
                    break;
                case "actorRepository.GetSearchPagesCount":
                    ProcessGetSearchPagesCount(request);
                    break;
                case "actorRepository.GetSearchPage":
                    ProcessGetSearchPage(request);
                    break;
            }
        }
        private void ProcessUpdate(Request request)
        {
            Actor actor = Actor.Parse(request.methodParametrs[0]);
            bool toReturn = service.actorRepository.Update(actor);
            Response<bool> response = new Response<bool>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetSearchPagesCount(Request request)
        {
            string valueX = request.methodParametrs[0];
            int toReturn = service.actorRepository.GetSearchPagesCount(valueX);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            Actor toReturn = service.actorRepository.GetById(id);
            Response<Actor> response = new Response<Actor>(){value = toReturn};
            SendResponse(response);
        }
        private void SendResponse<T>(Response<T> response)
        {
            string xmlResponse = ServerSerializer.SerializeResponse(response);
            byte[] message = Encoding.UTF8.GetBytes(xmlResponse);
            handler.Send(message);
            Console.WriteLine($"Response to {handler.RemoteEndPoint} was sent succesfully");
        }
        private void ProcessDeleteById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            int toReturn = service.actorRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessInsert(Request request)
        {
            Actor actor = Actor.Parse(request.methodParametrs[0]);
            int toReturn = service.actorRepository.Insert(actor);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetSearchPage(Request request)
        {
            string valueX = request.methodParametrs[0];
            int page = int.Parse(request.methodParametrs[1]);
            List<Actor> toReturn = service.actorRepository.GetSearchPage(valueX, page);
            Response<List<Actor>> response = new Response<List<Actor>>(){value = toReturn};
            SendResponse(response);
        }
    }
}
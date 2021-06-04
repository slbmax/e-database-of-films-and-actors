using System.Net;
using System.Net.Sockets;
using System.Text;
using ClassLib;
using System;
using System.Collections.Generic;
namespace RPC
{
    public class RoleReqProcessor
    {
        
        private Socket handler;
        private Service service;
        public RoleReqProcessor(Socket handler, Service service)
        {
            this.handler = handler;
            this.service = service;
        }
        public void ProcessRequest(Request request)
        {
            switch(request.methodName)
            {
                case "roleRepository.GetById":
                    ProcessGetById(request);
                    break;
                case "roleRepository.DeleteFilmById":
                    ProcessDeleteFilmById(request);
                    break;
                case "roleRepository.DeleteActorById":
                    ProcessDeleteActorById(request);
                    break;
                case "roleRepository.Insert":
                    ProcessInsert(request);
                    break;
                case "roleRepository.GetAllFilms":
                    ProcessGetAllFilms(request);
                    break;
                case "roleRepository.GetCast":
                    ProcessGetCast(request);
                    break;
            }
        }
        private void ProcessGetCast(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            Actor[] toReturn = service.roleRepository.GetCast(id);
            Response<Actor[]> response = new Response<Actor[]>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetAllFilms(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            Film[] toReturn = service.roleRepository.GetAllFilms(id);
            Response<Film[]> response = new Response<Film[]>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessInsert(Request request)
        {
            Role role = Role.Parse(request.methodParametrs[0]);
            int toReturn = service.roleRepository.Insert(role);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessDeleteFilmById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            int toReturn = service.roleRepository.DeleteFilmById(id);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessDeleteActorById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            int toReturn = service.roleRepository.DeleteActorById(id);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
         private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            Role toReturn = service.roleRepository.GetById(id);
            Response<Role> response = new Response<Role>(){value = toReturn};
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
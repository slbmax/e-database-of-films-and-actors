using System.Net;
using System.Net.Sockets;
using System.Text;
using ClassLib;
using System;
namespace RPC
{   
    public class UserRequestProcess
    {
        private Socket handler;
        private Service service;
        public UserRequestProcess(Socket handler, Service service)
        {
            this.handler = handler;
            this.service = service;
        }
        public void ProcessRequest(Request request)
        {
            switch(request.method)
            {
                case "userRepository.GetById":
                    ProcessGetById(request);
                    break;
                case "userRepository.DeleteById":
                    ProcessDelete(request);
                    break;
                case "userRepository.Insert":
                    ProcessInsert(request);
                    break;
                case "userRepository.GetByUsername":
                    ProcessGetByUsername(request);
                    break;
                case "userRepository.Update":
                    ProcessUpdate(request);
                    break;
            }
        }
        private void ProcessInsert(Request request)
        {
            User user = User.Parse(request.methodParametrs[0]);
            int toReturn = service.userRepository.Insert(user);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessDelete(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            int toReturn = service.userRepository.DeleteById(id);
            Response<int> response = new Response<int>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetById(Request request)
        {
            int id = int.Parse(request.methodParametrs[0]);
            User toReturn = service.userRepository.GetById(id);
            Response<User> response = new Response<User>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessGetByUsername(Request request)
        {
            string username = request.methodParametrs[0];
            User toReturn = service.userRepository.GetByUsername(username);
            Response<User> response = new Response<User>(){value = toReturn};
            SendResponse(response);
        }
        private void ProcessUpdate(Request request)
        {
            User user = User.Parse(request.methodParametrs[0]);
            bool toReturn = service.userRepository.Update(user);
            Response<bool> response = new Response<bool>(){value = toReturn};
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
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
    public class RemoteUserRepository
    {
        public Socket sender;
        public RemoteUserRepository(Socket sender)
        {
            this.sender = sender;
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
        public User GetById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                method = "userRepository.GetById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<User> response = GetResponse<User>();
            return response.value;
        }
        public int DeleteById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                method = "userRepository.DeleteById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int Insert(User user)
        {
            string[] parametrs = new string[] { user.UserCon() };
            Request request = new Request()
            {
                method = "userRepository.Insert",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public User GetByUsername(string username)
        {
            string[] parametrs = new string[] { username };
            Request request = new Request()
            {
                method = "userRepository.GetByUsername",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<User> response = GetResponse<User>();
            return response.value;
        }
        public bool Update(User user)
        {
            string[] parametrs = new string[] { user.UserCon() };
            Request request = new Request()
            {
                method = "userRepository.Update",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<bool> response = GetResponse<bool>();
            return response.value;
        }
    }
}
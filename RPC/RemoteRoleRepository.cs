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
    public class RemoteRoleRepository
    {
        public Socket sender;
        public RemoteRoleRepository(Socket sender)
        {
            this.sender = sender;
        }
        public Role GetById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "roleRepository.GetById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<Role> response = GetResponse<Role>();
            return response.value;
        }
        public int DeleteFilmById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "roleRepository.DeleteFilmById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int DeleteActorById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "roleRepository.DeleteActorById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int Insert(Role role)
        {
            string[] parametrs = new string[] { role.RoleCon() };
            Request request = new Request()
            {
                methodName = "roleRepository.Insert",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public Film[] GetAllFilms(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "roleRepository.GetAllFilms",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<Film[]> response = GetResponse<Film[]>();
            return response.value;
        }
        public Actor[] GetCast(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "roleRepository.GetCast",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<Actor[]> response = GetResponse<Actor[]>();
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
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
    public class RemoteActorRepository
    {
        public Socket sender;
        public RemoteActorRepository(Socket sender)
        {
            this.sender = sender;
        }
        public Actor GetById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "actorRepository.GetById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<Actor> response = GetResponse<Actor>();
            return response.value;
        }
        public int DeleteById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                methodName = "actorRepository.DeleteById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int Insert(Actor actor)
        {
            string[] parametrs = new string[] { actor.ActorCon() };
            Request request = new Request()
            {
                methodName = "actorRepository.Insert",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public bool Update(Actor actor)
        {
            string[] parametrs = new string[] { actor.ActorCon() };
            Request request = new Request()
            {
                methodName = "actorRepository.Update",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<bool> response = GetResponse<bool>();
            return response.value;
        }
        public int GetSearchPagesCount(string searchTitle)
        {
            string[] parametrs = new string[] { searchTitle };
            Request request = new Request()
            {
                methodName = "actorRepository.GetSearchPagesCount",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public List<Actor> GetSearchPage(string searchTitle, int page)
        {
            string[] parametrs = new string[] { searchTitle, page.ToString() };
            Request request = new Request()
            {
                methodName = "actorRepository.GetSearchPage",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<List<Actor>> response = GetResponse<List<Actor>>();
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
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
    public class RemoteFilmRepository
    {
        public Socket sender;
        public RemoteFilmRepository(Socket sender)
        {
            this.sender = sender;
        }
        public bool Update(Film film)
        {
            string[] parametrs = new string[] { film.FilmCon() };
            Request request = new Request()
            {
                method = "filmRepository.Update",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<bool> response = GetResponse<bool>();
            return response.value;
        }
        public Film GetById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                method = "filmRepository.GetById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<Film> response = GetResponse<Film>();
            return response.value;
        }
        public int DeleteById(int id)
        {
            string[] parametrs = new string[] { id.ToString() };
            Request request = new Request()
            {
                method = "filmRepository.DeleteById",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int Insert(Film film)
        {
            string[] parametrs = new string[] { film.FilmCon() };
            Request request = new Request()
            {
                method = "filmRepository.Insert",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public long GetCount()
        {
            string[] parametrs = new string[] { "" };
            Request request = new Request()
            {
                method = "filmRepository.GetCount",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<long> response = GetResponse<long>();
            return response.value;
        }
        public int GetSearchCount(string valueX)
        {
            string[] parametrs = new string[] { valueX };
            Request request = new Request()
            {
                method = "filmRepository.GetSearchCount",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public int[] GetAllIds()
        {
            string[] parametrs = new string[] { "" };
            Request request = new Request()
            {
                method = "filmRepository.GetAllIds",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int[]> response = GetResponse<int[]>();
            return response.value;
        }
        public int GetSearchPagesCount(string searchTitle)
        {
            string[] parametrs = new string[] { searchTitle };
            Request request = new Request()
            {
                method = "filmRepository.GetSearchPagesCount",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<int> response = GetResponse<int>();
            return response.value;
        }
        public List<Film> GetSearchPage(string searchTitle, int page)
        {
            string[] parametrs = new string[] { searchTitle, page.ToString() };
            Request request = new Request()
            {
                method = "filmRepository.GetSearchPage",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<List<Film>> response = GetResponse<List<Film>>();
            return response.value;
        }
        public List<Film> GetAll()
        {
            string[] parametrs = new string[] { "" };
            Request request = new Request()
            {
                method = "filmRepository.GetAll",
                methodParametrs = parametrs,
            };
            SendRequest(request);
            Response<List<Film>> response = GetResponse<List<Film>>();
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
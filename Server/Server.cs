using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ClassLib;
using System.Threading;
using RPC;
namespace Server
{
    public class Server
    {
        private Service service;
        public Server(Service service)
        {
            this.service = service;
        }
        public void Run()
        {
            IPAddress ipAddress = IPAddress.Loopback;
            int port = 3000;
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
 
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            try 
            {  
                listener.Bind(localEndPoint);  
                listener.Listen();
                while(true)
                {
                    Console.WriteLine($"Waiting for a connection on port {port}");
                    Socket handler = listener.Accept();

                    Thread newClientTh = new Thread(ProcessClient);
                    newClientTh.Start(handler);
                }
            }
            catch (Exception ex){Console.WriteLine(ex.Message);}
        }
        private void ProcessClient(object obj)
        {
            Socket handler = (Socket)obj;
            Console.WriteLine($"{handler.RemoteEndPoint} was successfully connected.");
            Console.WriteLine("Waiting for requests. .. ...");
            UserRequestProcess uProc = new UserRequestProcess(handler, service);
            FilmReqProcessor fProc = new FilmReqProcessor(handler, service);
            ActorReqProcessor aProc = new ActorReqProcessor(handler, service);
            ReviewReqProcessor rProc = new ReviewReqProcessor(handler, service);
            RoleReqProcessor roleProc = new RoleReqProcessor(handler, service);
            try{
                while(true)
                {
                    byte[] bytes = new byte[1024];
                    string xmlRequest = "";
                    while(true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        xmlRequest += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        if(xmlRequest.IndexOf("</request>")>-1) break;
                    }
                    Request request = ServerSerializer.DeserlizeRequest(xmlRequest);
                    Console.WriteLine($"{handler.RemoteEndPoint} --> {request.method}");
                    try
                    {
                        if(request.method.StartsWith("user"))
                            uProc.ProcessRequest(request);
                        if(request.method.StartsWith("film"))
                            fProc.ProcessRequest(request);
                        if(request.method.StartsWith("actor"))
                            aProc.ProcessRequest(request);
                        if(request.method.StartsWith("review"))
                            rProc.ProcessRequest(request);
                        if(request.method.StartsWith("role"))
                            roleProc.ProcessRequest(request);
                    }
                    catch
                    {
                        Console.WriteLine("Database connection error");
                        handler.Disconnect(false);
                        Console.WriteLine($"{handler.RemoteEndPoint} was disconnected"); break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception : {0}", ex.Message);
                Console.WriteLine($"{handler.RemoteEndPoint} was disconnected");
            }
        }
    }
}
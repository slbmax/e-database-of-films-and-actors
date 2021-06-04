using Microsoft.Data.Sqlite;
using System.Data;
using ClassLib;
using System;
using System.IO;
using RPC;
namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RemoteService repositories = new RemoteService();
            bool result = repositories.TryConnect();
            if(result == false)
            {
                Console.WriteLine("Cannot connect to server.");
                Environment.Exit(1);
            }
            GUI.SetService(repositories);
            GUI.RunInterface();

        
        }       
    }
}
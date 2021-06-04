using System;
using ClassLib;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Data;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            string databaseFileName =  @".\..\data\data.db";
            if(!File.Exists(databaseFileName))
            {
                Console.WriteLine("Error: invalid database filepath");
                Environment.Exit(1);
            }
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
            connection.Open();
            ConnectionState state = connection.State;
            if(state != ConnectionState.Open)
            {
                Console.WriteLine("Connection isn`t opened");
                Environment.Exit(1);
            }
            Service service = new Service(connection);
            Server server = new Server(service);
            server.Run();
        }
    }
}

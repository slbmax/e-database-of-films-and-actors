using Terminal.Gui;
using System.Collections.Generic;
using System.IO;
using ClassLib;
namespace ConsoleApp
{
    public static class GUI
    {
        private static Service repo;
        private static Toplevel top;
        private static User currUser;
        public static void RunInterface()
        {
            Application.Init();
            top = Application.Top;
            OnRegistration();
        }
        public static void SetService(Service service)
        {
            repo = service;
        }
        private static void OnQuit()
        {
            Application.RequestStop();
        }
        public static void OnRegistration()
        {
            RegistrationDialog dialog = new RegistrationDialog();
            dialog.SetRepository(repo.userRepository);
            top.Add(dialog);
            Application.Run();
            if(!dialog.canceled)
            {
                currUser = dialog.GetUser();
                MenuBar menu = new MenuBar(new MenuBarItem[] {
                    new MenuBarItem ("File", new MenuItem [] {
                        new MenuItem ("Export"," -- export reviews",null),
                        new MenuItem ("Import"," -- import reviews",null),
                        new MenuItem ("Report","",OnReport),
                        new MenuItem ("Exit","",OnQuit)
                    }),
                    new MenuBarItem ("Help",new MenuItem [] {
                        new MenuItem ("_About","",OnAboutButton)
                    })
                });
                MainWindow window = new MainWindow();
                window.SetService(repo);
                window.SetUser(currUser);
                top.Add(window,menu);
                Application.Run();
            }
        }      
        private static void OnAboutButton()
        {
            ProgramInfo dialog = new ProgramInfo();
            Application.Run(dialog);
        }
        private static void OnReport()
        {
            ReportDialog dialog = new ReportDialog();
            dialog.SetService(repo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Film film = dialog.GetFilm();
                string filePath = dialog.GetFilePath();
                ReportGeneration.SetData(film, filePath);
                ReportGeneration.Run();
            }
        }
    }
}
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
                        new MenuItem ("Export","export reviews",null),
                        new MenuItem ("Import","import reviews",null),
                        new MenuItem ("Exit","",OnQuit)
                    }),
                    new MenuBarItem ("Help",new MenuItem [] {
                        new MenuItem ("Help","",OnQuit)
                    })
                });
                MainWindow window = new MainWindow();
                window.SetRepositories(repo);
                window.SetUser(currUser);
                top.Add(window,menu);
                Application.Run();
            }
        }
        
    }
}
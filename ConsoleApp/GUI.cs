using Terminal.Gui;
using System.Collections.Generic;
using System.IO;
namespace ConsoleApp
{
    public static class GUI
    {
        private static Service repo;
        public static void RunInterface(Service repositories)
        {
            repo = repositories;
            Application.Init();
            Toplevel top = Application.Top;
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

            top.Add(menu,window);
            
            
            Application.Run();

        }
        private static void OnQuit()
        {
            Application.RequestStop();
        }
        
    }
}
using Terminal.Gui;
using System.Collections.Generic;
using System.IO;
using ClassLib;
using System;
using RPC;
using DataControl;
namespace ConsoleApp
{
    public static class GUI
    {
        private static RemoteService repo;
        private static Toplevel top;
        private static User currUser;
        public static void RunInterface()
        {
            Application.Init();
            top = Application.Top;
            OnRegistration();
        }
        public static void SetService(RemoteService service)
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
                        new MenuItem ("Export"," -- export reviews",OnExport),
                        new MenuItem ("Import"," -- import reviews",OnImport),
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
                double filmRating = repo.reviewRepository.GetAllFilmReviews(film.id).Count == 0
                ?0  : repo.reviewRepository.GetFilmRating(film.id);
                ReportGeneration.SetData(film, filePath,filmRating, repo.userRepository);
                ReportGeneration.Run();
                MessageBox.Query("Report","Report was generated successfull","OK");
            }

        }
        private static void OnExport()
        {
            ExportDialog dialog = new ExportDialog();
            dialog.SetService(repo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                List<Review> reviews = dialog.GetFilm().reviews;
                if(reviews.Count == 0)
                {
                    MessageBox.ErrorQuery("Export reviews","This film doesn`t have reviews yet","OK");
                    return;
                }
                string filePath = dialog.GetFilePath();
                XMLSerializer.Export(filePath+@"\Export "+ DateTime.Now.ToString().Replace(":", ".")+".xml", reviews);
                MessageBox.Query("Export reviews","Export was successfull","OK");
            }
        }
        private static void OnImport()
        {
            OpenDialog dialog = new OpenDialog("Open XML file", "Open?");
            string filePath = "";
            Application.Run(dialog);
            if (!dialog.Canceled)
            {
                NStack.ustring path = dialog.FilePath;
                filePath = path.ToString();
                XMLSerializer.SetService(repo);
                try{
                    XMLSerializer.Import(filePath, currUser.id);
                } catch(Exception ex){
                    MessageBox.ErrorQuery("Import",$"Invalid input file to import", "OK");
                }
            }
        }
    }
}
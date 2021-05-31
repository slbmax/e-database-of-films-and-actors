using Terminal.Gui;
using System;
using System.Collections.Generic;
namespace ConsoleApp
{
    public class CreateActorDialog : Dialog
    {
        public bool canceled;
        protected TextField actorFullnameInp;
        protected TextField actorCountryInp;
        protected TextField actorAgeInp;
        protected TextField actorRoles;
        protected FilmRepository filmRepo;
        protected int[] filmIntIds;
        
        public CreateActorDialog()
        {
            this.Title = "Create actor";
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.AddButton(cancelBut);
            this.AddButton(okBut);

            int posX = 25;
            int width = 40;
            Label actorFullnameLab = new Label(2,2,"Fullname:");
            actorFullnameInp = new TextField("")
            {
                X = posX, Y = Pos.Top(actorFullnameLab), Width =width 
            };
            this.Add(actorFullnameLab,actorFullnameInp);
            Label actorCountryLab = new Label(2,4,"Country:");
            actorCountryInp = new TextField("")
            {
                X = posX, Y = Pos.Top(actorCountryLab), Width =width 
            };
            this.Add(actorCountryLab,actorCountryInp);
            Label actorAgeLab = new Label(2,6,"Age:");
            actorAgeInp = new TextField("")
            {
                X = posX, Y = Pos.Top(actorAgeLab), Width =width 
            };
            this.Add(actorAgeLab,actorAgeInp);

            Label actorRolesLab = new Label(2,8,"Role in films (id):");
            actorRoles = new TextField("")
            {
                X = posX, Y = Pos.Top(actorRolesLab), Width =width 
            };
            this.Add(actorRolesLab,actorRoles);

            Label remarkLbl = new Label(2,12,$"Remark:\n -Actor`s age should be in range from 20 to 90;" +
            "\n -Ids should be written through commas, like: '1,2,3'");

            this.Add(remarkLbl);
        }
        public void SetRepository(FilmRepository filmRepository)
        {
            this.filmRepo = filmRepository;
        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnCreateDialogSubmit()
        {
            string error = "noerrors";
            int age = 0;
            if(actorFullnameInp.Text.ToString() == "")
                error = "Empty actor fullname";
            else if(actorCountryInp.Text.ToString() == "")
                error = "Empty actor country";
            else if(actorAgeInp.Text.ToString()=="" || !int.TryParse(actorAgeInp.Text.ToString(), out age))
                error = "Invalid actor year value";
            else if(age<20 || age>90)
                error = "Actor age value is in invalid range";
            else{
                string filmsId = actorRoles.Text.ToString();
                string[] arrayOfStrIds = filmsId.Split(',');
                
                if (arrayOfStrIds[0] == "" && arrayOfStrIds.Length == 1)
                {
                    error = "noerrors";
                    filmIntIds = new int[arrayOfStrIds.Length];
                }
                else{
                    filmIntIds = new int[arrayOfStrIds.Length];
                    for(int i = 0; i < filmIntIds.Length; i++)
                    {
                        try{
                            filmIntIds[i] = int.Parse(arrayOfStrIds[i]);
                        }
                        catch{
                            error = $"Non-integer film id '{arrayOfStrIds[i]}'";
                            break;
                        }
                    }
                    if(error == "noerrors")
                    {
                        List<int> uniqueId = new List<int>();
                        foreach(int id in filmIntIds)
                        {
                            if(uniqueId.Contains(id))
                            {
                                error = $"Same film id value '{id}'";
                                break;
                            }
                            Film film = filmRepo.GetById(id);
                            if(film == null)
                            {
                                error = $"Non-existing film with id '{id}'";
                                break;
                            }
                            uniqueId.Add(id);
                        }
                    }
                }
            }
            if(error == "noerrors")
            {
                this.canceled = false;
                Application.RequestStop();
            }
            else
                MessageBox.ErrorQuery("Error",$"{error}", "OK");
        }
        public Actor GetActor()
        {
            return new Actor()
            {
                fullname = actorFullnameInp.Text.ToString(),
                country = actorCountryInp.Text.ToString(),
                age = int.Parse(actorAgeInp.Text.ToString())
            };
        }
        public int[] GetFilmsId()
        {
            return filmIntIds;
        }
    }
}
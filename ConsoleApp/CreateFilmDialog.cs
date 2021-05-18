using Terminal.Gui;
using System;
using System.Collections.Generic;
namespace ConsoleApp
{
    public class CreateFilmDialog : Dialog
    {
        public bool canceled;
        protected TextField filmTitleInp;
        protected TextField filmGenreInp;
        protected TextField filmYearInp;
        protected TextField filmCast;
        protected ActorRepository actorRepo;
        protected FilmRepository filmRepo;
        protected int[] actorIntIds;
        public CreateFilmDialog()
        {
            this.Title = "Create film";
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.AddButton(cancelBut);
            this.AddButton(okBut);

            int posX = 20;
            int width = 40;
            Label filmTitleLab = new Label(2,2,"Title:");
            filmTitleInp = new TextField("")
            {
                X = posX, Y = Pos.Top(filmTitleLab), Width =width
            };
            this.Add(filmTitleLab,filmTitleInp);
            Label filmGenreLab = new Label(2,4,"Genre:");
            filmGenreInp = new TextField("")
            {
                X = posX, Y = Pos.Top(filmGenreLab), Width =width
            };
            this.Add(filmGenreLab,filmGenreInp);
            Label filmYearLab = new Label(2,6,"Year:");
            filmYearInp = new TextField("")
            {
                X = posX, Y = Pos.Top(filmYearLab), Width =width
            };
            this.Add(filmYearLab,filmYearInp);
            Label filmCastLab = new Label(2,8,"Cast (actor ids):");
            filmCast = new TextField("")
            {
                X = posX, Y = Pos.Top(filmCastLab), Width =width
            };
            this.Add(filmCastLab,filmCast);
            Label remarkLbl = new Label(2,12,$"Remark:\n -Year value should be in range from 2000 to 2021" +
            "\n -Ids should be written through commas, like: '1,2,3'; Maximum of film`s cast is 10 actors");

            this.Add(remarkLbl);

        }
        private void OnCreateDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetRepositories(ActorRepository actorRepository, FilmRepository filmRepository)
        {
            this.actorRepo = actorRepository;
            this.filmRepo = filmRepository;
        }
        private void OnCreateDialogSubmit()
        {
            string error = "noerrors";
            int year = 0;
            if(filmTitleInp.Text.ToString() == "")
                error = "Empty film title";
            else if(filmGenreInp.Text.ToString() == "")
                error = "Empty film genre";
            else if(filmYearInp.Text.ToString()=="" || !int.TryParse(filmYearInp.Text.ToString(), out year))
                error = "Invalid film year value";
            else if(year <2000 || year >2021)
                error = "Film year value is in invalid range";
            else{
                string actorsId = filmCast.Text.ToString();
                string[] arrayOfStrIds = actorsId.Split(',');
                if(arrayOfStrIds.Length >10)
                {
                    error = "Probably more than 10 actors are entered";
                }
                else if(arrayOfStrIds[0] == "")
                {
                    error = "noerrors";
                }
                 else{
                    actorIntIds = new int[arrayOfStrIds.Length];
                    for(int i = 0; i < actorIntIds.Length; i++)
                    {
                        try{
                            actorIntIds[i] = int.Parse(arrayOfStrIds[i]);
                        }
                        catch{
                            error = $"Non-integer actor id {arrayOfStrIds[i]}";
                            break;
                        }
                    }
                    if(error == "noerrors")
                    {
                        List<int> uniqueId = new List<int>();
                        foreach(int id in actorIntIds)
                        {
                            if(uniqueId.Contains(id))
                            {
                                error = $"Same actor id value {id}";
                                break;
                            }
                            Actor actor = actorRepo.GetById(id);
                            if(actor == null)
                            {
                                error = $"Non-existing actor with id {id}";
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
        public Film GetFilm()
        {
            return new Film()
            {
                title = filmTitleInp.Text.ToString(),
                genre = filmGenreInp.Text.ToString(),
                releaseYear = int.Parse(filmYearInp.Text.ToString())
            };
        }
        public int[] GetActorsId()
        {
            if(actorIntIds == null) throw new Exception("Empty id field");
            return actorIntIds;
        }
    }
}
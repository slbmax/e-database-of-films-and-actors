using Terminal.Gui;
using System.Collections.Generic;
using ClassLib;
using RPC;
namespace ConsoleApp
{
    public class OpenActorDialog : Window
    {
        public bool canceled;
        public bool deleted = false;
        public bool edited = false;
        private RemoteService service;
        private Label actorFullname;
        private Label actorCountry;
        private Label actorAge;
        private ListView allFilmsListView;
        private Actor actor;
        public Button deleteActor;
        public Button editActor;
        public User user;
        private int[] filmIntIds;
        public OpenActorDialog()
        {
            this.Title = "Open actor"; this.Width = Dim.Fill(); this.Height = Dim.Fill();
            Button cancelBut = new Button("Cancel") {X = Pos.Percent(87),Y = Pos.Percent(95)};
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.Add(cancelBut);

            int posX = 20;
            int width = 40;
            Label actorFullnameLab = new Label(2,2,"Fullname:");
            actorFullname = new Label(" ")
            {
                X = posX, Y = Pos.Top(actorFullnameLab), Width =width
            };
            this.Add(actorFullnameLab,actorFullname);
            Label actorCountryLab = new Label(2,4,"Country:");
            actorCountry = new Label(" ")
            {
                X = posX, Y = Pos.Top(actorCountryLab), Width =width
            };
            this.Add(actorCountryLab,actorCountry);
            Label actorAgeLab = new Label(2,6,"Age:");
            actorAge = new Label(" ")
            {
                X = posX, Y = Pos.Top(actorAgeLab), Width =width
            };
            this.Add(actorAgeLab,actorAge);

            Label actorRolesLab = new Label(2,8,"Roles:");
            this.Add(actorRolesLab);

            allFilmsListView = new ListView(new List<Film>())
            {
                X = posX, Y = Pos.Top(actorRolesLab), Width = Dim.Fill()-5, Height = 5
            };
            this.Add(allFilmsListView);

            deleteActor = new Button("Delete"){X = 2, Y = Pos.Bottom(actorRolesLab)+6};
            deleteActor.Clicked += OnDeleteActor;
            editActor = new Button("Edit"){X = Pos.Right(deleteActor)+2, Y = Pos.Top(deleteActor)};
            editActor.Clicked += OnEditActor;
            this.Add(deleteActor, editActor);
        }
        private void OnOpenDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetUser(User user)
        {
            this.user = user;
            if(user.role == "user")
            {
                deleteActor.Visible = false;
                editActor.Visible = false;
            }
        }
        private List<Film> GetListOfFilms()
        {
            List<Film> films = new List<Film>();
            for(int i = 0; i<actor.films.Length; i ++)
            {
                films.Add(actor.films[i]);
            }
            return films;
        }
        public void SetActor(Actor actor)
        {
            this.actor = actor;
            this.actorFullname.Text = actor.fullname;
            this.actorCountry.Text = actor.country;
            this.actorAge.Text = actor.age.ToString();
            allFilmsListView.SetSource(GetListOfFilms().Count != 0 ? GetListOfFilms() : new List<string>(){"Actor has not acted in a films yet"});
        }
        private void OnDeleteActor()
        {
            int index =MessageBox.Query("Delete actor","Are you sure?","Yes","No");
            if(index == 0)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditActor()
        {
            EditActor dialog = new EditActor();
            dialog.SetActor(actor);
            dialog.SetRepository(service.filmRepository);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.edited = true;
                Actor newActor = dialog.GetActor();
                newActor.id = actor.id;
                service.actorRepository.Update(newActor);
                service.roleRepository.DeleteActorById(actor.id);
                filmIntIds = dialog.GetFilmsId();
                if(filmIntIds != null)
                {
                    foreach(int id in filmIntIds)
                    {   
                        if(id ==0 ) continue;
                        Role role = new Role(){actor_id = newActor.id, film_id = id};
                        service.roleRepository.Insert(role);
                    }
                }

                newActor.films = service.roleRepository.GetAllFilms(newActor.id);
                actor = newActor;
                SetActor(actor);
                MessageBox.Query("Edit actor","Actor was edited succesfully","OK");
            }
        }
        public Actor GetActor()
        {
            return actor;
        }
        public int[] GetFilmsId()
        {
            return filmIntIds;
        }
        public void SetService(RemoteService filmRepo)
        {
            this.service = filmRepo;
        }
    }
}
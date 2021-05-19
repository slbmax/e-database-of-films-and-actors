using Terminal.Gui;
using System;
namespace ConsoleApp
{
    public class OpenActorDialog : Dialog
    {
        public bool canceled;
        public bool deleted = false;
        public bool edited = false;
        private ActorRepository actorRepo;
        private FilmRepository filmRepo;
        private TextField actorFullname;
        private TextField actorCountry;
        private TextField actorAge;
        private Button deleteActor;
        private Actor actor;
        private int[] filmIntIds;
        public OpenActorDialog()
        {
            this.Title = "Open actor";
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnOpenDialogCanceled;

            this.AddButton(cancelBut);

            int posX = 20;
            Label actorFullnameLab = new Label(2,2,"Fullname:");
            actorFullname = new TextField("")
            {
                X = posX, Y = Pos.Top(actorFullnameLab), Width =40
            };
            actorFullname.ReadOnly = true;
            this.Add(actorFullnameLab,actorFullname);
            Label actorCountryLab = new Label(2,4,"Country:");
            actorCountry = new TextField("")
            {
                X = posX, Y = Pos.Top(actorCountryLab), Width =40
            };
            actorCountry.ReadOnly = true;
            this.Add(actorCountryLab,actorCountry);
            Label actorAgeLab = new Label(2,6,"Age:");
            actorAge = new TextField("")
            {
                X = posX, Y = Pos.Top(actorAgeLab), Width =40
            };
            this.Add(actorAgeLab,actorAge);
            actorAge.ReadOnly = true;

            deleteActor = new Button("Delete"){X = 2, Y = Pos.Bottom(actorAge)+6};
            deleteActor.Clicked += OnDeleteActor;
            Button editActor = new Button("Edit"){X = Pos.Right(deleteActor)+2, Y = Pos.Bottom(actorAge)+6};
            editActor.Clicked += OnEditActor;
            this.Add(deleteActor, editActor);

            
        }
        private void OnOpenDialogCanceled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        public void SetActor(Actor actor)
        {
            this.actor = actor;
            this.actorFullname.Text = actor.fullname;
            this.actorCountry.Text = actor.country;
            this.actorAge.Text = actor.age.ToString();
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
            dialog.SetRepositories(actorRepo, filmRepo);
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                this.edited = true;
                actor = dialog.GetActor();
                filmIntIds = dialog.GetFilmsId();
                OnOpenDialogCanceled();
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
        public void SetRepositories(ActorRepository repository, FilmRepository filmRepo)
        {
            this.actorRepo = repository;
            this.filmRepo = filmRepo;
        }
    }
}
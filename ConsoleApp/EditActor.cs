using Terminal.Gui;
namespace ConsoleApp
{
    public class EditActor : CreateActorDialog
    {
        public EditActor()
        {
            this.Title = "Edit actor";
        }
        public void SetActor(Actor actor)
        {
            this.actorAgeInp.Text = actor.age.ToString();
            this.actorCountryInp.Text = actor.country;
            this.actorFullnameInp.Text = actor.fullname;
            string ids = "";
            for(int i =0; i<actor.films.Length; i++)
            {
                ids = ids + $"{actor.films[i].id}";
                if(i+1 != actor.films.Length)
                    ids = ids +","; 
            }
            this.actorRoles.Text = ids;
        }
    }
}
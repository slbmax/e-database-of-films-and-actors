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
        }
    }
}
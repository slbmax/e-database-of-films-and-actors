using Terminal.Gui;
namespace ConsoleApp
{
    public class CreateActorDialog : Dialog
    {
        public bool canceled;
        public TextField actorFullnameInp;
        public TextField actorCountryInp;
        public TextField actorAgeInp;
        public CreateActorDialog()
        {
            this.Title = "Create actor";
            Button okBut = new Button("OK");
            okBut.Clicked += OnCreateDialogSubmit;
            Button cancelBut = new Button("Cancel");
            cancelBut.Clicked += OnCreateDialogCanceled;
            this.AddButton(cancelBut);
            this.AddButton(okBut);

            int posX = 20;
            Label actorFullnameLab = new Label(2,2,"Fullname:");
            actorFullnameInp = new TextField("")
            {
                X = posX, Y = Pos.Top(actorFullnameLab), Width =40
            };
            this.Add(actorFullnameLab,actorFullnameInp);
            Label actorCountryLab = new Label(2,4,"Country:");
            actorCountryInp = new TextField("")
            {
                X = posX, Y = Pos.Top(actorCountryLab), Width =40
            };
            this.Add(actorCountryLab,actorCountryInp);
            Label actorAgeLab = new Label(2,6,"Age:");
            actorAgeInp = new TextField("")
            {
                X = posX, Y = Pos.Top(actorAgeLab), Width =40
            };
            this.Add(actorAgeLab,actorAgeInp);

            Label remarkLbl = new Label(2,10,"Remark: Actor`s age should be in range from 20 to 90");

            this.Add(remarkLbl);
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
    }
}
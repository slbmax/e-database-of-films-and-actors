using Terminal.Gui;
namespace ConsoleApp
{
    public class ProgramInfo : Dialog
    {
        public ProgramInfo()
        {
            this.Title = "Program info";
            Button cancelBut = new Button("OK");
            cancelBut.Clicked += OnCancelButtonClicked;
            this.AddButton(cancelBut);

            TextView info = new TextView()
            {
                X = Pos.Center(), Y = 8, Width = 60, Height = 10,
                Text = "Hello! This is a FilMax - an e-database of films and actors."+
                "\nHere you can: "+ "\n-create own reviews;" +
                "\n-view a list of all films, actors and other reviews;" +
                "\n-edit and delete your own reviews"
            };

            TextView author = new TextView()
            {
                X = 2, Y = 19 , Text = "Made by:\nMaxim Slobodzian, KP-02\nStudent of KPI", Width =25, Height = 5
            };
            this.Add(info, author);
        }
        private void OnCancelButtonClicked()
        {
            Application.RequestStop();
        }
    }
}
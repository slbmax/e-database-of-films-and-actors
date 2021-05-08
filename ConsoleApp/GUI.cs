using Terminal.Gui;
namespace ConsoleApp
{
    public static class GUI
    {
        public static void RunInterface()
        {
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Export","export reviews",null),
                    new MenuItem ("_Import","import reviews",null),
                    new MenuItem ("Exit","",OnQuit)
                }),
                new MenuBarItem ("_Help","",null)
            });
            Window win = new Window("e-database of films and actors")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(menu, win);

            int mainButtonsX = 2;
            Button createEntBut = new Button(mainButtonsX, 1, "Create new entity");
            createEntBut.Clicked += OnCreateButtonClicked;

            Button viewPageBut = new Button(mainButtonsX, 3, "View all entities");
            viewPageBut.Clicked += OnPageButtonClicked;

            Button viewEntity = new Button(mainButtonsX, 5, "Select entity");
            viewEntity.Clicked += OnViewEntityButtonClicked;

            Button deleteEntity = new Button(mainButtonsX, 7, "Delete entity");
            deleteEntity.Clicked += OnDeleteEntityButtonClicked;

            Button editEntity = new Button(mainButtonsX, 9, "Edit entity");
            editEntity.Clicked += OnEditEntityButtonClicked;
            
            win.Add(createEntBut, viewPageBut, viewEntity, deleteEntity, editEntity);
            Application.Run();

        }
        static void OnQuit()
        {
            Application.RequestStop();
        }
        static void OnCreateButtonClicked()
        {
            Window window = new Window("")
            {
                X = 30,
                Y = 2,
                Width = 30,
                Height = 12
            };
            Label label = new Label(1, 0, "Select entity to create:");
            Button cancelBut = new Button("Cancel");
            cancelBut.X = 1;
            cancelBut.Y = Pos.Bottom(window)-5;
            cancelBut.Clicked += OnQuit;

            Button filmBut = new Button(1,2,"Film");
            filmBut.Clicked += OnCreateFilmButton;

            Button actorBut = new Button(1,4,"Actor");
            actorBut.Clicked += OnCreateActorButton;

            Button reviewBut = new Button(1,6,"Review");
            reviewBut.Clicked += OnCreateReviewButton;
            

            window.Add(label, cancelBut, filmBut, actorBut, reviewBut);

            Application.Run(window);
        }
        static void OnPageButtonClicked()
        {

        }
        static void OnViewEntityButtonClicked()
        {

        }
        static void OnDeleteEntityButtonClicked()
        {

        }
        static void OnEditEntityButtonClicked()
        {

        }
        static void OnCreateFilmButton()
        {
            CreateFilmDialog dialog = new CreateFilmDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Film film = dialog.GetFilm();
            }
        }
        static void OnCreateActorButton()
        {
            CreateActorDialog dialog = new CreateActorDialog();
            Application.Run(dialog);
            if(!dialog.canceled)
            {
                Actor actor = dialog.GetActor();
            }
        }
        static void OnCreateReviewButton()
        {

        }

    }
}
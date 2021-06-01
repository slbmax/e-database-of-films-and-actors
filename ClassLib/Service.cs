using Microsoft.Data.Sqlite;
namespace ClassLib
{
    public class Service
    {
        public ActorRepository actorRepository;
        public FilmRepository filmRepository;
        public ReviewRepository reviewRepository;
        public UserRepository userRepository;
        public RoleRepository roleRepository;
        public Service(SqliteConnection connection)
        {
            this.actorRepository = new ActorRepository(connection);
            this.filmRepository = new FilmRepository(connection);
            this.reviewRepository = new ReviewRepository(connection);
            this.userRepository = new UserRepository(connection);
            this.roleRepository = new RoleRepository(connection);
        }
    }
}
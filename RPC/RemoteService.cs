using System.Net;
using System.Net.Sockets;
namespace RPC
{
    public class RemoteService
    {
        public RemoteActorRepository actorRepository;
        public RemoteFilmRepository filmRepository;
        public RemoteRoleRepository roleRepository;
        public RemoteReviewRepository reviewRepository;
        public RemoteUserRepository userRepository;
        public bool TryConnect()
        {
            IPAddress ipAddress = IPAddress.Loopback;
            int port = 3000;

            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            try
            {
                sender.Connect(remoteEP);
            }
            catch
            {
                return false;
            }
            actorRepository = new RemoteActorRepository(sender);
            filmRepository = new RemoteFilmRepository(sender);
            roleRepository = new RemoteRoleRepository(sender);
            reviewRepository = new RemoteReviewRepository(sender);
            userRepository = new RemoteUserRepository(sender);
            return true;
        }
    }
}
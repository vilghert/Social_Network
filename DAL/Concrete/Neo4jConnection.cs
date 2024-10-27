using Neo4j.Driver;

namespace Social_Network.DAL.Concrete
{
    public class Neo4JConnection : IDisposable
    {
        private readonly IDriver _driver;

        public Neo4JConnection(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        }

        public IDriver GetDriver()
        {
            return _driver;
        }
        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}

namespace IwbrDaemon.Types.Config 
{
    public class MongoDb
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Database { get; private set; }
        public MongoDb(string host, int port, string username, string password, string database)
        {
            Host = host;
            Port = port;
            Username = username;
            Database = database;

            Password = password.Replace("@","%40");

        }
    }
}
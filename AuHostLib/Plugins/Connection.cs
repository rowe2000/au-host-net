namespace AuHost.Plugins
{
    public class Connection
    {
        public Connection(Connector source, Connector destination)
        {
            Source = source;
            Destination = destination;
        }

        public Connector Source { get; }
        public Connector Destination { get; }
    }
}
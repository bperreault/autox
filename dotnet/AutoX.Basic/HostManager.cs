namespace AutoX.Basic
{
    public class HostManager
    {
        private static readonly HostManager Instance = new HostManager();

        private IHost _host;

        private HostManager()
        {
        }

        public static HostManager GetInstance()
        {
            return Instance;
        }

        public void Register(IHost host)
        {
            _host = host;
        }

        public IHost GetHost()
        {
            return _host;
        }
    }
}
using System.ServiceProcess;

namespace AutoX.WindowsService
{
    public partial class AutoXService : ServiceBase
    {
        public AutoXService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
        }

        protected override void OnStop()
        {
        }
    }
}

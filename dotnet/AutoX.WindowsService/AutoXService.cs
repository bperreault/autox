using System.ServiceProcess;
using AutoX.Basic;

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
            Log.Debug("AutoX Windows Service Start ...");
        }

        protected override void OnStop()
        {
            Log.Debug("AutoX Windows Service Stop ...");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoX.Basic;

namespace AutoX.Client.Core
{
    public class AutoClient
    {
        private Config _config;
        private Browser _browser;

        public AutoClient(Config config)
        {
            _config = config;
        }
        public AutoClient()
        {
            _config = Configuration.Clone();
        }


    }
}

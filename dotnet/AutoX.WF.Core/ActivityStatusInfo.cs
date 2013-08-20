namespace AutoX.WF.Core
{
    public class ActivityStatusInfo
    {
        private readonly string NameValue;
        private readonly string StatusValue;
        private readonly string SourceValue;

        public ActivityStatusInfo(string name, string status, string source)
        {
            NameValue = name;
            StatusValue = status;
            SourceValue = source;
        }

        public ActivityStatusInfo(string name, string status)
        {
            NameValue = name;
            StatusValue = status;
            SourceValue = null;
        }

        public string Source
        {
            get { return SourceValue; }
        }

        public string Name
        {
            get { return NameValue; }
        }

        public string Status
        {
            get { return StatusValue; }
        }
    }
}
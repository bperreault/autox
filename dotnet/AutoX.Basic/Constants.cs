using System.ComponentModel;
using System.Globalization;

namespace AutoX.Basic
{
    public static class Constants
    {
        public const string CONTENT = "Content";
        public const string ON_ERROR = "OnError";
        public const string DATA = "Data";
        public const string _ID = "_id";
        public const string _TYPE = "_type";
        public const string NAME = "Name";
        public const string _NAME = "name";
        public const string RESULT = "Result";
        public const string ACTION = "Action";
        public const string STEP = "Step";
        public const string INSTANCE_ID = "InstanceId";
        public const string PARENT_ID = "_parentId";
        public const string DATA_FORMAT = "DataFormat";
        public const string SCRIPT = "Script";
        public const string DATUM = "Datum";
        public const string UI_OBJECT = "UIObject";
        public const string ENABLE = "Enable";
        public const string DEFAULT_DATA = "DefaultData";
        public const string UI_ID = "UIId";
        public const string RUNTIME_ID = "RunTimeId";
        public const string SCRIPT_TYPE = "ScriptType";
        public const string SET_ENV = "SetEnv";
        public const string SUCCESS = "Success";
        public const string ERROR = "Error";

        public const string XPATH = "XPath";
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
    }

    public enum MaturityLevel
    {
        [Description("Backyard")]Backyard,
        [Description("Playground")]Playground,
        [Description("Stadium")]Stadium

    }
}
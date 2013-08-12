#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

#endregion

#endregion

namespace AutoX.Basic
{
    public static class Configuration
    {
        /// <summary>
        ///   Now we have below settings now:
        ///   DBConnectionString="mongodb://hostdb"
        ///   DBName = "Automation"
        ///   LogFormat="%newline%date [%thread] %-5level [%message%]%newline"
        ///   LogPath = "c:\temp"
        ///   LogName = "Automation.log"
        /// </summary>
        /// <param name="setting"> </param>
        /// <param name="defaultString"> </param>
        /// <returns> </returns>
        public static string Settings(string setting, string defaultString = null)
        {
            var value = ConfigurationManager.AppSettings[setting];
            if (!string.IsNullOrEmpty(value))
                return value;
            return defaultString;
        }

        public static string ConnectionString()
        {
            var conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            if (string.IsNullOrEmpty(conn))
                conn = Settings("Database", "mongodb://localhost");
            return conn;
        }

        public static NameValueCollection GetSettings()
        {
            return ConfigurationManager.AppSettings;
        }

        public static void Set(string key, string value)
        {
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationManager.AppSettings.Set(key, value);
        }

        public static void AddSettingsToXElement(XElement x)
        {
            foreach (object key in ConfigurationManager.AppSettings.Keys)
            {
                x.SetAttributeValue(key.ToString(), Settings(key.ToString()));
            }
        }

        public static string GetClientActionClass(string action)
        {
            var sClass = Settings(action, "");
            return string.IsNullOrEmpty(sClass) ? "AutoX.Client." + action : sClass;
        }

        public static bool HasKey(string key)
        {
            var keyValue = Settings(key);
            return keyValue != null;
        }

        public static ArrayList GetSupportedAction()
        {
            var ret = new ArrayList
            {
                "Enter",
                "Click",
                "Check",
                "Close",
                "Command",
                "Start",
                "Wait",
                "GetValue",
                "Existed",
                "Not Existed",
                "Verify Value"
            };
            var support = Settings("SupportedActions", "");
            var ss = support.Split(',', ';', '|');
            var sa = new ArrayList();
            foreach (string s in ss.Where(s => !string.IsNullOrEmpty(s)))
            {
                sa.Add(s);
            }
            return sa.Count > 0 ? sa : ret;
        }

        /// <summary>
        ///   we can only save the settings of editor, client; cannot save the settings of server side--web.config
        /// </summary>
        public static void SaveSettings()
        {
            var config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var app = config.AppSettings;
            foreach (object key in ConfigurationManager.AppSettings.Keys)
            {
                if (app.Settings[key.ToString()] == null)
                    app.Settings.Add(key.ToString(), Settings(key.ToString(), key.ToString()));
                app.Settings[key.ToString()].Value = Settings(key.ToString(), key.ToString());
                //ConfigurationManager.AppSettings.Set(key.ToString(),app.Settings[key.ToString()].Value);
            }
            foreach (var s in app.Settings.AllKeys)
            {
                ConfigurationManager.AppSettings.Set(s,app.Settings[s].Value);
            }

            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //AppSettingsSection app = config.AppSettings;
            config.Save(ConfigurationSaveMode.Modified);

            //config.SaveAs("c:\\test.cfg");
        }

        public static Config Clone()
        {
            var cfg = new Config();

            //var config =
            //    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            foreach (object key in ConfigurationManager.AppSettings.Keys)
            {
                cfg.Set(key.ToString(), Settings(key.ToString(), key.ToString()));
            }
            return cfg;
        }
    }
}
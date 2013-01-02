#region

using System.Diagnostics;

#endregion

namespace AutoX.Basic
{
    public static class Log
    {
        public static void Debug(string message)
        {
            Logger.GetInstance().Log().Debug(GetPosition() + message);
        }

        public static void Info(string message)
        {
            Logger.GetInstance().Log().Info(GetPosition() + message);
        }

        public static void Warn(string message)
        {
            Logger.GetInstance().Log().Warn(GetPosition() + message);
        }

        public static void Error(string message)
        {
            Logger.GetInstance().Log().Error(GetPosition() + message);
        }

        public static void Fatal(string message)
        {
            Logger.GetInstance().Log().Fatal(GetPosition() + message);
        }

        private static string GetPosition()
        {
            var st = new StackTrace(new StackFrame(2, true));

            var currentFile = st.GetFrame(0).GetFileName();
            var currentLine = st.GetFrame(0).GetFileLineNumber();
            return "File:" + currentFile + " Line:" + currentLine + "\t";
        }
    }
}
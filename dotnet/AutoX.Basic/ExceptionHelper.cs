using System;
using System.Text;

namespace AutoX.Basic
{
    public class ExceptionHelper
    {
        public static string FormatStackTrace(Exception exception)
        {
            var sb = new StringBuilder();
            sb.Append(exception.Message);
            sb.Append(exception.StackTrace);

            return exception.InnerException != null ? sb.Append(FormatStackTrace(exception.InnerException)).ToString() : sb.ToString();
        }

        public static string FormatStackTrace(string message, Exception exception)
        {
            return message + "\n" + FormatStackTrace(exception);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace AutoX.WindowsService
{
    public class HttpListenerWorkerRequest : HttpWorkerRequest
    {
        private readonly HttpListenerContext _context;
        private readonly string _virtualDir;
        private readonly string _physicalDir;

        public HttpListenerWorkerRequest(
            HttpListenerContext context, string vdir, string pdir)
        {
            if (null == context)
                throw new ArgumentNullException("context");
            if (null == vdir || vdir.Equals(""))
                throw new ArgumentException("vdir");
            if (null == pdir || pdir.Equals(""))
                throw new ArgumentException("pdir");

            _context = context;
            _virtualDir = vdir;
            _physicalDir = pdir;
        }

        // required overrides (abstract)
        public override void EndOfRequest()
        {
            _context.Response.OutputStream.Close();
            _context.Response.Close();

            //TODO watch this !!! _context.Close();
        }
        public override void FlushResponse(bool finalFlush)
        {
            _context.Response.OutputStream.Flush();
        }
        public override string GetHttpVerbName()
        {
            return _context.Request.HttpMethod;
        }
        public override string GetHttpVersion()
        {
            return string.Format("HTTP/{0}.{1}",
                _context.Request.ProtocolVersion.Major,
                _context.Request.ProtocolVersion.Minor);
        }
        public override string GetLocalAddress()
        {
            return _context.Request.LocalEndPoint.Address.ToString();
        }
        public override int GetLocalPort()
        {
            return _context.Request.LocalEndPoint.Port;
        }
        public override string GetQueryString()
        {
            var queryString = "";
            var rawUrl = _context.Request.RawUrl;
            var index = rawUrl.IndexOf('?');
            if (index != -1)
                queryString = rawUrl.Substring(index + 1);
            return queryString;
        }
        public override string GetRawUrl()
        {
            return _context.Request.RawUrl;
        }
        public override string GetRemoteAddress()
        {
            return _context.Request.RemoteEndPoint.Address.ToString();
        }
        public override int GetRemotePort()
        {
            return _context.Request.RemoteEndPoint.Port;
        }
        public override string GetUriPath()
        {
            return _context.Request.Url.LocalPath;
        }
        public override void SendKnownResponseHeader(int index, string value)
        {
            _context.Response.Headers[
                HttpWorkerRequest.GetKnownResponseHeaderName(index)] = value;
        }
        public override void SendResponseFromMemory(byte[] data, int length)
        {
            _context.Response.OutputStream.Write(data, 0, length);
        }
        public override void SendStatus(int statusCode, string statusDescription)
        {
            _context.Response.StatusCode = statusCode;
            _context.Response.StatusDescription = statusDescription;
        }
        public override void SendUnknownResponseHeader(string name, string value)
        {
            _context.Response.Headers[name] = value;
        }
        public override void SendResponseFromFile(
            IntPtr handle, long offset, long length) { }
        public override void SendResponseFromFile(
            string filename, long offset, long length) { }

        // additional overrides
        public override void CloseConnection()
        {
            //TODO Watch this !!! _context.Close();

            EndOfRequest();
        }
        public override string GetAppPath()
        {
            return _virtualDir;
        }
        public override string GetAppPathTranslated()
        {
            return _physicalDir;
        }
        public override int ReadEntityBody(byte[] buffer, int size)
        {
            return _context.Request.InputStream.Read(buffer, 0, size);
        }
        public override string GetUnknownRequestHeader(string name)
        {
            return _context.Request.Headers[name];
        }
        public override string[][] GetUnknownRequestHeaders()
        {
            var headers = _context.Request.Headers;
            var count = headers.Count;
            var headerPairs = new List<string[]>(count);
            for (var i = 0; i < count; i++)
            {
                var headerName = headers.GetKey(i);
                if (GetKnownRequestHeaderIndex(headerName) != -1) continue;
                var headerValue = headers.Get(i);
                headerPairs.Add(new string[] { headerName, headerValue });
            }
            var unknownRequestHeaders = headerPairs.ToArray();
            return unknownRequestHeaders;
        }
        public override string GetKnownRequestHeader(int index)
        {
            switch (index)
            {
                case HeaderUserAgent:
                    return _context.Request.UserAgent;
                default:
                    return _context.Request.Headers[GetKnownRequestHeaderName(index)];
            }
        }
        public override string GetServerVariable(string name)
        {
            // TODO: vet this list
            switch (name)
            {
                case "HTTPS":
                    return _context.Request.IsSecureConnection ? "on" : "off";
                case "HTTP_USER_AGENT":
                    return _context.Request.Headers["UserAgent"];
                default:
                    return null;
            }
        }
        public override string GetFilePath()
        {
            // TODO: this is a trick
            var s = _context.Request.Url.LocalPath;
            //Log.Debug("LocalPath:"+s);
            if (s.IndexOf(".aspx", StringComparison.Ordinal) != -1)
                s = s.Substring(0, s.IndexOf(".aspx", StringComparison.Ordinal) + 5);
            else if (s.IndexOf(".asmx", StringComparison.Ordinal) != -1)
                s = s.Substring(0, s.IndexOf(".asmx", StringComparison.Ordinal) + 5);
            return s;
        }
        public override string GetFilePathTranslated()
        {
            string s = GetFilePath();
            s = s.Substring(_virtualDir.Length);
            s = s.Replace('/', '\\');
            return _physicalDir + s;
        }

        public override string GetPathInfo()
        {
            var s1 = GetFilePath();
            var s2 = _context.Request.Url.LocalPath;
            return s1.Length == s2.Length ? "" : s2.Substring(s1.Length);
        }
    }
}
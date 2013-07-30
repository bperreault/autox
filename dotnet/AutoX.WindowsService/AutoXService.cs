using System.ServiceProcess;
using AutoX.Basic;
using System.Net;
using System.Threading;
using System.IO;
using System;

namespace AutoX.WindowsService
{
    public partial class AutoXService : ServiceBase
    {
        public AutoXService()
        {
            InitializeComponent();
        }

        HttpListener listener = new HttpListener();
        private Thread listenThread1;

        protected override void OnStart(string[] args)
        {
            Log.Debug("AutoX Windows Service Start ...");
            listener.Prefixes.Add("http://localhost/");
            listener.Start();
            Log.Debug("Listener start ...");
            this.listenThread1 = new Thread(new ParameterizedThreadStart(startlistener));
            listenThread1.Start();
            Log.Debug("Listener thread start ...");
            Thread.Sleep(60 * 5 * 1000);
        }

        private void startlistener(object s)
           {
   
               while (true)
               {
                  
                   ////blocks until a client has connected to the server
                   ProcessRequest();
   
               }
   
           }
   
   
           private void ProcessRequest()
           {
   
               var result = listener.BeginGetContext(ListenerCallback, listener);
               result.AsyncWaitHandle.WaitOne();
   
           }
   
           private void ListenerCallback(IAsyncResult result)
           {
               var context = listener.EndGetContext(result);
               Thread.Sleep(1000);
               var data_text = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
   
               //functions used to decode json encoded data.
               //JavaScriptSerializer js = new JavaScriptSerializer();
               //var data1 = Uri.UnescapeDataString(data_text);
               //string da = Regex.Unescape(data_text);
               // var unserialized = js.Deserialize(data_text, typeof(String));
               Log.Debug(data_text);
   
               var cleaned_data = System.Web.HttpUtility.UrlDecode(data_text);
   
               context.Response.StatusCode = 200;
               context.Response.StatusDescription = "OK";
   
               //use this line to get your custom header data in the request.
               //var headerText = context.Request.Headers["mycustomHeader"];
   
               //use this line to send your response in a custom header
               //context.Response.Headers["mycustomResponseHeader"] = "mycustomResponse";
               
               Log.Debug(cleaned_data);
               context.Response.Close();
           }

        protected override void OnStop()
        {
            Log.Debug("AutoX Windows Service Stop ...");
            listener.Stop();
        }
    }
}

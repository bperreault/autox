// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Diagnostics;
using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    public class Command : AbstractAction
    {
        public override XElement Act()
        {
            var sr = new StepResult(this);
            //because it is .net, so we can only work on dos
            DosCommand(Data);
            return sr.GetResult();
        }

        public static void DosCommand(string cmd, string param)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = cmd,
                    Arguments = param,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false
                }
            };
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();
        }

        public static void DosCommand(string param)
        {
            DosCommand("cmd", " /c " + param);
        }
    }
}
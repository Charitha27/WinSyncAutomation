using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using LoggerCollection;
using FileOperationsCollection;

namespace WaitForNewVMToComeUp
{
    class WaitForNewVMToComeUp
    {
        static void Main(string[] args)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.CreateLogFolder();
            string LogFilePath = NewLogObj.CreateLogFile();
            NewLogObj.WriteLogFile(LogFilePath, "Test : WaitForNewVMToComeUp", "info");
            NewLogObj.WriteLogFile(LogFilePath, "======================", "info");

            WaitForNewVMToComeUp Obj = new WaitForNewVMToComeUp();
            Obj.CallPythonScriptForIPCheck();


        }

        public void CallPythonScriptForIPCheck()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "Test : CallPythonScriptForIPCheck", "info");
            NewLogObj.WriteLogFile(LogFilePath, "======================", "info");
            FileOperations FileObj = new FileOperations();
            
            string InputFilePath = FileObj.GetInputFilePath(LogFilePath, "Inputs.txt");

            string PoolMasterServerIP = FileObj.GetInputPattern(InputFilePath, "PoolMasterServerIP");
            string Password = FileObj.GetInputPattern(InputFilePath, "XenServerPassword1");
            string XenServerUserName1 = FileObj.GetInputPattern(InputFilePath, "XenServerUserName1");
            string NewTemplateName = FileObj.GetInputPattern(InputFilePath, "NewTemplateName");

            ProcessStartInfo startInfo;
            Process process;
            string directory= Directory.GetCurrentDirectory();
            string script;

            startInfo = new ProcessStartInfo("python");
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            script = "GetVMsIP.py";
            string pyArgs = "http://" + PoolMasterServerIP + " " + XenServerUserName1 + " " + Password + " " + NewTemplateName;
            NewLogObj.WriteLogFile(LogFilePath, "pyArgs " + pyArgs, "info");
            startInfo.Arguments = script + " " + pyArgs;
            NewLogObj.WriteLogFile(LogFilePath, "Arguments " + startInfo.Arguments, "info");
            startInfo.Arguments = script;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            string s;
            while ((s = process.StandardOutput.ReadLine()) != null)
            {
                //do something with s
                Console.WriteLine(s);
            }

        }
    }
}

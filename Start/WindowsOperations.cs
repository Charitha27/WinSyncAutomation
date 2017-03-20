using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;


using LoggerCollection;
using GenericCollection;
using FileOperationsCollection;
//using BingTranslator;
using Start;

namespace WindowsOperationsCollection
{
    public class WindowsOperations
    {
        public int WaitTillSystemIsUp(string SystemIP,int WaitTimeOutInMins)
        {   
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string PingCmd = "ping " + SystemIP;
            NewLogObj.WriteLogFile(LogFilePath, "ping cmd : " + PingCmd, "info");

            String CmdResult=GenericObj.StartCmdExecutionAndWaitForCompletion(PingCmd);
            Regex RegPattern = new Regex("\r\n");
            //Replace the newlines
            //CmdResult = RegPattern.Replace(CmdResult, " ");
            string [] TempList=RegPattern.Split(CmdResult);
            CmdResult = TempList[8];
            //foreach (string temp in TempList)
            //{
            //    CmdResult = CmdResult + temp + " ";
            //}
            string SystemLocale = GenericObj.GetSystemLocale();
            string TranslatedCmdResult = null;
            NewLogObj.WriteLogFile(LogFilePath, "Ping Result: "  + CmdResult, "info");
            
            if (!Regex.IsMatch(SystemLocale, "en-US"))
            {
                //BingTranslator MyBingObj = new BingTranslator();
                BingTranslator MyBingObj = new BingTranslator();
                //string BingLanguage = MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);
                //TranslatedCmdResult = MyBingObj.TranslateString(CmdResult, "english");
                string Locale = MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);
                //Antwort von 10.105.74.7: Bytes=32 Zeit<1ms TTL=63
                TranslatedCmdResult = MyBingObj.GenerateAccessTokenAndStartTranslation(CmdResult, Locale, "en");
            }
            else
            {
                TranslatedCmdResult = CmdResult;
            }
            TranslatedCmdResult = TranslatedCmdResult.ToLower();
            TranslatedCmdResult = Regex.Replace(TranslatedCmdResult, "\\s", "");
            //int PingStats = FileObj.CheckStringForPattern(TranslatedCmdResult, "Packets: Sent = 4, Received = 4, Lost = 0", null);
           // int PingStats = FileObj.CheckStringForPattern(TranslatedCmdResult, "packets:sent=4,received=4,lost=0", null);
            int PingStats = FileObj.CheckStringForPattern(TranslatedCmdResult, "packets:sent=4,received=4", null);
            //int PingStats = FileObj.CheckStringForPattern(CmdResult, TranslatedCmdResult, null);
            //int PingStats = FileObj.CheckUnicodeStringForPattern(CmdResult, TranslatedCmdResult, null);
            if (PingStats == 1)
            {
                NewLogObj.WriteLogFile(LogFilePath, "System " + SystemIP + " is up", "info");
                return 1;
            }
            int Timer = 0;
            int WaitTimeOutInMilliSecs = WaitTimeOutInMins * 60 * 1000;
            while (PingStats == -1 && Timer < WaitTimeOutInMilliSecs)
            {
                CmdResult = GenericObj.StartCmdExecutionAndWaitForCompletion(PingCmd);

                TempList = RegPattern.Split(CmdResult);
                CmdResult = TempList[8];
                if (!Regex.IsMatch(SystemLocale, "en-US"))
                {
                    BingTranslator MyBingObj = new BingTranslator();
                    //string BingLanguage = MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);
                    //TranslatedCmdResult = MyBingObj.TranslateString(CmdResult, "english");
                    string Locale = MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);
                    TranslatedCmdResult = MyBingObj.GenerateAccessTokenAndStartTranslation(CmdResult, Locale, "en");
                }
                else
                {
                    TranslatedCmdResult = CmdResult;
                }
                //PingStats = FileObj.CheckStringForPattern(CmdResult, "Packets: Sent = 4, Received = 4, Lost = 0", null);
                //PingStats = FileObj.CheckStringForPattern(CmdResult, TranslatedCmdResult, null);
                TranslatedCmdResult = TranslatedCmdResult.ToLower();
                TranslatedCmdResult=Regex.Replace(TranslatedCmdResult,"\\s","");
               //PingStats = FileObj.CheckStringForPattern(TranslatedCmdResult, "Packets: Sent = 4, Received = 4, Lost = 0", null);
                //PingStats = FileObj.CheckStringForPattern(TranslatedCmdResult, "packets:sent=4,received=4,lost=0", null);
                PingStats = FileObj.CheckStringForPattern(TranslatedCmdResult, "packets:sent=4,received=4", null);
                if (PingStats == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "System " + SystemIP+" is up", "info");
                    return 1;
                }
                Thread.Sleep(10000);
                Timer = Timer + 10000;
            }
            if (PingStats == -1 && Timer >= WaitTimeOutInMilliSecs)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for System " + SystemIP + " to be up", "info");
                return -1;
            }
            return -1;
        }

        public void AuthenticateToRemoteMachine(string RemoteMcIp,string UserName,string Password )
        {
           // `net use \\\\$RemoteMcIp /user:$UserName $Passwd`;
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            string LogFilePath = NewLogObj.GetLogFilePath();
           // string AuthenticateCmd = "net use \\\\" + RemoteMcIp+" /user:"+UserName +" "+Password ;
            string AuthenticateCmd = "net use " + RemoteMcIp + " /user:" + UserName + " " + Password;
            String CmdResult = GenericObj.StartCmdExecutionAndWaitForCompletion(AuthenticateCmd);

            string SystemLocale = GenericObj.GetSystemLocale();
            string TranslatedCmdResult = null;
            NewLogObj.WriteLogFile(LogFilePath, "AuthenticateToRemoteMachine Result: " + RemoteMcIp + " " + CmdResult, "info");
            if (!Regex.IsMatch(SystemLocale, "en-US"))
            {
                BingTranslator MyBingObj = new BingTranslator();
                string Locale=MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);
                //TranslatedCmdResult = MyBingObj.TranslateString(CmdResult, "English");
                TranslatedCmdResult = MyBingObj.GenerateAccessTokenAndStartTranslation(CmdResult, Locale, "en");
                //TranslatedCmdResult = MyBingObj.GenerateAccessTokenAndStartTranslation(CmdResult, "de", "en");
            }
            else
            {
                TranslatedCmdResult = CmdResult;
            }

            NewLogObj.WriteLogFile(LogFilePath, "AuthenticateToRemoteMachine Result after translation: " + RemoteMcIp + " " + TranslatedCmdResult, "info");
            //CmdResult = "The command completed successfully.\r\n\r\n"
           // if (Regex.IsMatch(TranslatedCmdResult, "command completed successfully",RegexOptions.IgnoreCase))
            if (Regex.IsMatch(TranslatedCmdResult, "successfully", RegexOptions.IgnoreCase))
            {
                NewLogObj.WriteLogFile(LogFilePath, "AuthenticateToRemoteMachine : " + RemoteMcIp + " success ", "info");
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "AuthenticateToRemoteMachine : " + RemoteMcIp + " failed ", "warn");
            }
            
        }
    }
}

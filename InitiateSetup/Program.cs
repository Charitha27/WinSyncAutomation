using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using TestDataGeneratorCollection;
namespace InitiateSetup
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                string CurrentDirPath = Directory.GetCurrentDirectory();


                //int Index = CurrentDirPath.IndexOf("InitiateSetup");
                // CurrentDirPath = CurrentDirPath.Substring(0, Index);
                //string LogFolderPath = CurrentDirPath + "\\RunTest\\bin\\Debug\\Logs";
                string LogFolderPath = CurrentDirPath + "\\Logs";
                string ReRunLogFolderPath = CurrentDirPath + "\\ReRunLogs";
                //Console.WriteLine(LogFolderPath);
                bool LogfolderExists = Directory.Exists(LogFolderPath);
                bool ReRunLogFolderPathExists = Directory.Exists(ReRunLogFolderPath);
                if (!LogfolderExists)
                {
                    Directory.CreateDirectory(LogFolderPath);
                }
                else
                {
                    //Empty the dir & create
                    System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(LogFolderPath);
                    foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                    foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
                    Directory.CreateDirectory(LogFolderPath);

                }
                if (!ReRunLogFolderPathExists)
                {
                    Directory.CreateDirectory(ReRunLogFolderPath);
                }
                else
                {
                    //Empty the dir & create
                    System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(ReRunLogFolderPath);
                    foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
                    foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
                    Directory.CreateDirectory(ReRunLogFolderPath);

                }
                string TestReportFile = LogFolderPath + "\\TestSummary.html";
                FileStream LF = File.Create(TestReportFile);
                LF.Close();
                StreamWriter SW = File.AppendText(TestReportFile);
                SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><td align=\"center\" colspan=\"1\" ><font size=\"5\"><b> TestSummary </b></font></td></tr>");
                SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><th align=\"center\">TestName</th><th align=\"center\">Status</th><th align=\"center\">LogFile</th><th align=\"center\">FailureSummary</th><th align=\"center\">ScreenShot</th><th align=\"center\">GuiZardLogs</th><tr>");
                SW.Close();

                string ReRunTestReportFile = ReRunLogFolderPath + "\\TestSummary.html";
                LF = File.Create(ReRunTestReportFile);
                LF.Close();
                SW = File.AppendText(ReRunTestReportFile);
                SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><td align=\"center\" colspan=\"1\" ><font size=\"5\"><b> ReRunTestSummary </b></font></td></tr>");
                //SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><th align=\"center\">TestName</th><th align=\"center\">Status</th><th align=\"center\">LogFile</th><th align=\"center\">GuiZardLogs</th><tr>");
                SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><th align=\"center\">TestName</th><th align=\"center\">Status</th><th align=\"center\">LogFile</th><th align=\"center\">FailureSummary</th><th align=\"center\">ScreenShot</th><th align=\"center\">GuiZardLogs</th><tr>");
                SW.Close();

                string GuizardFailureLogs = "C:\\GuizardResults";
                if (Directory.Exists(GuizardFailureLogs))
                {
                    System.IO.DirectoryInfo GuizardFailDir = new System.IO.DirectoryInfo(GuizardFailureLogs);
                    foreach (System.IO.FileInfo file in GuizardFailDir.GetFiles()) file.Delete();
                    foreach (System.IO.DirectoryInfo subDirectory in GuizardFailDir.GetDirectories()) subDirectory.Delete(true);
                }
                else
                {
                    Directory.CreateDirectory(GuizardFailureLogs);
                }

                //Emptying C:\TestResults folder
                Console.WriteLine("\n Emptying GuizardDefaultStorage Location. Will take some time depending on the folder size..\n");
                Console.WriteLine("\nPlease wait..\n");
                string GuizardDefaultRecordLocation = "C:\\Testresults";
                if (Directory.Exists(GuizardDefaultRecordLocation))
                {
                    System.IO.DirectoryInfo GuizardDefaultRecordDir = new System.IO.DirectoryInfo(GuizardDefaultRecordLocation);
                    foreach (System.IO.FileInfo file in GuizardDefaultRecordDir.GetFiles()) file.Delete();
                    foreach (System.IO.DirectoryInfo subDirectory in GuizardDefaultRecordDir.GetDirectories()) subDirectory.Delete(true);
                }

                TestDataGenerator TestDataObj = new TestDataGenerator();
                TestDataObj.GenerateRUFile();
                //Directory.CreateDirectory(LogFolderPath);
            }
            catch(Exception Ex)
            {

            }
        }
        public void RenameFile(string SourceFilePath, string RenamedFilePath)
        {
            try
            {
                string fileName = System.IO.Path.GetFileName(SourceFilePath);

                System.IO.File.Move(SourceFilePath, RenamedFilePath);
            }
            catch
            {

            }

        }
    }
}

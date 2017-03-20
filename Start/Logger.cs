using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace LoggerCollection
{
    public class Logger
    {
        public string LogFolderPath;
        public string LogFileFullPath;

        public void CreateLogFolder()
        {
            string CurrentDirPath = Directory.GetCurrentDirectory();
            //Console.WriteLine(CurrentDirPath);
            //int CurrDirLength = CurrentDirPath.Length;
            //LogFolderPath = CurrentDirPath.Substring(0, CurrDirLength - 10);
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            //Remove the .txt part
            CurrentFileName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
            string ReRunPattern = "^ReRun";
            if (Regex.IsMatch(CurrentFileName, ReRunPattern))
            {
                LogFolderPath = CurrentDirPath + "\\ReRunLogs\\" + CurrentFileName;
            }
            else
            {
                LogFolderPath = CurrentDirPath + "\\Logs\\" + CurrentFileName;
            }
            //Console.WriteLine(LogFolderPath);
            bool LogfolderExists = Directory.Exists(LogFolderPath);
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
            
        }

        public string CreateLogFile()
        {
            string CurrentDirPath = Directory.GetCurrentDirectory();
            
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            //Remove the .txt part
            CurrentFileName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
            string FailureLogFileFullPath = null;
            string ReRunPattern = "^ReRun";
            if (Regex.IsMatch(CurrentFileName, ReRunPattern))
            {
                LogFileFullPath = CurrentDirPath + "\\ReRunLogs\\" + CurrentFileName + "\\" + CurrentFileName + ".html";
                //LogFileFullPath = CurrentDirPath + "\\Logs\\log.log";
                FileStream LF = File.Create(LogFileFullPath);
                // string FailureLogFileFullPath = CurrentDirPath + "\\Logs\\"+CurrentFileName+"\\"+CurrentFileName+"_Failures.log";
                FailureLogFileFullPath = CurrentDirPath + "\\ReRunLogs\\" + CurrentFileName + "\\" + CurrentFileName + "_Failures.html";
                FileStream LF1 = File.Create(FailureLogFileFullPath);
                LF.Close();
                LF1.Close();
            }
            else
            {
                LogFileFullPath = CurrentDirPath + "\\Logs\\" + CurrentFileName + "\\" + CurrentFileName + ".html";
                //LogFileFullPath = CurrentDirPath + "\\Logs\\log.log";
                FileStream LF = File.Create(LogFileFullPath);
                // string FailureLogFileFullPath = CurrentDirPath + "\\Logs\\"+CurrentFileName+"\\"+CurrentFileName+"_Failures.log";
                FailureLogFileFullPath = CurrentDirPath + "\\Logs\\" + CurrentFileName + "\\" + CurrentFileName + "_Failures.html";
                FileStream LF1 = File.Create(FailureLogFileFullPath);
                LF.Close();
                LF1.Close();
            }

            //Add the test name info in the file
            StreamWriter SW = File.AppendText(LogFileFullPath);
            SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><td align=\"center\" colspan=\"1\" ><font size=\"5\"><b>"+CurrentFileName+"</b></font></td></tr>");
            SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><th align=\"center\">   TimeStamp   </th><th align=\"center\">      Execution Logs     </th><th align=\"center\">Status</th><tr>");
            SW.Close();

            SW = File.AppendText(FailureLogFileFullPath);
            SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><td align=\"center\" colspan=\"1\" ><font size=\"5\"><b>" + CurrentFileName + "</b></font></td></tr>");
            SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><th align=\"center\">   TimeStamp   </th><th align=\"center\">      Execution Logs     </th><th align=\"center\">Status</th><tr>");
            SW.Close();
            return LogFileFullPath;

        }

        public void CreateTestSummaryFile(string TestReportFile)
        {
            //string TestReportFile = Directory.GetCurrentDirectory() + "\\Logs\\TestSummary.html";
            FileStream LF = File.Create(TestReportFile);
            LF.Close();
            StreamWriter SW = File.AppendText(TestReportFile);
            SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><td align=\"center\" colspan=\"1\" ><font size=\"5\"><b> TestSummary </b></font></td></tr>");
            SW.WriteLine("<table width=\"90%\" cellpadding=\"1\" cellspacing=\"5\" align=\"center\" border=1><tr><th align=\"center\">TestName</th><th align=\"center\">Status</th><th align=\"center\">LogFile</th><th align=\"center\">GuiZardLogs</th><tr>");
            SW.Close();
        }

        public void WriteToTestSummaryFile(string TestName,string Status,string GuizardResultsPath,string FailSummary)
        {
            string ReRunPattern = "^ReRun";
            string TestReportFile = null;
            string TestLogFilePath = null;
            string TestFailureLogFilePath = null;
            string FailureScreenShotPath = null;
            if (Regex.IsMatch(TestName, ReRunPattern))
            {
                TestReportFile = Directory.GetCurrentDirectory() + "\\ReRunLogs\\TestSummary.html";
                TestLogFilePath = Directory.GetCurrentDirectory() + "\\ReRunLogs\\" + TestName + "\\" + TestName + ".html";
                TestFailureLogFilePath = Directory.GetCurrentDirectory() + "\\ReRunLogs\\" + TestName + "\\" + TestName + "_Failures.html";
                FailureScreenShotPath = Directory.GetCurrentDirectory() + "\\ReRunLogs\\" + TestName + "\\" + TestName + ".jpeg";
            }
            else
            {
                TestReportFile = Directory.GetCurrentDirectory() + "\\Logs\\TestSummary.html";
                TestLogFilePath = Directory.GetCurrentDirectory() + "\\Logs\\" + TestName + "\\" + TestName + ".html";
                TestFailureLogFilePath = Directory.GetCurrentDirectory() + "\\Logs\\" + TestName + "\\" + TestName + "_Failures.html";
                FailureScreenShotPath = Directory.GetCurrentDirectory() + "\\Logs\\" + TestName + "\\" + TestName + ".jpeg";
            }
            
            
            if (!File.Exists(TestReportFile))
            {
                CreateTestSummaryFile(TestReportFile);
            }
            StreamWriter SW = File.AppendText(TestReportFile);

            if (Regex.IsMatch(Status, "pass", RegexOptions.IgnoreCase))
            {
                //SW.WriteLine("<tr><td align=\"center\"><font color=\"green\">" + TestName + " </td><td align=\"center\"><font color=\"green\">" + Status + " </td><td align=\"center\"><font color=\"green\"><a href=file:" + TestLogFilePath + ">Log File</a></td><td align=\"center\"><font color=\"green\"><a href=" + GuizardResultsPath + ">GuizardResultsPath</a></td><tr>");
                SW.WriteLine("<tr><td align=\"center\"><font color=\"green\">" + TestName + " </td><td align=\"center\"><font color=\"green\">" + Status + " </td><td align=\"center\"><font color=\"green\"><a href=file:" + TestLogFilePath + ">Log File</a></td><td align=\"center\"><font color=\"green\">" + "NA" + " </td><td align=\"center\"><font color=\"green\">" + "NA" + " </td><td align=\"center\"><font color=\"green\"><a href=" + GuizardResultsPath + ">GuizardResultsPath</a></td><tr>");
            }
            else if (Regex.IsMatch(Status, "fail", RegexOptions.IgnoreCase))
            {
                //SW.WriteLine("<tr><td align=\"center\"><font color=\"red\">" + TestName + " </td><td align=\"center\"><font color=\"red\">" + Status + " </td><td align=\"center\"><font color=\"red\"><a href=file:" + TestFailureLogFilePath + ">Log File</a></td><td align=\"center\"><font color=\"red\"><a href=" + GuizardResultsPath + ">GuizardResultsPath</a></td><tr>");
                //string FailureScreenShotPath=Directory.GetCurrentDirectory() + "\\Logs\\" + TestName + "\\FailureScreenShot.jpeg";
                 
                if (File.Exists(FailureScreenShotPath))
                {
                    //SW.WriteLine("<tr><td align=\"center\"><font color=\"red\">" + TestName + " </td><td align=\"center\"><font color=\"red\">" + Status + " </td><td align=\"center\"><font color=\"red\"><a href=file:" + TestFailureLogFilePath + ">Log File</a></td><td align=\"center\"><font color=\"red\"><a href=" + GuizardResultsPath + ">GuizardResultsPath</a></td><tr>");
                    SW.WriteLine("<tr><td align=\"center\"><font color=\"red\">" + TestName + " </td><td align=\"center\"><font color=\"red\">" + Status + " </td><td align=\"center\"><font color=\"red\"><a href=file:" + TestFailureLogFilePath + ">Log File</a></td><td align=\"center\"><font color=\"red\">" + FailSummary + " </td><td align=\"center\"><font color=\"red\"><a href=" + FailureScreenShotPath + ">FailureScreenShotPath</a></td><td align=\"center\"><font color=\"red\"><a href=" + GuizardResultsPath + ">GuizardResultsPath</a></td><tr>");
                }
                else
                {
                    SW.WriteLine("<tr><td align=\"center\"><font color=\"red\">" + TestName + " </td><td align=\"center\"><font color=\"red\">" + Status + " </td><td align=\"center\"><font color=\"red\"><a href=file:" + TestFailureLogFilePath + ">Log File</a></td><td align=\"center\"><font color=\"red\">" + FailSummary + " </td><td align=\"center\"><font color=\"red\">" + "NoFailScreenShots" + " </td><td align=\"center\"><font color=\"red\"><a href=" + GuizardResultsPath + ">GuizardResultsPath</a></td><tr>");
                }
            }
            SW.Close();
        }
        public string GetLogFilePath()
        {
            string CurrentDirPath = Directory.GetCurrentDirectory();
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            CurrentFileName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
            string ReRunPattern = "^ReRun";
            if (Regex.IsMatch(CurrentFileName, ReRunPattern))
            {
                LogFileFullPath = CurrentDirPath + "\\ReRunLogs\\" + CurrentFileName + "\\" + CurrentFileName + ".html";
            }
            else
            {
                //LogFileFullPath = CurrentDirPath + "\\Logs\\log.log";
                LogFileFullPath = CurrentDirPath + "\\Logs\\" + CurrentFileName + "\\" + CurrentFileName + ".html";
            }
            return LogFileFullPath;
        }


        public void WriteLogFile(string LogFilePath1, string TextToWrite, string Status)
        {
            if (File.Exists(LogFilePath1))
            {
                //StreamWriter SW = new StreamWriter(LogFileFullPath);
                StreamWriter SW = File.AppendText(LogFilePath1);
                 DateTime CurrentTime = DateTime.Now;
                if (string.Equals(Status, "Pass", StringComparison.CurrentCultureIgnoreCase))
                {
                    //SW.WriteLine(CurrentTime + ": " + TextToWrite+" : PASS <br>");
                    SW.WriteLine("<tr><td><font color=\"green\">" + CurrentTime + " </td><td><font color=\"green\">" + TextToWrite + " </td><td align=\"center\"><font color=\"green\">PASS</td><tr>");
                }
                else if(string.Equals(Status, "info", StringComparison.CurrentCultureIgnoreCase))
                {
                    //SW.WriteLine(CurrentTime + ": " + TextToWrite+" : INFO <br>");
                    SW.WriteLine("<tr><td>" + CurrentTime + " </td><td>" + TextToWrite + " </td><td align=\"center\">INFO</td><tr>");
                }
                else if(string.Equals(Status, "fail", StringComparison.CurrentCultureIgnoreCase))
                {
                    //SW.WriteLine(CurrentTime + ": " + TextToWrite+" : FAIL <br>");
                    SW.WriteLine("<tr><td> <font color=\"red\">" + CurrentTime + ": </td><td><font color=\"red\">" + TextToWrite + " </td><td align=\"center\"><font color=\"red\">FAIL</td><tr>");
                    string CurrentDirPath = Directory.GetCurrentDirectory();
                    string CurrentFileName = Environment.GetCommandLineArgs()[1];
                    CurrentFileName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
                    string FailureLogFileFullPath = null;
                    string ReRunPattern = "^ReRun";
                    if (Regex.IsMatch(CurrentFileName, ReRunPattern))
                    {
                        FailureLogFileFullPath = CurrentDirPath + "\\ReRunLogs\\" + CurrentFileName + "\\" + CurrentFileName + "_Failures.html";
                    }
                    else
                    {
                        FailureLogFileFullPath = CurrentDirPath + "\\Logs\\" + CurrentFileName + "\\" + CurrentFileName + "_Failures.html";
                    }
                    StreamWriter SW1 = File.AppendText(FailureLogFileFullPath);
                   // SW1.WriteLine(CurrentTime + ": " + TextToWrite + " : FAIL <br>");
                    SW1.WriteLine("<tr><td><font color=\"red\">" + CurrentTime + " </td><td><font color=\"red\">" + TextToWrite + " </td><td align=\"center\"><font color=\"red\">FAIL</td><tr>");
                    SW1.Flush();
                    SW1.Close();

                }
                else if (string.Equals(Status, "warn", StringComparison.CurrentCultureIgnoreCase))
                {
                    //SW.WriteLine(CurrentTime + ": " + TextToWrite+" : FAIL <br>");
                    SW.WriteLine("<tr><td> <font color=\"blue\">" + CurrentTime + ": </td><td><font color=\"blue\">" + TextToWrite + " </td><td align=\"center\"><font color=\"blue\">WARN</td><tr>");
                    string CurrentDirPath = Directory.GetCurrentDirectory();
                    string CurrentFileName = Environment.GetCommandLineArgs()[1];
                    CurrentFileName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
                    string ReRunPattern = "^ReRun";
                    string FailureLogFileFullPath = null;
                    if (Regex.IsMatch(CurrentFileName, ReRunPattern))
                    {
                        FailureLogFileFullPath = CurrentDirPath + "\\ReRunLogs\\" + CurrentFileName + "\\" + CurrentFileName + "_Failures.html";
                    }
                    else
                    {
                        FailureLogFileFullPath = CurrentDirPath + "\\Logs\\" + CurrentFileName + "\\" + CurrentFileName + "_Failures.html";
                    }
                    StreamWriter SW1 = File.AppendText(FailureLogFileFullPath);
                    // SW1.WriteLine(CurrentTime + ": " + TextToWrite + " : FAIL <br>");
                    SW1.WriteLine("<tr><td><font color=\"blue\">" + CurrentTime + " </td><td><font color=\"blue\">" + TextToWrite + " </td><td align=\"center\"><font color=\"blue\">WARN</td><tr>");
                    SW1.Flush();
                    SW1.Close();

                }
                SW.Flush();
                SW.Close();
            }
            else
            {
               // Console.WriteLine(LogFilePath1 + "does not exist");
            }
            
        }

        public void CaptureScreenShot()
        {
            
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            //Remove the .txt part
            CurrentFileName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            string RerunFilePattern = "^ReRun";
            string LogFolderPath = null;
            if (Regex.IsMatch(CurrentFileName, RerunFilePattern))
            {
                LogFolderPath = Directory.GetCurrentDirectory() + "\\ReRunLogs\\" + CurrentFileName;
            }
            else
            {
                LogFolderPath = Directory.GetCurrentDirectory() + "\\Logs\\" + CurrentFileName;
            }
            
            try
            {
                Bitmap BitmapImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics GraphicsObj = Graphics.FromImage(BitmapImage as Image);
                GraphicsObj.CopyFromScreen(0, 0, 0, 0, BitmapImage.Size);
                //string FailScreenShotPath = LogFolderPath + "\\FailureScreenShot.jpeg";
                string FailScreenShotPath = LogFolderPath + "\\"+CurrentFileName+".jpeg";
                BitmapImage.Save(FailScreenShotPath, ImageFormat.Jpeg);
            }
            catch(Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, CurrentFileName+ "Screen shot capturing failed. " + Ex.ToString(), "warn");
            }
        }

        static int fileCount = 1;
        public void CaptureScreenShotsAtEveryStage()
        {
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            //Remove the .txt part
            CurrentFileName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            string RerunFilePattern = "^ReRun";
            
            string LogFolderPath = Regex.IsMatch(CurrentFileName, RerunFilePattern) ? Directory.GetCurrentDirectory() + "\\ReRunLogs\\" :
                Directory.GetCurrentDirectory() + "\\Logs\\";

            LogFolderPath = LogFolderPath + "\\Screenshots";
            Directory.CreateDirectory(LogFolderPath);

            try
            {
                var image = CaptureActiveWindow();
                image.Save(LogFolderPath + "\\" + fileCount++.ToString() + "_" + CurrentFileName + ".jpeg", ImageFormat.Jpeg);
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, CurrentFileName + "Screen shot capturing failed. " + Ex.ToString(), "warn");
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static Image CaptureDesktop()
        {
            return CaptureWindow(GetDesktopWindow());
        }

        public static Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(GetForegroundWindow());
        }

        public static Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new Rect();
            GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);
            }

            return result;
        }
        public void capturescreenshot()
        {

        }
    }
}

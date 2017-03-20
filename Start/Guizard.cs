using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Automation;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using FileOperationsCollection;
using GUICollection;
using GenericCollection;

using LoggerCollection;

namespace GuizardCollection
{
    public class Guizard
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        public string ExePath="C:\\Users\\Administrator\\Desktop\\GUIzard\\GUIZard_3.2.03\\GUIzard_3 2012-01-04 -RTM version 3.2.03-\\GUIzard_3.exe";

        //public void StartTestWithGuiZard(string GuiZardLocation)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "StartTestWithGuiZard", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "==============", "info");
        //    //start guizard
        //    AutomationElement GuiZardObj= StartGuiZard(GuiZardLocation);
        //    if (GuiZardObj == null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "GuiZardObj is null", "fail");
        //        NewLogObj.WriteLogFile(LogFilePath, "Unable to start GUizard test. Exiting application", "fail");
        //        FileOperations FileObj = new FileOperations();
        //        FileObj.ExitTestEnvironment();
        //    }
        //    AutomationElement TestNameLabel = ReturnLocationOfTextNameTextBox();
        //    if (TestNameLabel == null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "TestNameLabel on GuiZardObj is null", "fail");
        //        NewLogObj.WriteLogFile(LogFilePath, "Unable to start GUizard test since TestNameLabel not found. Exiting application", "fail");
        //        FileOperations FileObj = new FileOperations();
        //        FileObj.ExitTestEnvironment();
        //    }
        //    //Start test and new test btns are located relative to testname text box
        //    ClickNewTestButton(TestNameLabel);
        //    ClickStartTestButton(TestNameLabel);

        //    //Get results folder path
        //    //string ResultsFolder = GetResultsFolderPath();
        //    //if (ResultsFolder == null)
        //    //{
        //    //    NewLogObj.WriteLogFile(LogFilePath, "ResultsFolder on GuiZardObj is null", "fail");
                
        //    //}
        //    //return ResultsFolder;

        //}
        //public AutomationElement StartGuiZard(string ExePath)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "StartGuiZard", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "==============", "info");
        //    if (!File.Exists(ExePath))
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "*************** GuiZard installed location does not exist. Exiting ..********", "fail");
        //        FileOperations FileObj = new FileOperations();
        //        FileObj.ExitTestEnvironment();
        //    }
        //    System.Diagnostics.Process P = System.Diagnostics.Process.Start(ExePath);
        //    Thread.Sleep(3000);
        //    AutomationElement GuiZardObj =AutomationElement.FromHandle(P.MainWindowHandle);
        //    if (GuiZardObj != null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "GuiZardObj found", "info");
        //        return GuiZardObj;
        //    }
        //    else
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "***************GuiZardObj not found. Exiting ..********", "fail");
        //        FileOperations FileObj = new FileOperations();
        //        FileObj.ExitTestEnvironment();
        //        return null;
        //    }
        //}

        public AutomationElement GetGuiZardObject()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "StartGuiZard", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==============", "info");

            PropertyCondition typeCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, "MainForm");
            AutomationElement GuiZardObj = AutomationElement.RootElement.FindFirst(TreeScope.Children, typeCondition);
            if (GuiZardObj != null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "GuiZardObj found", "info");
                return GuiZardObj;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "***************GuiZardObj not found********", "fail");
                return null;
            }

        }

        //private AutomationElement GetTableElement(AutomationElement GuiZardObj)
        //{
        //    PropertyCondition cond = new PropertyCondition(AutomationElement.AutomationIdProperty, "listViewSummary");
        //    AutomationElement targetTableElement = GuiZardObj.FindFirst(TreeScope.Descendants, cond);

        //    PropertyCondition cond3 = new PropertyCondition(AutomationElement.AutomationIdProperty,"Group 1");
        //    AutomationElement targetTableElement2 = targetTableElement.FindFirst(TreeScope.Descendants, cond3);
        //    return targetTableElement;
        //}

        //public string WaitTillTimeout(string WindowNameAsInTools, string WindowDescription)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "WaitTillTimeout", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "==================", "info");

        //    AutomationElement GuiZardObj = GetGuiZardObject();
        //    int TimeOut = 300000; //5 mins
        //    int Timer = 0;
        //    string Result = VerifyGuiZardHasFinishedCapturing(WindowNameAsInTools,WindowDescription);
        //    if (Result != null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Required Window " + WindowDescription + " captured " , "info");
        //        return Result;
        //    }
        //    while (Result == null && (Timer < TimeOut))
        //    {
        //        Result = VerifyGuiZardHasFinishedCapturing(WindowNameAsInTools, WindowDescription);
        //        if (Result != null)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Required Window " + WindowDescription + " captured " , "info");
        //            return Result;
        //        }
        //        Timer = Timer + 3000;

        //    }
        //    if (Timer >= TimeOut)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "****Required Window " + WindowDescription + " not captured even after timeout.*** ", "fail");
        //        return null;

        //    }
        //    return null;
        //}

        //public string VerifyGuiZardHasFinishedCapturing(string WindowNameAsInTools,string WindowDescription)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "VerifyGuiZardHasFinishedCapturing", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "================================", "info");
            
        //    AutomationElement GuiZardObj = GetGuiZardObject();
        //    AutomationElement TableObj= GetTableElement(GuiZardObj);
        //    if (TableObj == null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Unable to get the GUizard Tableobj", "fail");
        //        NewLogObj.WriteLogFile(LogFilePath, "****Required Window " + WindowDescription + " not captured as table obj is null.*** ", "fail");
        //        return null;
        //    }
        //    PropertyCondition DataItemCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem);
        //    //int WindowFoundFlag = 0;
        //    //int TimeOut = 300000; //5 mins
        //    //int Timer = 0;
        //    //Thread.Sleep(5000);
        //    AutomationElementCollection NodeColl;
            
        //    //Get all the dataitems
        //    NodeColl = TableObj.FindAll(TreeScope.Descendants, DataItemCondition);
        //    int length = NodeColl.Count;
        //    foreach (AutomationElement AutoObj in NodeColl)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Current Window in list " + AutoObj.Current.Name, "info");
        //        string CurrentWindowInList = AutoObj.Current.Name;
        //        //if (string.Compare(CurrentWindowInList, WindowNameAsInTools) == 0)
        //        if (Regex.IsMatch(CurrentWindowInList, WindowNameAsInTools, RegexOptions.CultureInvariant))
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Required Window "+ WindowDescription+" captured " + AutoObj.Current.Name, "info");
        //           return WindowDescription;
        //        }
        //    }
        //    return null;
            
        //}

        public string GetResultsFolderPath()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GetResultsFolderPath", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            
            AutomationElement GuiZardObj = GetGuiZardObject();
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition SplitContObjCond = new PropertyCondition(AutomationElement.AutomationIdProperty, "SplitContainer1");
            //AutomationElement SplitContObj = GuiZardObj.FindFirst(TreeScope.Descendants, SplitContObjCond);
            AutomationElement SplitContObj = GuiObj.FindAutomationElement(GuiZardObj, SplitContObjCond, TreeScope.Descendants, "SplitContObjCond", 1, LogFilePath);
            PropertyCondition GrpBoxCond = new PropertyCondition(AutomationElement.AutomationIdProperty, "GroupBoxSummary");
            //AutomationElement GroupBoxSummaryObj = SplitContObj.FindFirst(TreeScope.Descendants, GrpBoxCond);
            AutomationElement GroupBoxSummaryObj = GuiObj.FindAutomationElement(SplitContObj, GrpBoxCond, TreeScope.Descendants, "GrpBoxCond", 1, LogFilePath);

            
            PropertyCondition ResultsLabelCond = new PropertyCondition(AutomationElement.AutomationIdProperty, "LabelResults");
            AutomationElement ResultsLabel = GroupBoxSummaryObj.FindFirst(TreeScope.Descendants, ResultsLabelCond);
            if (ResultsLabel == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Unable to get the results label", "fail");
                return null;
            }
            string ResultsFolder = ResultsLabel.Current.Name;
            NewLogObj.WriteLogFile(LogFilePath, "Results label text : " + ResultsFolder, "info");
            return ResultsFolder;
            
        }

        public string VerifyGuiZardResults(string TestName)
        {
            Logger NewLogObj = new Logger();
            FileOperations NewFileObj = new FileOperations();
            string LogFilePath = NewLogObj.GetLogFilePath();
            //NewLogObj.WriteLogFile(LogFilePath, "VerifyGuiZardResults", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            FileOperations FileObj=new FileOperations();
           // CreateGuizardSkipTestFile();
            Thread.Sleep(5);
            //string GuizardResultsPath = GetResultsFolderPath();
            string GuizardResultsPath=FileObj.GetLatestFileInDirectory("C:\\TestResults");
            NewLogObj.WriteLogFile(LogFilePath, "GuizardResultsPath " + GuizardResultsPath, "info");
            if (GuizardResultsPath == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "GuizardResultsPath is null " + GuizardResultsPath, "fail");
                return "-1";
              //  return 0;
            }
            string GuizardSummaryResults = GuizardResultsPath + "//SummaryData.xml";
            string ResultsFolderPath=null;
            if(File.Exists(GuizardSummaryResults))
            {
                string[] lines = System.IO.File.ReadAllLines(GuizardSummaryResults);
                NewLogObj.WriteLogFile(LogFilePath, "Start verifying Guizard results", "info");
                int FailFlag=0;
                int FirstCopyFlag = 0;
                foreach (string line in lines)
                {
                    if (Regex.IsMatch(line, "FAIL"))
                    {
                        //Extracting the window caption
                        int IndexOfQuotes1=line.IndexOf("\"");
                        string temp = line.Substring(IndexOfQuotes1 + 1);
                        int IndexOfQuotes2 = temp.IndexOf("\"");
                        string WindowCaption = line.Substring(3, IndexOfQuotes2 + IndexOfQuotes1);
                        //NewLogObj.WriteLogFile(LogFilePath, "Failure found in GUIZARD at "+line, "fail");

                        //Have to copy the fail screen shots to some loctaion
                        string FolderName=ExtractFailFolderName(line);
                        if (FolderName != null)
                        {
                          
                            //Copying the files to a common folder
                            string FailFolderPath = GuizardResultsPath + "//" + FolderName;
                            NewLogObj.WriteLogFile(LogFilePath, "Failure found in GUIZARD at "+WindowCaption+". Screenshots path " + FailFolderPath, "fail");
                            string DestinationFolderPath = "C:\\GuizardResults\\" + TestName;
                            ResultsFolderPath = DestinationFolderPath;
                            
                            if (!(Directory.Exists(DestinationFolderPath)))
                            {
                                Directory.CreateDirectory(DestinationFolderPath);
                            }
                            //Copying the xml file
                            if (FirstCopyFlag == 0)
                            {
                                System.IO.File.Copy(GuizardSummaryResults, DestinationFolderPath+"\\SummaryData.xml", true);
                            }
                            FirstCopyFlag = 1;
                            DestinationFolderPath = DestinationFolderPath+ "\\" + FolderName;
                            if (!Directory.Exists(DestinationFolderPath))
                            {
                                Directory.CreateDirectory(DestinationFolderPath);
                            }
                            //Extract the failed screen shot file name
                            int IndexOfScreenshot = line.IndexOf("screenshot");
                            //14 - removing screenshot="\
                            temp = line.Substring(IndexOfScreenshot + 14);
                            //screenshot="\Overlapping\Overlapping_0011_'NewNetwork' Properties.jpg" />
                            int IndexOfSlash = temp.IndexOf("\\");
                            int IndexOfQuotes = temp.IndexOf("\"");
                            string ScreenShotName = temp.Substring(IndexOfSlash + 1, IndexOfQuotes - IndexOfSlash-1);
                            string ScreenShotpath=FailFolderPath+"\\"+ScreenShotName;
                            if (File.Exists(ScreenShotpath))
                            {
                                //NewFileObj.CopyContentsOfAFolder(FailFolderPath, DestinationFolderPath);
                                NewFileObj.CopyFileToAFolder(ScreenShotpath, DestinationFolderPath);
                            }
                        }

                        FailFlag = 1;
                    
                    }
                }
                if (FailFlag == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "No fails found in GUIZARD ", "pass");
                    //ResultsFolderPath = GuizardResultsPath;
                    ResultsFolderPath = "C:\\TestResults\\" + TestName;
                    return ResultsFolderPath;
                }
                else
                {
                    //NewLogObj.WriteLogFile(LogFilePath, "Fails found in GUIZARD ", "fail");
                    return ResultsFolderPath;
                }
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, GuizardSummaryResults + "GuizardSummaryResults file not found ", "fail");
                return "-1";
            }


        }

        public void RenameGuizardFolderWithTestCaseName()
        {
            FileOperations FileObj = new FileOperations();
            string GuizardResultsPath = FileObj.GetLatestFileInDirectory("C:\\TestResults");
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            //Remove the .txt part
            string TestName = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
            //Renaimg the Guziard directory to that of test na,e
            string NewGuizardFolderName = "C:\\TestResults\\" + TestName;
            if (Directory.Exists(NewGuizardFolderName))
            {
                System.IO.DirectoryInfo GuizardFailDir = new System.IO.DirectoryInfo(NewGuizardFolderName);
                foreach (System.IO.FileInfo file in GuizardFailDir.GetFiles()) file.Delete();
                foreach (System.IO.DirectoryInfo subDirectory in GuizardFailDir.GetDirectories()) subDirectory.Delete(true);
                Directory.Delete(NewGuizardFolderName);
            }
            Directory.Move(GuizardResultsPath, NewGuizardFolderName);
        }
        //public string VerifyIfWindowScreenShotCaptured(string Window, string WindowDescription)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "GetResultsFolderPath", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
        //    string ResultsFolderPath = GetResultsFolderPath();
        //    if (ResultsFolderPath == null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the results folder path", "fail");
        //        return null;
        //    }
        //    ResultsFolderPath = ResultsFolderPath + "//WindowsTested";
        //    int Timeout = 300000; //5 min
        //    int Timer = 0;
        //    //Read the directory
        //    while (Timer <= Timeout)
        //    {
        //        string[] filePaths = Directory.GetFiles(ResultsFolderPath);
               
        //        foreach (string file in filePaths)
        //        {
        //            //Extracting file name alone
        //            int length = file.Length;
        //            int Index = file.LastIndexOf("\\");
        //            string filename = file.Substring(Index + 1);
        //            if (Regex.IsMatch(file, Window, RegexOptions.CultureInvariant))
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "Finished capturing window " + WindowDescription, "info");
        //                return Window;
        //            }
        //        }
        //        NewLogObj.WriteLogFile(LogFilePath, " window not yet captured " + WindowDescription, "info");
        //        Timer = Timer + 5000; //Wait for 3 secs
        //    }
        //    NewLogObj.WriteLogFile(LogFilePath, WindowDescription+"  not yet captured even aftre timeout " , "fail");
        //    return null;
        //}

        public string GetGuizardLocation()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            //NewLogObj.WriteLogFile(LogFilePath, "GetGuizardLocation", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");

            FileOperations FileObj = new FileOperations();
            string InputFilePath = FileObj.GetInputFilePath(LogFilePath, "Inputs.txt");

            string GuiZardLocationWithExeName = FileObj.GetInputPattern(InputFilePath, "GuiZardLocation");
            int Index = GuiZardLocationWithExeName.LastIndexOf("\\");
            string GuiZardLocation = GuiZardLocationWithExeName.Substring(0, Index);
            return GuiZardLocation;
        }

        public void StartNewTest(string GuiZardLocation)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ClickStartNewTestButton", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            
            FileOperations FileObj = new FileOperations();
            //string InputFilePath = FileObj.GetInputFilePath(LogFilePath, "Inputs.txt");
            //Delete Testcompleted file
            //string GuZardLocation = GetGuizardLocation();
            try
            {
                string TestCompleFile = GuiZardLocation + "\\GUIzardCompletedTest";
                int WaitTimeOut = 30000; //30 sec 
                int Timer = 1;
                while (File.Exists(TestCompleFile) && Timer < WaitTimeOut)
                {
                    File.Delete(TestCompleFile);
                    Timer = Timer + 1;
                }
                //string StopTestFile = GuiZardLocation + "\\GUIzardStop";
                string StopTestFile = GuiZardLocation + "\\GUIzardClose";
                Timer = 1;
                while (File.Exists(StopTestFile) && Timer < WaitTimeOut)
                {
                    File.Delete(StopTestFile);
                    Timer = Timer + 1;
                }
                string SkipTestFile = GuiZardLocation + "\\GUIzardSkipTest";
                Timer = 1;
                while (File.Exists(SkipTestFile) && Timer < WaitTimeOut)
                {
                    File.Delete(SkipTestFile);
                    Timer = Timer + 1;
                }


                //string GuiZardLocation = FileObj.GetInputPattern(InputFilePath, "GuiZardLocation");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //startInfo.FileName = "C:\\Users\\Administrator\\Desktop\\GUIzard\\GUIzard_3.5.04\\GUIzard_3.exe";
                startInfo.FileName = GuiZardLocation + "\\GUIzard_3.exe";

                //startInfo.Arguments = " +name=test1 +results=C:\\Testresults +d +o +b +m +w +t +a +start";// –name=test1 –results=C:\\Testresults –d –o –b –start";
                startInfo.Arguments = " +name=test1 +results=C:\\Testresults +d +o +b +m +t +a +start";// ???name=test1 ???results=C:\\Testresults ???d ???o ???b ???start";
                //startInfo.Arguments =  " -name=test1 -results=C:\\Testresults -d -o -b -m -w -t -a -start";// –name=test1 –results=C:\\Testresults 
                //string cmd=  "C:\\Users\\Administrator\\Desktop\\GUIzard\\GUIzard_3.5.04\\GUIzard_3.exe +name=test1 +results=C:\\Testresults +d +o +b +m +w +t +a +start";
                //System.Diagnostics.Process.Start(cmd);
                process.StartInfo = startInfo;
                process.Start();
            }
            catch(Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at StartNewTest "+Ex.ToString(), "info");
            }
       }

        public AutomationElement ReturnLocationOfTextNameTextBox()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ClickStartNewTestButton", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            AutomationElement GuiZardObj = GetGuiZardObject();
            
            PropertyCondition ToolStripCond = new PropertyCondition(AutomationElement.AutomationIdProperty, "ToolStrip1");
            AutomationElement ToolStripObj = GuiZardObj.FindFirst(TreeScope.Descendants, ToolStripCond);

           PropertyCondition TestNameLabelCond = new PropertyCondition(AutomationElement.NameProperty, "test1");
            //PropertyCondition TestNameLabelCond = new PropertyCondition(AutomationElement.AutomationIdProperty, "4786108");
            AutomationElement TestNameLabel = ToolStripObj.FindFirst(TreeScope.Descendants, TestNameLabelCond);
            if (TestNameLabel == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Unable to get the results label", "fail");
                return null;
            }
            return TestNameLabel;
            
        }

        //public string ClickNewTestButton(AutomationElement TestNameLabel)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "ClickStartNewTestButton", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
        //    //Toolstrip of guizard does not support UI automation
        //    //Hence cannot directly get the coordinates of new test btn
        //    // Getting the cordiantes of test name text box and deriving new test corodinates from that
            
        //    System.Windows.Point TestNameLocationBottomRight = TestNameLabel.Current.BoundingRectangle.BottomRight;
        //    double Height = TestNameLabel.Current.BoundingRectangle.Height;
        //    double TestNameXAxis = Convert.ToInt32(TestNameLocationBottomRight.X);


        //    //Trial & error
        //    //Deriving New Test btn position from test name label
        //    //double NewTestBtnXAxis = TestNameXAxis + 12;
        //    double NewTestBtnXAxis = TestNameXAxis + 22;
        //    double NewTestBtnYAxis = Convert.ToInt32(TestNameLocationBottomRight.Y) - (Height / 2);
        //    System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(NewTestBtnXAxis), Convert.ToInt32(NewTestBtnYAxis));
        //    NewLogObj.WriteLogFile(LogFilePath, "Moving mouse to" + TestNameLabel.Current.Name, "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "Bounding rect location  " + Convert.ToInt32(NewTestBtnXAxis) + Convert.ToInt32(NewTestBtnYAxis), "info");
        //    Microsoft.Test.Input.Mouse.MoveTo(p);
        //    Thread.Sleep(3000);
        //    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
        //    Thread.Sleep(2000);
        //    return "done";

        //}

        //public string ClickStartTestButton(AutomationElement TestNameLabel)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "ClickStartTestButton", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
        //    //Toolstrip of guizard does not support UI automation
        //    //Hence cannot directly get the coordinates of new test btn
        //    // Getting the cordiantes of test name text box and deriving new test corodinates from that
        //    AutomationElement GuiZardObj = GetGuiZardObject();
        //    SetFocusOnGuizard();
            
        //    System.Windows.Point TestNameLocationBottomRight = TestNameLabel.Current.BoundingRectangle.BottomRight;
        //    double Height = TestNameLabel.Current.BoundingRectangle.Height;
        //    double TestNameXAxis = Convert.ToInt32(TestNameLocationBottomRight.X);

        //    //Trial & error
        //    //Deriving New Test btn position from test name label
        //    //double NewTestBtnXAxis = TestNameXAxis + 400;
        //    double NewTestBtnXAxis = TestNameXAxis + 450;
        //    double NewTestBtnYAxis = Convert.ToInt32(TestNameLocationBottomRight.Y) - (Height / 2);
        //    System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(NewTestBtnXAxis), Convert.ToInt32(NewTestBtnYAxis));
        //    NewLogObj.WriteLogFile(LogFilePath, "Moving mouse to" + TestNameLabel.Current.Name, "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "Bounding rect location  " + Convert.ToInt32(NewTestBtnXAxis) + Convert.ToInt32(NewTestBtnYAxis), "info");
        //    Microsoft.Test.Input.Mouse.MoveTo(p);
        //    Thread.Sleep(3000);
        //    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
        //    Thread.Sleep(2000);
        //    return "done";

        //}

        //public string ClickStopTestButton(AutomationElement TestNameLabel)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "ClickStopTestButton", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
        //    //Toolstrip of guizard does not support UI automation
        //    //Hence cannot directly get the coordinates of new test btn
        //    // Getting the cordiantes of test name text box and deriving new test corodinates from that
        //    AutomationElement GuiZardObj = GetGuiZardObject();
        //    SetFocusOnGuizard();

        //    System.Windows.Point TestNameLocationBottomRight = TestNameLabel.Current.BoundingRectangle.BottomRight;
        //    double Height = TestNameLabel.Current.BoundingRectangle.Height;
        //    double TestNameXAxis = Convert.ToInt32(TestNameLocationBottomRight.X);

        //    //Trial & error
        //    //Deriving New Test btn position from test name label
        //    //double NewTestBtnXAxis = TestNameXAxis + 440;
        //    double NewTestBtnXAxis = TestNameXAxis + 490;
        //    double NewTestBtnYAxis = Convert.ToInt32(TestNameLocationBottomRight.Y) - (Height / 2);
        //    System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(NewTestBtnXAxis), Convert.ToInt32(NewTestBtnYAxis));
        //    NewLogObj.WriteLogFile(LogFilePath, "Moving mouse to" + TestNameLabel.Current.Name, "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "Bounding rect location  " + Convert.ToInt32(NewTestBtnXAxis) + Convert.ToInt32(NewTestBtnYAxis), "info");
        //    Microsoft.Test.Input.Mouse.MoveTo(p);
        //    Thread.Sleep(3000);
        //    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
        //    Thread.Sleep(2000);
        //    return "done";

        //}
        
        public void SetFocusOnGuizard()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SetFocusOnGuizard", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            
            AutomationElement GuiZardObj = GetGuiZardObject();
            IntPtr hwnd = (IntPtr)GuiZardObj.Current.NativeWindowHandle;
            SetForegroundWindow(hwnd);
            Thread.Sleep(2000);
            SetActiveWindow(hwnd);
            Thread.Sleep(2000);
        }

        public string VerifyTestCompletedFilePresent(string GuZardLocation,string WindowDescription)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "VerifyTestCompletedFilePresent", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=========================", "info");
            //Toolstrip of guizard does not support UI automation
            int timer = 0;
            //int TimeOut = 600000;
            int TimeOut = 30000; // 1 min
            //string GuZardLocation = GetGuizardLocation();
            string TestCompletedFile = GuZardLocation + "\\GUIzardCompletedTest";
            //NewLogObj.WriteLogFile(LogFilePath, " TestCompletedFile " + TestCompletedFile, "info");
            Console.WriteLine("Checking if  Guizard TestCompletedFile is present");
            while (timer < TimeOut)
            {
                if (File.Exists(TestCompletedFile))
                {
                    NewLogObj.WriteLogFile(LogFilePath, " Fisnihed capturing the window " + WindowDescription, "info");
                    Console.WriteLine(" Guizard TestCompletedFile is present");
                    NewLogObj.WriteLogFile(LogFilePath, " Deleting the  file" + TestCompletedFile, "info");
                    Console.WriteLine(" Deleting the  file Guizard TestCompletedFile");
                    //Try deleting the file
                    int FileDeleteTimout = 30000;
                    int FileDeleteTimer = 0;
                    try
                    {
                        File.Delete(TestCompletedFile);
                    }
                    catch (Exception Ex)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, " Exception while deleteing the file, Retry ", "info");
                        Thread.Sleep(1000);
                        while ((File.Exists(TestCompletedFile) && FileDeleteTimer < FileDeleteTimout))
                        {
                            NewLogObj.WriteLogFile(LogFilePath, " Retry Deleting the  file" + TestCompletedFile , "info");
                            Console.WriteLine("Retry Deleting the Guizard TestCompletedFile");
                            try
                            {
                                File.Delete(TestCompletedFile);
                            }
                            catch (Exception Ex1)
                            {
                                Thread.Sleep(1000);
                                FileDeleteTimer = FileDeleteTimer + 1000;
                            }
                            
                        }
                        if (File.Exists(TestCompletedFile) && FileDeleteTimer >= FileDeleteTimout)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, " Deleting the  file" + TestCompletedFile + " failed even after several retyr", "fail");
                            Console.WriteLine(" Deleting the  file" + TestCompletedFile + " failed even after several retyr");
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, " Deleting the  file" + TestCompletedFile + " success", "info");
                            Console.WriteLine("Deleting the  file" + TestCompletedFile + " success");
                        }
                    }
                    //catch (Exception Ex)
                    //{
                    //    NewLogObj.WriteLogFile(LogFilePath, " Exception while deleteing the file " + Ex.ToString(), "info");
                    //    Thread.Sleep(2000);
                    //}

                    return WindowDescription;
                }
                else
                {
                    timer = timer + 1000;
                    Thread.Sleep(1000);
                }
            }
            //NewLogObj.WriteLogFile(LogFilePath, "Guizard Unable to capture the window "+WindowDescription+" evn after timeout " , "fail");
           // Console.WriteLine("GUizard Unable to capture the window evn after timeout");
            return null;
        }

        public void CreateGuizardStopTestFile(string GuZardLocation)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            //NewLogObj.WriteLogFile(LogFilePath, "CreateGuizardStopTestFile", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "=========================", "info");
            Console.WriteLine("Creating GUizard StopTestFile");
            //string GuZardLocation = GetGuizardLocation();
            string GUIzardClose = GuZardLocation + "\\GUIzardClose";
            //NewLogObj.WriteLogFile(LogFilePath, " GUIzardClose " + GUIzardClose, "info");
            try
            {
                File.Create(GUIzardClose);
                //Will take sometime for GUizard to close
                Thread.Sleep(10000);
            }
            catch
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception creating GuizardClose file", "info");
            }
        }

        public void CreateGuizardSkipTestFile()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CreateGuizardStopTestFile", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=========================", "info");

            string GuZardLocation = GetGuizardLocation();
            string GUIzardSkipTest = GuZardLocation + "\\GUIzardSkipTest";
            NewLogObj.WriteLogFile(LogFilePath, " GUIzardSkipTest " + GUIzardSkipTest, "info");
            File.Create(GUIzardSkipTest);
        }

        //public void StopTestGuiZardThroughUI()
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "StopTestGuiZard", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=========================", "info");
        //    Guizard NewGuiZardObj = new Guizard();
        //    //While execution focus will ot shift to guizard
        //    int GuizardFocusShiftTimeout = 300000;
        //    int timer = 0;
        //    int GuiZardFocusShiftFlag=0;

        //    SetFocusOnGuizard();
        //    AutomationElementIdentity GuiObj = new AutomationElementIdentity();
        //    string Window = GuiObj.GetCurrentActiveWinodwText();
        //    NewLogObj.WriteLogFile(LogFilePath, "Active window title "+Window, "info");
        //    if(Regex.IsMatch(Window,"GUIzard"))
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "GuiZard is set active", "info");
        //        AutomationElement TestnameLabelobj = NewGuiZardObj.ReturnLocationOfTextNameTextBox();
        //        GuiZardFocusShiftFlag=1;
        //        NewGuiZardObj.ClickStopTestButton(TestnameLabelobj);
        //    }
        //    else
        //    {
        //        while(timer<GuizardFocusShiftTimeout && GuiZardFocusShiftFlag==0)
        //        {
        //            SetFocusOnGuizard();
        //            Window = GuiObj.GetCurrentActiveWinodwText();
        //            NewLogObj.WriteLogFile(LogFilePath, "Active window title "+Window, "info");
        //            if(Regex.IsMatch(Window,"GUIzard"))
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "GuiZard is set active", "info");
        //                AutomationElement TestnameLabelobj = NewGuiZardObj.ReturnLocationOfTextNameTextBox();
        //                GuiZardFocusShiftFlag=1;
        //                NewGuiZardObj.ClickStopTestButton(TestnameLabelobj);
        //            }
        //            else
        //            {
        //                timer=timer+3000;
        //                Thread.Sleep(3000);

        //            }
        //        }
        //        if((timer>=GuizardFocusShiftTimeout) && (GuiZardFocusShiftFlag==0))
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Unable to shift focus to Guizard even aftre timeout. Test will not be stopped ", "fail");
        //        }

        //    }
             
        //}

        //Making sure that Guizard is closed to avoid unnecessary sceen captures
        public int WaitTillGuizardClose()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            Generic NewGenericObj = new Generic();
            int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
            int Timer = 0;
            int WaitTimeOut = 60000;//1 min
            while (ProcessStatus == 1 && Timer<WaitTimeOut)
            {
                Console.WriteLine("Waiting for Guizard to close");
                NewLogObj.WriteLogFile(LogFilePath, "Waiting for Guizard to close ", "info");
                Thread.Sleep(500);
                Timer = Timer + 500;
                ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
            }
            if (ProcessStatus == 1 && Timer >= WaitTimeOut)
            {
                Console.WriteLine("Timeout Waiting for Guizard to close");
                NewLogObj.WriteLogFile(LogFilePath, "Timeout Waiting for Guizard to close. Killing Guizard ", "warn");
                NewGenericObj.KillProcess("GUIzard_3", LogFilePath);
                Thread.Sleep(2000);
                RenameGuizardFolderWithTestCaseName();
                return -1;
            }
            if (ProcessStatus == -1 && Timer <= WaitTimeOut)
            {
                Console.WriteLine("Guizard  closed successfully");
                NewLogObj.WriteLogFile(LogFilePath, "Guizard closed successfully ", "info");
                RenameGuizardFolderWithTestCaseName();
                return 1;
            }
            return -1;

        }

        public string ExtractFailFolderNameOld(string Line)
        {
            //Guizard format
            //<window caption="[0003] XenCenter" result="FAIL" frameworkId="WinForm" detailsFilename="\MissingAccessKey\MissingAccessKey_0003_XenCenter.xml" group="MissingAccessKeyGroup" screenshot="\MissingAccessKey\MissingAccessKey_0003_XenCenter.jpg" />
            //Check if the linehas detailsFilename
            string FOlderName;
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            if (!Regex.IsMatch(Line, "detailsFilename"))
            {
                NewLogObj.WriteLogFile(LogFilePath, Line + " detailsFilename missing. Not in required format.. Returning.. ", "info");
                return null;
            }
            int Index1=Line.IndexOf("detailsFilename");
            int Index2 = Line.IndexOf("group");
            //Extracting sub string afetr Index
            string TempString = Line.Substring(Index1,Index2);
            int Len = "detailsFilename".Length;
            //detailsFilename="\MissingAccessKey\MissingAccessKey_0003_XenCenter.xml"
            TempString = TempString.Substring(Len + 3);
            int InedxOfSlash = TempString.IndexOf("\\");
            FOlderName = TempString.Substring(0, InedxOfSlash);
            //Find the pattern after '"\' to extract folder
            return FOlderName;
        }

        public string ExtractFailFolderName(string Line)
        {
            //Guizard format
            //<window caption="[0003] XenCenter" result="FAIL" frameworkId="WinForm" detailsFilename="\MissingAccessKey\MissingAccessKey_0003_XenCenter.xml" group="MissingAccessKeyGroup" screenshot="\MissingAccessKey\MissingAccessKey_0003_XenCenter.jpg" />
            //Check if the linehas detailsFilename
            string FOlderName;
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            if (!Regex.IsMatch(Line, "detailsFilename"))
            {
                NewLogObj.WriteLogFile(LogFilePath, Line + " detailsFilename missing. Not in required format.. Returning.. ", "info");
                return null;
            }
            int Index1 = Line.IndexOf("detailsFilename");
            string TempString = Line.Substring(Index1);
            // detailsFilename="\MissingAccessKey\MissingAccessKey_0003_XenCenter.xml" group="MissingAccessKeyGroup" screenshot="\MissingAccessKey\MissingAccessKey_0003_XenCenter.jpg" />

            Index1 = TempString.IndexOf(" ");
            TempString = TempString.Substring(0, Index1);
            // TempString- detailsFilename="\MissingAccessKey\MissingAccessKey_0003_XenCenter.xml"

            int Len = "detailsFilename".Length;
            //detailsFilename="\MissingAccessKey\MissingAccessKey_0003_XenCenter.xml"
            TempString = TempString.Substring(Len + 3);
            int InedxOfSlash = TempString.IndexOf("\\");
            FOlderName = TempString.Substring(0, InedxOfSlash);
            //Find the pattern after '"\' to extract folder
            return FOlderName;



        }
        //public void KillGuizard()
        //{

        //    System.Diagnostics.Process[] Proc = System.Diagnostics.Process.GetProcessesByName("GUIzard_3.exe");
        //    Console.WriteLine("done");
        //}
    }

    


    

 
    
}

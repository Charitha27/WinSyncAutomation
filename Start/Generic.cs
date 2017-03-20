using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Management;

using LoggerCollection;
using FileOperationsCollection;
using GUICollection;

namespace GenericCollection
{
    public class Generic
    {

        public void StartProcess(string ProcessPath,string ProcessName, string LogFilePath,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            if (File.Exists(ProcessPath))
            {
              
                System.Diagnostics.Process.Start(ProcessPath);
                Process[] pname = Process.GetProcessesByName(ProcessName);
               
                if (pname.Length == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath,ProcessName+ " launched successfully", "info");
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ProcessName+" launch failed ", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "***EXiting application from StartProcess**", "fail");
                        Environment.Exit(1);
                    }
                    
                }
            }
        }

        public void KillProcess(string ProcessName, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                int IndexOfDot = ProcessName.IndexOf(".");
                if (IndexOfDot != -1)
                {
                    ProcessName = ProcessName.Substring(0, IndexOfDot);
                }
                Process[] pname = Process.GetProcessesByName(ProcessName);

                if (pname.Length != 0)
                {
                    Console.WriteLine("Killing process " + ProcessName);
                    NewLogObj.WriteLogFile(LogFilePath, "Killing process " + ProcessName, "info");
                    pname[0].Kill();
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ProcessName + " process not running ", "info");
                }
            }
            catch
            {

            }
        }

        public int CheckIfProcessIsRunning(string ProcessName)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            //Check if there are "." like .exe, extensions
            try
            {
                if (ProcessName.Contains("."))
                {
                    int IndexOfDot = ProcessName.IndexOf(".");
                    if (IndexOfDot != -1)
                    {
                        ProcessName = ProcessName.Substring(0, IndexOfDot);
                    }
                }
                Process[] pname = Process.GetProcessesByName(ProcessName);
                //foreach (System.Diagnostics.Process myProc in System.Diagnostics.Process.GetProcesses())
                //{
                //    string Name = myProc.ProcessName;
                //    if (string.Compare(myProc.ProcessName,ProcessName)==0)
                //    {
                //        NewLogObj.WriteLogFile(LogFilePath, "Process " + ProcessName + "is running", "info");
                //        return 1;
                //    }
                //    //else
                //    //{
                //    //    NewLogObj.WriteLogFile(LogFilePath, "Process " + ProcessName + "is not running", "info");
                //    //    return -1;
                //    //}
                //}
                //return -1;
                if (pname.Length == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Process " + ProcessName + "is not running", "info");
                    return -1;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Process " + ProcessName + "is running", "info");
                    return 1;
                }
            }
            catch
            {
                return -1;
            }
        }

        public void StartCmdExecutionAndExit(string command)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                //proc.WaitForExit();
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }
        public string StartCmdExecutionAndWaitForCompletion(string command)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
                string result = proc.StandardOutput.ReadToEnd();
                NewLogObj.WriteLogFile(LogFilePath,"Result of execution of cmd "+command+" : *"+result+"*","info");
                return result;
            }
            catch (Exception objException)
            {
                // Log the exception
                return null;
            }
        }


        public string GetOSName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            if (Regex.IsMatch(result, "windows 8", RegexOptions.IgnoreCase))
            {
                return "Windows8";
            }
            if (Regex.IsMatch(result, "windows 7", RegexOptions.IgnoreCase))
            {
                return "Windows7";
            }
            if (Regex.IsMatch(result, "windows server", RegexOptions.IgnoreCase))
            {
                return "WindowsServer";
            }
            return null;

        }
        public string GetSystemLocale()
        {
            string CurrentLocale = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            return CurrentLocale;
        }

        public string CheckIfProductLocalizedForCurrentLocale()
        {
            Logger NewLogObj = new Logger();
            FileOperations NewFileObj = new FileOperations();
            string LogFilePath = NewLogObj.LogFileFullPath;
            NewLogObj.WriteLogFile(LogFilePath, "CheckIfProductLocalizedForCurrentLocale", "info");
            NewLogObj.WriteLogFile(LogFilePath, "====================================", "info");
            
            string CurrentLocale = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            if (Regex.IsMatch(CurrentLocale, "CN", RegexOptions.CultureInvariant))
            {
                NewLogObj.WriteLogFile(LogFilePath, "Product is localized for current locale " + CurrentLocale, "info");
                return CurrentLocale;
            }
            else if (Regex.IsMatch(CurrentLocale, "JP", RegexOptions.CultureInvariant))
            {
                NewLogObj.WriteLogFile(LogFilePath, "Product is localized for current locale " + CurrentLocale, "info");
                return CurrentLocale;
            }
            NewLogObj.WriteLogFile(LogFilePath, "Product is not localized for current locale " + CurrentLocale, "info");
            return null;

        }

        // This can be used when the window with the same automtaion id, will have multiple child windows
       
        // eg: All the NEw VM windows and their panel tops have the same wuotmation id. 
        //Using this to get the label of each windows
        public AutomationElement VerifyWindowOnScreen(AutomationElement XenCenterObj,string ParentAutomationID,string ChildAutomationID, string ExpectedWindowLabel, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "VerifyWindowOnScreen", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            NewLogObj.WriteLogFile(LogFilePath, "Label to be checked " + ExpectedWindowLabel, "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition NewVMWindowCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ParentAutomationID, 1, LogFilePath);
            AutomationElement NewVMWindowObj = GuiObj.FindAutomationElement(XenCenterObj, NewVMWindowCondition, TreeScope.Descendants, ParentAutomationID, 1, LogFilePath);
            ////Locate the panel on window
            //PropertyCondition PanelWindowCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "panelTop", 1, LogFilePath);
            //AutomationElement PanelWindowObj = GuiObj.FindAutomationElement(NewVMWindowObj, PanelWindowCondition, TreeScope.Descendants, "New VM", 1, LogFilePath);
            //Locate the panel on window
            PropertyCondition LabelWindowCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ChildAutomationID, 1, LogFilePath);
            AutomationElement LabelWindowObj = GuiObj.FindAutomationElement(NewVMWindowObj, LabelWindowCondition, TreeScope.Descendants, ChildAutomationID, 1, LogFilePath);
            NewLogObj.WriteLogFile(LogFilePath, "Panel Label found " + LabelWindowObj.Current.Name, "info");
            if (string.Compare(ExpectedWindowLabel, LabelWindowObj.Current.Name) == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Expectde window found  " + LabelWindowObj.Current.Name, "info");
                return NewVMWindowObj;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "window found is not the expected one  " + LabelWindowObj.Current.Name, "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from TypeInXenCenterSearchBox as main menunot found**", "fail");
                    Environment.Exit(1);
                    return null;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    return null;
                }

            }


        }

        public AutomationElement FindAndClickBtn(AutomationElement ParentObj, string BtnAutomationID, string ElementName, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "FindAndClickBtn", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            NewLogObj.WriteLogFile(LogFilePath, "ElementName to be checked " + ElementName, "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition BtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", BtnAutomationID, 1, LogFilePath);
            AutomationElement BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnCondition, TreeScope.Descendants, ElementName, 1, LogFilePath);
            if (BtnObj == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "BtnObj is null for " + ElementName, "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Exiting application from FindAndClickBtn as " + ElementName+ " is null", "fail");
                    Environment.Exit(1);
                    
                }
                return null;
            }
            GuiObj.ClickButton(BtnObj, 0, ElementName, 1, LogFilePath);
            return BtnObj;

        }

        public AutomationElement VerifyIfAnElementExistUnderXenCenterTree(AutomationElement XenCenterObj, string ElementName,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "VerifyIfAnElementExistUnderXenCenterTree", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            NewLogObj.WriteLogFile(LogFilePath, "ElementName to be checked " + ElementName, "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "Checking in Xencenter tree for element " + ElementName, "info");
            //Verify if pool is added from Xencenter tree, by checking if a object with that name exists
            //Xencenter treeview
            PropertyCondition TreeReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "treeView", 1, LogFilePath);
            AutomationElement ServerPane = GuiObj.FindAutomationElement(XenCenterObj, TreeReturnCondition, TreeScope.Descendants, "Server Paniewe V", 0, LogFilePath);

            PropertyCondition XenCenterParentCondition = GuiObj.SetPropertyCondition("NameProperty", "XenCenter", 1, LogFilePath);
            AutomationElement XenCenterParentElement = GuiObj.FindAutomationElement(ServerPane, XenCenterParentCondition, TreeScope.Descendants, "XenCenterParentElement", 0, LogFilePath);


            ExpandCollapsePattern expPattern = XenCenterParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;

            expPattern.Expand();

            PropertyCondition AddedEleReturnCondition = GuiObj.SetPropertyCondition("NameProperty", ElementName, 1, LogFilePath);
            AutomationElement XenCenterAddedeElement = GuiObj.FindAutomationElement(ServerPane, AddedEleReturnCondition, TreeScope.Descendants, "XenCenterParentElement", 0, LogFilePath);

            if (XenCenterAddedeElement != null)
            {
                NewLogObj.WriteLogFile(LogFilePath, ElementName + "Exist in Xencenter tree", "info");
                return XenCenterAddedeElement;

            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, ElementName + "does not exist in Xencenter tree", "info");
                return null;

            }
        }

        //Return true if string has unicode chars else false
        public bool IsUnicode(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);

        }
    }
}

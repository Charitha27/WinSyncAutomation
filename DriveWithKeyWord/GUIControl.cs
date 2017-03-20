using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using Microsoft.Test;

using LoggerCollection;
using GUICollection;
using FileOperationsCollection;
using TestInputFileParsingCollection;
using GenericCollection;
using GuizardCollection;
using KeyWordAPIsCollection;

namespace GUIControlCollection
{
    class GUIControl
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);
        public AutomationElement ParentObj = null;
        
        
        //*****************************
        // Wait for a window to appear
        // ParamType - AutomationID or ElementName 
        //FindParentFirstCall added to set the scope as children for 1st time call & for rest set as descendenets
        //*****************************
        //Will wait for the guizard to finsih capturing
        public AutomationElement WaitWindow(AutomationElement ParentObj, string ScopeType, string ParamType, string ParamValue,int TimeOutInSecs, int TerminateStatus, string GuizardLocation)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            List <PropertyCondition> WindowConditionList=new List<PropertyCondition>();
            PropertyCondition WindowCondition=null;
            PropertyCondition WindowCondition2=null;
                        
            NewLogObj.WriteLogFile(LogFilePath, "WaitWindow", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            KeyWordAPIs KeywordApiObj=new KeyWordAPIs();
           
            WindowConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            WindowCondition=WindowConditionList[0];
            WindowCondition2=WindowConditionList[1];
            
            AutomationElement WindowObj;
            TreeScope Scope;
           // if (string.Compare(ScopeType, "Root") == 0 && FindParentFirstCall==1)
            
            if (string.Compare(ScopeType, "Root") == 0)
            {
                //WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, TreeScope.Children, "Window ParamValue", TerminateStatus, LogFilePath);
                Scope = TreeScope.Children;
                
            }
            else
            {
                //WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, TreeScope.Descendants, "Window ParamValue", TerminateStatus, LogFilePath);
                Scope = TreeScope.Descendants;

            }
            if(Regex.IsMatch(ParamType,"name",RegexOptions.IgnoreCase) && KeyWordAPIs.PartiallyLocalized==1) 
            {
                WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition,WindowCondition2, Scope, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);

            }
            else
            {
                 //Waiting for timeout secs happen in FindAutomationElement
                WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, Scope, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
            }
            if (WindowObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for window with " + ParamType + " - " + ParamValue + " to appear. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for window with " + ParamType + " - " + ParamValue + " to appear. Resuming as termiateonfailure is no", "warn");
                return null;
            }
            
            NewLogObj.WriteLogFile(LogFilePath, "WindowObj found ", "info");
            IntPtr hwnd = (IntPtr)WindowObj.Current.NativeWindowHandle;
            SetForegroundWindow(hwnd);
            Thread.Sleep(2000);
            SetActiveWindow(hwnd);
            Thread.Sleep(2000);
            Generic NewGenericObj = new Generic();
            int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
            if (ProcessStatus == 1)
            {
                Guizard NewGuiZardObj = new Guizard();
                NewGuiZardObj.VerifyTestCompletedFilePresent(GuizardLocation, ParamValue);


                //#############################################################################
                //while GUIzard is capturing the windows, it is filling up the controls in the window with a value 1.
                //Slep added to make sure that Guizard finish captuing the window
                //Thread.Sleep(4000);
                //#############################################################################
            }
            
            return WindowObj;

        }

        //Will check the guizard only based on status of GuizardSkip param
        //SkipGuizard =1 -- will not wait for guizard tof finish
        //SkipGuizard =0 -- will wait for guizard tof finish
        public AutomationElement WaitWindow(AutomationElement ParentObj, string ScopeType, string ParamType, string ParamValue, int TimeOutInSecs, int TerminateStatus, string GuizardLocation,int SkipGuizard)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            List<PropertyCondition> WindowConditionList = new List<PropertyCondition>();
            PropertyCondition WindowCondition = null;
            PropertyCondition WindowCondition2 = null;

            NewLogObj.WriteLogFile(LogFilePath, "WaitWindow", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();

            WindowConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            WindowCondition = WindowConditionList[0];
            WindowCondition2 = WindowConditionList[1];

            AutomationElement WindowObj;
            TreeScope Scope;
            // if (string.Compare(ScopeType, "Root") == 0 && FindParentFirstCall==1)

            if (string.Compare(ScopeType, "Root") == 0)
            {
                //WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, TreeScope.Children, "Window ParamValue", TerminateStatus, LogFilePath);
                Scope = TreeScope.Children;

            }
            else
            {
                //WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, TreeScope.Descendants, "Window ParamValue", TerminateStatus, LogFilePath);
                Scope = TreeScope.Descendants;

            }
            if (Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase) && KeyWordAPIs.PartiallyLocalized == 1)
            {
                WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, WindowCondition2, Scope, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);

            }
            else
            {
                //Waiting for timeout secs happen in FindAutomationElement
                WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, Scope, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
            }
            if (WindowObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for window with " + ParamType + " - " + ParamValue + " to appear. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for window with " + ParamType + " - " + ParamValue + " to appear. Resuming as termiateonfailure is no", "warn");
                return null;
            }

            NewLogObj.WriteLogFile(LogFilePath, "WindowObj found ", "info");
            IntPtr hwnd = (IntPtr)WindowObj.Current.NativeWindowHandle;
            SetForegroundWindow(hwnd);
            Thread.Sleep(2000);
            SetActiveWindow(hwnd);
            Thread.Sleep(2000);
            Generic NewGenericObj = new Generic();
            if (SkipGuizard == 0)
            {
                int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
                if (ProcessStatus == 1)
                {
                    Guizard NewGuiZardObj = new Guizard();
                    NewGuiZardObj.VerifyTestCompletedFilePresent(GuizardLocation, ParamValue);


                    //#############################################################################
                    //while GUIzard is capturing the windows, it is filling up the controls in the window with a value 1.
                    //Slep added to make sure that Guizard finish captuing the window
                    //Thread.Sleep(4000);
                    //#############################################################################
                }
            }
            if (string.Compare(ScopeType, "Root") == 0)
            {
                ParentObj = WindowObj;
            }
            return WindowObj;

        }

        public AutomationElement WaitWindowWithPattern(AutomationElement ParentObj, string ScopeType, string ElementToSearchControlType,string PatternValue, int TimeOutInSecs, int TerminateStatus, string GuizardLocation, int SkipGuizard)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
  
            NewLogObj.WriteLogFile(LogFilePath, "WaitWindowWithPattern", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            
            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            
            AutomationElement WindowObj=null;
            TreeScope Scope;
            
            if (string.Compare(ScopeType, "Root") == 0)
            {
                //WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, TreeScope.Children, "Window ParamValue", TerminateStatus, LogFilePath);
                Scope = TreeScope.Children;

            }
            else
            {
                //WindowObj = GuiObj.FindAutomationElement(ParentObj, WindowCondition, TreeScope.Descendants, "Window ParamValue", TerminateStatus, LogFilePath);
                Scope = TreeScope.Descendants;

            }
            Regex RegPattern = new Regex(PatternValue);
            PropertyCondition PropCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", ElementToSearchControlType, 0, LogFilePath);
            AutomationElementCollection AutoElementColl = ParentObj.FindAll(Scope, PropCondition);
            foreach (AutomationElement AutoObj in AutoElementColl)
            {
                string AutoObjName = AutoObj.Current.Name;
                if (!string.IsNullOrWhiteSpace(AutoObjName))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Current AutoObj name " + AutoObjName, "info");
                    Match PatternMatch = RegPattern.Match(AutoObjName);
                    if (PatternMatch.Success)
                    {
                        WindowObj = AutoObj;
                        break;
                        //return AutoObj;
                    }
                }
            }
            
            if (WindowObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for window with " + PatternValue + " to appear. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for window with " + PatternValue + " to appear. Resuming as termiateonfailure is no", "warn");
                return null;
            }

            NewLogObj.WriteLogFile(LogFilePath, "WindowObj found ", "info");
            IntPtr hwnd = (IntPtr)WindowObj.Current.NativeWindowHandle;
            SetForegroundWindow(hwnd);
            Thread.Sleep(2000);
            SetActiveWindow(hwnd);
            Thread.Sleep(2000);
            Generic NewGenericObj = new Generic();
            if (SkipGuizard == 0)
            {
                int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
                if (ProcessStatus == 1)
                {
                    Guizard NewGuiZardObj = new Guizard();
                    NewGuiZardObj.VerifyTestCompletedFilePresent(GuizardLocation, PatternValue);


                    //#############################################################################
                    //while GUIzard is capturing the windows, it is filling up the controls in the window with a value 1.
                    //Slep added to make sure that Guizard finish captuing the window
                    //Thread.Sleep(4000);
                    //#############################################################################
                }
            }
            if (string.Compare(ScopeType, "Root") == 0)
            {
                ParentObj = WindowObj;
            }
            return WindowObj;

        }

        public AutomationElement TypeInTextBox(AutomationElement ParentObj,string ParamType, string ParamValue,string TextToType, int Index,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "TypeInTextBox", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
           // PropertyCondition TBCondition = SetParamTypeBasedPropertyCondition(ParamType,ParamValue);
            List <PropertyCondition> TBConditionList=new List<PropertyCondition>();
            TBConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue); 
            PropertyCondition TBCondition=TBConditionList[0];
            PropertyCondition TBCondition1=TBConditionList[1];
            
            KeyWordAPIs KeywordApiObj=new KeyWordAPIs();
            AutomationElement TBObj=null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    TBObj = GuiObj.FindAutomationElementWithIndex(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                //TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition,TBCondition1, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
            }
            else
            {
                //TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
                if (Index == 0)
                {
                    TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    TBObj = GuiObj.FindAutomationElementWithIndex(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            if (TBObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Textbox with " + ParamType + " - " + ParamValue + " in TypeInTextBox. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Textbox with " + ParamType + " - " + ParamValue + " in TypeInTextBox. Resuming as termiateonfailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "Textbox found ", "info");
            GuiObj.SetTextBoxText(TBObj, TextToType, "TextBox", TerminateStatus, LogFilePath);
            return TBObj;

        }

        public AutomationElement CheckIfTextBoxHasExpectedValue(AutomationElement ParentObj, string ParamType, string ParamValue, string ExpectedValue,int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfTextBoxHasExpectedValue", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            // PropertyCondition TBCondition = SetParamTypeBasedPropertyCondition(ParamType,ParamValue);
            List<PropertyCondition> TBConditionList = new List<PropertyCondition>();
            TBConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition TBCondition = TBConditionList[0];
            PropertyCondition TBCondition1 = TBConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement TBObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    TBObj = GuiObj.FindAutomationElementWithIndex(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                //TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition,TBCondition1, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
            }
            else
            {
                //TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
                if (Index == 0)
                {
                    TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    TBObj = GuiObj.FindAutomationElementWithIndex(ParentObj, TBCondition, TBCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            if (TBObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Textbox with " + ParamType + " - " + ParamValue + " in TypeInTextBox. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Textbox with " + ParamType + " - " + ParamValue + " in TypeInTextBox. Resuming as termiateonfailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "Textbox found ", "info");
            
            string ActualValue=GuiObj.GetTextBoxText(TBObj, ParamValue, TerminateStatus, LogFilePath);
            if (string.Compare(ActualValue, ExpectedValue) == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "ActualValue " + ActualValue + "  in Textbox is same as  ExpectedValue" + ExpectedValue, "pass");
            }
            else
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "ActualValue " + ActualValue + "  in Textbox is different from  ExpectedValue" + ExpectedValue , "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
               NewLogObj.WriteLogFile(LogFilePath, "ActualValue " + ActualValue + "  in Textbox is different from  ExpectedValue" + ExpectedValue+" Resuming as termiateonfailure is no", "warn");
                return null;
            }
            return TBObj;

        }

        public AutomationElement ClickHyperLinkObj(AutomationElement ParentObj, string ParamType, string ParamValue, int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "ClickHyperLinkObj", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            // PropertyCondition TBCondition = SetParamTypeBasedPropertyCondition(ParamType,ParamValue);
            List<PropertyCondition> HLConditionList = new List<PropertyCondition>();
            HLConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition HLCondition = HLConditionList[0];
            PropertyCondition HLCondition1 = HLConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement HLObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    HLObj = GuiObj.FindAutomationElement(ParentObj, HLCondition, HLCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    HLObj = GuiObj.FindAutomationElementWithIndex(ParentObj, HLCondition, HLCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                //TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition,TBCondition1, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
            }
            else
            {
                //TBObj = GuiObj.FindAutomationElement(ParentObj, TBCondition, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
                if (Index == 0)
                {
                    HLObj = GuiObj.FindAutomationElement(ParentObj, HLCondition, HLCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    HLObj = GuiObj.FindAutomationElementWithIndex(ParentObj, HLCondition, HLCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            if (HLObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Hyperlink with " + ParamType + " - " + ParamValue + " in ClickHyperLinkObj. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Hyperlink with " + ParamType + " - " + ParamValue + " in ClickHyperLinkObj. Resuming as termintaeonfailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "Textbox found ", "info");
            GuiObj.ClickHyperLink(HLObj, "Hyperlink", TerminateStatus, LogFilePath);
            return HLObj;

        }

        public AutomationElement SetDocObjText(AutomationElement ParentObj, string ParamType, string ParamValue, string TextToType, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "SetDocumentObjText", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition DocObjCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List <PropertyCondition> DocObjConditionList=new List<PropertyCondition>();
            DocObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue); 
            PropertyCondition DocObCondition=DocObjConditionList[0];
            PropertyCondition DocObCondition1=DocObjConditionList[1];
            
            KeyWordAPIs KeywordApiObj=new KeyWordAPIs();
            AutomationElement DocObj=null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                DocObj = GuiObj.FindAutomationElement(ParentObj, DocObCondition,DocObCondition1, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
            }
            else
            {
                 DocObj = GuiObj.FindAutomationElement(ParentObj, DocObCondition, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
            }

            
            if (DocObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for DocumentObj with "+ParamType+" - "+ParamValue+" in SetDocumentObjText.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for DocumentObj with " + ParamType + " - " + ParamValue + " in SetDocumentObjText.Resuming as termintaeonfailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "DocObj found ", "info");
            //Unable to set the values of document obj using setvalue method
            //Using workaround of bounding recatngle
            GuiObj.SetDocumentObjText(DocObj, TextToType, "Doc obj", TerminateStatus, LogFilePath);
            return DocObj;

        }

        public AutomationElement ClickBtn(AutomationElement ParentObj, string ParamType, string ParamValue, int Index,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
             string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "ClickBtn", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List <PropertyCondition> BtnObjConditionList=new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue); 
            PropertyCondition BtnObCondition=BtnObjConditionList[0];
            PropertyCondition BtnObCondition1=BtnObjConditionList[1];
            
            KeyWordAPIs KeywordApiObj=new KeyWordAPIs();
            AutomationElement BtnObj=null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition,BtnObCondition1, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
                    
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition,BtnObCondition1, TreeScope.Descendants, "Window ParamValue", Index,TimeOutInSecs, TerminateStatus, LogFilePath);
                    
                }
            }
            else
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue",TimeOutInSecs, TerminateStatus, LogFilePath);
                    
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", Index,TimeOutInSecs, TerminateStatus, LogFilePath);
                    
                }
            }
               
            if (BtnObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for BtnObj with"+ ParamType+" - "+ParamValue+" in ClickBtn. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for BtnObj with "+ ParamType+" - "+ParamValue+" in ClickBtn. Resuming as TerminateOnFailure is no","warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "Preparing to click on btn with  " + ParamType + " and value " + ParamValue, "info");
            Console.WriteLine("Preparing to click on btn with  " + ParamType + " and value " + ParamValue);
            NewLogObj.WriteLogFile(LogFilePath, "Button found ", "info");
            GuiObj.ClickButton(BtnObj, 0, ParamValue, TerminateStatus, LogFilePath);
            return BtnObj;

        }

        public AutomationElement GtListElementAndSelect(AutomationElement ParentObj, string ParamType, string ParamValue,string ItemToSelect,int SelectWithIndex,int Index, int SelectItemIndex,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "GtListElementAndSelect", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);

            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue); 
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ListObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    ListObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);

                }
                else
                {
                    ListObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);

                }
            }
            else
            {
                if (Index == 0)
                {
                    ListObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);

                }
                else
                {
                    ListObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);

                }
            }

            
            if (ListObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for ListObj with " + ParamType + " - " + ParamValue + " in GtListElementAndSelect. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for ListObj with " + ParamType + " - " + ParamValue + " in GtListElementAndSelect. Resuming as TerminateonFailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "ListObj with " + ParamType + " and value " + ParamValue+" found", "info");
            Console.WriteLine("Preparing to select item from ListObj");
            AutomationElement ListItem = GuiObj.GetListElementAndSelect(ListObj, SelectWithIndex, SelectItemIndex, ItemToSelect, TerminateStatus);

            return ListItem;

        }

        public AutomationElement ExpandCmbBox(AutomationElement ParentObj, string ParamType, string ParamValue,int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "ExpandComboBox", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue); 
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement CmbObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    CmbObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    CmbObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    CmbObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    CmbObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            
            if (CmbObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for CmbObj with " + ParamType + " - " + ParamValue + " in ExpandCmbBox. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for CmbObj with " + ParamType + " - " + ParamValue + " in ExpandCmbBox. Resuming as TerminateOnFailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "ComboBox with " + ParamType + " and value " + ParamValue + " found", "info");
            Console.WriteLine("Preparing to expand ComboBox");
            GuiObj.ExpandComboBox(CmbObj, TerminateStatus);

            return CmbObj;

        }

        public AutomationElement SetRdBtn(AutomationElement ParentObj, string ParamType, string ParamValue,int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "SetRdBtn", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue); 
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement BtnObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
       
            if (BtnObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for RdBtn with" + ParamType + " - " + ParamValue + " in SetRdBtn. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for RdBtn with" + ParamType + " - " + ParamValue + " in SetRdBtn. Resuming as TerminateonFailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "Rdbtn found ", "info");
            
            GuiObj.SetRadioButton(BtnObj, "RdBtn", TerminateStatus, LogFilePath);
            return BtnObj;

        }

        public AutomationElement CheckCheckBox(AutomationElement ParentObj, string ParamType, string ParamValue,int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckCheckBox", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
           // PropertyCondition ChkBoxCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ChkBoxObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    ChkBoxObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChkBoxObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    ChkBoxObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChkBoxObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (ChkBoxObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Checkbox with" + ParamType + " - " + ParamValue + " in CheckCheckBox.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Checkbox with" + ParamType + " - " + ParamValue + " in CheckCheckBox.Resuming as TerminateonFailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "ChkBox found ", "info");
            string ToggleState=GuiObj.CheckToggleState(ChkBoxObj, TerminateStatus);
            if (string.Compare(ToggleState, "On")==0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Checkbox already checked.", "info");
            }
            else if (string.Compare(ToggleState, "Off") == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Checkbox checking.", "info");
                GuiObj.ToggleCheckBoxState(ChkBoxObj, TerminateStatus);
            }
            
            return ChkBoxObj;
        }

        public AutomationElement UnCheckCheckBox(AutomationElement ParentObj, string ParamType, string ParamValue,int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "UnCheckCheckBox", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            
            // PropertyCondition ChkBoxCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ChkBoxObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    ChkBoxObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChkBoxObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    ChkBoxObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChkBoxObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (ChkBoxObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Checkbox with" + ParamType + " - " + ParamValue + " in UnCheckCheckBox. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Checkbox with" + ParamType + " - " + ParamValue + " in UnCheckCheckBox.Resuming as TerminateonFailure is no", "warn");
                return null;
            }
            NewLogObj.WriteLogFile(LogFilePath, "ChkBox found ", "info");
            string ToggleState = GuiObj.CheckToggleState(ChkBoxObj, TerminateStatus);
            if (string.Compare(ToggleState, "Off") == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Checkbox already unchecked.", "info");
            }
            else if (string.Compare(ToggleState, "On") == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Checkbox unchecking.", "info");
                GuiObj.ToggleCheckBoxState(ChkBoxObj, TerminateStatus);
            }

            return ChkBoxObj;
        }

        public AutomationElement InvokeMenu(AutomationElement ParentObj, string ParamType, string MainMenuValue, int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "InvokeMenu", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            
            // PropertyCondition ChkBoxCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, MainMenuValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement MainMenuObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            
            if (MainMenuObj != null)
            {
                InvokePattern invokePattern = MainMenuObj.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                if (invokePattern != null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invoking menuitem " + MainMenuValue, "info");
                    invokePattern.Invoke();
                    return MainMenuObj;
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "invokePattern is null for main menu with "+ParamType+"-"+MainMenuValue+". Exiting application from InvokeMenu as invokePattern is null", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "invokePattern is null for main menu with " + ParamType + "-" + MainMenuValue+". Resuming as terminateOnFailure is no", "warn");
                    return null;
                }
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "MainMenuObj with " +ParamType+"-"+MainMenuValue+" is null", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
        }

        public AutomationElement InvokeMenu(AutomationElement ParentObj,string ParamType, string MainMenuValue,string SubmenuValue, int Index,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "InvokeMenu", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //ParentObj = WaitWindow(AutomationElement.RootElement, "Root", "name", "XenCenter", 10, 1);
            //PropertyCondition MenuCondition = SetParamTypeBasedPropertyCondition(ParamType, MainMenuValue);
           
            // PropertyCondition ChkBoxCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, MainMenuValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement MainMenuObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            if (MainMenuObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "MainMenuObj with " + ParamType + "-" + MainMenuValue + " is null", "fail");
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "MainMenuObj with " + ParamType + "-" + MainMenuValue + " is null.Resuming as terminateOnFailure is no", "warn");
                }
            }

            List<PropertyCondition> SubMenuConditionList = SetParamTypeBasedPropertyCondition(ParamType, SubmenuValue);
            AutomationElement SubMenuObj = GuiObj.FindAutomationElement(MainMenuObj, SubMenuConditionList[0], TreeScope.Descendants, "SubMenu", TerminateStatus, LogFilePath);
            if (SubMenuObj != null)
            {
                InvokePattern invokePattern = SubMenuObj.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                if (invokePattern != null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invoking menuitem " + MainMenuValue, "info");
                    invokePattern.Invoke();
                    return SubMenuObj;
                            
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "invokePattern is null for submenu with " + ParamType+"-"+SubmenuValue+". Exiting application from InvokeMenu as invokePattern is null**", "fail");
                        FileObj.ExitTestEnvironment();
                        return null;
                     }
                    NewLogObj.WriteLogFile(LogFilePath, "invokePattern is null for submenu with " + ParamType + "-" + SubmenuValue + ". Resuming as terminateOnFailure is no", "warn");
                    return null;
                }
            }
            else
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj with "+ ParamType+"-"+SubmenuValue+" is null"+". Exiting application ", "fail");
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj with " + ParamType + "-" + SubmenuValue + " is null" + ". Resuming as terminateOnFailure is no", "warn");
                return null;
            }
        }
        public AutomationElement InvokeMenuTree(AutomationElement ParentObj, string ParamType, string MainMenuValue, string SubmenuValue1, string SubmenuValue2,int Index,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "InvokeMenuTree", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //ParentObj = WaitWindow(AutomationElement.RootElement, "Root", "name", "XenCenter", 10, 1);
            //PropertyCondition MenuCondition = SetParamTypeBasedPropertyCondition(ParamType, MainMenuValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, MainMenuValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement MainMenuObj = null;

            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, 0, LogFilePath);
                }
                else
                {
                    MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, 0, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, 0, LogFilePath);
                }
                else
                {
                    MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, 0, LogFilePath);
                }
            }
           
            if (MainMenuObj == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "MainMenuObj with " + ParamType + " - " + MainMenuValue + " is null from InvokeMenuTree. Repeating serach", "warn");
                //Retry once more
                Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                Thread.Sleep(1000);
                
                if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
                {
                    if (Index == 0)
                    {
                        MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                    else
                    {
                        MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                }
                else
                {
                    if (Index == 0)
                    {
                        MainMenuObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                    else
                    {
                        MainMenuObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                }
            }
            if (MainMenuObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "MainMenuObj with "+ ParamType+" - "+MainMenuValue+ " is null from InvokeMenuTree.Exiting application from InvokeMenu", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "MainMenuObj with " + ParamType + " - " + MainMenuValue + " is null from InvokeMenuTree.. Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            List<PropertyCondition> SubMenuCondition1List = SetParamTypeBasedPropertyCondition(ParamType, SubmenuValue1);
            AutomationElement SubMenuObj1 = GuiObj.FindAutomationElement(MainMenuObj, SubMenuCondition1List[0], TreeScope.Descendants, "SubMenu1", 0, LogFilePath);
            if (SubMenuObj1 == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj1 with " + ParamType + " - " + SubmenuValue1 + " is null from InvokeMenuTree. Repeating serach", "warn");
                SubMenuObj1 = GuiObj.FindAutomationElement(MainMenuObj, SubMenuCondition1List[0], TreeScope.Descendants, "SubMenu1", TerminateStatus, LogFilePath);
            }
            if (SubMenuObj1 != null)
            {
                List<PropertyCondition> SubMenuCondition2List = SetParamTypeBasedPropertyCondition(ParamType, SubmenuValue2);
                AutomationElement SubMenuObj2 = GuiObj.FindAutomationElement(SubMenuObj1, SubMenuCondition2List[0], TreeScope.Descendants, "SubMenu2", 0, LogFilePath);
                if (SubMenuObj2 == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj2 with " + ParamType + " - " + SubmenuValue2 + " is null from InvokeMenuTree. Repeating serach on Mainmenu", "warn");
                    SubMenuObj2 = GuiObj.FindAutomationElement(MainMenuObj, SubMenuCondition2List[0], TreeScope.Descendants, "SubMenu2", 1, LogFilePath);
                    //Repeating serach on mainmenu obj
                    //SubMenuObj2 = GuiObj.FindAutomationElement(MainMenuObj, SubMenuCondition2List[0], TreeScope.Descendants, "SubMenu2", 1, LogFilePath);

                }
                if (SubMenuObj2 != null)
                {
                    InvokePattern invokePattern = SubMenuObj2.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    if (invokePattern != null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invoking sub menuitem " + SubmenuValue1 + " - " + SubmenuValue2, "info");
                        invokePattern.Invoke();
                        return SubMenuObj2;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "invokePattern is null for " + SubmenuValue1 + " - " + SubmenuValue2, "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "invokePattern is null for " + SubmenuValue1 + " - " + SubmenuValue2+". Exiting application from InvokeMenu as invokePattern is nul", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                        return null;
                    }
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj2 with " + ParamType + " - " + SubmenuValue2 + " is null. Exiting application", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj2 with " + ParamType + " - " + SubmenuValue2 + " is null. Resuming as terminateOnFailure is no", "warn");
                    return null;
                }
            }
            else
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj1 with " + ParamType + " - " + SubmenuValue1 + " is null. Exiting application ", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj1 with " + ParamType + " - " + SubmenuValue1 + " is null. Resuming as terminateOnFailure is no ", "warn");
                return null;
            }
        }

        //public PropertyCondition SetParamTypeBasedPropertyCondition(string ParamType, string ParamValue)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();

        //    AutomationElementIdentity GuiObj = new AutomationElementIdentity();

        //    NewLogObj.WriteLogFile(LogFilePath, "SetParamTypeBasedPropertyCondition", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
        //    ParamType = ParamType.ToLower();
        //    PropertyCondition PropCondition = null;
        //    if (string.Compare(ParamType, "automationid") == 0)
        //    {
        //        PropCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ParamValue, 0, LogFilePath);
        //    }
        //    else if (string.Compare(ParamType, "name") == 0)
        //    {
        //        PropCondition = GuiObj.SetPropertyCondition("NameProperty", ParamValue, 0, LogFilePath);
        //     }
        //    else if (string.Compare(ParamType, "mappedname") == 0)
        //    {
        //        PropCondition = GuiObj.SetPropertyCondition("NameProperty", ParamValue, 0, LogFilePath);
        //    }
        //    if (PropCondition == null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "PropCondition is null..", "fail");
        //        FileOperations FileObj = new FileOperations();
        //        FileObj.ExitTestEnvironment();
        //    }

        //    return PropCondition;
        //}


        public List<PropertyCondition> SetParamTypeBasedPropertyCondition(string ParamType,string ParamValue)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "SetParamTypeBasedPropertyCondition", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            PropertyCondition PropCondition=null;

            //For partially localized products
            PropertyCondition PropCondition2 = null;

            List <PropertyCondition> PropertyConditionList=new List<PropertyCondition>();

            if (string.Compare(ParamType, "automationid") == 0)
            {
                PropCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ParamValue, 0, LogFilePath);
            }
            else if (string.Compare(ParamType, "class") == 0)
            {
                PropCondition = GuiObj.SetPropertyCondition("ClassNameProperty", ParamValue, 0, LogFilePath);
            }
            else if (string.Compare(ParamType, "name") == 0)
            {
                PropCondition = GuiObj.SetPropertyCondition("NameProperty", ParamValue, 0, LogFilePath);
                KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
                if (KeyWordAPIs.PartiallyLocalized == 1)
                {
                    if (KeyWordAPIs.LocEnStringMappingForPartialLocalization.ContainsKey(ParamValue))
                    {
                        string EnValue = KeyWordAPIs.LocEnStringMappingForPartialLocalization[ParamValue];
                        PropCondition2 = GuiObj.SetPropertyCondition("NameProperty", EnValue, 0, LogFilePath);
                    }
                }
            }
            else if (string.Compare(ParamType, "mappedname") == 0)
            {
                PropCondition = GuiObj.SetPropertyCondition("NameProperty", ParamValue, 0, LogFilePath);
            }

            else if (string.Compare(ParamType, "controltype") == 0)
            {
                PropCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", ParamValue, 0, LogFilePath);
            }

            if (PropCondition == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCondition is null using "+ParamType+" - "+ParamValue, "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();
            }
            PropertyConditionList.Add(PropCondition);
            PropertyConditionList.Add(PropCondition2);
            return PropertyConditionList;
        }

        public void SelectTabItemFromTabControl(AutomationElement ParentObj, string ParentTabCntrlParamType, string ParentTabCntrlParamValue, string ChildTabCntrlParamType,string ChildTabCntrlParamValue, int Index,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "SelectTabItemFromTabControl", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===========================", "info");
            ParentTabCntrlParamType = ParentTabCntrlParamType.ToLower();
           // PropertyCondition TabParnetCondition = SetParamTypeBasedPropertyCondition(ParentTabCntrlParamType, ParentTabCntrlParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParentTabCntrlParamType, ParentTabCntrlParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement TabParnetObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentTabCntrlParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    TabParnetObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    TabParnetObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    TabParnetObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    TabParnetObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            if (TabParnetObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Parent tab control with " + ParentTabCntrlParamType + " - " + ParentTabCntrlParamValue + " in  SelectTabItemFromTabControl.Exiting application", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Parent tab control with " + ParentTabCntrlParamType + " - " + ParentTabCntrlParamValue + " in  SelectTabItemFromTabControl.Resuming as terminateOnFailure is no ", "warn");
            }

            NewLogObj.WriteLogFile(LogFilePath, "Parent tab cntrl obj found ", "info");
            GuiObj.SelectItemFromParent(TabParnetObj, ChildTabCntrlParamType, ChildTabCntrlParamValue, TerminateStatus);
        }

        //public AutomationElement CheckTreeForElement(AutomationElement ParentObj, string ParentIdentityType, string ParentIdentity, string ElemntNameToClick, int TerminateStatus)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();

        //    AutomationElementIdentity GuiObj = new AutomationElementIdentity();
        //    FileOperations FileObj = new FileOperations();

        //    NewLogObj.WriteLogFile(LogFilePath, "CheckTreeForElement", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=================", "info");

        //    PropertyCondition TreeCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Tree", 1, LogFilePath);
        //    PropertyCondition TreeAutoIdCondition = null;
        //    //Look for an element of type ControlType.Tree in the parent objecti
        //    if (Regex.IsMatch(ParentIdentityType, "automationid", RegexOptions.CultureInvariant))
        //    {
        //        TreeAutoIdCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ParentIdentity, 1, LogFilePath);  
        //    }
        //    else if(Regex.IsMatch(ParentIdentityType, "name", RegexOptions.CultureInvariant))
        //    {
        //        TreeAutoIdCondition = GuiObj.SetPropertyCondition("NameProperty", ParentIdentity, 1, LogFilePath); 
        //    }
        //    if(TreeAutoIdCondition==null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Unable to set TreeAutoIdCondition CheckTreeForElement", "fail");
        //        return null;
        //    }
            
        //    AndCondition CombineCondition = new AndCondition(TreeAutoIdCondition, TreeCondition);

        //    AutomationElement TreeObj = ParentObj.FindFirst(TreeScope.Descendants, CombineCondition);
        //    //AutomationElement TreeObj = GuiObj.FindAutomationElement(ParentObj, TreeCondition, TreeScope.Descendants, "Tree", TerminateStatus, LogFilePath);
        //    //AutomationElementCollection TreeObjColl = ParentObj.FindAll(TreeScope.Descendants, TreeCondition);
            
        //    if (TreeObj == null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Unable to find object of type ControlType.Tree in Parent obj", "fail");
        //        if (TerminateStatus == 1)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnTreeElement as ControlType.Tree element is null**", "fail");
        //            FileObj.ExitTestEnvironment();
        //        }
        //        return null;
        //    }
        //    //Look for an element of type ControlType.TreeItem in the tree object
        //    //PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
        //    AutomationElement TreeElementFound=GuiObj.CheckTreeForElementAndClickIfReqd(TreeObj, ElemntNameToClick, 0, "0", LogFilePath);
        //    return TreeElementFound;

        //}\

       
        public AutomationElement ChkTreeForElementAndClickIfReqd(AutomationElement ParentObj, string ParentIdentityType, string ParentIdentity, string ChildIdentityType, string ChildIdentity, int ClickRequired, string MouseBtn, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "ChkTreeForElementAndClickIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            //Look for an element of type ControlType.Tree in the parent object
            PropertyCondition TreeCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Tree", 1, LogFilePath);

            //  There could be multiple trees in the parent obj
            // Get all the trees, and find if the parent elemnt  in each tree matches with the  ParentIdentity supplied
            AutomationElementCollection NodeColl = ParentObj.FindAll(TreeScope.Descendants, TreeCondition);
            AutomationElement ParentSearchElemnt = null;
            //AutomationElement ParnetSearchElemntTree = null;
            foreach (AutomationElement TreeObj in NodeColl)
            {
                //Check if the TreeObj is the parent element
                //Users could supply a "Controltype.tree" or "Controltype.treeitem as parent"

                if (string.Compare(ParentIdentityType, "name") == 0)
                {
                    if (string.Compare(TreeObj.Current.Name, ParentIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeObj.Current.Name + " found in tree ", "info");
                        ParentSearchElemnt = TreeObj;
                        //ParnetSearchElemntTree = TreeObj;
                        break;
                    }
                }
                else if (string.Compare(ParentIdentityType, "automationid") == 0)
                {
                    if (string.Compare(TreeObj.Current.AutomationId, ParentIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeObj.Current.AutomationId + " found in tree ", "info");
                        ParentSearchElemnt = TreeObj;
                       // ParnetSearchElemntTree = TreeObj;
                        break;
                    }
                }
                // In each tree, check the 1st element name (user has provided the tree parent element name & this should be the 1st element)
                PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                //User should have supplied the parent tree eemnt name, which would be the 1st item
                //AutomationElement TreeParentObj = TreeObj.FindFirst(TreeScope.Descendants, TreeItemCondition);
                AutomationElementCollection TreeParentObjColl = TreeObj.FindAll(TreeScope.Descendants, TreeItemCondition);
                foreach (AutomationElement TreeParentObj in TreeParentObjColl)
                {
                    //AutomationElement TreeParentObj=TreeObj.FindFirst(TreeScope.Descendants,TreeItemCondition);

                    if (string.Compare(ParentIdentityType,"name")==0)
                    {
                        if (string.Compare(TreeParentObj.Current.Name, ParentIdentity) == 0)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeParentObj.Current.Name + " found in tree ", "info");
                            ParentSearchElemnt = TreeParentObj;
                            ExpandCollapsePattern expPattern = ParentSearchElemnt.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                            NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                            
                            if (string.Compare(expPattern.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                            {
                                expPattern.Expand();
                                Thread.Sleep(1000);
                            }
                            //ParnetSearchElemntTree = TreeObj;
                            break;
                        }
                    }
                    else if(string.Compare(ParentIdentityType ,"automationid")==0)
                    {
                        if (string.Compare(TreeParentObj.Current.AutomationId, ParentIdentity) == 0)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeParentObj.Current.AutomationId + " found in tree ", "info");
                            ParentSearchElemnt = TreeParentObj;
                            ExpandCollapsePattern expPattern = ParentSearchElemnt.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                            NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");

                            if (string.Compare(expPattern.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                            {
                                expPattern.Expand();
                                Thread.Sleep(1000);
                            }
                           // ParnetSearchElemntTree = TreeObj;
                            break;
                        }
                    }
                    
                }
                //if (ParentSearchElemnt != null)
                //{
                //    break;
                //}

            }

            // Check in parent tree obj, if sch an element exist
            //if (ParnetSearchElemntTree == null)
            if (ParentSearchElemnt== null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Exiting application from ChkTreeForElementAndClickIfReqd ", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Resuming as terminateOnFailure is no  ", "warn");
                return null;
            }
            //AutomationElement SerachElementFound = GuiObj.CheckTreeForElementAndClickIfReqd(ParnetSearchElemntTree, ChildIdentityType, ChildIdentity, ClickRequired, MouseBtn, LogFilePath);
            AutomationElement SerachElementFound = GuiObj.CheckTreeForElementAndClickIfReqd(ParentSearchElemnt, ChildIdentityType, ChildIdentity, ClickRequired, MouseBtn,TerminateStatus, LogFilePath);
            if (SerachElementFound == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element SearchElemnt with " + ChildIdentityType + " - " + ChildIdentity + ". Exiting application from ChkTreeForElementAndClickIfReqd ", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element SearchElemnt with " + ChildIdentityType + " - " + ChildIdentity + ". Resuming as terminateOnFailure is no  ", "warn");
                return null;
            }
            
            return SerachElementFound;

        }

        public AutomationElement ChkTreeForElementAndExpandIfReqd(AutomationElement ParentObj, string ParentIdentityType, string ParentIdentity, string ChildIdentityType, string ChildIdentity, int ExpansionRequired, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "ChkTreeForElementAndExpandIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            //Look for an element of type ControlType.Tree in the parent object
            PropertyCondition TreeCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Tree", 1, LogFilePath);

            //  There could be multiple trees in the parent obj
            // Get all the trees, and find if the parent elemnt  in each tree matches with the  ParentIdentity supplied
            AutomationElementCollection NodeColl = ParentObj.FindAll(TreeScope.Descendants, TreeCondition);
            AutomationElement ParentSearchElemnt = null;
            AutomationElement ParnetSearchElemntTree = null;
            foreach (AutomationElement TreeObj in NodeColl)
            {
                //Check if the TreeObj is the parent element
                //Users could supply a "Controltype.tree" or "Controltype.treeitem as parent"

                if (string.Compare(ParentIdentityType, "name") == 0)
                {
                    if (string.Compare(TreeObj.Current.Name, ParentIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeObj.Current.Name + " found in tree ", "info");
                        ParentSearchElemnt = TreeObj;
                        ParnetSearchElemntTree = TreeObj;
                        break;
                    }
                }
                else if (string.Compare(ParentIdentityType, "automationid") == 0)
                {
                    if (string.Compare(TreeObj.Current.AutomationId, ParentIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeObj.Current.AutomationId + " found in tree ", "info");
                        ParentSearchElemnt = TreeObj;
                        ParnetSearchElemntTree = TreeObj;
                        break;
                    }
                }
                // In each tree, check the 1st element name (user has provided the tree parent element name & this should be the 1st element)
                PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                //User should have supplied the parent tree eemnt name, which would be the 1st item
                //AutomationElement TreeParentObj = TreeObj.FindFirst(TreeScope.Descendants, TreeItemCondition);
                AutomationElementCollection TreeParentObjColl = TreeObj.FindAll(TreeScope.Descendants, TreeItemCondition);
                foreach (AutomationElement TreeParentObj in TreeParentObjColl)
                {
                    //AutomationElement TreeParentObj=TreeObj.FindFirst(TreeScope.Descendants,TreeItemCondition);

                    if (string.Compare(ParentIdentityType, "name") == 0)
                    {
                        if (string.Compare(TreeParentObj.Current.Name, ParentIdentity) == 0)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeParentObj.Current.Name + " found in tree ", "info");
                            ParentSearchElemnt = TreeParentObj;
                            ParnetSearchElemntTree = TreeObj;
                            break;
                        }
                    }
                    else if (string.Compare(ParentIdentityType, "automationid") == 0)
                    {
                        if (string.Compare(TreeParentObj.Current.AutomationId, ParentIdentity) == 0)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeParentObj.Current.AutomationId + " found in tree ", "info");
                            ParentSearchElemnt = TreeParentObj;
                            ParnetSearchElemntTree = TreeObj;
                            break;
                        }
                    }

                }
            }

            // Check in parent tree obj, if sch an element exist
            if (ParnetSearchElemntTree == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element  ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Exiting application from ChkTreeForElementAndClickIfReqd ", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Resuming as terminateOnFailure is no  ", "warn");
                return null;
            }
            AutomationElement SerachElementFound = GuiObj.CheckTreeForElementAndExpandIfReqd(ParnetSearchElemntTree, ChildIdentityType, ChildIdentity, ExpansionRequired, LogFilePath);

            return SerachElementFound;

        }


        // Give the index of the item to select. Can be used when the tree items does not have any autoid or name to distinguish them
        public AutomationElement SelectItemFromTreeBasedOnIndex(AutomationElement ParentObj, string ParentIdentityType, string ParentIdentity,int ParentIndexIfDuplicatesExist, int ChildIndex,int ClickRequired, string MouseBtn, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromTreeBasedOnIndex", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            //Look for an element of type ControlType.Tree in the parent object
            PropertyCondition TreeCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Tree", 1, LogFilePath);

            //  There could be multiple trees in the parent obj
            // Get all the trees, and find if the parent elemnt  in each tree matches with the  ParentIdentity supplied
            AutomationElementCollection NodeColl = ParentObj.FindAll(TreeScope.Descendants, TreeCondition);
            AutomationElement ParentSearchElemnt = null;
            AutomationElement ParnetSearchElemntTree = null;
            if (string.Compare(ParentIdentityType, "controltype")==0)
            {
                ParentIdentity = ParentIdentity.ToLower();
                if (string.Compare(ParentIdentity, "tree") == 0)
                {
                    if (NodeColl.Count > ParentIndexIfDuplicatesExist)
                    {
                        ParentSearchElemnt = NodeColl[ParentIndexIfDuplicatesExist];
                        ParnetSearchElemntTree = NodeColl[ParentIndexIfDuplicatesExist];
                    }
                }
                if (string.Compare(ParentIdentity, "treeitem") == 0)
                {
                    if (NodeColl.Count == 1)
                    {
                        ParentSearchElemnt = NodeColl[ParentIndexIfDuplicatesExist + 1];
                        PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                        AutomationElementCollection ParentNodeColl = ParentObj.FindAll(TreeScope.Descendants, TreeItemCondition);
                        ParnetSearchElemntTree = ParentNodeColl[ParentIndexIfDuplicatesExist + 1];
                    }
                    else
                    {
                        //May have to change this logic, when there are producst ahving more than 1 tree exists
                        ParentSearchElemnt = NodeColl[0];
                        PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                        AutomationElementCollection ParentNodeColl = ParentObj.FindAll(TreeScope.Descendants, TreeItemCondition);
                        ParnetSearchElemntTree = ParentNodeColl[ParentIndexIfDuplicatesExist + 1];
                    }
                }

            }
            else
            {
                foreach (AutomationElement TreeObj in NodeColl)
                {
                    //Check if the TreeObj is the parent element
                    //Users could supply a "Controltype.tree" or "Controltype.treeitem as parent"

                    if (string.Compare(ParentIdentityType, "name") == 0)
                    {
                        if (string.Compare(TreeObj.Current.Name, ParentIdentity) == 0)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeObj.Current.Name + " found in tree ", "info");
                            ParentSearchElemnt = TreeObj;
                            ParnetSearchElemntTree = TreeObj;
                            break;
                        }
                    }
                    else if (string.Compare(ParentIdentityType, "automationid") == 0)
                    {
                        if (string.Compare(TreeObj.Current.AutomationId, ParentIdentity) == 0)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeObj.Current.AutomationId + " found in tree ", "info");
                            ParentSearchElemnt = TreeObj;
                            ParnetSearchElemntTree = TreeObj;
                            break;
                        }
                    }
                }
            }
                
            // Check in parent tree obj, if sch an element exist
            if (ParnetSearchElemntTree == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element  ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Exiting application from SelectItemFromTreeBasedOnIndex ", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Resuming as terminateOnFailure is no  ", "warn");
                return null;
            }
            AutomationElement SerachElementFound = GuiObj.SelectItemFromTreeBasedOnIndex(ParnetSearchElemntTree, ChildIndex, ClickRequired, MouseBtn, LogFilePath);
            if (SerachElementFound == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element  SearchElemntTree with " + ChildIndex +" Exiting application from SelectItemFromTreeBasedOnIndex ", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element  SearchElemntTree with " + ChildIndex +" Exiting application from SelectItemFromTreeBasedOnIndex"+". Resuming as terminateOnFailure is no  ", "warn");
                return null;
            }
            return SerachElementFound;

        }

        public void ChkTreeIfElementIsLeafNodeAndTerminateIfReqd(AutomationElement ParentObj, string ParentIdentityType, string ParentIdentity, string ElemntNameToClick, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "ChkTreeIfElementIsLeafNodeAndClickIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            //Look for an element of type ControlType.Tree in the parent object
            PropertyCondition TreeCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Tree", 1, LogFilePath);

            //  There could be multiple trees in the parent obj
            // Get all the trees, and find if the parent elemnt  in each tree matches with the  ParentIdentity supplied
            AutomationElementCollection NodeColl = ParentObj.FindAll(TreeScope.Descendants, TreeCondition);
            AutomationElement ParentSearchElemnt = null;
            AutomationElement ParnetSearchElemntTree = null;
            foreach (AutomationElement TreeObj in NodeColl)
            {
                // In each tree, check the 1st element name (user has provided the tree parent element name & this should be the 1st element)
                PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                
                AutomationElement TreeParentObj = TreeObj.FindFirst(TreeScope.Descendants, TreeItemCondition);

                if (ParentIdentityType == "name")
                {
                    if (string.Compare(TreeParentObj.Current.Name, ParentIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeParentObj.Current.Name + " found in tree ", "info");
                        ParentSearchElemnt = TreeParentObj;
                        ParnetSearchElemntTree = TreeObj;
                        break;
                    }
                }
                else if (ParentIdentityType == "automationid")
                {
                    if (string.Compare(TreeParentObj.Current.AutomationId, ParentIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Search element " + TreeParentObj.Current.AutomationId + " found in tree ", "info");
                        ParentSearchElemnt = TreeParentObj;
                        ParnetSearchElemntTree = TreeObj;
                        break;
                    }
                }
                
            }

            // Check in parent tree obj, if sch an element exist
            if (ParnetSearchElemntTree == null)
            {
               // NewLogObj.WriteLogFile(LogFilePath, "Unable to find the ParnetSearchElemntTree", "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Exiting application from ChkTreeIfElementIsLeafNodeAndTerminateIfReqd ", "fail");
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element ParnetSearchElemntTree with " + ParentIdentityType + " - " + ParentIdentity + ". Resuming as terminateOnFailure is no  ", "warn");

            }
            int SearchStatus = GuiObj.CheckTreeIfElementIsLeafNodeAndTerminateIfReqd(ParnetSearchElemntTree, ElemntNameToClick, TerminateStatus, LogFilePath);
        }
        public int StartProcess(string ExeFullPath, string ProcessName,string KillCurrentRunningProcess)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            FileOperations FileObj = new FileOperations();
            NewLogObj.WriteLogFile(LogFilePath, "StartProcess", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============", "info");

            if (!File.Exists(ExeFullPath))
            {
                NewLogObj.WriteLogFile(LogFilePath, ExeFullPath+" does not exist", "fail");
                Console.WriteLine(ExeFullPath + " does not exist");
                FileObj.ExitTestEnvironment();

            }
            string Pattern="."+"exe"+"$";
            
            //if process name has ".exe" in it remove
            if (Regex.IsMatch(ProcessName, Pattern))
            {
                ProcessName = Regex.Replace(ProcessName, Pattern, "");
                
            }
            Process[] pname = Process.GetProcessesByName(ProcessName);
            //************************To be uncommented********************************
            if (pname.Length != 0)
            {
                if (KillCurrentRunningProcess != null)
                {
                    KillCurrentRunningProcess = KillCurrentRunningProcess.ToLower();
                    if (string.Compare(KillCurrentRunningProcess, "yes") == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, ProcessName + " already running. Killing the process", "info");
                        Generic NewGenericObj = new Generic();
                        NewGenericObj.KillProcess(ProcessName, LogFilePath);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, ProcessName + " already running. Attching to process", "info");
                        return 0;
                    }
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ProcessName + " already running. Attching to process", "info");
                    return 0;
                }
            }
            //************************To be uncommented***********************************
            System.Diagnostics.Process.Start(ExeFullPath);
            int WaitTimer = 0; int WaitTimeOut = 60000; //1 min
            Thread.Sleep(2000);
            pname = Process.GetProcessesByName(ProcessName);
            if (pname.Length == 0 && WaitTimer <= WaitTimeOut)
            {
                NewLogObj.WriteLogFile(LogFilePath, "waiting for process to launch", "info");
                Thread.Sleep(2000);
                WaitTimer = WaitTimer + 2000;
                pname = Process.GetProcessesByName(ProcessName);
                if (pname.Length != 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, ProcessName + " launched successfully", "info");
                    return 0;
                }
            }
            if (pname.Length == 0 && WaitTimer >= WaitTimeOut)
            {
                NewLogObj.WriteLogFile(LogFilePath, ProcessName + " launcheing timeout", "fail");
                return 0;
            }
            if (pname.Length != 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, ProcessName + " launched successfully", "info");
                return 0;
            }
            return 0;
        }

        public void StartTest(string Testname)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.CreateLogFolder();
            string LogFilePath = NewLogObj.CreateLogFile();
            NewLogObj.WriteLogFile(LogFilePath, "Test : "+ Testname, "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============", "info");
            TestInputFileParsing FileParseObj = new TestInputFileParsing();
            FileParseObj.PrerequisitesValidation();
            FileParseObj.ValidateTestInputFile(Testname+".txt");
        }

       // public AutomationElement WaitTillContrilIsActive(string ParamType, string ParamValue, string ParentWindowIdentifierType, string ParentWindowIdentifier, int TimeOutInSecs, int TerminateStatus)
        //public AutomationElement WaitTillContrilIsActive(string ParamType, string ParamValue, int TimeOutInSecs, int TerminateStatus)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();

        //    AutomationElementIdentity GuiObj = new AutomationElementIdentity();

        //    NewLogObj.WriteLogFile(LogFilePath, "WaitTillContrilIsActive", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            

        //    //Tried to find the container window 1st
        //    PropertyCondition WindowCondition = SetParamTypeBasedPropertyCondition(ParentWindowIdentifierType, ParentWindowIdentifier);
        //    AutomationElement ParentObj = AutomationElement.RootElement.FindFirst(TreeScope.Children, WindowCondition);
        //    int timer = 0;
        //    if (ParentObj == null && timer < TimeOutInSecs)
        //    {
        //        Thread.Sleep(2000);
        //        timer = timer + 2000;
        //        ParentObj = AutomationElement.RootElement.FindFirst(TreeScope.Children, WindowCondition);
        //    }
        //    if (ParentObj == null && timer >= TimeOutInSecs)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for ParentObj in WaitTillContrilIsActive", "fail");
        //        if (TerminateStatus == 1)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Exiting application..", "fail");
        //            FileOperations FileObj = new FileOperations();
        //            FileObj.ExitTestEnvironment();
        //            return null;
        //        }
        //        return null;
        //    }
        //    if (ParentObj != null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Parent Obj found", "info");
        //        ParamType = ParamType.ToLower();

        //        PropertyCondition SerachCntrlCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);

        //        AutomationElement SearchObj = GuiObj.FindAutomationElement(ParentObj, SerachCntrlCondition, TreeScope.Descendants, "Window ParamValue", TerminateStatus, LogFilePath);
        //        timer = 0;
        //        while (SearchObj == null && timer < TimeOutInSecs)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "SearchObj is null.. Waiting for SearchObj " + ParamValue, "info");
        //            Thread.Sleep(2000);
        //            timer = timer + 1000;
        //            Console.WriteLine("Waiting for the control to be active");
        //            NewLogObj.WriteLogFile(LogFilePath, "Waiting for the control to be active", "info");
        //            ParentObj = AutomationElement.RootElement.FindFirst(TreeScope.Children, WindowCondition);
        //            SearchObj = GuiObj.FindAutomationElement(ParentObj, SerachCntrlCondition, TreeScope.Descendants, "Window ParamValue", TerminateStatus, LogFilePath);
        //        }
        //        if (SearchObj == null && timer >= TimeOutInSecs)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for ParentObj in WaitTillContrilIsActive", "fail");
        //            if (TerminateStatus == 1)
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "Exiting application..", "fail");
        //                FileOperations FileObj = new FileOperations();
        //                FileObj.ExitTestEnvironment();
        //                return null;
        //            }
        //            return null;
        //        }
        //        NewLogObj.WriteLogFile(LogFilePath, "SearchObj found ", "info");
        //        timer = 0;
        //        while (!SearchObj.Current.IsEnabled && timer<TimeOutInSecs)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Waiting for SearchObj to be enabled ", "info");
        //            Thread.Sleep(2000);
        //            if (SearchObj.Current.IsEnabled)
        //            {
        //                break;
        //            }
        //            timer++;
        //        }

        //        if (!SearchObj.Current.IsEnabled && timer >= TimeOutInSecs)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Timeout Waiting for SearchObj to be enabled ", "fail");
        //            if (TerminateStatus == 1)
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "Exiting application..", "fail");
        //                FileOperations FileObj = new FileOperations();
        //                FileObj.ExitTestEnvironment();
        //                return null;
        //            }
        //            return null;
        //        }
        //        return ParentObj;
        //    }
        //    return null;
        //}

        public AutomationElement WaitTillContrilIsActive(AutomationElement ParentObj, string ParamType,string ParamValue, int Index,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "WaitTillContrilIsActive", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");

            if (ParentObj != null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Parent Obj found", "info");
                ParamType = ParamType.ToLower();

                //PropertyCondition SerachCntrlCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
                List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
                BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
                PropertyCondition BtnObCondition = BtnObjConditionList[0];
                PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

                KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
                AutomationElement SearchObj = null;
                if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
                {
                    if (Index == 0)
                    {
                        SearchObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                    else
                    {
                        SearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                }
                else
                {
                    if (Index == 0)
                    {
                        SearchObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                    else
                    {
                        SearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                }

                
                NewLogObj.WriteLogFile(LogFilePath, "SearchObj found ", "info");
                if (SearchObj == null)
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for control with" + ParamType + " - " + ParamValue + " in WaitTillContrilIsActive. Exiting application..", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for control with" + ParamType + " - " + ParamValue + " in WaitTillContrilIsActive. Resuming as terminateOnFailure is no ", "warn");
                    return null;

                }
                int timer = 0;
                while (!SearchObj.Current.IsEnabled && timer < TimeOutInSecs)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Waiting for SearchObj to be enabled ", "info");
                    Thread.Sleep(2000);
                    if (SearchObj.Current.IsEnabled)
                    {
                        //break;
                        return ParentObj;
                    }
                    timer=timer+2000;
                }

                if (!SearchObj.Current.IsEnabled && timer >= TimeOutInSecs)
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Timeout Waiting for control " + ParamType + " - " + ParamValue + " to be enabled. Exiting application..", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout Waiting for control " + ParamType + " - " + ParamValue + " to be enabled. Resuming as terminateOnFailure is no", "warn");
                    return null;
                }
                return ParentObj;
            }
            return null;
        }

        //Params -  SelectItemFromDataGridView 
        //ItemVal - can be in any of below formats
        // 2 
        //Row 2
        //Row2
         //SearchWithIndex - set this as 1, is grid has to be serached based on index
        //if this is anything other than 1, grid will be searched based on item name mantioned in ItemVal
        public AutomationElement SelectItemFromDataGridView(AutomationElement ParentObj, string ParamType, string ParamValue, int SearchWithIndex,string ItemVal,int CheckBoxToBeChecked,int Index,int GetColumnValueFlag,int ColumNum,string ExpectedColumnValue,int ClickOnFirstColumn,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition GridCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement GridObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (GridObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + ParamType + " - " + ParamValue + " in SelectItemFromDataGrid.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + ParamType + " - " + ParamValue + " in SelectItemFromDataGrid.Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            if (SearchWithIndex == 1)
            {
                // Regex RowPattern1=new Regex("[^(a-zA-Z)+(\\s)*(?<RowCount>(0-9)+)$]"); //row2,Row2,row 2
                Regex RowPattern1 = new Regex("^(row)?(\\s)*(?<RowCount>([0-9])+)$", RegexOptions.IgnoreCase); //row2,Row2,row 2,2
                Match match = RowPattern1.Match(ItemVal);
                string TmpRowIndex;
                int RowIndex;
                if (match.Success)
                {
                    TmpRowIndex = match.Groups["RowCount"].Value;
                    Int32.TryParse(TmpRowIndex, out RowIndex);
                    if (GetColumnValueFlag == 1)
                    {
                        GuiObj.SelectItemFromDataGridWithIndexAndExtractColumnValue(GridObj, RowIndex, ColumNum,ExpectedColumnValue, 1, TerminateStatus);
                        return GridObj;
                    }
                    if (ClickOnFirstColumn == 1)
                    {
                        GuiObj.SelectItemFromDataGridWithIndexAndClickOnFirstColumn(GridObj, RowIndex, TerminateStatus);
                        return GridObj;
                    }
                    if (CheckBoxToBeChecked == 0)
                    {
                        GuiObj.SelectItemFromDataGridWithIndex(GridObj, RowIndex, CheckBoxToBeChecked, TerminateStatus);
                        return GridObj;
                    }
                    else
                    {
                        GuiObj.SelectItemFromDataGridWithIndexAndCheckCheckBoxIfReqd(GridObj, RowIndex, CheckBoxToBeChecked,"Left", TerminateStatus);
                        return GridObj;
                    }
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Rowindex mentioned not in proper fromat from SelectItemFromDataGridView.Exiting application..", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Rowindex mentioned not in proper fromat from SelectItemFromDataGridView.Resuming as terminateOnFailure is no", "warn");
                }
            }
            else
            {
                GuiObj.SelectItemFromDataGridWithNameAndCheckCheckBoxIfReqd(GridObj, ItemVal, CheckBoxToBeChecked, TerminateStatus);
            }
            
            return GridObj;

        }

        public AutomationElement SelectItemFromDataGridView(AutomationElement ParentObj, string ParamType, string ParamValue, int SearchWithIndex, string ItemVal, int DuplicatesIndex, string ClickMouseBtn, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition GridCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement GridObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (DuplicatesIndex == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", DuplicatesIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (DuplicatesIndex == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", DuplicatesIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (GridObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + ParamType + " - " + ParamValue + " in SelectItemFromDataGrid.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + ParamType + " - " + ParamValue + " in SelectItemFromDataGrid.Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            if (SearchWithIndex == 1)
            {
                // Regex RowPattern1=new Regex("[^(a-zA-Z)+(\\s)*(?<RowCount>(0-9)+)$]"); //row2,Row2,row 2
                Regex RowPattern1 = new Regex("^(row)?(\\s)*(?<RowCount>([0-9])+)$", RegexOptions.IgnoreCase); //row2,Row2,row 2,2
                Match match = RowPattern1.Match(ItemVal);
                string TmpRowIndex;
                int RowIndex;
                if (match.Success)
                {
                    TmpRowIndex = match.Groups["RowCount"].Value;
                    Int32.TryParse(TmpRowIndex, out RowIndex);
                    GuiObj.SelectItemFromDataGridWithIndexAndCheckCheckBoxIfReqd(GridObj, RowIndex, 0, ClickMouseBtn, TerminateStatus);
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Rowindex mentioned not in proper fromat from SelectItemFromDataGridView.Exiting application..", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Rowindex mentioned not in proper fromat from SelectItemFromDataGridView.Resuming as terminateOnFailure is no", "warn");
                }
            }
            else
            {
                GuiObj.SelectItemFromDataGridWithNameAndCheckCheckBoxIfReqd(GridObj, ItemVal, 0, TerminateStatus);
            }

            return GridObj;

        }

        public AutomationElement CheckItemPresentInDataGridview(AutomationElement ParentObj, string DataGridParamType, string DataGridParamValue, int DuplicatesIndex, int ColumNum, string ExpectedColumnValue,int CheckCheckbox,int ClickRequired,string ClickMouseBn, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckItemPresentInDataGridview", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            DataGridParamType = DataGridParamType.ToLower();
            //PropertyCondition GridCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(DataGridParamType, DataGridParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement GridObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(DataGridParamType, "name", RegexOptions.IgnoreCase))
            {
                if (DuplicatesIndex == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", DuplicatesIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (DuplicatesIndex == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", DuplicatesIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (GridObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + DataGridParamType + " - " + DataGridParamValue + " in SelectItemFromDataGrid.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + DataGridParamType + " - " + DataGridParamValue + " in SelectItemFromDataGrid.Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            GuiObj.CheckDataGridColumnsForAnItemAndSelect(GridObj, ColumNum, ExpectedColumnValue,CheckCheckbox, ClickRequired, ClickMouseBn, TerminateStatus);
            return GridObj;
        }

        public AutomationElement CheckPatternPresentInDataGridviewAndExtract(AutomationElement ParentObj, string DataGridParamType, string DataGridParamValue, int DuplicatesIndex, int ColumNum, string SearchPattern, int CheckCheckbox, int ClickRequired, string ClickMouseBn, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckPatternPresentInDataGridviewAndExtract", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==========================================", "info");
            DataGridParamType = DataGridParamType.ToLower();
            //PropertyCondition GridCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(DataGridParamType, DataGridParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement GridObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(DataGridParamType, "name", RegexOptions.IgnoreCase))
            {
                if (DuplicatesIndex == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", DuplicatesIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (DuplicatesIndex == 0)
                {
                    GridObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    GridObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", DuplicatesIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (GridObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + DataGridParamType + " - " + DataGridParamValue + " in SelectItemFromDataGrid.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for GridObj with " + DataGridParamType + " - " + DataGridParamValue + " in SelectItemFromDataGrid.Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            GuiObj.CheckDataGridColumnsForAPatternExtractAndSelectSame(GridObj, ColumNum, SearchPattern, CheckCheckbox, ClickRequired,ClickMouseBn, TerminateStatus);
            return GridObj;
        }


        public int CheckContainerRawViewIfElementPresent(AutomationElement ParentObj, string ParentParamType, string ParentParamValue, int ParentSelectIndex, string ChildParamType, string ChildValue, string SearchElementExpectedName,int ClickRequired, string ClickMouseBtn,int TimeOutInSecs,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            FileOperations FileObj = new FileOperations();
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "ClickOnElementAtGivenPosition", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParentParamType = ParentParamType.ToLower();
            //PropertyCondition ParentSearchCondition = SetParamTypeBasedPropertyCondition(ParentParamType, ParentParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParentParamType, ParentParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ParentSearchObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentParamType, "name", RegexOptions.IgnoreCase))
            {
                if (ParentSelectIndex == 0)
                {
                    ParentSearchObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentSearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", ParentSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ParentSelectIndex == 0)
                {
                    ParentSearchObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentSearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", ParentSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (ParentSearchObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj with" + ParentParamType + " - " + ParentParamValue + " in ClickOnElementAtBeginning. Exiting application..", "fail");
                    
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj with" + ParentParamType + " - " + ParentParamValue + " in ClickOnElementAtBeginning.Resuming as terminateOnFailure is no", "warn");
                return -1;
            }
            PropertyCondition ElementSearchCondition = null;
            List<PropertyCondition> ElementSearchConditionList = new List<PropertyCondition>();
            if (Regex.IsMatch(ChildParamType, "automationid", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("automationid", ChildValue);
            }
            else if (Regex.IsMatch(ChildParamType, "name", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("name", ChildValue);
            }
            else if (Regex.IsMatch(ChildParamType, "controltype", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("controltype", ChildValue);
            }
            else if (Regex.IsMatch(ChildParamType, "class", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("class", ChildValue);
            }

            if (ElementSearchConditionList[0] == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "ElementSearchCondition is null " + ChildParamType + " - " + ChildValue + " at ClickOnElementAtBeginning. Exiting application..", "fail");
                    
                    FileObj.ExitTestEnvironment();

                }
                NewLogObj.WriteLogFile(LogFilePath, "ElementSearchCondition is null " + ChildParamType + " - " + ChildValue + " at ClickOnElementAtBeginning. Resuming as terminateOnFailure is no.", "warn");
                return -1;

            }
            AutomationElementCollection ChildrenCollection = ParentSearchObj.FindAll(TreeScope.Descendants, ElementSearchConditionList[0]);
            foreach (AutomationElement ChildObj in ChildrenCollection)
            {
                AutomationElement Status = GuiObj.CheckInRawViewIfExpectedElementIsPresent(ChildObj, SearchElementExpectedName,ClickRequired,ClickMouseBtn);
                if (Status !=null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, SearchElementExpectedName + "found in  ChildObj " + ChildObj.Current.Name, "pass");
                    return 1;
                }
            }
            
            if (TerminateStatus == 1)
            {
                NewLogObj.WriteLogFile(LogFilePath, SearchElementExpectedName + " not found. Exiting ", "fail");
                FileObj.ExitTestEnvironment();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, SearchElementExpectedName + " not found. Resuming as TerminateOnFailure is no ", "warn");
            }
            return -1;   

        }



        //Use this if click has to be done on an object inside a contauner
        //Position - can be left,right or middle
        public AutomationElement ClickOnElementAtGivenPosition(AutomationElement ParentObj, string ParentParamType, string ParentParamValue,int ParentSelectIndex, string ChildParamType,string ChildValue,string Position, int ChildSelectIndex,string ClickMouseBtn,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "ClickOnElementAtGivenPosition", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParentParamType = ParentParamType.ToLower();
            //PropertyCondition ParentSearchCondition = SetParamTypeBasedPropertyCondition(ParentParamType, ParentParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParentParamType, ParentParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ParentSearchObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentParamType, "name", RegexOptions.IgnoreCase))
            {
                if (ParentSelectIndex == 0)
                {
                    ParentSearchObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentSearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "List ParamValue", ParentSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ParentSelectIndex == 0)
                {
                    ParentSearchObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentSearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "List", ParentSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            
            if (ParentSearchObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj with" + ParentParamType + " - " + ParentParamValue + " in ClickOnElementAtBeginning. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();

                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj with" + ParentParamType + " - " + ParentParamValue + " in ClickOnElementAtBeginning.Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            PropertyCondition ElementSearchCondition=null;
            List<PropertyCondition> ElementSearchConditionList = new List<PropertyCondition>();
            if(Regex.IsMatch(ChildParamType,"automationid",RegexOptions.IgnoreCase))
            {
               ElementSearchConditionList= SetParamTypeBasedPropertyCondition("automationid", ChildValue);
            }
            else if (Regex.IsMatch(ChildParamType, "name", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("name", ChildValue);
            }
            else if (Regex.IsMatch(ChildParamType, "controltype", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("controltype", ChildValue);
            }
            else if (Regex.IsMatch(ChildParamType, "class", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("class", ChildValue);
            }

            if (ElementSearchConditionList[0] == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "ElementSearchCondition is null " + ChildParamType + " - " + ChildValue + " at ClickOnElementAtBeginning. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();

                }
                NewLogObj.WriteLogFile(LogFilePath, "ElementSearchCondition is null " + ChildParamType + " - " + ChildValue + " at ClickOnElementAtBeginning. Resuming as terminateOnFailure is no.", "warn");
                return null;

            }
            //AutomationElement ChildSearchObj = GuiObj.FindAutomationElement(ParentSearchObj, ElementSearchCondition, TreeScope.Descendants, "Window ParamValue", TerminateStatus, LogFilePath);
            AutomationElement ChildSearchObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ChildParamType, "name", RegexOptions.IgnoreCase))
            {
                if (ChildSelectIndex == 0)
                {
                    ChildSearchObj = GuiObj.FindAutomationElement(ParentSearchObj, ElementSearchConditionList[0], ElementSearchConditionList[1], TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChildSearchObj = GuiObj.FindAutomationElementWithIndex(ParentSearchObj, ElementSearchConditionList[0], ElementSearchConditionList[1], TreeScope.Descendants, "List ParamValue", ChildSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ChildSelectIndex == 0)
                {
                    ChildSearchObj = GuiObj.FindAutomationElement(ParentSearchObj, ElementSearchConditionList[0], TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChildSearchObj = GuiObj.FindAutomationElementWithIndex(ParentSearchObj, ElementSearchConditionList[0], TreeScope.Descendants, "List", ChildSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            
            if (ChildSearchObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj " + ChildParamType + " - " + ChildValue + " in ClickOnElementAtBeginning. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();

                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj " + ChildParamType + " - " + ChildValue + " in ClickOnElementAtBeginning. Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            
            if (string.Compare(Position, "Left") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(ChildSearchObj, LogFilePath, ClickMouseBtn, "Left", 10);
            }
            else if (string.Compare(Position, "Right") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(ChildSearchObj, LogFilePath, ClickMouseBtn, "Right", 10);
            }
            else if (string.Compare(Position, "Middle") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClick(ChildSearchObj, LogFilePath, ClickMouseBtn);
            }
            return ChildSearchObj;
        }

        //Use this if click has to be done on an object(not  inside a container)
        public AutomationElement ClickOnElementAtGivenPosition(AutomationElement ParentObj, string ParamType, string ParamValue, string Position, int ChildSelectIndex, string ClickMouseBtn,int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "ClickOnElementAtGivenPosition", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            PropertyCondition ElementSearchCondition = null;
            List<PropertyCondition> ElementSearchConditionList = new List<PropertyCondition>();
            if (Regex.IsMatch(ParamType, "automationid", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("automationid", ParamValue);
            }
            else if (Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("name", ParamValue);
            }
            else if (Regex.IsMatch(ParamType, "controltype", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("controltype", ParamValue);
            }
            else if (Regex.IsMatch(ParamType, "class", RegexOptions.IgnoreCase))
            {
                ElementSearchConditionList = SetParamTypeBasedPropertyCondition("class", ParamValue);
            }

            if (ElementSearchConditionList[0] == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "ElementSearchCondition with " + ParamType + " - " + ParamValue + " is null at ClickOnElementAtGivenPosition.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "ElementSearchCondition with " + ParamType + " - " + ParamValue + " is null at ClickOnElementAtGivenPosition.Resuming as terminateOnFailure is no", "warn");
                return null;

            }
            //AutomationElement ChildSearchObj = GuiObj.FindAutomationElement(ParentSearchObj, ElementSearchCondition, TreeScope.Descendants, "Window ParamValue", TerminateStatus, LogFilePath);
            AutomationElement ChildSearchObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (ChildSelectIndex == 0)
                {
                    ChildSearchObj = GuiObj.FindAutomationElement(ParentObj, ElementSearchConditionList[0], ElementSearchConditionList[1], TreeScope.Descendants, "List ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChildSearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, ElementSearchConditionList[0], ElementSearchConditionList[1], TreeScope.Descendants, "List ParamValue", ChildSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ChildSelectIndex == 0)
                {
                    ChildSearchObj = GuiObj.FindAutomationElement(ParentObj, ElementSearchConditionList[0], TreeScope.Descendants, "List", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ChildSearchObj = GuiObj.FindAutomationElementWithIndex(ParentObj, ElementSearchConditionList[0], TreeScope.Descendants, "List", ChildSelectIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (ChildSearchObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj with " + ParamType + " - " + ParamValue + " in ClickOnElementAtGivenPosition. Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for SearchObj with " + ParamType + " - " + ParamValue + " in ClickOnElementAtGivenPosition. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            if (string.Compare(Position, "Left") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(ChildSearchObj, LogFilePath, ClickMouseBtn, "Left", 10);
            }
            else if (string.Compare(Position, "Right") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(ChildSearchObj, LogFilePath, ClickMouseBtn, "Right", 10);
            }
            else if (string.Compare(Position, "Middle") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClick(ChildSearchObj, LogFilePath, ClickMouseBtn);
            }
            return ChildSearchObj;
        }

        //Use this, if click has to be done on an object which is already identified
        public AutomationElement ClickOnElementAtGivenPosition(AutomationElement ElementToClick, string Position, string ClickMouseBtn, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "ClickOnElementAtGivenPosition", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            
            if (ElementToClick == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "ElementToClick is null in ClickOnElementAtGivenPosition.Exiting application..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();

                }
                NewLogObj.WriteLogFile(LogFilePath, "ElementToClick is null in ClickOnElementAtGivenPosition. Resuming as terminateOnFailure is no..", "warn");
                return null;
            }
            if (string.Compare(Position, "Left") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(ElementToClick, LogFilePath, ClickMouseBtn, "Left", 10);
            }
            else if (string.Compare(Position, "Right") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(ElementToClick, LogFilePath, ClickMouseBtn, "Right", 10);
            }
            else if (string.Compare(Position, "Middle") == 0)
            {
                GuiObj.GetPositionFromBoundingRectangleAndClick(ElementToClick, LogFilePath, ClickMouseBtn);
            }
            return ElementToClick;
        }

        public AutomationElement FindElementInsideParentContainer(AutomationElement ParentWindowObj, string ParentCntrType, string ParentCntrValue, int ParentCntrDuplicateIndex, string ElementParamType, string ElementParamValue, int ElementDuplicateIndex, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "FindElementInsideParentContainer", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=======================================", "info");
            ParentCntrType = ParentCntrType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> ParentObjConditionList = new List<PropertyCondition>();
            ParentObjConditionList = SetParamTypeBasedPropertyCondition(ParentCntrType, ParentCntrValue);
            PropertyCondition ParentCondition = ParentObjConditionList[0];
            PropertyCondition ParentCondition1 = ParentObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ParentCntrObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentCntrType, "name", RegexOptions.IgnoreCase))
            {
                if (ParentCntrDuplicateIndex == 0)
                {
                    ParentCntrObj = GuiObj.FindAutomationElement(ParentWindowObj, ParentCondition, ParentCondition1, TreeScope.Descendants, ParentCntrValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentCntrObj = GuiObj.FindAutomationElementWithIndex(ParentWindowObj, ParentCondition, ParentCondition1, TreeScope.Descendants, ParentCntrValue, ParentCntrDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ParentCntrDuplicateIndex == 0)
                {
                    ParentCntrObj = GuiObj.FindAutomationElement(ParentWindowObj, ParentCondition, TreeScope.Descendants, ParentCntrValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentCntrObj = GuiObj.FindAutomationElementWithIndex(ParentWindowObj, ParentCondition, TreeScope.Descendants, ParentCntrValue, ParentCntrDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (ParentCntrValue == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Parent cntr Element with " + ParentCntrType + " - " + ParentCntrValue + " does not exists. Exiting application from CheckIfElementExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Parent cntr  Element with " + ParentCntrType + " - " + ParentCntrValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            List<PropertyCondition> ElementObjConditionList = new List<PropertyCondition>();
            ParentObjConditionList = SetParamTypeBasedPropertyCondition(ElementParamType, ElementParamValue);
            PropertyCondition ElementCondition = ElementObjConditionList[0];
            PropertyCondition ElementCondition1 = ElementObjConditionList[1];

            AutomationElement ElementObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentCntrType, "name", RegexOptions.IgnoreCase))
            {
                if (ElementDuplicateIndex == 0)
                {
                    ElementObj = GuiObj.FindAutomationElement(ParentCntrObj, ElementCondition, ElementCondition1, TreeScope.Descendants, ElementParamValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                    return ElementObj;
                }
                else
                {
                    ElementObj = GuiObj.FindAutomationElementWithIndex(ParentCntrObj, ElementCondition, ElementCondition1, TreeScope.Descendants, ElementParamValue, ElementDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                    return ElementObj;
                }
            }
            else
            {
                if (ElementDuplicateIndex == 0)
                {
                    ElementObj = GuiObj.FindAutomationElement(ParentCntrObj, ElementCondition, TreeScope.Descendants, ElementParamValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                    return ElementObj;
                }
                else
                {
                    ElementObj = GuiObj.FindAutomationElementWithIndex(ParentCntrObj, ElementCondition, TreeScope.Descendants, ElementParamValue, ElementDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                    return ElementObj;
                }
            }
            if (ElementObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " does not exists. Exiting application from CheckIfElementExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Parent cntr  Element with " + ElementParamType + " - " + ElementParamValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            return null;
        }
        public AutomationElement CheckIfElementIsActive(AutomationElement ParentObj, string ParamType, string ParamValue, int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementIsActive", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement BtnObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (BtnObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " does not exists. Exiting application from CheckIfElementIsActive..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            //Check if element is enabled
            if (BtnObj.Current.IsEnabled)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " is enabled.", "info");
            }
            else
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " is not enabled. Exiting application from CheckIfElementIsActive..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " is not enabled. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }           
            return null;
        }
        public AutomationElement CheckIfElementIsActive(AutomationElement ParentObj, string ParentCntrType, string ParentCntrValue, int ParentCntrDuplicateIndex, string ElementParamType, string ElementParamValue, int ElementDuplicateIndex, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementIsActive", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            AutomationElement BtnObj = FindElementInsideParentContainer(ParentObj, ParentCntrType, ParentCntrValue, ParentCntrDuplicateIndex, ElementParamType, ElementParamValue, ElementDuplicateIndex, TimeOutInSecs, TerminateStatus);
            if (BtnObj != null)
            {
                //Check if element is enabled
                if (BtnObj.Current.IsEnabled)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " is enabled.", "info");
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " is not enabled. Exiting application from CheckIfElementIsNotActive..", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " is not enabled. Resuming as terminateOnFailure is no.", "warn");
                    return null;
                }
            }
            return null;
        }

        public AutomationElement CheckIfElementIsNotActive(AutomationElement ParentObj, string ParamType, string ParamValue, int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementIsActive", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement BtnObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (BtnObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " does not exists. Exiting application from CheckIfElementIsNotActive..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            //Check if element is enabled
            if (!BtnObj.Current.IsEnabled)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " is not enabled.", "info");
            }
            else
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " is enabled. Exiting application from CheckIfElementIsNotActive..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " is enabled. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            return null;
        }

        public AutomationElement CheckIfElementIsNotActive(AutomationElement ParentObj, string ParentCntrType, string ParentCntrValue, int ParentCntrDuplicateIndex, string ElementParamType, string ElementParamValue, int ElementDuplicateIndex, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementIsNotActive", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            AutomationElement BtnObj = FindElementInsideParentContainer(ParentObj,ParentCntrType,ParentCntrValue,ParentCntrDuplicateIndex,ElementParamType,ElementParamValue,ElementDuplicateIndex,TimeOutInSecs,TerminateStatus);
            if (BtnObj != null)
            {
                //Check if element is enabled
                if (!BtnObj.Current.IsEnabled)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " is not enabled.", "info");
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " is enabled. Exiting application from CheckIfElementIsNotActive..", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " is enabled. Resuming as terminateOnFailure is no.", "warn");
                    return null;
                }
            }
            return null;
        }
        public AutomationElement CheckIfElementExists(AutomationElement ParentObj, string ParamType, string ParamValue, int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementExists", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement BtnObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);

                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);

                }
            }
            else
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);

                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", Index, TimeOutInSecs, TerminateStatus, LogFilePath);

                }
            }

            if (BtnObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with "+ParamType+" - "+ParamValue+" does not exists. Exiting application from CheckIfElementExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue +" exists", "info");
            }
            return null;
            

        }

        public AutomationElement CheckIfElementExists(AutomationElement ParentObj, string ParentCntrType, string ParentCntrValue, int ParentCntrDuplicateIndex, string ElementParamType, string ElementParamValue, int ElementDuplicateIndex, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementExists", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParentCntrType = ParentCntrType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> ParentObjConditionList = new List<PropertyCondition>();
            ParentObjConditionList = SetParamTypeBasedPropertyCondition(ParentCntrType, ParentCntrValue);
            PropertyCondition ParentCondition = ParentObjConditionList[0];
            PropertyCondition ParentCondition1 = ParentObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ParentCntrObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentCntrType, "name", RegexOptions.IgnoreCase))
            {
                if (ParentCntrDuplicateIndex == 0)
                {
                    ParentCntrObj = GuiObj.FindAutomationElement(ParentObj, ParentCondition, ParentCondition1, TreeScope.Descendants, ParentCntrValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentCntrObj = GuiObj.FindAutomationElementWithIndex(ParentObj, ParentCondition, ParentCondition1, TreeScope.Descendants, ParentCntrValue, ParentCntrDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ParentCntrDuplicateIndex == 0)
                {
                    ParentCntrObj = GuiObj.FindAutomationElement(ParentObj, ParentCondition, TreeScope.Descendants, ParentCntrValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentCntrObj = GuiObj.FindAutomationElementWithIndex(ParentObj, ParentCondition, TreeScope.Descendants, ParentCntrValue, ParentCntrDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (ParentCntrValue == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Parent cntr Element with " + ParentCntrType + " - " + ParentCntrValue + " does not exists. Exiting application from CheckIfElementExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Parent cntr  Element with " + ParentCntrType + " - " + ParentCntrValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            List<PropertyCondition> ElementObjConditionList = new List<PropertyCondition>();
            ParentObjConditionList = SetParamTypeBasedPropertyCondition(ElementParamType, ElementParamValue);
            PropertyCondition ElementCondition = ElementObjConditionList[0];
            PropertyCondition ElementCondition1 = ElementObjConditionList[1];

            AutomationElement ElementObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentCntrType, "name", RegexOptions.IgnoreCase))
            {
                if (ElementDuplicateIndex == 0)
                {
                    ElementObj = GuiObj.FindAutomationElement(ParentCntrObj, ElementCondition, ElementCondition1, TreeScope.Descendants, ElementParamValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ElementObj = GuiObj.FindAutomationElementWithIndex(ParentCntrObj, ElementCondition, ElementCondition1, TreeScope.Descendants, ElementParamValue, ElementDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ElementDuplicateIndex == 0)
                {
                    ElementObj = GuiObj.FindAutomationElement(ParentCntrObj, ElementCondition, TreeScope.Descendants, ElementParamValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ElementObj = GuiObj.FindAutomationElementWithIndex(ParentCntrObj, ElementCondition, TreeScope.Descendants, ElementParamValue, ElementDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            if (ElementObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " does not exists. Exiting application from CheckIfElementExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Parent cntr  Element with " + ElementParamType + " - " + ElementParamValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            return null;
        }
        public AutomationElement CheckIfElementDoesNotExists(AutomationElement ParentObj, string ParamType, string ParamValue, int Index, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementDoesNotExists", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==========================", "info");
            ParamType = ParamType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> BtnObjConditionList = new List<PropertyCondition>();
            BtnObjConditionList = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            PropertyCondition BtnObCondition = BtnObjConditionList[0];
            PropertyCondition BtnObCondition1 = BtnObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement BtnObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    //BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, BtnObCondition1, TreeScope.Descendants, "Window ParamValue", TimeOutInSecs, TerminateStatus, LogFilePath);
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", LogFilePath);
                    
                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", LogFilePath);

                }
            }
            else
            {
                if (Index == 0)
                {
                    BtnObj = GuiObj.FindAutomationElement(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", LogFilePath);

                }
                else
                {
                    BtnObj = GuiObj.FindAutomationElementWithIndex(ParentObj, BtnObCondition, TreeScope.Descendants, "Window ParamValue", Index, LogFilePath);

                }
            }

            if (BtnObj != null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " exists. Exiting application from CheckIfElementDoesNotExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue + " exists.Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ParamType + " - " + ParamValue +" does not exists", "info");
            }
            return null;
        }

        public AutomationElement CheckIfElementDoesNotExists(AutomationElement ParentObj, string ParentCntrType, string ParentCntrValue, int ParentCntrDuplicateIndex, string ElementParamType, string ElementParamValue, int ElementDuplicateIndex, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            NewLogObj.WriteLogFile(LogFilePath, "CheckIfElementDoesNotExists", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            ParentCntrType = ParentCntrType.ToLower();
            //PropertyCondition BtnCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> ParentObjConditionList = new List<PropertyCondition>();
            ParentObjConditionList = SetParamTypeBasedPropertyCondition(ParentCntrType, ParentCntrValue);
            PropertyCondition ParentCondition = ParentObjConditionList[0];
            PropertyCondition ParentCondition1 = ParentObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement ParentCntrObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentCntrType, "name", RegexOptions.IgnoreCase))
            {
                if (ParentCntrDuplicateIndex == 0)
                {
                    ParentCntrObj = GuiObj.FindAutomationElement(ParentObj, ParentCondition, ParentCondition1, TreeScope.Descendants, ParentCntrValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentCntrObj = GuiObj.FindAutomationElementWithIndex(ParentObj, ParentCondition, ParentCondition1, TreeScope.Descendants, ParentCntrValue, ParentCntrDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ParentCntrDuplicateIndex == 0)
                {
                    ParentCntrObj = GuiObj.FindAutomationElement(ParentObj, ParentCondition, TreeScope.Descendants, ParentCntrValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ParentCntrObj = GuiObj.FindAutomationElementWithIndex(ParentObj, ParentCondition, TreeScope.Descendants, ParentCntrValue, ParentCntrDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }

            if (ParentCntrValue == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Parent cntr Element with " + ParentCntrType + " - " + ParentCntrValue + " does not exists. Exiting application from CheckIfElementExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Parent cntr  Element with " + ParentCntrType + " - " + ParentCntrValue + " does not exists. Resuming as terminateOnFailure is no.", "warn");
                return null;
            }
            List<PropertyCondition> ElementObjConditionList = new List<PropertyCondition>();
            ParentObjConditionList = SetParamTypeBasedPropertyCondition(ElementParamType, ElementParamValue);
            PropertyCondition ElementCondition = ElementObjConditionList[0];
            PropertyCondition ElementCondition1 = ElementObjConditionList[1];

            AutomationElement ElementObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(ParentCntrType, "name", RegexOptions.IgnoreCase))
            {
                if (ElementDuplicateIndex == 0)
                {
                    ElementObj = GuiObj.FindAutomationElement(ParentCntrObj, ElementCondition, ElementCondition1, TreeScope.Descendants, ElementParamValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ElementObj = GuiObj.FindAutomationElementWithIndex(ParentCntrObj, ElementCondition, ElementCondition1, TreeScope.Descendants, ElementParamValue, ElementDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            else
            {
                if (ElementDuplicateIndex == 0)
                {
                    ElementObj = GuiObj.FindAutomationElement(ParentCntrObj, ElementCondition, TreeScope.Descendants, ElementParamValue, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
                else
                {
                    ElementObj = GuiObj.FindAutomationElementWithIndex(ParentCntrObj, ElementCondition, TreeScope.Descendants, ElementParamValue, ElementDuplicateIndex, TimeOutInSecs, TerminateStatus, LogFilePath);
                }
            }
            if (ElementObj != null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " exists. Exiting application from CheckIfElementDoesNotExists..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " exists.Resuming as terminateOnFailure is no", "warn");
                return null;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Element with " + ElementParamType + " - " + ElementParamValue + " does not exists", "info");
            }
            return null;
        }
        //ParentSearchObj = null, search under desktop, else under active window
        //Need to capture the popup menu bar first. Mostly will be of controltype controltype.menu or controltype.popupmenu

        public AutomationElement LookForElementWithPattern(AutomationElement ParentSearchElement, string ElementToSearchControlType, string Pattern, int ClickRequired, string ClickMouseBtn,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "LookForElementWithPattern", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            AutomationElement FoundElement = GuiObj.LookForElementWithPattern(ParentSearchElement, ElementToSearchControlType, Pattern,ClickRequired,ClickMouseBtn, TerminateStatus);
            return FoundElement;

        }

        public AutomationElement CapturePopUp(AutomationElement ParentSearchObj,string MenuBarParamType,string MenuBarParamValue, string PopupMenuParamType,string PopupMenuParamValue, int Index,int InvokeMenuItem, int TimeOutInSecs, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

            NewLogObj.WriteLogFile(LogFilePath, "CapturePopUp", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            MenuBarParamType = MenuBarParamType.ToLower();

            if (ParentSearchObj == null)
            {
                ParentSearchObj = AutomationElement.RootElement;
            }
            // PropertyCondition ChkBoxCondition = SetParamTypeBasedPropertyCondition(ParamType, ParamValue);
            List<PropertyCondition> MenuBarObjConditionList = new List<PropertyCondition>();
            MenuBarObjConditionList = SetParamTypeBasedPropertyCondition(MenuBarParamType, MenuBarParamValue);
            PropertyCondition MenuBarObCondition = MenuBarObjConditionList[0];
            PropertyCondition MenuBarObCondition1 = MenuBarObjConditionList[1];

            KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
            AutomationElement MenuBarObj = null;
            if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(MenuBarParamType, "name", RegexOptions.IgnoreCase))
            {
                if (Index == 0)
                {
                    //Search under desktop
                    MenuBarObj = GuiObj.FindAutomationElement(ParentSearchObj, MenuBarObCondition, MenuBarObCondition1, TreeScope.Children, "Popup menu bar", TimeOutInSecs, 0, LogFilePath);
                }
                else
                {
                    MenuBarObj = GuiObj.FindAutomationElementWithIndex(ParentSearchObj, MenuBarObCondition, MenuBarObCondition1, TreeScope.Children, "Popup menu bar", Index, TimeOutInSecs, 0, LogFilePath);
                }
            }
            else
            {
                if (Index == 0)
                {
                    MenuBarObj = GuiObj.FindAutomationElement(ParentSearchObj, MenuBarObCondition, TreeScope.Children, "Popup menu bar", TimeOutInSecs, 0, LogFilePath);
                }
                else
                {
                    MenuBarObj = GuiObj.FindAutomationElementWithIndex(ParentSearchObj, MenuBarObCondition, TreeScope.Children, "Popup menu bar", Index, TimeOutInSecs, 0, LogFilePath);
                }
            }

            if (MenuBarObj == null)
            {
                //Send click & try once more
                NewLogObj.WriteLogFile(LogFilePath, "Retry for context menu", "info");
                Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                Thread.Sleep(1000);
                if (KeyWordAPIs.PartiallyLocalized == 1 && Regex.IsMatch(MenuBarParamType, "name", RegexOptions.IgnoreCase))
                {
                    if (Index == 0)
                    {
                        //Search under desktop
                        MenuBarObj = GuiObj.FindAutomationElement(ParentSearchObj, MenuBarObCondition, MenuBarObCondition1, TreeScope.Children, "Popup menu bar", TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                    else
                    {
                        MenuBarObj = GuiObj.FindAutomationElementWithIndex(ParentSearchObj, MenuBarObCondition, MenuBarObCondition1, TreeScope.Children, "Popup menu bar", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                }
                else
                {
                    if (Index == 0)
                    {
                        MenuBarObj = GuiObj.FindAutomationElement(ParentSearchObj, MenuBarObCondition, TreeScope.Children, "Popup menu bar", TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                    else
                    {
                        MenuBarObj = GuiObj.FindAutomationElementWithIndex(ParentSearchObj, MenuBarObCondition, TreeScope.Children, "Popup menu bar", Index, TimeOutInSecs, TerminateStatus, LogFilePath);
                    }
                }
                if (MenuBarObj == null)
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "MenuBarObj with " + MenuBarParamType + "- " + MenuBarParamValue + " is null. Exiting application from CapturePopUp as MenuBarObj is null**", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "MenuBarObj with " + MenuBarParamType + "- " + MenuBarParamValue + " is null. Resuming as terminateOnFailure is no", "warn");
                    return null;
                }
            }
            List<PropertyCondition> PopUpMenuConditionList = SetParamTypeBasedPropertyCondition(PopupMenuParamType, PopupMenuParamValue);
            AutomationElement PopUpMenuObj = GuiObj.FindAutomationElement(MenuBarObj, PopUpMenuConditionList[0], TreeScope.Descendants, "PopUpMenu", TerminateStatus, LogFilePath);
            if (PopUpMenuObj != null)
            {
                int PatternSupport = GuiObj.CheckIfPatternIsSupported(PopUpMenuObj, "Invoke");
                if (PatternSupport == 1)
                {
                    InvokePattern invokePattern = PopUpMenuObj.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    if (invokePattern != null)
                    {
                        if (InvokeMenuItem == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Invoking popupmenuitem " + PopupMenuParamValue, "info");
                            invokePattern.Invoke();
                            
                        }
                        return PopUpMenuObj;
                    }
                    else
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "*invokePattern is null for "+PopupMenuParamType+ " - "+PopupMenuParamValue+".Exiting application from CapturePopUp as popup menu invokePattern is null**", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                        NewLogObj.WriteLogFile(LogFilePath, "*invokePattern is null for " + PopupMenuParamType + " - " + PopupMenuParamValue + ".Resuming as terminateOnFailure is no", "warn");
                        return null;
                    }
                }
                else
                {
                    if (InvokeMenuItem == 1)
                    {
                        PatternSupport = GuiObj.CheckIfPatternIsSupported(PopUpMenuObj, "ExpandCollapse");
                        if (PatternSupport == 1)
                        {
                            ExpandCollapsePattern expPattern = PopUpMenuObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                            expPattern.Expand();
                            return PopUpMenuObj;
                        }
                        else
                        {
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "No patterns are supported on PopUpMenuObj with " + PopupMenuParamType + " - " + PopupMenuParamValue + ". Exiting application from CapturePopUp as popup menu PopUpMenuObj does not support any pattrens**", "fail");
                                FileObj.ExitTestEnvironment();
                            }
                            NewLogObj.WriteLogFile(LogFilePath, "No patterns are supported on PopUpMenuObj with " + PopupMenuParamType + " - " + PopupMenuParamValue + ". Resuming as terminateOnFailure is no", "warn");
                            return null;

                        }
                    }
                    return null;
                }
            }
            else
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "PopUpMenuObj with " + PopupMenuParamType + " - " + PopupMenuParamValue + " is null", "fail");
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "PopUpMenuObj with " + PopupMenuParamType + " - " + PopupMenuParamValue + " is null", "warn");
                    return null;
                }
            }
        }

        public int ClickOnIconFromSystemTrayNotificationArea(string IconParamType, string IconParamValue,string ClickMouseBtn,int TimeOutInSecs,int TerminateStatus)
        {
            //Find system tray 1st
           
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ClickOnIconFromSystemTrayNotificationArea", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=========================================", "info");
            List<PropertyCondition> NotifyTrayConditionList = new List<PropertyCondition>();
            NotifyTrayConditionList = SetParamTypeBasedPropertyCondition("Class", "TrayNotifyWnd");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            FileOperations FileObj = new FileOperations();

           // AutomationElement NotifyTrayObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, NotifyTrayConditionList[0], TreeScope.Children, "Notify Tray", TimeOutInSecs, TerminateStatus, LogFilePath);
            //AutomationElement NotifyTrayObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, NotifyTrayConditionList[0], TreeScope.Children, "Notify Tray", TimeOutInSecs, TerminateStatus, LogFilePath);
            //if (NotifyTrayObj == null)
            //{
            //    NewLogObj.WriteLogFile(LogFilePath, "Unable to find NotifyTrayObj", "fail");
            //    if (TerminateStatus == 1)
            //    {
            //        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectIconFromSystemTrayNotificationArea. Unable to find NotifyTrayObj", "fail");
            //        FileObj.ExitTestEnvironment();
            //    }
            //}

            List<PropertyCondition> NotificationChevronBtnConditionList = new List<PropertyCondition>();
            NotificationChevronBtnConditionList = SetParamTypeBasedPropertyCondition("Name", "NotificationChevron");
            //AutomationElement NotificationChevronBtn = GuiObj.FindAutomationElement(NotifyTrayObj, NotifyTrayConditionList[0], TreeScope.Descendants, "NotificationChevronBtn", TimeOutInSecs, TerminateStatus, LogFilePath);
            AutomationElement NotificationChevronBtn = GuiObj.FindAutomationElement(AutomationElement.RootElement, NotificationChevronBtnConditionList[0], TreeScope.Descendants, "NotificationChevronBtn", TimeOutInSecs, TerminateStatus, LogFilePath);
            if (NotificationChevronBtn == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find NotificationChevronBtn", "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Exiting application from ClickOnIconFromSystemTrayNotificationArea. Unable to find NotificationChevronBtn", "fail");
                    FileObj.ExitTestEnvironment();
                    return -1;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find NotificationChevronBtn in ClickOnIconFromSystemTrayNotificationArea.Resuming as terminateOnFailure is no", "warn");
                return -1;
            }

            GuiObj.ClickButton(NotificationChevronBtn,1,"NotificationChevronBtn",TerminateStatus,LogFilePath);
            //Thread.Sleep(2000);
            //Check if NotificationOverflow window is present
            List<PropertyCondition> NotificationOverflowWindowConditionList = new List<PropertyCondition>();
            NotificationOverflowWindowConditionList = SetParamTypeBasedPropertyCondition("Name", "NotificationOverflow");
            AutomationElement NotificationOverflowPane = GuiObj.FindAutomationElement(AutomationElement.RootElement, NotificationOverflowWindowConditionList[0], TreeScope.Children, "NotificationOverflowPane", TimeOutInSecs, TerminateStatus, LogFilePath);
            if (NotificationOverflowPane == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Exiting application from ClickOnIconFromSystemTrayNotificationArea. Unable to find NotificationOverflowPane", "fail");
                    FileObj.ExitTestEnvironment();
                    return -1;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find NotificationOverflowPane in ClickOnIconFromSystemTrayNotificationArea.Resuming as terminateOnFailure is no", "warn");
                return -1;
            }
            List<PropertyCondition> IconToSelectConditionList = new List<PropertyCondition>();
            IconToSelectConditionList = SetParamTypeBasedPropertyCondition(IconParamType, IconParamValue);
            AutomationElement IconToSelectObj = GuiObj.FindAutomationElement(NotificationOverflowPane, IconToSelectConditionList[0], TreeScope.Descendants, IconParamValue, TimeOutInSecs, TerminateStatus, LogFilePath);
            if (IconToSelectObj == null)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element IconToSelectObj with " + IconParamType + " - " + IconParamValue + ". Exiting application from SelectIconFromSystemTrayNotificationArea. Unable to find IconToSelectObj", "fail");
                    FileObj.ExitTestEnvironment();
                    return -1;
                }
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element IconToSelectObj with " + IconParamType + " - " + IconParamValue + ". Resuming as terminateOnFailure is no", "warn");
                return -1;
            }
            //GuiObj.ClickButton(IconToSelectObj, 0, IconParamValue, TerminateStatus, LogFilePath);
            ClickOnElementAtGivenPosition(IconToSelectObj, "Middle", ClickMouseBtn, TimeOutInSecs, TerminateStatus);
            Console.WriteLine(IconToSelectObj);
            return 1;
        }

        
        public void SendKeysToType(string Key)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SendKeysToType", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=========", "info");
            NewLogObj.WriteLogFile(LogFilePath, "SendKeys on " + Key, "info");
            System.Windows.Forms.SendKeys.SendWait(Key);
        }
        public void SendKeys(string Key)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SendKeys", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=========", "info");
            NewLogObj.WriteLogFile(LogFilePath, "SendKeys on " + Key, "info");
            Key = Key.ToLower();
            if (string.Compare(Key, "tab") == 0)
            {
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Tab);
            }
            else if (string.Compare(Key, "enter") == 0)
            {
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Enter);
            }
            else if (string.Compare(Key, "down") == 0)
            {
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Down);
            }
            else if (string.Compare(Key, "right") == 0)
            {
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Right);
            }
            else if (string.Compare(Key, "space") == 0)
            {
                //Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Space);
                System.Windows.Forms.SendKeys.SendWait(" ");
            }
            else if (string.Compare(Key, "up") == 0)
            {
                Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.Up);
            }
            else if (string.Compare(Key, "ctrla") == 0)
            {
                System.Windows.Forms.SendKeys.SendWait("^(a)");
            }
           
        }

       

        public void ExplicitWaitFotGuizardToFinishCapturing(string GuizardLocation,string WindowName)
        {
            Generic NewGenericObj = new Generic();
            int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
            if (ProcessStatus == 1)
            {
                Guizard NewGuiZardObj = new Guizard();
                NewGuiZardObj.VerifyTestCompletedFilePresent(GuizardLocation, WindowName);


                //#############################################################################
                //while GUIzard is capturing the windows, it is filling up the controls in the window with a value 1.
                //Slep added to make sure that Guizard finish captuing the window
                //Thread.Sleep(4000);
                //#############################################################################
            }
        }
    }
}

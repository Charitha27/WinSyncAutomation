using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Test;


//using MbUnit;

using GUICollection;
using LoggerCollection;
using TestAPICollection;
using Microsoft.Test.Input;
using FileOperationsCollection;
using GuizardCollection;

namespace XenCenterOperations
{
    

    public class XenCenter
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

       // public string XenCenterDefaultInstallLocation = "C:\\Program Files\\Citrix\\XenCenter\\XenCenter.exe";

        public string GetText(string FilPath)
        {
            FileStream fs = new FileStream(FilPath, FileMode.Open);

            StreamReader t = new StreamReader(fs, Encoding.UTF8);

            String s = t.ReadLine();
            Console.WriteLine(s);
            fs.Close();
            return s;

        }

        public void OpenXenCenter(string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            int Retry = 1;
            int RetryCount=0;
            FileOperations FileObj = new FileOperations();
            string InputFilePath = FileObj.GetInputFilePath(LogFilePath, "Inputs.txt");

            string XenCenterInstalledLocation=FileObj.GetInputPattern(InputFilePath, "LocationToInstallXenCenter");
            XenCenterInstalledLocation = XenCenterInstalledLocation + "\\XenCenter.exe";
            Process[] pname = Process.GetProcessesByName("XenCenterMain.exe");
            if (pname.Length != 0)
                NewLogObj.WriteLogFile(LogFilePath,"Xencenter already opened", "info");
            else     
            {
                NewLogObj.WriteLogFile(LogFilePath,"Xencenter not opened. Trying to open", "info");
                if(File.Exists(XenCenterInstalledLocation))
                {
                    System.Diagnostics.Process.Start(XenCenterInstalledLocation);
                    Thread.Sleep(2000);
                    pname = Process.GetProcessesByName("XenCenterMain.exe");

                    if (pname.Length == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath,"Xencenter launched successfully", "info");
                    }
                    else
                    {
                        if (RetryCount < Retry)
                        {
                            NewLogObj.WriteLogFile(LogFilePath,"Xencenter launch failed.. Retrying", "info");
                            OpenXenCenter(LogFilePath);
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath,"Xencenter launch failed even after max retry of "+Retry, "fail");
                            NewLogObj.WriteLogFile(LogFilePath,"***EXiting application**", "fail");
                            //Application.Exit();
                        }
                    }

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath,"Xencenter.exe not found in" + XenCenterInstalledLocation, "fail");
                    NewLogObj.WriteLogFile(LogFilePath,"***Exiting Application***", "fail");
                }

            }
        }

        public AutomationElement SetFocusOnWindow(AutomationElement ParentObject, string PropertyCondName, string PropertyCondData, TreeScope Scope, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "SetFocusOnWindow", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            AutomationElementIdentity NewElement = new AutomationElementIdentity();
           PropertyCondition WindowReturnCondition = NewElement.SetPropertyCondition(PropertyCondName, PropertyCondData, 1, LogFilePath);
           int WaitTimeOut = 30000; // 5min
           int timer = 0;
            if (WindowReturnCondition != null)
            {
                
                AutomationElement NewWindowObj = NewElement.FindAutomationElement(ParentObject, WindowReturnCondition, Scope, ElementName, 0, LogFilePath);
                if (NewWindowObj != null)
                {
                    
                    //NewGuiZardObj.StartNewTest();
                    NewLogObj.WriteLogFile(LogFilePath,"Object with name " + ElementName + "found", "info");
                    IntPtr hwnd = (IntPtr)NewWindowObj.Current.NativeWindowHandle;
                    SetForegroundWindow(hwnd);
                    Thread.Sleep(2000);
                    SetActiveWindow(hwnd);
                    Thread.Sleep(2000);
                    MaximizeXenCenter(NewWindowObj);
                   // NewGuiZardObj.VerifyTestCompletedFilePresent("XenCenter Main Window");
                    return NewWindowObj;

                }
                else
                {
                    while (timer < WaitTimeOut && NewWindowObj == null)
                    {
                        NewWindowObj = NewElement.FindAutomationElement(ParentObject, WindowReturnCondition, Scope, ElementName, 0, LogFilePath);
                        NewLogObj.WriteLogFile(LogFilePath, "Waiting for Object with name " + ElementName , "info");
                        if (NewWindowObj == null)
                        {
                            timer = timer + 3000;
                            Thread.Sleep(3000);
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Object with name" + ElementName + "found", "info");
                            IntPtr hwnd = (IntPtr)NewWindowObj.Current.NativeWindowHandle;
                            SetForegroundWindow(hwnd);
                            Thread.Sleep(2000);
                            SetActiveWindow(hwnd);
                            Thread.Sleep(2000);

                            return NewWindowObj;

                        }
                    }
                    if (timer < WaitTimeOut && NewWindowObj == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Object with name" + ElementName + "not found from SetFocusOnWindow", "fail");
                        if (TerminateStatus == 1)
                        {

                            NewLogObj.WriteLogFile(LogFilePath, "***Exiting Application from SetFocusOnWindow***", "fail");
                            //Application.Exit();
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                    return null;

                }
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath,"WindowReturnCondition is null", "fail");
                if (TerminateStatus == 1)
                {

                    NewLogObj.WriteLogFile(LogFilePath,"***Exiting Application from SetFocusOnWindow***", "fail");
                    //Application.Exit();
                }
                return null;

            }

        }

        public void AddNewServer()
        {
            System.Windows.Forms.SendKeys.SendWait("%s");
            Thread.Sleep(1000);
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            Thread.Sleep(1000);
        }

        public AutomationElement GetXenCenterMainMenuObject(AutomationElement XenCenterObj,string LogFilePath,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "GetXenCenterMainMenuObject", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition MainMenuReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "MainMenuBar", 1, LogFilePath);
            AutomationElement MainMenuObj = GuiObj.FindAutomationElement(XenCenterObj, MainMenuReturnCondition, TreeScope.Descendants, "XenCenter Main menu", 1, LogFilePath);
            if (MainMenuObj!=null)
            {
                return MainMenuObj;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "XenCenterMainMenu object is null" , "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");

                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from GetXenCenterMainMenuObject as main menunot found**" , "fail");

                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    return null;

                }
            }
        }

        public void InvokeXenCenterSubMenuItem(AutomationElement XenCenterObj,string MenuName,string MenuItem,string LogFilePath,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "InvokeXenCenterSubMenuItem", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            try
            {
                AutomationElement XenCenterMainMenuObj = GetXenCenterMainMenuObject(XenCenterObj, LogFilePath, TerminateStatus);
                if (XenCenterMainMenuObj != null)
                {
                    AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                    PropertyCondition MainMenuReturnCondition = GuiObj.SetPropertyCondition("NameProperty", MenuName, TerminateStatus, LogFilePath);
                    AutomationElement MainMenuObj = GuiObj.FindAutomationElement(XenCenterMainMenuObj, MainMenuReturnCondition, TreeScope.Descendants, "XenCenter Sub menu", 1, LogFilePath);
                    //ExpandCollapsePattern expPattern = MainMenuObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                    //expPattern.Expand();
                    PropertyCondition SubMenuReturnCondition = GuiObj.SetPropertyCondition("NameProperty", MenuItem, 1, LogFilePath);
                    AutomationElement SubMenuObj = GuiObj.FindAutomationElement(MainMenuObj, SubMenuReturnCondition, TreeScope.Descendants, MenuItem, TerminateStatus, LogFilePath);
                    if (SubMenuObj != null)
                    {
                        InvokePattern invokePattern = SubMenuObj.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        if (invokePattern != null)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Invoking menuitem" + MenuItem, "info");
                            invokePattern.Invoke();
                            
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "invokePattern is null", "fail");
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");

                                NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from GetXenCenterMainMenuObject as invokePattern is null**", "fail");
                                FileOperations FileObj = new FileOperations();
                                FileObj.ExitTestEnvironment();

                            }
                        }
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "SubMenuObj is null", "fail");
                        
                    }
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath,"XenCenterMainMenuObj is null","fail");
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath,"Exception at InvokeXenCenterSubMenuItem"+Ex.ToString(),"fail" );
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");

                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from GetXenCenterMainMenuObject as main menunot found**", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                   

                }
            }

        }

        public AutomationElement TypeInXenCenterSearchBox(AutomationElement XenCenterObj, string TextToType, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "TypeInXenCenterSearchBox", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            try
            {
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition WindowReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "TreeSearchBox", 1, LogFilePath);
                AutomationElement SearchDialogObj = GuiObj.FindAutomationElement(XenCenterObj, WindowReturnCondition, TreeScope.Descendants, "Search Box", 1, LogFilePath);
                PropertyCondition TextBoxReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "textBox1", 1, LogFilePath);
                AutomationElement SearchTextDialogObj = GuiObj.FindAutomationElement(SearchDialogObj, TextBoxReturnCondition, TreeScope.Descendants, "Search Box", 1, LogFilePath);
                GuiObj.SetTextBoxText(SearchTextDialogObj, TextToType, "Search Box", 1, LogFilePath);
                string TextSet = GuiObj.GetTextBoxText(SearchTextDialogObj, "Search Box Text", 1, LogFilePath);
                //MbUnit.Framework.Assert.AreEqual(TextToType, TextSet);
                return SearchDialogObj;
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at TypeInXenCenterSearchBox" + Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");

                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from TypeInXenCenterSearchBox as main menunot found**", "fail");

                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    return null;
                }
            }

        }

        public AutomationElement SelectFromXenCenterTree(AutomationElement XenCenterObj,String TextToSelect,int TerminateStatus,string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "SelectFromXenCenterTree", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
               // AutomationElement SearchObj=TypeInXenCenterSearchBox(XenCenterObj, TextToSelect, 1, LogFilePath);
                PropertyCondition TreeReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "treeView", 1, LogFilePath);
                AutomationElement ServerPane = GuiObj.FindAutomationElement(XenCenterObj, TreeReturnCondition, TreeScope.Descendants, "Server Paniewe V", 0, LogFilePath);

                PropertyCondition XenCenterParentCondition = GuiObj.SetPropertyCondition("NameProperty", "XenCenter", 1, LogFilePath);
                AutomationElement XenCenterParentElement = GuiObj.FindAutomationElement(ServerPane, XenCenterParentCondition, TreeScope.Descendants, "XenCenterParentElement", 0, LogFilePath);
                ExpandCollapsePattern expPattern = XenCenterParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                Thread.Sleep(1000);
                expPattern.Expand();

                PropertyCondition XenCenterElementCondition = GuiObj.SetPropertyCondition("NameProperty", TextToSelect, 1, LogFilePath);
                AutomationElement XenCenterElementToSearch = GuiObj.FindAutomationElement(ServerPane, XenCenterElementCondition, TreeScope.Descendants, "XenCenterTextToSearch", 1, LogFilePath);
                ExpandCollapsePattern expPattern1 = XenCenterElementToSearch.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                Thread.Sleep(1000);
                expPattern1.Expand();
                //var e = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right);
                //e.RoutedEvent = Mouse.MouseDownEvent;
                //XenCenterElementToSearch.OnMouseDown(e);
                return null;
               

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at SelectFromXenCenterTree" + Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");

                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from SelectFromXenCenterTree as main menunot found**", "fail");

                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    return null;
                }
            }

        }

        //#######################################################################################################################################
        // Will do a Left mouse Click on the XenCenter element 
        //#######################################################################################################################################
        public AutomationElement ClickOnXenCenterNode(AutomationElement XenCenterObj, string LogFilePath, int TerminateStatus)
        {

            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "ClickOnXenCenterNode", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                // AutomationElement SearchObj=TypeInXenCenterSearchBox(XenCenterObj, TextToSelect, 1, LogFilePath);
                PropertyCondition TreeReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "treeView", 1, LogFilePath);
                AutomationElement ServerPane = GuiObj.FindAutomationElement(XenCenterObj, TreeReturnCondition, TreeScope.Descendants, "Server Paniewe V", 0, LogFilePath);

                PropertyCondition XenCenterParentCondition = GuiObj.SetPropertyCondition("NameProperty", "XenCenter", 1, LogFilePath);
                AutomationElement XenCenterParentElement = GuiObj.FindAutomationElement(ServerPane, XenCenterParentCondition, TreeScope.Descendants, "XenCenterParentElement", 0, LogFilePath);
                ExpandCollapsePattern expPattern = XenCenterParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                Thread.Sleep(1000);
                if (string.Compare(expPattern.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                {
                    expPattern.Expand();
                }
                TestAPI TestApiObj = new TestAPI();
                TestApiObj.ClickLeftBtnOnAutomationElement(XenCenterParentElement, 1, LogFilePath);
                return XenCenterParentElement;
                
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at ClickOnXenCenterNode" + Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnXenCenterNode as main menunot found**", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    return null;
                }
            }
        }

        //#######################################################################################################################################
        // Will do a Left mouse Click on the element which has to be clicked on the server node list
         //#######################################################################################################################################
        public AutomationElement ClickOnXenCenterServerNode(AutomationElement XenCenterObj,string ElementToClick,string LogFilePath,int TerminateStatus)
        {
            
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "ClickOnXenCenterServerNode", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                // AutomationElement SearchObj=TypeInXenCenterSearchBox(XenCenterObj, TextToSelect, 1, LogFilePath);
                PropertyCondition TreeReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "treeView", 1, LogFilePath);
                AutomationElement ServerPane = GuiObj.FindAutomationElement(XenCenterObj, TreeReturnCondition, TreeScope.Descendants, "Server Paniewe V", 0, LogFilePath);

                PropertyCondition XenCenterParentCondition = GuiObj.SetPropertyCondition("NameProperty", "XenCenter", 1, LogFilePath);
                AutomationElement XenCenterParentElement = GuiObj.FindAutomationElement(ServerPane, XenCenterParentCondition, TreeScope.Descendants, "XenCenterParentElement", 0, LogFilePath);
                ExpandCollapsePattern expPattern = XenCenterParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                Thread.Sleep(1000);
                if(string.Compare(expPattern.Current.ExpandCollapseState.ToString(),"Collapsed")==0)
                {
                    expPattern.Expand();
                }
                TestAPI TestApiObj = new TestAPI();
                TestApiObj.ClickLeftBtnOnAutomationElement(XenCenterParentElement, 1, LogFilePath);
                //AutomationElement SearchElement=GuiObj.GetTheTreeElement(XenCenterParentElement, ElementToClick, LogFilePath);
                AutomationElement SearchElement = GuiObj.CheckTreeForElementAndClickIfReqd(XenCenterParentElement, ElementToClick,1,"Left",LogFilePath);
                if (SearchElement != null)
                {
                    //TestApiObj.ClickLeftBtnOnAutomationElement(SearchElement, 1, LogFilePath);
                    //SelectionItemPattern SelectionPattern = SearchElement.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                    //SelectionPattern.Select();
                    GuiObj.GetPositionFromBoundingRectangleAndClick(SearchElement, LogFilePath,"Left");
                    return SearchElement;
                    
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SearchElement returnd is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnXenCenterNode as SearchElement returnd is null**", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                        return null;
                    }

                }

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at ClickOnXenCenterNode" + Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnXenCenterNode as main menunot found**", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    return null;
                 }
            }
        }

        public void ConnectToXenserver(AutomationElement XenCenterObj,string ServerName, string LogFilePath, int TerminateStatus,string Password)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "ConnectToXenserver", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================", "info");
            int ConnecToServerTimeOut = 60000;
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                AutomationElement XenCenterNodeFound = ClickOnXenCenterNode(XenCenterObj, LogFilePath, TerminateStatus);
                if (XenCenterNodeFound != null)
                {
                   // AutomationElement ServerNodeFound = GuiObj.GetTheTreeElement(XenCenterNodeFound, ServerName, LogFilePath);
                    AutomationElement ServerNodeFound = GuiObj.CheckTreeForElementAndClickIfReqd(XenCenterNodeFound, ServerName,0,"0", LogFilePath);

                    if (ServerNodeFound == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "ServerNodeFound is null" , "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                            NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ConnectToXenserver as ConnecToServerTimeOut**", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                        }

                    }

                    
                    //PropertyCondition ServerNodeCondition = GuiObj.SetPropertyCondition("NameProperty", ServerName, 1, LogFilePath);
                    //AutomationElement ServerNodeFound = GuiObj.FindAutomationElement(XenCenterNodeFound, ServerNodeCondition, TreeScope.Descendants, "Server node", 0, LogFilePath);
                    ExpandCollapsePattern expPattern = ServerNodeFound.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                    Thread.Sleep(1000);
                    string CurrentState = expPattern.Current.ExpandCollapseState.ToString();
                    NewLogObj.WriteLogFile(LogFilePath, "ServerNodeFound CurrentState " + CurrentState, "info");
                    if (string.Compare(CurrentState, "LeafNode") == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "ServerNodeFound is leaf node ", "info");
                        GuiObj.GetPositionFromBoundingRectangleAndClick(ServerNodeFound, LogFilePath, "Left");
                        Thread.Sleep(3000);
                        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                        Thread.Sleep(3000);
                        System.Windows.Forms.SendKeys.SendWait("c");
                        Thread.Sleep(1000);

                        //Check for Add new server window
                        PropertyCondition WindowReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "AddServerDialog", 1, LogFilePath);
                        AutomationElement ServerDialogObj = GuiObj.FindAutomationElement(XenCenterObj, WindowReturnCondition, TreeScope.Descendants, "Add New Server Window", 1, LogFilePath);
                        PropertyCondition PasswordReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "PasswordTextBox", 1, LogFilePath);
                        AutomationElement PasswordTextBoxObj = GuiObj.FindAutomationElement(ServerDialogObj, PasswordReturnCondition, TreeScope.Descendants, "PasswordTextBox", 1, LogFilePath);

                        GuiObj.SetTextBoxText(PasswordTextBoxObj, Password, "PasswordTextBox", 1, LogFilePath);

                        PropertyCondition AddBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "AddButton", 1, LogFilePath);
                        AutomationElement AddBtnObj = GuiObj.FindAutomationElement(ServerDialogObj, AddBtnCondition, TreeScope.Descendants, "AddBtn", 0, LogFilePath);
                        GuiObj.ClickButton(AddBtnObj, 1, "AddBtn", 1, LogFilePath);

                        //PropertyCondition ConnecToServerDialogReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "ConnectingToServerDialog", 1, LogFilePath);
                        //AutomationElement ConnecToServerDialogObj = GuiObj.FindAutomationElement(XenCenterObj, ConnecToServerDialogReturnCondition, TreeScope.Descendants, "ConnectingToServerDialog", 1, LogFilePath);
                        //if (ConnecToServerDialogObj != null)
                        //{
                        NewLogObj.WriteLogFile(LogFilePath, "ConnecToServerDialogObj found ", "info");
                        int Timer = 0;
                        while ((string.Compare(CurrentState, "LeafNode") == 0) && (Timer < ConnecToServerTimeOut))
                        {
                            expPattern = ServerNodeFound.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            CurrentState = expPattern.Current.ExpandCollapseState.ToString();
                            NewLogObj.WriteLogFile(LogFilePath, "ServerNodeFound CurrentState " + CurrentState, "info");
                            if (string.Compare(CurrentState, "LeafNode") == 0)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Server still the leaf node ", "info");
                                Thread.Sleep(5000);
                                Timer = Timer + 5000;
                            }
                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Server is not the leaf node. Server added successfully ", "info");

                                break;
                            }
                        }
                        if (Timer >= ConnecToServerTimeOut)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "ServerNodeFound is null even after wait of " + ConnecToServerTimeOut, "fail");
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                                NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ConnectToXenserver as ConnecToServerTimeOut**", "fail");
                                FileOperations FileObj = new FileOperations();
                                FileObj.ExitTestEnvironment();
                            }
                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                            }

                        }
                        //}
                        //else
                        //{
                        //    NewLogObj.WriteLogFile(LogFilePath, "ConnecToServerDialogObj not found ", "fail");

                        //}
                    }
                    else
                    {
                        GuiObj.GetPositionFromBoundingRectangleAndClick(ServerNodeFound, LogFilePath, "Left");
                    }

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "ServerNodeFound is null at ConnectToXenserver", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ConnectToXenserver as main menunot found**", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    }

                }


            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at ConnectToXenserver" + Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ConnectToXenserver as main menunot found**", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                }
            }


        }
        //#########################################################################################
        // Check the Xencenter server node tree for an element
        //Will perform a click on the found element if 'Click' is set to 1. ClickMouseBtn should be either left or right. will simulate right or left click
        //#########################################################################################
        public AutomationElement CheckXenCenterServerNodetree(AutomationElement XenCenterObj, string ElementToCheck, string LogFilePath, int TerminateStatus,int ClickRequired,string ClickMouseBtn)
        {

            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "CheckXenCenterServerNodetree", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                PropertyCondition TreeReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "treeView", 1, LogFilePath);
                AutomationElement ServerPane = GuiObj.FindAutomationElement(XenCenterObj, TreeReturnCondition, TreeScope.Descendants, "Server Paniewe V", 0, LogFilePath);

                PropertyCondition XenCenterParentCondition = GuiObj.SetPropertyCondition("NameProperty", "XenCenter", 1, LogFilePath);
                AutomationElement XenCenterParentElement = GuiObj.FindAutomationElement(ServerPane, XenCenterParentCondition, TreeScope.Descendants, "XenCenterParentElement", 0, LogFilePath);
                if (ClickRequired == 1)
                {
                    int PatternSupport = GuiObj.CheckIfPatternIsSupported(XenCenterParentElement, "ExpandCollapse");
                    if (PatternSupport == 1)
                    {
                        ExpandCollapsePattern expPattern = XenCenterParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                        Thread.Sleep(1000);
                        if (string.Compare(expPattern.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                        {
                            expPattern.Expand();
                        }
                    }
                }
                TestAPI TestApiObj = new TestAPI();
                //TestApiObj.ClickLeftBtnOnAutomationElement(XenCenterParentElement, 1, LogFilePath);
                //AutomationElement SearchElement = GuiObj.GetTheTreeElement(XenCenterParentElement, ElementToCheck, LogFilePath);
                AutomationElement SearchElement = GuiObj.CheckTreeForElementAndClickIfReqd(XenCenterParentElement, ElementToCheck, 0,"0",LogFilePath);
                
                if (SearchElement != null && ClickRequired==1) 
                {
                    //TestApiObj.ClickLeftBtnOnAutomationElement(SearchElement, 1, LogFilePath);
                    //SelectionItemPattern SelectionPattern = SearchElement.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                    //SelectionPattern.Select();
                    GuiObj.GetPositionFromBoundingRectangleAndClick(SearchElement, LogFilePath, ClickMouseBtn);
                    
                }
                if (SearchElement != null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SearchElement found", "info");
                    return SearchElement;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SearchElement returnd is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckXenCenterServerNodetree as SearchElement returnd is null**", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                        return null;
                    }

                }

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at ClickOnXenCenterNode" + Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnXenCenterNode as main menunot found**", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                    return null;
                }
            }
        }

        public void MaximizeXenCenter(AutomationElement XenCenterObj)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "MaximizeXenCenter", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition MaximizedBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "Maximize", 1, LogFilePath);
            AutomationElement MaximizedBtn = GuiObj.FindAutomationElement(XenCenterObj, MaximizedBtnCondition, TreeScope.Descendants, "XenCenter Maximize Btn", 0, LogFilePath);
            if (MaximizedBtn != null)
            {
                GuiObj.ClickButton(MaximizedBtn, 0, "MaximizedBtn", 1, LogFilePath);
            }

        }

        public AutomationElement WiatTillXenCenterHandleIsObtained()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "WiatTillXenCenterHandleIsObtained", "info");
            NewLogObj.WriteLogFile(LogFilePath, "================================", "info");
            XenCenter NewOprtorObj = new XenCenter();
            AutomationElement NewAutoObj = NewOprtorObj.SetFocusOnWindow(AutomationElement.RootElement, "NameProperty", "XenCenter", TreeScope.Children, "XenCenter", 1, LogFilePath);
            int WaitTimeOut = 300000;//5 mins
            int timer = 0;
            while (NewAutoObj == null && timer < WaitTimeOut)
            {
                NewAutoObj = NewOprtorObj.SetFocusOnWindow(AutomationElement.RootElement, "NameProperty", "XenCenter", TreeScope.Children, "XenCenter", 1, LogFilePath);
                Thread.Sleep(3000);
                timer = timer + 3000;
                NewLogObj.WriteLogFile(LogFilePath, "Waiting for Xencenter to open", "info");
                Console.WriteLine("Waiting for Xencenter to open ");
            }
            if (NewAutoObj == null && timer >= WaitTimeOut)
            {
                Console.WriteLine("Timeout waiting for Xencenter to open  after installation.Exiting.. ");
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Xencenter to open  after installation.Exiting.. ", "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();

            }
            NewLogObj.WriteLogFile(LogFilePath, "Xencenter has opened  after installation", "info");
            Console.WriteLine("Xencenter has opened  after installation ");

            Guizard NewGuiZardObj = new Guizard();
            NewGuiZardObj.StartNewTest("GUizardlocation");//Edited for keywr
           // NewGuiZardObj.VerifyTestCompletedFilePresent("XenCenter Main Window"); --> Commenting as this takes a lot of time
            return NewAutoObj;
        }

    }

    
}

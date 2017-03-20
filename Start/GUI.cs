using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Test;
using System.Windows;

using LoggerCollection;
using TestAPICollection;
using FileOperationsCollection;
using GenericCollection;
using Start;

namespace GUICollection
{
    public class AutomationElementIdentity
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

         [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hwnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        public static AutomationElement SuperParentWindowObj;

        public string GetCurrentActiveWinodwText()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GetCurrentActiveWinodwText", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            // get handle
            IntPtr handle = GetForegroundWindow();

            // get title
            const int count = 512;
            var text = new StringBuilder(count);

            if (GetWindowText(handle, text, count) > 0)
            {
                //Console.WriteLine(text.ToString());
                string WindowTitle = text.ToString();
                NewLogObj.WriteLogFile(LogFilePath, "ActiveWindowTitle " + WindowTitle, "info");
                return WindowTitle;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Unable to get ActiveWindowTitle " , "info");
                return null;
            }

        }
        public ControlType SetControlType(string Item,string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                Item = Item.ToLower();
                if ((string.Compare(Item, "button") == 0) || string.Compare(Item, "controltype.button") == 0)
                {
                    return ControlType.Button;
                }
                else if ((string.Compare(Item, "calendar") == 0) || string.Compare(Item, "controltype.calendar") == 0)
                {
                    return ControlType.Calendar;
                }
                else if ((string.Compare(Item, "treeitem") == 0)|| (string.Compare(Item, "controltype.treeitem") == 0))
                {
                    return ControlType.TreeItem;
                }
                else if ((string.Compare(Item, "tree") == 0) || (string.Compare(Item, "controltype.tree") == 0))
                {
                    return ControlType.Tree;
                }
                else if ((string.Compare(Item, "listitem") == 0) || (string.Compare(Item, "controltype.listitem") == 0))
                {
                    return ControlType.ListItem;
                }
                else if ((string.Compare(Item, "pane") == 0) || (string.Compare(Item, "controltype.pane") == 0))
                {
                    return ControlType.Pane;
                }
                else if ((string.Compare(Item, "custom") == 0) || (string.Compare(Item, "controltype.custom") == 0))
                {
                    return ControlType.Custom;
                }
                else if ((string.Compare(Item, "combobox") == 0) ||(string.Compare(Item, "controltype.combobox") == 0))
                {
                    return ControlType.ComboBox;
                }
                else if ((string.Compare(Item, "checkbox") == 0) ||(string.Compare(Item, "controltype.checkbox") == 0))
                {
                    return ControlType.CheckBox;
                }
                else if ((string.Compare(Item, "radiobutton") == 0) || (string.Compare(Item, "controltype.radiobutton") == 0))
                {
                    return ControlType.RadioButton;
                }
                else if ((string.Compare(Item, "dataitem") == 0) || (string.Compare(Item, "controltype.dataitem") == 0))
                {
                    return ControlType.DataItem;
                }
                else if ((string.Compare(Item, "datagrid") == 0) || (string.Compare(Item, "controltype.datagrid") == 0))
                {
                    return ControlType.DataGrid;
                }
                else if ((string.Compare(Item, "menu") == 0) || (string.Compare(Item, "controltype.menu") == 0))
                {
                    return ControlType.Menu;
                }
                else if ((string.Compare(Item, "menuitem") == 0) || (string.Compare(Item, "controltype.menuitem") == 0))
                {
                    return ControlType.MenuItem;
                }
                else if ((string.Compare(Item, "header") == 0) || (string.Compare(Item, "controltype.header") == 0))
                {
                    return ControlType.Header;
                }
                else if ((string.Compare(Item, "headeritem") == 0) || (string.Compare(Item, "controltype.headeritem") == 0))
                {
                    return ControlType.HeaderItem;
                }
                else if ((string.Compare(Item, "edit") == 0) || (string.Compare(Item, "controltype.edit") == 0))
                {
                    return ControlType.Edit;
                }
                else if ((string.Compare(Item, "group") == 0) || (string.Compare(Item, "controltype.group") == 0))
                {
                    return ControlType.Group;
                }
                else if ((string.Compare(Item, "document") == 0) || (string.Compare(Item, "controltype.document") == 0))
                {
                    return ControlType.Document;
                }
                else if ((string.Compare(Item, "window") == 0) || (string.Compare(Item, "controltype.window") == 0))
                {
                    return ControlType.Window;
                }
                else if ((string.Compare(Item, "hyperlink") == 0) || (string.Compare(Item, "controltype.hyperlink") == 0))
                {
                    return ControlType.Hyperlink;
                }
                return null;
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath,"Exception at SetControlType."+Ex.ToString(),"info");
                return null;
            }

        }

        public PropertyCondition SetPropertyCondition(string PropertyType, string PropertyName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                string[] UIAutomationProperties = { "AcceleratorKeyProperty", "AutomationIdProperty","BoundingRectangleProperty", "ClickablePointProperty", "ControlTypeProperty", "HelpTextProperty", "IsContentElementProperty", "IsControlElementProperty", "IsKeyboardFocusableProperty", "LabeledByProperty", "LocalizedControlTypeProperty", "NameProperty","ClassNameProperty"};
                int FlagFound = 0;
                for (int i = 0; i < UIAutomationProperties.Length; i++)
                {
                    if (string.Compare(PropertyType, UIAutomationProperties[i]) == 0)
                    {
                        PropertyCondition typeCondition;
                        FlagFound = 1;
                        switch (i)
                        {
                            case 0:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.AcceleratorKeyProperty, PropertyName);
                                    break;
                                }
                            case 1:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 2:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.BoundingRectangleProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 3:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.ClickablePointProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 4:
                                {

                                    ControlType ControlTypeObj = SetControlType(PropertyName, LogFilePath);
                                    typeCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlTypeObj);
                                    return typeCondition;
                                }
                            case 5:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.HelpTextProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 6:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.IsContentElementProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 7:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.IsKeyboardFocusableProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 8:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.LabeledByProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 9:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 10:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.IsContentElementProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 11:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.NameProperty, PropertyName);
                                    return typeCondition;
                                }
                            case 12:
                                {
                                    typeCondition = new PropertyCondition(AutomationElement.ClassNameProperty, PropertyName);
                                    return typeCondition;
                                }
                        }

                    }

                }
                if (FlagFound == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to set property condition for " + PropertyName + "for" + PropertyType, "fail");
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath,"Exception at SetPropertyCondition"+Ex.ToString(),"info");
                return null;
            }
        }

        //Serach for pattern in element Name
        public AutomationElement LookForElementWithPattern(AutomationElement ParentSearchElement, string ElementToSearchControlType, string Pattern,int ClickRequired,string ClickMouseBtn, int TerminateStatus)
        {
           
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "LookForElementsWithPattern", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==========================", "info");
            string ElementIdentifer = null;
            FileOperations FileObj = new FileOperations();
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            Regex RegPattern = new Regex(Pattern);

            try
            {
                //AutomationElementCollection AutoElementColl=ParentSearchElement.FindAll(TreeScope.Children,);
                PropertyCondition PropCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", ElementToSearchControlType, 0, LogFilePath);
                AutomationElementCollection AutoElementColl = ParentSearchElement.FindAll(TreeScope.Descendants, PropCondition);
                foreach (AutomationElement AutoObj in AutoElementColl)
                {
                    string AutoObjName = AutoObj.Current.Name;
                    if (!string.IsNullOrWhiteSpace(AutoObjName))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Current AutoObj name " + AutoObjName, "info");
                        Match PatternMatch = RegPattern.Match(AutoObjName);
                        if (PatternMatch.Success)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Pattern "+Pattern+" matches with element "+AutoObjName, "info");
                            if (ClickRequired == 1)
                            {
                                GetPositionFromBoundingRectangleAndClickBasedOnInput(AutoObj, LogFilePath, ClickMouseBtn, "Left", 20);
                            }
                            return AutoObj;
                        }
                    }
                }
                
                if (TerminateStatus == 1)
                {
                    string TextToWrite = "Unable to find the automation element that matches pattern" + Pattern + "Exiting application ";
                    NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                    FileObj.ExitTestEnvironment();
                    return null;
                }
                else
                {
                    string TextToWrite = "Unable to find the automation element that matches pattern" + Pattern + ". Resuming as TerminateOnFailure is No ";
                    NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "warn");
                    return null;
                }
                

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at LookForElementWithPattern in finding element " + ElementIdentifer + ". " + Ex.ToString(), "info");
                return null;
            }

        }

        public AutomationElement FindAutomationElement(AutomationElement ParentSearchElement, PropertyCondition PropCond, TreeScope Scope, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            string ElementIdentifer = null;
            FileOperations FileObj = new FileOperations();
            if (PropCond != null)
            {
                ElementIdentifer = PropCond.Value.ToString();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCond is null at FindAutomationElement", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
            try
            {
                AutomationElement Foundelement = ParentSearchElement.FindFirst(Scope, PropCond);
                //return Foundelement;
                if (Foundelement != null)
                {
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = Foundelement;
                    }
                    return Foundelement;
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        string TextToWrite = "Unable to find the automation element " + ElementIdentifer + "Exiting application ";
                        NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                        //NewLogObj.WriteLogFile(LogFilePath, "***Exiting application****", "fail");
                        //Application.Exit();
                       
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    else
                    {
                        string TextToWrite = "Unable to find the automation element " + ElementIdentifer + ". Resuming as TerminateOnFailure is No ";
                        NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "warn");
                        return null;
                    }
                }

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement in finding element " + ElementIdentifer+ ". "+Ex.ToString(), "info");
                return null;
            }

        }

        //Will wait for timeout secs for the element
        public AutomationElement FindAutomationElement(AutomationElement ParentSearchElement, PropertyCondition PropCond, TreeScope Scope, string ElementName, int TimeOut,int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            string ElementIdentifer = null;
            try
            {
                if (PropCond != null)
                {
                    ElementIdentifer = PropCond.Value.ToString();
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "PropCond is null at FindAutomationElement", "fail");
                    FileObj.ExitTestEnvironment();
                    return null;
                }
            }
            catch
            {

            }
            try
            {
                AutomationElement Foundelement = ParentSearchElement.FindFirst(Scope, PropCond);
                int Timer = 0;
                //return Foundelement;
                if (Foundelement != null)
                {
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = Foundelement;
                    }
                    return Foundelement;
                }
                else
                {
                    while (Foundelement == null && Timer <= TimeOut)
                    {
                        Thread.Sleep(500);
                        Timer = Timer + 500;
                        Foundelement = ParentSearchElement.FindFirst(Scope, PropCond);
                        if (Foundelement != null)
                        {
                            if (AutomationElement.RootElement == ParentSearchElement)
                            {
                                SuperParentWindowObj = Foundelement;
                            }
                            return Foundelement;
                        }

                    }
                    if (Foundelement == null && Timer >= TimeOut)
                    {
                        if (TerminateStatus == 1)
                        {
                            string TextToWrite = "Unable to find the automation element " + ElementIdentifer + " Exiting application";
                            NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                            //NewLogObj.WriteLogFile(LogFilePath, "***Exiting application****", "fail");
                            //Application.Exit();
                           
                            FileObj.ExitTestEnvironment();
                            return null;
                        }
                        else
                        {
                            string TextToWrite = "Unable to find the automation element " + ElementIdentifer + ". Resuming as TerminateOnFailure is No ";
                            NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "warn");
                            return null;
                        }
                    }
                }
                return null;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement in finding element " + ElementIdentifer+ ". " + Ex.ToString(), "info");
                return null;
            }
        }

        //For partially localized products, passing 2 conditions
        public AutomationElement FindAutomationElement(AutomationElement ParentSearchElement, PropertyCondition PropCond1,PropertyCondition PropCond2, TreeScope Scope, string ElementName, int TimeOut, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            string ElementIdentifer1 = null;
            string ElementIdentifer2 = null;
            if (PropCond1 != null)
            {
                ElementIdentifer1 = PropCond1.Value.ToString();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCond1 is null at FindAutomationElement", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
            if (PropCond2 != null)
            {
                ElementIdentifer2 = PropCond2.Value.ToString();
            }
           
            try
            {
                NewLogObj.WriteLogFile(LogFilePath, "Starting serach with propertycondition1 " + ElementIdentifer1, "info");
                AutomationElement Foundelement = ParentSearchElement.FindFirst(Scope, PropCond1);
                int Timer = 0;
                //return Foundelement;
                if (Foundelement != null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element found with propertycondition1" + ElementIdentifer1, "info");
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = Foundelement;
                    }
                    return Foundelement;
                }
                else
                {
                    while (Foundelement == null && Timer <= TimeOut)
                    {
                        Thread.Sleep(500);
                        Timer = Timer + 500;
                        Foundelement = ParentSearchElement.FindFirst(Scope, PropCond1);
                        if (Foundelement != null)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Element found with propertycondition1" + ElementIdentifer1, "info");
                            if (AutomationElement.RootElement == ParentSearchElement)
                            {
                                SuperParentWindowObj = Foundelement;
                            }
                            return Foundelement;
                        }
                        else
                        {
                            Foundelement = ParentSearchElement.FindFirst(Scope, PropCond2);
                            if (Foundelement != null)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Element found with propertycondition2", "info");
                                if (AutomationElement.RootElement == ParentSearchElement)
                                {
                                    SuperParentWindowObj = Foundelement;
                                }
                                return Foundelement;
                            }
                        }

                    }
                    
                    if (Foundelement == null && Timer >= TimeOut)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element not found with propertycondition1 "+ ElementIdentifer1+" & propertycondition2 "+ElementIdentifer2, "fail");
                        if (TerminateStatus == 1)
                        {
                            string TextToWrite = "Unable to find the automation element with" + ElementIdentifer1 + "and " + ElementIdentifer2 + " Exiting application";
                            NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                            //NewLogObj.WriteLogFile(LogFilePath, "***Exiting application****", "fail");
                            //Application.Exit();
                            
                            FileObj.ExitTestEnvironment();
                            return null;
                        }
                        else
                        {
                            string TextToWrite = "Unable to find the automation element with" + ElementIdentifer1 + "and " +ElementIdentifer2;
                            NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                            return null;
                        }
                    }
                }
                return null;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement in finding with "+ElementIdentifer1 + "and " +ElementIdentifer2 +" " + Ex.ToString(), "info");
                return null;
            }
        }

        //Can be applied for finidng optional elements.Will not log  a fail even if the element is not available.
        public AutomationElement FindAutomationElement(AutomationElement ParentSearchElement, PropertyCondition PropCond, TreeScope Scope, string ElementName, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            string ElementIdentifer = null;
            FileOperations FileObj = new FileOperations();
            if (PropCond != null)
            {
                ElementIdentifer = PropCond.Value.ToString();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCond is null at FindAutomationElement", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
            try
            {
                AutomationElement Foundelement = ParentSearchElement.FindFirst(Scope, PropCond);
                 //return Foundelement;
                if (Foundelement != null)
                {
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = Foundelement;
                    }
                    return Foundelement;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the optional automation element " + ElementIdentifer, "info");
                }
                return null;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement" + Ex.ToString(), "info");
                return null;
            }
        }

        //Optional element . ItemIndex - if more than 1 autoelement with same propcondition, select based on idex
        public AutomationElement FindAutomationElementWithIndex(AutomationElement ParentSearchElement, PropertyCondition PropCond1, PropertyCondition PropCond2, TreeScope Scope, string ElementName, int ItemIndex, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            string ElementIdentifer1 = null;
            string ElementIdentifer2 = null;
            FileOperations FileObj = new FileOperations();
            if (PropCond1 != null)
            {
                ElementIdentifer1 = PropCond1.Value.ToString();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCond1 is null at FindAutomationElement", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
            if (PropCond2 != null)
            {
                ElementIdentifer2 = PropCond2.Value.ToString();
            }
            try
            {
                NewLogObj.WriteLogFile(LogFilePath, "Starting serach with propertycondition1 " + ElementIdentifer1, "info");
                AutomationElementCollection SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond1);
                AutomationElement SearchElement = null;
                
                if (SearchElementColl.Count >= ItemIndex)
                {
                    //Index in array starts from 0, where as ppl specify indexfrm 1
                    SearchElement = SearchElementColl[ItemIndex - 1];
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = SearchElement;
                    }
                    return SearchElement;
                }
                else
                {
                    SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond1);
                    if (SearchElementColl.Count >= ItemIndex)
                    {
                        //Index in array starts from 0, where as ppl specify indexfrm 1
                        SearchElement = SearchElementColl[ItemIndex - 1];
                        return SearchElement;
                    }
                    if (SearchElement == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element not found with propertycondition1 " + ElementIdentifer1, "info");
                        NewLogObj.WriteLogFile(LogFilePath, "Strating search with propertycondition2 " + ElementIdentifer2, "info");
                        
                        //Starting serach with 2nd prop condition
                        SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond2);

                        if (SearchElementColl.Count >= ItemIndex)
                        {
                            //Index in array starts from 0, where as ppl specify indexfrm 1
                            SearchElement = SearchElementColl[ItemIndex - 1];
                            return SearchElement;
                        }
                      }
                    if (SearchElement == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element not found with propertycondition2 " + ElementIdentifer2 + " also", "info");
                        return null;
                    }
                }
                return null;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement in search with " + ElementIdentifer1 + "and " + ElementIdentifer2 + Ex.ToString(), "info");
                return null;
            }
        }

        //Optional element. ItemIndex - if more than 1 autoelement with same propcondition, select based on idex
        public AutomationElement FindAutomationElementWithIndex(AutomationElement ParentSearchElement, PropertyCondition PropCond, TreeScope Scope, string ElementName, int ItemIndex,string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            AutomationElement SearchElement = null;
            FileOperations FileObj = new FileOperations();
            string ElementIdentifer = null;
            if (PropCond != null)
            {
                ElementIdentifer = PropCond.Value.ToString();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCond is null at FindAutomationElement", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
            try
            {
                //AutomationElement Foundelement = ParentSearchElement.FindFirst(Scope, PropCond);
                AutomationElementCollection SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond);
                if (SearchElementColl.Count >= ItemIndex)
                {
                    //Index in array starts from 0, where as ppl specify indexfrm 1
                    SearchElement = SearchElementColl[ItemIndex - 1];
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = SearchElement;
                    }
                    return SearchElement;
                }
                else
                {
                    SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond);
                    if (SearchElementColl.Count >= ItemIndex)
                    {
                        //Index in array starts from 0, where as ppl specify indexfrm 1
                        SearchElement = SearchElementColl[ItemIndex - 1];
                        return SearchElement;
                    }
                    if (SearchElement == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element  " + ElementIdentifer + " with index " + ItemIndex + "Resuming as TerminateOnFailure is no", "warn");
                        return null;
                    }
                    return null;
                }

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement in search with " + ElementIdentifer + " ." + Ex.ToString(), "info");
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element " + ElementIdentifer, "info"); ;
                return null;
            }

        }
        public AutomationElement FindAutomationElement(AutomationElement ParentSearchElement, AndCondition PropCond, TreeScope Scope, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            
            try
            {
                AutomationElement Foundelement = ParentSearchElement.FindFirst(Scope, PropCond);
                if (Foundelement != null)
                {
                    if (AutomationElement.RootElement==ParentSearchElement)
                    {
                        SuperParentWindowObj = Foundelement;
                    }
                    return Foundelement;
                }
                else
                {
                    if (TerminateStatus == 1)
                    {
                        string TextToWrite = "Unable to find the automation element" + ElementName + " Exiting application";
                        NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                       // NewLogObj.WriteLogFile(LogFilePath, "***Exiting application****", "fail");
                        //Application.Exit();
                      
                        FileObj.ExitTestEnvironment();
                        return null;
                    }
                    else
                    {
                        string TextToWrite = "Unable to find the automation element" + ElementName + TerminateStatus + "is 0";
                        NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                        return null;
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement" + Ex.ToString(), "info");
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element " + ElementName, "info"); ;
                return null;
            }
        }

        //ItemIndex - if more than 1 autoelement with same propcondition, select based on idex
        public AutomationElement FindAutomationElementWithIndex(AutomationElement ParentSearchElement, PropertyCondition PropCond1, PropertyCondition PropCond2, TreeScope Scope, string ElementName, int ItemIndex, int TimeOut, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            string ElementIdentifer1 = null;
            string ElementIdentifer2 = null;
            FileOperations FileObj = new FileOperations();
            if (PropCond1 != null)
            {
                ElementIdentifer1 = PropCond1.Value.ToString();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCond1 is null at FindAutomationElement", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
            if (PropCond2 != null)
            {
                ElementIdentifer2 = PropCond2.Value.ToString();
            }
            try
            {
                NewLogObj.WriteLogFile(LogFilePath, "Starting serach with propertycondition1 " + ElementIdentifer1, "info");
                AutomationElementCollection SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond1);
                AutomationElement SearchElement = null;
                int Timer = 0;
                if (SearchElementColl.Count >= ItemIndex)
                {
                    //Index in array starts from 0, where as ppl specify indexfrm 1
                    SearchElement = SearchElementColl[ItemIndex - 1];
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = SearchElement;
                    }
                    return SearchElement;
                }
                else
                {
                    while (SearchElement == null && Timer <= TimeOut)
                    {
                        Thread.Sleep(500);
                        Timer = Timer + 500;
                        SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond1);
                        if (SearchElementColl.Count >= ItemIndex)
                        {
                            //Index in array starts from 0, where as ppl specify indexfrm 1
                            SearchElement = SearchElementColl[ItemIndex - 1];
                            return SearchElement;
                        }

                    }
                    if (SearchElement == null && Timer >= TimeOut)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element not found with propertycondition1 " + ElementIdentifer1, "info");
                        NewLogObj.WriteLogFile(LogFilePath, "Strating search with propertycondition2 " + ElementIdentifer2, "info");
                        Timer = 0;
                        //Starting serach with 2nd prop condition
                        SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond2);

                        if (SearchElementColl.Count >= ItemIndex)
                        {
                            //Index in array starts from 0, where as ppl specify indexfrm 1
                            SearchElement = SearchElementColl[ItemIndex - 1];
                            return SearchElement;
                        }
                        Timer = 0;
                        while (SearchElementColl == null && Timer <= TimeOut)
                        {
                            Thread.Sleep(1000);
                            Timer = Timer + 1000;
                            SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond2);
                            if (SearchElementColl.Count >= ItemIndex)
                            {
                                //Index in array starts from 0, where as ppl specify indexfrm 1
                                SearchElement = SearchElementColl[ItemIndex - 1];
                                return SearchElement;
                            }

                        }
                    }
                    if (SearchElement == null && Timer >= TimeOut)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element not found with propertycondition2 "+ElementIdentifer2+" also", "info");
                        if (TerminateStatus == 1)
                        {
                            string TextToWrite = "Unable to find the automation element with " + ElementIdentifer1 + "and "+ElementIdentifer2+ " Exiting application";
                            NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                            //NewLogObj.WriteLogFile(LogFilePath, "***Exiting application****", "fail");
                            //Application.Exit();
                            
                            FileObj.ExitTestEnvironment();
                            return null;
                        }
                        else
                        {
                            string TextToWrite = "Unable to find the automation element with " + ElementIdentifer1 + "and " + ElementIdentifer2;
                            NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                            return null;
                        }
                    }
                }
                return null;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement in search with " + ElementIdentifer1 + "and " + ElementIdentifer2 + Ex.ToString(), "info");
                return null;
            }
        }

        //ItemIndex - if more than 1 autoelement with same propcondition, select based on idex
        public AutomationElement FindAutomationElementWithIndex(AutomationElement ParentSearchElement, PropertyCondition PropCond, TreeScope Scope, string ElementName, int ItemIndex,int TimeoutInSecs,int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            AutomationElement SearchElement=null;
            FileOperations FileObj = new FileOperations();
            string ElementIdentifer =null;
            if (PropCond != null)
            {
                ElementIdentifer = PropCond.Value.ToString();
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "PropCond is null at FindAutomationElement", "fail");
                FileObj.ExitTestEnvironment();
                return null;
            }
            try
            {
                //AutomationElement Foundelement = ParentSearchElement.FindFirst(Scope, PropCond);
                AutomationElementCollection SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond);
                if (SearchElementColl.Count >= ItemIndex)
                {
                   //Index in array starts from 0, where as ppl specify indexfrm 1
                    SearchElement = SearchElementColl[ItemIndex - 1];
                    if (AutomationElement.RootElement == ParentSearchElement)
                    {
                        SuperParentWindowObj = SearchElement;
                    }
                    return SearchElement;
                }
                else
                {
                    int timer = 0;
                    while (SearchElement == null && timer <= TimeoutInSecs)
                    {
                        Thread.Sleep(500);
                        timer = timer + 500;
                        SearchElementColl = ParentSearchElement.FindAll(Scope, PropCond);
                        if (SearchElementColl.Count >= ItemIndex)
                        {
                            //Index in array starts from 0, where as ppl specify indexfrm 1
                            SearchElement = SearchElementColl[ItemIndex - 1];
                            return SearchElement;
                        }

                    }
                    if (SearchElement == null && timer >= TimeoutInSecs)
                    {
                        //NewLogObj.WriteLogFile(LogFilePath, "Unable to find the element "+ElementIdentifer+" with index " + ItemIndex, "fail");
                        if (TerminateStatus == 1)
                        {
                            string TextToWrite = "Unable to find the automation element " + ElementIdentifer + " Exiting application";
                            NewLogObj.WriteLogFile(LogFilePath, TextToWrite, "fail");
                            // NewLogObj.WriteLogFile(LogFilePath, "***Exiting application****", "fail");
                            //Application.Exit();

                            FileObj.ExitTestEnvironment();
                            return null;
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element " + ElementIdentifer + " with index " + ItemIndex + "Resuming as TerminateOnFailure is no", "warn");
                        }
                        return null;
                    }
                    return null;
                }
                
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at FindAutomationElement in search with " +ElementIdentifer+" ."+ Ex.ToString(), "info");
                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element " + ElementIdentifer, "info"); ;
                return null;
            }

        }
        
        public void ClickButton(AutomationElement ButtonObj, int SetFocus, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
                       
            try
            {

                if (ButtonObj != null)
                {
                    if (ButtonObj.Current.AutomationId != null)
                    {
                        Console.WriteLine("sending click on btn " + ButtonObj.Current.AutomationId);
                    }
                    
                    if (SetFocus == 1)
                    {
                        //Set focus to the button in case it is not active
                        ButtonObj.SetFocus();
                    }
                    //Get invoke pattern and invoke it to press the button
                    NewLogObj.WriteLogFile(LogFilePath, "sending click on " + ElementName, "info");
                    InvokePattern invPattern = ButtonObj.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    invPattern.Invoke();
                    Thread.Sleep(1000);
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform ClickButton", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickButton as object" + ElementName + "not found**", "fail");
                        //Application.Exit();
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
                NewLogObj.WriteLogFile(LogFilePath, "Execption at ClickButton" + Ex.ToString(), "info");
               
            }


        }

        public void ClickHyperLink(AutomationElement HyperlinkObj, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {

                if (HyperlinkObj != null)
                {
                    
                    //Get invoke pattern and invoke it to press the button
                    NewLogObj.WriteLogFile(LogFilePath, "sending click on " + ElementName, "info");
                    int PatternSupport=CheckIfPatternIsSupported(HyperlinkObj, "InvokePattern");
                    if (PatternSupport == 1)
                    {
                        InvokePattern invPattern = HyperlinkObj.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        invPattern.Invoke();
                    }
                    else
                    {
                        //GetPositionFromBoundingRectangleAndClickBasedOnInput(HyperlinkObj, LogFilePath, "Left", "Left", 20);
                        GetPositionFromBoundingRectangleAndClick(HyperlinkObj, LogFilePath, "Left");
                    }
                    Thread.Sleep(1000);
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform ClickButton", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickButton as object" + ElementName + "not found**", "fail");
                        //Application.Exit();
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
                NewLogObj.WriteLogFile(LogFilePath, "Execption at ClickHyperLink" + Ex.ToString(), "info");

            }


        }


        public void SetTextBoxText(AutomationElement TextBoxObj, String TextToSet, string ElementName, int TerminateStatus, string LogFilePath)
        {
            
            Logger NewLogObj = new Logger();
            NewLogObj.WriteLogFile(LogFilePath, "SetTextBoxText", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===============", "info");
            NewLogObj.WriteLogFile(LogFilePath, "setting textbox " + ElementName+" to "+TextToSet, "info");
            try
            {
                if (TextBoxObj != null)
                {

                    ValuePattern valPattern = TextBoxObj.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    valPattern.SetValue(TextToSet);
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform SetTextBoxText", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from SetTextBoxText as object" + ElementName + "not found**", "fail");
                        //Application.Exit();
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");

                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at SetTextBoxText" + Ex.ToString(), "info");
               
            }

        }


        public string GetTextBoxText(AutomationElement TextBoxObj, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            object objPattern;
            try
            {
                if (TextBoxObj != null)
                {
                    if (TextBoxObj.TryGetCurrentPattern(ValuePattern.Pattern, out objPattern) == true)
                    {
                        ValuePattern valPattern = TextBoxObj.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        String controlText = valPattern.Current.Value;
                        return controlText;
                    }
                    else if (TextBoxObj.TryGetCurrentPattern(ValuePattern.Pattern, out objPattern) == true)
                    {
                        TextPattern txtPattern = TextBoxObj.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                        String controlText = txtPattern.DocumentRange.GetText(-1);
                        return controlText;
                    }
                    return null;

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform SetTextBoxText", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from SetTextBoxText as object" + ElementName + "not found**", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        //Application.Exit();
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");

                    }
                    return null;
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at SetTextBoxText" + Ex.ToString(), "info");
                return null;
            }

        }

        public int SetDocumentObjText(AutomationElement DocumentObj, String TextToSet, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                if (DocumentObj != null)
                {
                    DocumentObj.SetFocus();
                    object objPattern;
                    if (DocumentObj.TryGetCurrentPattern(ValuePattern.Pattern, out objPattern) == true)
                    {
                        ValuePattern ValPattern = DocumentObj.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        NewLogObj.WriteLogFile(LogFilePath, "Valuepattern supported. Setting the btext", "info");
                        ValPattern.SetValue(TextToSet);
                        return 1;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Valuepattern not supported. Text cannot be set by this method", "info");
                        //Applying workaround
                        //GetClickablePointAndClick(DocumentObj, "Document obj", 1);
                        //GetPositionFromBoundingRectangleAndClick(DocumentObj, LogFilePath, "Left");
                        GetPositionFromBoundingRectangleAndClickBasedOnInput(DocumentObj, LogFilePath, "Left", "Left", 10);
                        Thread.Sleep(1000);
                        System.Windows.Forms.SendKeys.SendWait(TextToSet);
                        //System.Windows.Forms.Clipboard.SetDataObject(TextToSet);
                        //System.Windows.Forms.SendKeys.SendWait("^(v)");
                        Thread.Sleep(1000);
                        return 1;
                    }
                    
                    
                  }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform SetDocumentObjText", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from SetDocumentObjText as object" + ElementName + "not found**", "fail");
                        //Application.Exit();
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                        return -1;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                        return -1;

                    }
                }
                
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at SetDocumentObjText" + Ex.ToString(), "info");
                return 0;
            }
        }

        public string GetDocumentObjText(AutomationElement DocumentObj, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                if (DocumentObj != null)
                {
                    DocumentObj.SetFocus();
                    TextPattern TxtPattern = DocumentObj.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                    string text = TxtPattern.DocumentRange.GetText(-1);
                    return text;

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform SetDocumentObjText", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from SetDocumentObjText as object" + ElementName + "not found**", "fail");
                        //Application.Exit();
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");

                    }
                    return null;
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at GetDocumentObjText" + Ex.ToString(), "info");
                return null;

            }
        }

        public void SetRadioButton(AutomationElement RadioBtnObj, string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                if (RadioBtnObj != null)
                {
                    SelectionItemPattern p = RadioBtnObj.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                    p.Select();
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform SetRadioButton", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");

                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from SetRadioButton as object" + ElementName + "not found**", "fail");

                        //Application.Exit();
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
                NewLogObj.WriteLogFile(LogFilePath, "Execption at SetRadioButton" + Ex.ToString(), "info");
            }
        }

        //Return the sate of expandcollpasib;e item
        //Can also use this to verify if the item is expandable or is the leaf node
        //Application - In Xencenter when no servers are added, Xencenter will be the leaf node
        
        public string ReturnExpandCollapseState(AutomationElement Element)
        {
            ExpandCollapsePattern expPattern = Element.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            return expPattern.Current.ExpandCollapseState.ToString();
        }

        public void SelectTabItem(AutomationElement TabItemObj,string ElementName, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                if (TabItemObj != null)
                {
                    TabItemObj.SetFocus();
                   
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, ElementName + "object is null" + "Cannot perform SetDocumentObjText", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from SelectTabItem as object" + ElementName + "not found**", "fail");
                        //Application.Exit();
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
                NewLogObj.WriteLogFile(LogFilePath, "Execption at SelectTabItem" + Ex.ToString(), "info");

            }
        }

        //Can be used to select child items in tree having no name or autoid to distingusih
        public AutomationElement SelectItemFromTreeBasedOnIndex(AutomationElement ParentElement, int ChildItemIndex, int ClickRequired, string MouseBtn, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            TestAPI TestApiObj = new TestAPI();
            int ElementFound = 0;
            AutomationElement FoundElement = null;
            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromTreeBasedOnIndex", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                //Check if the parent elemnt passed is a leaf node or can be expanded
                int PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentElement, "ExpandCollapse");
                if (PatternSupport == 1)
                {
                    ExpandCollapsePattern expPattern = ParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                    string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                    NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                    if (string.Compare(ExpandState, "LeafNode") == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + ParentElement.Current.Name + " Cannot traverse the tree anymore ", "info");
                        return null;
                    }
                }
                while (ElementFound == 0)
                {
                    //Get all the tree items under the xencenter node
                    PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                    
                    AutomationElementCollection NodeColl = ParentElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                    AutomationElement ChildElement = null;
                    if (NodeColl.Count > ChildItemIndex)
                    {
                        ChildElement = NodeColl[ChildItemIndex];
                        if (ClickRequired == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Sending click on element found " + ChildElement.Current.Name, "info");
                            // if(Regex.IsMatch(MouseBtn,"left",RegexOptions.CultureInvariant))
                            if (Regex.IsMatch(MouseBtn, "left", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickLeftBtnOnAutomationElement(ChildElement, 1, LogFilePath);
                            }
                            else if (Regex.IsMatch(MouseBtn, "right", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickRightBtnOnAutomationElement(ChildElement, 1, LogFilePath);
                            }
                        }
                        ElementFound = 1;
                        FoundElement = ChildElement;
                        return FoundElement;
                    }
                }
                return FoundElement;
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at CheckTreeForElementAndClickIfReqd" + Ex.ToString(), "info");
                return FoundElement;
            }

        }

        // ############################################################################################################################
        //Will traverse a tree element to get the search element
        // ############################################################################################################################
        //public AutomationElement GetTheTreeElement(AutomationElement ParentElement, string ElementToClick, string LogFilePath)
        public AutomationElement CheckTreeForElementAndClickIfReqd(AutomationElement ParentElement, string ChildIdentityType, string ChildIdentity, int ClickRequired, string MouseBtn,int TerminateStatus,string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            TestAPI TestApiObj=new TestAPI();
            int ElementFound = 0;
            AutomationElement FoundElement = null;
            NewLogObj.WriteLogFile(LogFilePath, "CheckTreeForElementAndClickIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                if (Regex.IsMatch(ChildIdentityType, "name", RegexOptions.IgnoreCase))
                {

                    string ParentElementName = ParentElement.Current.Name;
                    if (string.Compare(ParentElementName, ChildIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element found in tree " + ParentElementName, "info");
                        if (ClickRequired == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Sending click on element found " + ParentElement.Current.Name, "info");
                            // if(Regex.IsMatch(MouseBtn,"left",RegexOptions.CultureInvariant))
                            if (Regex.IsMatch(MouseBtn, "left", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickLeftBtnOnAutomationElement(ParentElement, 1, LogFilePath);
                            }
                            else if (Regex.IsMatch(MouseBtn, "right", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickRightBtnOnAutomationElement(ParentElement, 1, LogFilePath);
                            }
                        }
                        return ParentElement;
                    }
                }
                else if (Regex.IsMatch(ChildIdentityType, "automationid", RegexOptions.IgnoreCase))
                {

                    string ParentElementAutoId = ParentElement.Current.AutomationId;
                    if (string.Compare(ParentElementAutoId, ChildIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element found in tree " + ParentElementAutoId, "info");
                        if (ClickRequired == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Sending click on element found " + ParentElement.Current.Name, "info");
                            // if(Regex.IsMatch(MouseBtn,"left",RegexOptions.CultureInvariant))
                            if (Regex.IsMatch(MouseBtn, "left", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickLeftBtnOnAutomationElement(ParentElement, 1, LogFilePath);
                            }
                            else if (Regex.IsMatch(MouseBtn, "right", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickRightBtnOnAutomationElement(ParentElement, 1, LogFilePath);
                            }
                        }
                        return ParentElement;
                    }
                }
                //Check if the parent elemnt passed is a leaf node or can be expanded
                int PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentElement, "ExpandCollapse");
                if (PatternSupport == 1)
                {
                    ExpandCollapsePattern expPattern = ParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                    string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                    NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                    if (string.Compare(ExpandState, "LeafNode") == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + ParentElement.Current.Name + " Cannot traverse the tree anymore ", "info");
                        return null;
                    }
                    if (string.Compare(expPattern.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                    {
                        expPattern.Expand();
                        Thread.Sleep(1000);
                    }
                }
                while (ElementFound == 0)
                {
                    //Get all the tree items under the xencenter node
                    PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                    NewLogObj.WriteLogFile(LogFilePath, "Starting serach in the parent tree for " + ChildIdentity, "info");
                    AutomationElementCollection NodeColl = ParentElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                    //Flag set to 0 when the serach on a subtree starts
                    //Expanding all the nodes before starting the serach
                    foreach (AutomationElement AutoObj in NodeColl)
                    {
                        PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                        if (PatternSupport == 1)
                        {
                            ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                            {
                                expPattern2.Expand();
                                Thread.Sleep(500);
                            }
                        }
                    }
                    //Searching once more after expand
                    NodeColl = ParentElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                    foreach (AutomationElement AutoObj in NodeColl)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Current Tree element name in the Xencenter tree " + AutoObj.Current.Name, "info");
                            
                            //SearchCriteria="AutoObj.Current.Name;
                           if((Regex.IsMatch(ChildIdentityType,"name",RegexOptions.IgnoreCase)) || (Regex.IsMatch(ChildIdentityType,"automationid",RegexOptions.IgnoreCase)))
                            {
                                if ((string.Compare(AutoObj.Current.Name, ChildIdentity) == 0)||string.Compare(AutoObj.Current.AutomationId, ChildIdentity) == 0)
                                {
                                    // Required element found
                                    NewLogObj.WriteLogFile(LogFilePath, "Required element found " + AutoObj.Current.Name, "info");
                                    PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                                    if (PatternSupport == 1)
                                    {
                                        ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                        if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                                        {
                                            expPattern2.Expand();
                                            Thread.Sleep(1000);
                                        }
                                    }
                                    if (ClickRequired == 1)
                                    {
                                        NewLogObj.WriteLogFile(LogFilePath, "Sending click on element found " + AutoObj.Current.Name, "info");
                                        // if(Regex.IsMatch(MouseBtn,"left",RegexOptions.CultureInvariant))
                                        if (Regex.IsMatch(MouseBtn, "left", RegexOptions.IgnoreCase))
                                        {
                                            TestApiObj.ClickLeftBtnOnAutomationElement(AutoObj, 1, LogFilePath);
                                        }
                                        else if (Regex.IsMatch(MouseBtn, "right", RegexOptions.IgnoreCase))
                                        {
                                            TestApiObj.ClickRightBtnOnAutomationElement(AutoObj, 1, LogFilePath);
                                        }
                                    }
                                    ElementFound = 1;
                                    FoundElement = AutoObj;
                                    return FoundElement;
                                }
  
                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Expanding tree item", "info");
                                PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                                if (PatternSupport == 1)
                                {
                                    ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                    
                                    if(string.Compare(expPattern2.Current.ExpandCollapseState.ToString(),"LeafNode")!=0)
                                    {
                                        expPattern2.Expand();
                                        Thread.Sleep(500);
                                    }
                                }
                            }
                        }

                    }
                    if (ElementFound == 0)
                    {
                        String ParentIdentity = null;
                        string ParentTreename = ParentElement.Current.Name;
                        if (ParentTreename.Length > 0)
                        {
                            ParentTreename = ParentIdentity;
                        }
                        else
                        {
                            ParentIdentity = ParentElement.Current.AutomationId;
                        }

                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element of type" + ChildIdentityType + " - " + ChildIdentity + " in parent tree " + ParentIdentity + " Exiting...", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element of type" + ChildIdentityType + " - " + ChildIdentity + " in parent tree " + ParentIdentity + " Resuming as TerminateOnFailure is no", "warn");
                        }
                    }
                    //If control comes out of loop, mans that element not found
                    //Has to traverse each tree element again to find the 
                    
                    //foreach (AutomationElement AutoObj in NodeColl)
                    //{
                    //    FoundElement = GuiObj.CheckTreeForElementAndClickIfReqd(AutoObj, ChildIdentityType,ChildIdentity,ClickRequired,MouseBtn, LogFilePath);
                    //    if (FoundElement != null)
                    //    {
                    //        return FoundElement;
                    //    }
                        
                    //}
                    return FoundElement;
                    
                }
                return FoundElement;

            }
            catch(Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at CheckTreeForElementAndClickIfReqd" + Ex.ToString(), "info");
                return FoundElement;
            }

        }

        public AutomationElement CheckTreeForElementAndExpandIfReqd(AutomationElement ParentElement, string ChildIdentityType, string ChildIdentity, int ExpansionRequired, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            TestAPI TestApiObj = new TestAPI();
            int ElementFound = 0;
            AutomationElement FoundElement = null;
            NewLogObj.WriteLogFile(LogFilePath, "CheckTreeForElementAndExpandIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            int PatternSupport;
            ExpandCollapsePattern expPattern;
            try
            {
                if (Regex.IsMatch(ChildIdentityType, "name", RegexOptions.IgnoreCase))
                {

                    string ParentElementName = ParentElement.Current.Name;
                    
                    if (string.Compare(ParentElementName, ChildIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element found in tree " + ParentElementName, "info");
                        PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentElement, "ExpandCollapse");
                        if (PatternSupport == 1)
                        {
                            expPattern = ParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            expPattern.Expand();
                        }
                        //Expanding all tree items
                        PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                       
                        AutomationElementCollection NodeColl = ParentElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                        foreach (AutomationElement AutoObj in NodeColl)
                        {
                            PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                            if (PatternSupport == 1)
                            {
                                expPattern = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                expPattern.Expand();
                            }
                        }
                        return ParentElement;
                    }
                }
                else if (Regex.IsMatch(ChildIdentityType, "automationid", RegexOptions.IgnoreCase))
                {

                    string ParentElementAutoId = ParentElement.Current.AutomationId;
                    if (string.Compare(ParentElementAutoId, ChildIdentity) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element found in tree " + ParentElementAutoId, "info");
                        PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentElement, "ExpandCollapse");
                        if (PatternSupport == 1)
                        {
                            expPattern = ParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            expPattern.Expand();
                        }
                        //Expanding all tree items
                        PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);

                        AutomationElementCollection NodeColl = ParentElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                        foreach (AutomationElement AutoObj in NodeColl)
                        {
                            PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                            if (PatternSupport == 1)
                            {
                                expPattern = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                expPattern.Expand();
                            }
                        }
                        return ParentElement;
                    }
                }
                //Check if the parent elemnt passed is a leaf node or can be expanded
                PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentElement, "ExpandCollapse");
                if (PatternSupport == 1)
                {
                    expPattern = ParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                    string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                    NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                    if (string.Compare(ExpandState, "LeafNode") == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + ParentElement.Current.Name + " Cannot traverse the tree anymore ", "info");
                        return null;
                    }
                }
                while (ElementFound == 0)
                {
                    //Get all the tree items under the xencenter node
                    PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                    NewLogObj.WriteLogFile(LogFilePath, "Starting serach in the parent tree for " + ChildIdentity, "info");
                    AutomationElementCollection NodeColl = ParentElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                    //Flag set to 0 when the serach on a subtree starts

                    foreach (AutomationElement AutoObj in NodeColl)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Current Tree element name in the Xencenter tree " + AutoObj.Current.Name, "info");

                        //SearchCriteria="AutoObj.Current.Name;
                        if ((Regex.IsMatch(ChildIdentityType, "name", RegexOptions.IgnoreCase)) || (Regex.IsMatch(ChildIdentityType, "automationid", RegexOptions.IgnoreCase)))
                        {
                            if ((string.Compare(AutoObj.Current.Name, ChildIdentity) == 0) || string.Compare(AutoObj.Current.AutomationId, ChildIdentity) == 0)
                            {
                                // Required element found
                                NewLogObj.WriteLogFile(LogFilePath, "Required element found " + AutoObj.Current.Name, "info");
                                PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                                if (PatternSupport == 1)
                                {
                                    ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                    Thread.Sleep(1000);
                                    if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                                    {
                                        expPattern2.Expand();
                                    }
                                }
                                
                                ElementFound = 1;
                                FoundElement = AutoObj;
                                return FoundElement;
                            }

                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Expanding tree item", "info");
                                PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                                if (PatternSupport == 1)
                                {
                                    ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                    Thread.Sleep(1000);
                                    if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "LeafNode") != 0)
                                    {
                                        expPattern2.Expand();
                                    }
                                }
                            }
                        }

                    }
                    //If control comes out of loop, mans that element not found
                    //Has to traverse each tree element again to find the 

                    foreach (AutomationElement AutoObj in NodeColl)
                    {
                        FoundElement = GuiObj.CheckTreeForElementAndExpandIfReqd(AutoObj, ChildIdentityType, ChildIdentity, ExpansionRequired, LogFilePath);
                        if (FoundElement != null)
                        {
                            return FoundElement;
                        }

                    }
                    return FoundElement;

                }
                return FoundElement;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at CheckTreeForElementAndClickIfReqd" + Ex.ToString(), "info");
                return FoundElement;
            }

        }
       
        public int CheckSubTreeForElementAndClickIfReqd(AutomationElement ParentTreeElement, string SubTreeParentElementName, string SubTreeChildElementToClickName, int ClickRequired, string MouseBtn, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            TestAPI TestApiObj = new TestAPI();

            NewLogObj.WriteLogFile(LogFilePath, "CheckSubTreeForElementAndClickIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                if (ParentTreeElement == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, ParentTreeElement + " is null ", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckSubTreeForElementAndClickIfReqd ", "fail");
                        //Application.Exit();
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return -1;
                }
                string ParentElementName = ParentTreeElement.Current.Name;
                AutomationElement SubTreeParentElement = null;
                int SubTreeParentElementFound = 0;
                AutomationElement SubTreeChildElement = null;
                int SubTreeChildElementFound = 0;
                if (string.Compare(ParentElementName, SubTreeParentElementName) == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, ParentTreeElement + " ParentTreeElement " + SubTreeParentElementName + " SubTreeParentElementName are same ", "info");
                    SubTreeParentElement = ParentTreeElement;
                    SubTreeParentElementFound = 1;
                    int PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentTreeElement, "ExpandCollapse");
                    if (PatternSupport == 1)
                    {
                        ExpandCollapsePattern expPattern = ParentTreeElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                        string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                        NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                        if (string.Compare(ExpandState, "LeafNode") == 0)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + ParentElementName + " Cannot traverse the tree anymore ", "fail");
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                                NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckSubTreeForElementAndClickIfReqd ", "fail");
                                //Application.Exit();
                                FileOperations FileObj = new FileOperations();
                                FileObj.ExitTestEnvironment();
                                return -1;
                            }
                            return -1;
                        }
                    }

                }
                else
                {
                    //Find the subtree parent automation element
                    PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                    NewLogObj.WriteLogFile(LogFilePath, "Starting serach in the parent tree for " + SubTreeParentElementName, "info");
                    AutomationElementCollection NodeColl = ParentTreeElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                    //Flag set to 0 when the serach on a subtree starts

                    foreach (AutomationElement AutoObj in NodeColl)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Current Tree element name in the tree " + AutoObj.Current.Name, "info");
                        if (string.Compare(AutoObj.Current.Name, SubTreeParentElementName) == 0)
                        {
                            // Required element found
                            NewLogObj.WriteLogFile(LogFilePath, "Required SubTree Parent element found " + AutoObj.Current.Name, "info");
                            SubTreeParentElementFound = 1;
                            SubTreeParentElement = AutoObj;
                            int PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                            if (PatternSupport == 1)
                            {
                                ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                Thread.Sleep(1000);
                                if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                                {
                                    expPattern2.Expand();
                                }
                                else if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "LeafNode") == 0)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at SubTreeParentElementName " + SubTreeParentElementName + " Cannot traverse the tree anymore ", "fail");
                                    if (TerminateStatus == 1)
                                    {
                                        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                                        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckSubTreeForElementAndClickIfReqd ", "fail");
                                        //Application.Exit();
                                        FileOperations FileObj = new FileOperations();
                                        FileObj.ExitTestEnvironment();
                                        return -1;
                                    }
                                    return -1;
                                }
                            }
                            break;

                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Expanding tree item", "info");
                            int PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                            if (PatternSupport == 1)
                            {
                                ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                Thread.Sleep(1000);
                                if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "LeafNode") != 0)
                                {
                                    expPattern2.Expand();
                                }
                            }
                        }

                    }
                    if (SubTreeParentElementFound == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, SubTreeParentElementFound + " is null ", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                            NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckSubTreeForElementAndClickIfReqd ", "fail");
                            //Application.Exit();
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                            
                        }
                        return -1;
                    }
                }

                //Check for all children under subtree
                PropertyCondition SubTreeChildItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                NewLogObj.WriteLogFile(LogFilePath, "Starting serach in the sub tree parent  for " + SubTreeChildElementToClickName, "info");
                AutomationElementCollection SubTreeNodeColl = SubTreeParentElement.FindAll(TreeScope.Descendants, SubTreeChildItemCondition);


                foreach (AutomationElement AutoObj in SubTreeNodeColl)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Current Tree element name in the Xencenter tree " + AutoObj.Current.Name, "info");
                    if (string.Compare(AutoObj.Current.Name, SubTreeChildElementToClickName) == 0)
                    {
                        // Required element found
                        NewLogObj.WriteLogFile(LogFilePath, "Required element found " + AutoObj.Current.Name, "info");
                        SubTreeChildElement = AutoObj;
                        SubTreeChildElementFound = 1;
                        int PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                        if (PatternSupport == 1)
                        {
                            ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            Thread.Sleep(1000);
                            if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "Collapsed") == 0)
                            {
                                expPattern2.Expand();
                            }
                        }
                        if (ClickRequired == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Sending click on element found " + AutoObj.Current.Name, "info");
                            // if(Regex.IsMatch(MouseBtn,"left",RegexOptions.CultureInvariant))
                            if (Regex.IsMatch(MouseBtn, "left", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickLeftBtnOnAutomationElement(AutoObj, 1, LogFilePath);
                            }
                            else if (Regex.IsMatch(MouseBtn, "right", RegexOptions.IgnoreCase))
                            {
                                TestApiObj.ClickRightBtnOnAutomationElement(AutoObj, 1, LogFilePath);
                            }
                        }

                        return 1;

                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Expanding tree item", "info");
                        int PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                        if (PatternSupport == 1)
                        {
                            ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            Thread.Sleep(1000);
                            if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "LeafNode") != 0)
                            {
                                expPattern2.Expand();
                            }
                        }
                    }

                }
                return -1;
                ////If control comes out of loop, mans that element not found
                ////Has to traverse each tree element again to find the 

                //foreach (AutomationElement AutoObj in NodeColl)
                //{
                //    FoundElement = GuiObj.CheckTreeForElementAndClickIfReqd(AutoObj, ElementToClick, ClickRequired, MouseBtn, LogFilePath);
                //    if (FoundElement != null)
                //    {
                //        return FoundElement;
                //    }

                //}
                //return FoundElement;


                //return FoundElement;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at CheckTreeForElementAndClickIfReqd" + Ex.ToString(), "info");
                return -1;
            }

        }

        public int CheckTreeIfElementIsLeafNodeAndTerminateIfReqd(AutomationElement ParentElement, string ElementToCheck,int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            TestAPI TestApiObj = new TestAPI();
            int ElementFound = 0;
            AutomationElement FoundElement = null;
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            NewLogObj.WriteLogFile(LogFilePath, "CheckTreeIfElementIsLeafNodeAndTerminateIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=========================================", "info");
            try
            {
                string ParentElementName = ParentElement.Current.Name;
                int PatternSupport;
                if (string.Compare(ParentElementName, ElementToCheck) == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Element found in tree " + ParentElementName, "info");
                    if (string.Compare(ParentElementName, ElementToCheck) == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Element to check i sthe parent element ", "info");
                        PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentElement, "ExpandCollapse");
                        if (PatternSupport == 1)
                        {
                            ExpandCollapsePattern expPattern = ParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                            NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                            if (string.Compare(ExpandState, "LeafNode") == 0)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + ParentElementName , "info");
                                if (TerminateStatus == 1)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + ParentElementName , "fail");
                                    // if(Regex.IsMatch(MouseBtn,"left",RegexOptions.CultureInvariant))
                                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckTreeIfElementIsLeafNodeAndTerminateIfReqd ", "fail");
                                    //Application.Exit();
                                    FileOperations FileObj = new FileOperations();
                                    FileObj.ExitTestEnvironment();
                                    return -1;
                                   
                                }
                            }
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "ExpandCollapse pattern not supported in the Parent tree passed ", "fail");
                            NewLogObj.WriteLogFile(LogFilePath, "Cannot check for leaf node", "fail");
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                                NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckTreeIfElementIsLeafNodeAndTerminateIfReqd ", "fail");
                                //Application.Exit();
                                FileOperations FileObj = new FileOperations();
                                FileObj.ExitTestEnvironment();
                                return -1;
                            }                            
                        }
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Leaf node not found at  "+ElementToCheck, "info");
                    return -1;
                }
                //Check if the parent elemnt passed is a leaf node or can be expanded
                PatternSupport = GuiObj.CheckIfPatternIsSupported(ParentElement, "ExpandCollapse");
                if (PatternSupport == 1)
                {
                    ExpandCollapsePattern expPattern = ParentElement.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                    string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                    NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                    if (string.Compare(ExpandState, "LeafNode") == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + ParentElementName + " Cannot traverse the tree anymore ", "info");
                        NewLogObj.WriteLogFile(LogFilePath, "Search element "+ElementToCheck+"  not found at tree" + ParentElementName + " Cannot traverse the tree anymore ", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                            NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckTreeIfElementIsLeafNodeAndTerminateIfReqd ", "fail");
                            //Application.Exit();
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                            return -1;
                        }  
                        return -1;
                    }
                }
                while (ElementFound == 0)
                {
                    //Get all the tree items under the xencenter node
                    PropertyCondition TreeItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "TreeItem", 1, LogFilePath);
                    NewLogObj.WriteLogFile(LogFilePath, "Starting serach in the parent tree for " + ElementToCheck, "info");
                    AutomationElementCollection NodeColl = ParentElement.FindAll(TreeScope.Descendants, TreeItemCondition);
                    //Flag set to 0 when the serach on a subtree starts

                    foreach (AutomationElement AutoObj in NodeColl)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Current Tree element name in the Xencenter tree " + AutoObj.Current.Name, "info");
                        if (string.Compare(AutoObj.Current.Name, ElementToCheck) == 0)
                        {
                            // Required element found
                            NewLogObj.WriteLogFile(LogFilePath, "Required element found " + AutoObj.Current.Name, "info");
                            PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                            if (PatternSupport == 1)
                            {
                                ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                Thread.Sleep(1000);
                                if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "LeafNode") == 0)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Leaf node found at " + AutoObj.Current.Name , "fail");
                                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckTreeIfElementIsLeafNodeAndTerminateIfReqd ", "fail");
                                    
                                    FileOperations FileObj = new FileOperations();
                                    FileObj.ExitTestEnvironment();
                                    return -1;
                                }
                                else 
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Leaf node not found at" + AutoObj.Current.Name, "info");
                                    return -1;
                                }
                            }
                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "ExpandCollapse pattern not supported in the tree passed ", "fail");
                                NewLogObj.WriteLogFile(LogFilePath, "Cannot check for leaf node", "fail");
                                if (TerminateStatus == 1)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from CheckTreeIfElementIsLeafNodeAndTerminateIfReqd ", "fail");
                                    //Application.Exit();
                                    FileOperations FileObj = new FileOperations();
                                    FileObj.ExitTestEnvironment();
                                    return -1;
                                }   
                            }
                            ElementFound = 1;
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Expanding tree item", "info");
                            PatternSupport = GuiObj.CheckIfPatternIsSupported(AutoObj, "ExpandCollapse");
                            if (PatternSupport == 1)
                            {
                                ExpandCollapsePattern expPattern2 = AutoObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                                Thread.Sleep(1000);
                                if (string.Compare(expPattern2.Current.ExpandCollapseState.ToString(), "LeafNode") != 0)
                                {
                                    expPattern2.Expand();
                                }
                            }
                        }
                    }
                    //If control comes out of loop, mans that element not found
                    //Has to traverse each tree element again to find the 

                    foreach (AutomationElement AutoObj in NodeColl)
                    {
                        int ElemntFound = GuiObj.CheckTreeIfElementIsLeafNodeAndTerminateIfReqd(AutoObj, ElementToCheck, TerminateStatus, LogFilePath);
                        if (ElemntFound != -1)
                        {
                            return -1;
                        }
                    }
                    return -1;
                }
                return -1;
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at CheckTreeIfElementIsLeafNodeAndTerminateIfReqd" + Ex.ToString(), "info");
                return -1;
            }
        }

        // Select an item from list
        //Can either pass elemnt index to select. for this option set 'SerachByIndex' param to 1. Then list item will be selected based on index
        // if 'SelectByIndex' is set to 0, list will be seahed for element name to select
        public AutomationElement GetListElementAndSelect(AutomationElement ListObj,int SearchByIndex,int Index, string ElemntToSelect, int TermianteStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GetListElementAndSelect", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            NewLogObj.WriteLogFile(LogFilePath, "ElemntToSelect to be select " + ElemntToSelect, "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            if (ListObj == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "ListObj is null" , "fail");
                if (TermianteStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Exiting application from GetListElementAndSelect..", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                return null;
            }
            try
            {
                PropertyCondition ListItemCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "ListItem", 1, LogFilePath);
                NewLogObj.WriteLogFile(LogFilePath, "Starting serach in the parent tree for " + ElemntToSelect, "info");
                AutomationElementCollection NodeColl = ListObj.FindAll(TreeScope.Descendants, ListItemCondition);

                //If list has to be serached based on index
                if (SearchByIndex == 1)
                {
                    //Check if index is with in lmits
                    if (NodeColl.Count > Index)
                    {
                        AutomationElement ItemToSelect = NodeColl[Index];
                        SelectionItemPattern SelectionPattern = ItemToSelect.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                        if (ItemToSelect.Current.IsEnabled)
                        {
                            try
                            {
                                SelectionPattern.Select();
                                Thread.Sleep(2000);
                                return ItemToSelect;
                            }
                            catch (Exception Ex)
                            {
                                ExpandComboBox(ListObj, TermianteStatus);
                                Thread.Sleep(1000);
                                GetPositionFromBoundingRectangleAndClick(ItemToSelect, LogFilePath, "Left");
                                return ItemToSelect;
                            }
                        }
                        else
                        {
                            ExpandComboBox(ListObj, TermianteStatus);
                            Thread.Sleep(1000);
                            GetPositionFromBoundingRectangleAndClick(ItemToSelect, LogFilePath, "Left");
                        }
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, Index + "Index is out of range. Cannot select likst item", "fail");
                        if (TermianteStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from GetListElementAndSelect..", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        return null;

                    }
                }
                // When ist has to be serached for element name

                foreach (AutomationElement AutoObj in NodeColl)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Current List element name in the list " + AutoObj.Current.Name, "info");
                    if (string.Compare(AutoObj.Current.Name, ElemntToSelect) == 0)
                    {
                        // Required element found
                        NewLogObj.WriteLogFile(LogFilePath, "Required element found " + AutoObj.Current.Name, "info");
                        SelectionItemPattern SelectionPattern = AutoObj.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                        SelectionPattern.Select();
                        return AutoObj;

                    }

                }
                return null;
            }
            
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath,"Exception at GetListElementAndSelect " + Ex.ToString(), "fail");
                return null;
            }
        }

        public int MouseMovementAndClickForWindows8(AutomationElement ElementForMouseAction,string MouseBtn,string Position,int XCordinate,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
                if (ElementForMouseAction == null)
                {
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "ElementForMouseAction is null at MouseMovementAndClickForWindows8. Exiting.." , "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "ElementForMouseAction is null at MouseMovementAndClickForWindows8. Resuming as TerminateOnFailure is no", "warn");
                    return -1;
                }
                IntPtr handle = IntPtr.Zero;
                Point point;
                int x = 0;
                int y = 0;
                double width = 0;
                double height = 0;
                int Wid=0;
                int Hig=0;
                bool coordinatesFound = false;
                System.Windows.Point TopLeftPoint = ElementForMouseAction.Current.BoundingRectangle.Location;
                height = ElementForMouseAction.Current.BoundingRectangle.Height;
                width = ElementForMouseAction.Current.BoundingRectangle.Width;
                handle = (IntPtr)ElementForMouseAction.Current.NativeWindowHandle;
                Wid = Convert.ToInt32(width);
                Hig = Convert.ToInt32(height);
                if ((width > 0) && (height > 0))
                {
                    point = new Point();
                    x = Convert.ToInt32(TopLeftPoint.X + (Wid / 2));
                    y = Convert.ToInt32(TopLeftPoint.Y + (Hig / 2));

                    if (string.Compare(Position, "left") == 0)
                    {

                        x = Convert.ToInt32(TopLeftPoint.X) + (XCordinate);
                    }
                    else if (string.Compare(Position, "right") == 0)
                    {

                        x = Convert.ToInt32(TopLeftPoint.X) + (Wid - XCordinate);
                    }
                    else
                    {
                        x = Convert.ToInt32(TopLeftPoint.X + (Wid / 2));
                    }

                    coordinatesFound = ClientToScreen(handle, ref point);
                    SetCursorPos(Convert.ToInt32(point.X + x), Convert.ToInt32(point.Y + y));
                    Thread.Sleep(2000);
                    MouseBtn = MouseBtn.ToLower();
                    if (string.Compare(MouseBtn, "left") == 0)
                    {
                        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    }
                    else if (string.Compare(MouseBtn, "right") == 0)
                    {
                        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                    }
                   Thread.Sleep(2000);
                   
                    return 1;
                }
                else
                {
                    string ObjectName = ElementForMouseAction.Current.Name;
                    string ElementIdentifier = null;
                    if (string.Compare(ObjectName, "") == 0)
                    {
                        string AutoId = ElementForMouseAction.Current.AutomationId;
                        ElementIdentifier = AutoId;
                    }
                    else
                    {
                        ElementIdentifier = ObjectName;
                    }
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Width and Height derived for object "+ElementIdentifier+"  is 0 at MouseMovementAndClickForWindows8. Exiting..", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    NewLogObj.WriteLogFile(LogFilePath, "Width and Height derived for object " + ElementIdentifier + "  is 0 at MouseMovementAndClickForWindows8. Resuming as TerminateOnFailure is no", "warn");
                    return -1;
                }

            }
            catch
            {
                return -1;
            }
        }
        public void GetPositionFromBoundingRectangleAndClick(AutomationElement AutoObj, string LogFilePath, string ClickMouseBtn)
        {
            Logger NewLogObj = new Logger();
            
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                System.Windows.Point TopLeftPoint = AutoObj.Current.BoundingRectangle.Location;
                double Height = AutoObj.Current.BoundingRectangle.Height;
                double Width=AutoObj.Current.BoundingRectangle.Width;
                if (TopLeftPoint != null)
                {
                    double ModifiedXAxis = Convert.ToInt32(TopLeftPoint.X) + (Width / 2);
                    double ModifiedYAxis = Convert.ToInt32(TopLeftPoint.Y) + (Height / 2);
                    //System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(TopLeftPoint.X), Convert.ToInt32(TopLeftPoint.Y));
                    System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(ModifiedXAxis), Convert.ToInt32(ModifiedYAxis));
                    NewLogObj.WriteLogFile(LogFilePath, "Moving mouse to" + AutoObj.Current.Name, "info");
                    NewLogObj.WriteLogFile(LogFilePath, "Bounding rect location  " + Convert.ToInt32(ModifiedXAxis) + Convert.ToInt32(ModifiedYAxis), "info");

                    MouseMovementAndClickForWindows8(AutoObj, ClickMouseBtn, null, 0, 0);
                    //MouseMovementAndClickForWindows8(AutoObj, ClickMouseBtn, "left", 30, 0);
                    //MouseMovementAndClickForWindows8(AutoObj, ClickMouseBtn, Position, XCordinate, 0);
                    //Generic NewGenericObj = new Generic();
                    //string OSName = NewGenericObj.GetOSName();
                    //if ((string.Compare(OSName, "Windows7") == 0))
                    //{
                    //    Microsoft.Test.Input.Mouse.MoveTo(p);
                    //   // Thread.Sleep(3000);
                    //    Thread.Sleep(1000);
                    //    if (string.Compare(ClickMouseBtn, "Left") == 0)
                    //    {
                    //        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    //    }
                    //    else if (string.Compare(ClickMouseBtn, "Right") == 0)
                    //    {
                    //        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                    //    }
                    //   Thread.Sleep(2000);
                       
                    //}
                    //else
                    //{
                    //    MouseMovementAndClickForWindows8(AutoObj, ClickMouseBtn,null,0, 0);
                    //}
                    //Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                    //Thread.Sleep(3000);
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Bounding rect location is null for " + AutoObj.Current.Name, "fail");
                }

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at GetPositionFromBoundingRectangle" + Ex.ToString(), "info");
                
            }

        }

        //For certain controls it is required to send the controls to near left most point/right most point
        //Position as left or right. Will move 'XCordinate' points to left or right
        public void GetPositionFromBoundingRectangleAndClickBasedOnInput(AutomationElement AutoObj, string LogFilePath, string ClickMouseBtn,string Position,int XCordinate)
        {
            Logger NewLogObj = new Logger();

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                System.Windows.Point TopLeftPoint = AutoObj.Current.BoundingRectangle.Location;
                Position = Position.ToLower();
                ClickMouseBtn = ClickMouseBtn.ToLower();

                double Height = AutoObj.Current.BoundingRectangle.Height;
                double Width = AutoObj.Current.BoundingRectangle.Width;
                if (TopLeftPoint != null)
                {
                    double ModifiedXAxis = Convert.ToInt32(TopLeftPoint.X) + (Width / 2);
                    if (string.Compare(Position, "left") == 0)
                    {

                        ModifiedXAxis = Convert.ToInt32(TopLeftPoint.X) + (XCordinate);
                    }
                    else if (string.Compare(Position, "right") == 0)
                    {

                        ModifiedXAxis = Convert.ToInt32(TopLeftPoint.X) + (Width - XCordinate);
                    }
                    double ModifiedYAxis = Convert.ToInt32(TopLeftPoint.Y) + (Height / 2);
                    //System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(TopLeftPoint.X), Convert.ToInt32(TopLeftPoint.Y));
                    System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(ModifiedXAxis), Convert.ToInt32(ModifiedYAxis));
                    NewLogObj.WriteLogFile(LogFilePath, "Moving mouse to" + AutoObj.Current.Name, "info");
                    NewLogObj.WriteLogFile(LogFilePath, "Bounding rect location  " + Convert.ToInt32(ModifiedXAxis) + Convert.ToInt32(ModifiedYAxis), "info");

                    MouseMovementAndClickForWindows8(AutoObj, ClickMouseBtn, Position, XCordinate, 0);
                    //Generic NewGenericObj = new Generic();
                    //string OSName = NewGenericObj.GetOSName();
                    //if ((string.Compare(OSName, "Windows7") == 0))
                    //{
                    //    Microsoft.Test.Input.Mouse.MoveTo(p);
                    //    //Thread.Sleep(2000);
                    //    Thread.Sleep(2000);
                    //    if (string.Compare(ClickMouseBtn, "left") == 0)
                    //    {
                    //        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                    //    }
                    //    else if (string.Compare(ClickMouseBtn, "right") == 0)
                    //    {
                    //        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                    //    }
                    //    Thread.Sleep(2000);
                        
                    //}
                    //else
                    //{
                    //    MouseMovementAndClickForWindows8(AutoObj, ClickMouseBtn,Position,XCordinate, 0);
                    //}
                    //Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                    //Thread.Sleep(3000);
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Bounding rect location is null for " + AutoObj.Current.Name, "fail");
                }

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at GetPositionFromBoundingRectangle" + Ex.ToString(), "info");

            }

        }

        public void ExpandComboBox(AutomationElement ComboBoxObj,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ExpandComboBox", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===============", "info");

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            if (ComboBoxObj == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "SearchElement is null from ExpandComboBox ", "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Exiting application from ExpandComboBox ", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
            }
            int PatternSupport = GuiObj.CheckIfPatternIsSupported(ComboBoxObj, "ExpandCollapse");
            if (PatternSupport == 1)
            {
                ExpandCollapsePattern expPattern = ComboBoxObj.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                string ExpandState = expPattern.Current.ExpandCollapseState.ToString();
                NewLogObj.WriteLogFile(LogFilePath, "ExpandState " + ExpandState, "info");
                expPattern.Expand();
            }
        }
        //#######################################################################################################
        // Can be used to select an item from a container. Select item from combobox, select item from tab control etc
        //#######################################################################################################
        public void SelectItemFromParent(AutomationElement ParentObj, string ItemToSelectType,string ItemToSelect,int terminatestatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromParent", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            try
            {
                PropertyCondition SearchReturnCondition = null;
                ItemToSelectType=ItemToSelectType.ToLower();
                if (Regex.IsMatch(ItemToSelectType,"automationid"))
                {
                    SearchReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ItemToSelect, 1, LogFilePath);
                }
                else if (Regex.IsMatch(ItemToSelectType,"class"))
                {
                    SearchReturnCondition = GuiObj.SetPropertyCondition("ClassNameProperty", ItemToSelect, 0, LogFilePath);
                }
                else if (Regex.IsMatch(ItemToSelectType, "name"))
                {
                    SearchReturnCondition = GuiObj.SetPropertyCondition("NameProperty", ItemToSelect, 1, LogFilePath);
                }
                else if (Regex.IsMatch(ItemToSelectType, "controltype"))
                {
                    SearchReturnCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", ItemToSelect, 1, LogFilePath);
                }
                if (SearchReturnCondition == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SearchReturnCondition is null from SelectItemFromParent ", "fail");
                    if (terminatestatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromParent ", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                }
                AutomationElement SearchElement = GuiObj.FindAutomationElement(ParentObj, SearchReturnCondition, TreeScope.Descendants, "ItemToSelect", 0, LogFilePath);
                if (SearchElement == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SearchElement is null from SelectItemFromParent ", "fail");
                    if (terminatestatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromParent ", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                }
                SelectionItemPattern SelectionPattern = SearchElement.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                if (SelectionPattern != null)
                {
                    SelectionPattern.Select();
                }
                else
                {
                    GetPositionFromBoundingRectangleAndClick(SearchElement, LogFilePath, "Left");
                }
                NewLogObj.WriteLogFile(LogFilePath, ItemToSelect+" selected" , "info");
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Execption at SelectItemFromParent" + Ex.ToString(), "info");

            }
        }

        public void ToggleCheckBoxState(AutomationElement CheckBoxObj,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ToggleCheckBoxState", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            try
            {
                TogglePattern pattern = CheckBoxObj.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
                ToggleState state = pattern.Current.ToggleState;
                pattern.Toggle();
                Thread.Sleep(1000);
                
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at ToggleCheckBoxState "+Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ToggleCheckBoxState***", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");

                }

            }
            
        }

        public string CheckToggleState(AutomationElement CheckBoxObj, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckToggleState", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==================", "info");
            try
            {
                object objPattern;
                //TogglePattern pattern = CheckBoxObj.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
                
                CheckBoxObj.TryGetCurrentPattern(TogglePattern.Pattern, out objPattern);
                TogglePattern togPattern = objPattern as TogglePattern;
                //ToggleState state = pattern.Current.ToggleState;
                if (togPattern.Current.ToggleState == ToggleState.On)
                 {
                    return "On";
                }
                else
                {
                    return "Off";
                }
            }  
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at ToggleCheckBoxState " + Ex.ToString(), "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ToggleCheckBoxState***", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");

                }
                return null;

            }

        }
        public int CheckIfPatternIsSupported(AutomationElement AutoObj, string LookForPattern)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckIfPatternIsSupported", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            try
            {
                AutomationPattern[] patterns = AutoObj.GetSupportedPatterns();
                int PatternFound = 0;
                foreach (AutomationPattern pattern in patterns)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Pattern name " + Automation.PatternName(pattern) + " Programmatic name " + pattern.ProgrammaticName, "info");
                    if (Regex.IsMatch(Automation.PatternName(pattern), LookForPattern))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, LookForPattern + " supported. Current pattern name " + Automation.PatternName(pattern), "info");
                        PatternFound = 1;
                        break;
                    }

                }
                if (PatternFound == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, LookForPattern + " not supported", "info");

                }
                return PatternFound;

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  CheckIfPatternIsSupported "+Ex.ToString(), "info");
                return 0;

            }

        }

        public int GetClickablePointAndClick(AutomationElement AutoObj, string ElementName, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GetClickablePointAndClick", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            try
            {
                if (AutoObj == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "GetClickablePointAndClick -  AutoObj is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from GetClickablePointAndClick -  AutoObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return -1;
                }
                System.Windows.Point ClickablePoint= AutoObj.GetClickablePoint();
               
               System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(ClickablePoint.X), Convert.ToInt32(ClickablePoint.Y));

               Microsoft.Test.Input.Mouse.MoveTo(p);
               

               //Thread.Sleep(2000);
               Thread.Sleep(1000);
               Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  GetClickablePointAndClick " + Ex.ToString(), "info");
                //return 0;

            }
            return 0;


        }

        //Often data grids will have rows, where items can be slecetde by checking the chekbox eg:Xencenter license mgr
        //if CheckBoxToBeChecked is set as 1, will check the check box associated with it
        public int SelectItemFromDataGridWithIndex(AutomationElement DataGridObj, int ChildIndexToSelect, int CheckBoxToBeChecked, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            try
            {
                if (DataGridObj == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid -  AutoObj is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  AutoObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return -1;
                }
                

                DataGridObj.SetFocus();
                Thread.Sleep(1000);
                ////For seleceting the data grid otem
                //for (int i = 0; i < ItemIndex; i++)
                //{
                //    System.Windows.Forms.SendKeys.SendWait("{DOWN}");
                //    Thread.Sleep(1000);
                //}
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                
                AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);
                int TotalGridChildren = GridChildren.Count;
                //Each Grid child will have multiple childitems(like checkbox, label etc)   
                //Finding the no of childern for each grid child
                AutomationElement FirstChild = GridChildren[0];
                //1st row will be the header
                //if (string.Compare(FirstChild.Current.Name, "Top Row") == 0)
                //{
                    // ChildIndexToSelect = ChildIndexToSelect + 1;
                    FirstChild = GridChildren[1];
               // }
                PropertyCondition GrandChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                AutomationElementCollection GrandChildrenColl = FirstChild.FindAll(TreeScope.Descendants, GrandChildrenCondition);
                int NoOfGrandChildren = GrandChildrenColl.Count;
                int NoOfGridDirectChildren = 0;
                if (NoOfGrandChildren > 0)
                {
                    NoOfGridDirectChildren = TotalGridChildren / NoOfGrandChildren;
                }

                int ElementIndexToSelect = ((ChildIndexToSelect) * (NoOfGrandChildren + 1)) +1;

                //if (NoOfGridDirectChildren > ChildIndexToSelect && NoOfGridDirectChildren>ElementIndexToSelect)
                if (NoOfGridDirectChildren > ChildIndexToSelect)
                {
                    AutomationElement ChildObj = GridChildren[ElementIndexToSelect];
                    GuiObj.GetPositionFromBoundingRectangleAndClick(ChildObj, LogFilePath, "Left");

                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for row index corresponding to  " + ChildIndexToSelect + " from SelectItemFromDataGridWithIndex", "info");
                }

            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  SelectItemFromDataGridWithIndex " + Ex.ToString(), "info");
                //return 0;

            }
            return 0;

        }

        public int SelectItemFromDataGridWithIndexAndClickOnFirstColumn(AutomationElement DataGridObj, int ChildIndexToSelect, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGridWithIndexAndClickOnFirstColumn", "info");
            NewLogObj.WriteLogFile(LogFilePath, "================================================", "info");
            try
            {
                if (DataGridObj == null)
                {

                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  DataGridObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGridWithIndexAndClickOnFirstColumn -  DataGridObj is null. Resuming as TerminateOnFailure is no", "warn");
                    }
                    return -1;
                }

                DataGridObj.SetFocus();
                Thread.Sleep(1000);
                int ElementFoundFlag = 0;
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition GridChildrenCondition2 = GuiObj.SetPropertyCondition("ControlTypeProperty", "DataItem", 1, LogFilePath);
                AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition2);
                if (GridChildren.Count > 0)
                {
                    if (GridChildren.Count > ChildIndexToSelect)
                    {
                        AutomationElement ChildObj = GridChildren[ChildIndexToSelect];
                        ElementFoundFlag = 1;
                        GetPositionFromBoundingRectangleAndClickBasedOnInput(ChildObj, LogFilePath, "Left", "left", 20);
                        return 1;
                    }

                    if (ElementFoundFlag == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for row  " + ChildIndexToSelect + " from SelectItemFromDataGridWithIndexAndClickOnFirstColumn", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGridWithIndexAndClickOnFirstColumn", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                }
                //if grid children are not type of type data item
                else
                {
                    Generic GenericObj = new Generic();
                    PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    //PropertyCondition GridNameCondition = GuiObj.SetPropertyCondition("NameProperty", "行 " + ItemIndex, 1, LogFilePath);
                    //string ItemToSelect = "T-36";
                    //AndCondition CombinedCondition = new AndCondition(GridChildrenCondition, GridNameCondition);
                    //AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, CombinedCondition);
                    GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);
                    int TotalGridChildren = GridChildren.Count;
                    //Each Grid child will have multiple childitems(like checkbox, label etc)   
                    //Finding the no of childern for each grid child
                    AutomationElement FirstChild = GridChildren[0];
                    string SystemLocale = GenericObj.GetSystemLocale();

                    NewLogObj.WriteLogFile(LogFilePath, "Grid first child name: " + FirstChild.Current.Name, "info");
                    String GridFirstChildName = FirstChild.Current.Name;
                    if (!Regex.IsMatch(SystemLocale, "en-US"))
                    {
                        //BingTranslator MyBingObj = new BingTranslator();
                        BingTranslator MyBingObj = new BingTranslator();
                        string Locale = MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);

                        GridFirstChildName = MyBingObj.GenerateAccessTokenAndStartTranslation(GridFirstChildName, Locale, "en");
                    }
                    GridFirstChildName = GridFirstChildName.ToLower();
                    //1st row will be the header
                    //Detecting if header for chinese header is 首行 - translated as First line
                    // For JP トップの行 - translated as line of top
                    //For en Top row
                    //if (string.Compare(FirstChild.Current.Name, "Top Row") == 0)
                    if (Regex.IsMatch(GridFirstChildName, "top") || Regex.IsMatch(GridFirstChildName, "first"))
                    {
                        // ChildIndexToSelect = ChildIndexToSelect + 1;
                        FirstChild = GridChildren[1];
                    }
                    
                    PropertyCondition GrandChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    AutomationElementCollection GrandChildrenColl = FirstChild.FindAll(TreeScope.Descendants, GrandChildrenCondition);
                    int NoOfGrandChildren = GrandChildrenColl.Count;
                    int NoOfGridDirectChildren = 0;
                    if (NoOfGrandChildren > 0)
                    {
                        NoOfGridDirectChildren = TotalGridChildren / NoOfGrandChildren;
                    }

                    //Assuming that the elemnt to be checked will be in the row ItemIndex
                    // AutomationElement ChildObj=GridChildren[0];
                    //if(GridChildren.Count>ChildIndexToSelect)

                    //Grid row index will always starts from 0,

                    //int ElementIndexToSelect = (ChildIndexToSelect * (NoOfGrandChildren + 1)) - 2;
                    int ElementIndexToSelect = ((ChildIndexToSelect) * (NoOfGrandChildren + 1)) + 1;

                    //if (NoOfGridDirectChildren > ChildIndexToSelect && NoOfGridDirectChildren>ElementIndexToSelect)
                    if (NoOfGridDirectChildren > ChildIndexToSelect)
                    {
                        AutomationElement ChildObj = GridChildren[ElementIndexToSelect];
                        ElementFoundFlag = 1;
                        PropertyCondition RowChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                        AutomationElementCollection RowChildren = ChildObj.FindAll(TreeScope.Children, RowChildrenCondition);
                        if (RowChildren.Count > 0)
                        {
                            AutomationElement FirstColumnObj = RowChildren[0];
                            GuiObj.GetPositionFromBoundingRectangleAndClick(FirstColumnObj, LogFilePath, "Left");
                        }
                        else
                        {
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for 1st column corresponding to row " + ElementIndexToSelect, "fail");
                                FileOperations FileObj = new FileOperations();
                                FileObj.ExitTestEnvironment();
                            }
                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for 1st column corresponding to row " + ElementIndexToSelect + " Resuming as TerminateOnFailure is no", "warn");
                            }
                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for row  " + ChildIndexToSelect + " from SelectItemFromDataGridWithIndex.Exiting application from SelectItemFromDataGridWithIndexAndClickOnFirstColumn", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for row  " + ChildIndexToSelect + " from SelectItemFromDataGridWithIndexAndClickOnFirstColumn.Resuming as terminateonfailure is no", "fail");
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  SelectItemFromDataGridWithIndex " + Ex.ToString(), "info");
                //return 0;
            }
            return 0;
        }
        public int SelectItemFromDataGridWithIndexAndCheckCheckBoxIfReqd(AutomationElement DataGridObj, int ChildIndexToSelect, int CheckBoxToBeChecked, string ClickMouseBtn,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGridWithIndexAndCheckCheckBoxIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "================================================", "info");
            try
            {
                if (DataGridObj == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid -  AutoObj is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  AutoObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return -1;
                }

                DataGridObj.SetFocus();
                Thread.Sleep(1000);
                int ElementFoundFlag = 0;
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition GridChildrenCondition2 = GuiObj.SetPropertyCondition("ControlTypeProperty", "DataItem", 1, LogFilePath);
                AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition2);
                if (GridChildren.Count > 0)
                {
                    if (GridChildren.Count > ChildIndexToSelect)
                    {
                        AutomationElement ChildObj = GridChildren[ChildIndexToSelect];
                        ElementFoundFlag = 1;
                        GetPositionFromBoundingRectangleAndClickBasedOnInput(ChildObj, LogFilePath, "Left", "left", 20);
                        return 1;
                    }

                    if (ElementFoundFlag == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for row  " + ChildIndexToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGridWithIndexAndCheckCheckBoxIfReqd", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                }
                //if grid children are not type of type data item
                else
                {
                    Generic GenericObj = new Generic();
                    PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    //PropertyCondition GridNameCondition = GuiObj.SetPropertyCondition("NameProperty", "行 " + ItemIndex, 1, LogFilePath);
                    //string ItemToSelect = "T-36";
                    //AndCondition CombinedCondition = new AndCondition(GridChildrenCondition, GridNameCondition);
                    //AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, CombinedCondition);
                    GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);
                    int TotalGridChildren = GridChildren.Count;
                    //Each Grid child will have multiple childitems(like checkbox, label etc)   
                    //Finding the no of childern for each grid child
                    AutomationElement FirstChild = GridChildren[0];
                    string SystemLocale = GenericObj.GetSystemLocale();
                    
                    NewLogObj.WriteLogFile(LogFilePath, "Grid first child name: " + FirstChild.Current.Name, "info");
                    String GridFirstChildName = FirstChild.Current.Name;
                    if (!Regex.IsMatch(SystemLocale, "en-US"))
                    {
                        //BingTranslator MyBingObj = new BingTranslator();
                        BingTranslator MyBingObj = new BingTranslator();
                        string Locale = MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);
                        
                        GridFirstChildName = MyBingObj.GenerateAccessTokenAndStartTranslation(GridFirstChildName, Locale, "en");
                    }
                    GridFirstChildName=GridFirstChildName.ToLower();
                    //1st row will be the header
                    //Detecting if header for chinese header is 首行 - translated as First line
                    // For JP トップの行 - translated as line of top
                    //For en Top row
                    //if (string.Compare(FirstChild.Current.Name, "Top Row") == 0)
                    if(Regex.IsMatch(GridFirstChildName,"top") ||Regex.IsMatch(GridFirstChildName,"first"))
                    {
                       // ChildIndexToSelect = ChildIndexToSelect + 1;
                        FirstChild = GridChildren[1];
                    }
                    PropertyCondition GrandChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    AutomationElementCollection GrandChildrenColl = FirstChild.FindAll(TreeScope.Descendants, GrandChildrenCondition);
                    int NoOfGrandChildren = GrandChildrenColl.Count;
                    int NoOfGridDirectChildren = 0;
                    if (NoOfGrandChildren > 0)
                    {
                        NoOfGridDirectChildren = TotalGridChildren / NoOfGrandChildren;
                    }

                    //Assuming that the elemnt to be checked will be in the row ItemIndex
                    // AutomationElement ChildObj=GridChildren[0];
                    //if(GridChildren.Count>ChildIndexToSelect)

                    //Grid row index will always starts from 0,

                    //int ElementIndexToSelect = (ChildIndexToSelect * (NoOfGrandChildren + 1)) - 2;
                    int ElementIndexToSelect = ((ChildIndexToSelect) * (NoOfGrandChildren + 1)) +1;

                    //if (NoOfGridDirectChildren > ChildIndexToSelect && NoOfGridDirectChildren>ElementIndexToSelect)
                    if (NoOfGridDirectChildren > ChildIndexToSelect)
                    {

                        AutomationElement ChildObj = GridChildren[ElementIndexToSelect];
                        ElementFoundFlag = 1;
                        AutomationPattern[] patterns = ChildObj.GetSupportedPatterns();
                        object valuePattern = null;

                        if (ChildObj.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;

                            if (CheckBoxToBeChecked == 1)
                            {
                                //Check if the pattern to check is supported
                                if (Regex.IsMatch(CurrValue, "checked", RegexOptions.IgnoreCase) || Regex.IsMatch(CurrValue, "true", RegexOptions.IgnoreCase) || Regex.IsMatch(CurrValue, "false", RegexOptions.IgnoreCase))
                                {
                                    //Check if it is alreday checked
                                    string UnCheckedStatus2 = "Unchecked";
                                    string UnCheckedStatus3 = "False";
                                    if (Regex.IsMatch(CurrValue, UnCheckedStatus2) || Regex.IsMatch(CurrValue, UnCheckedStatus3))
                                    {
                                        //changing the value  property as checked is not checking the checkbox. 
                                        //CurrValue = Regex.Replace(CurrValue, UnCheckedStatus, ";checked;");
                                        //((ValuePattern)valuePattern).SetValue(CurrValue);

                                        //Workaround using bounding rectangle as setting value property is not checking the checkbox
                                        PropertyCondition RowChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                                        AutomationElementCollection RowChildren = ChildObj.FindAll(TreeScope.Children, RowChildrenCondition);
                                        //AutomationElementCollection RowChildren = ChildObj.FindAll(TreeScope.Descendants, RowChildrenCondition);
                                        if (RowChildren.Count > 0)
                                        {
                                            foreach (AutomationElement RowChildObj in RowChildren)
                                            {
                                                object valuePattern1 = null;
                                                if (RowChildObj.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern1))
                                                {
                                                    string ChildCurrValue = ((ValuePattern)valuePattern1).Current.Value;
                                                    //string UnCheckedStatus1 = "Unchecked";
                                                    if (Regex.IsMatch(ChildCurrValue, UnCheckedStatus2) || Regex.IsMatch(ChildCurrValue, UnCheckedStatus3))
                                                    {
                                                        //Get the bounding rectangle position & click
                                                        GuiObj.GetPositionFromBoundingRectangleAndClick(RowChildObj, LogFilePath, ClickMouseBtn);
                                                        break;

                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Sometimes unable to get the child row elements using find all - workaround
                                            List<AutomationElement> RowChildrenColl = new List<AutomationElement>();
                                            for (int i = 1; i <= NoOfGrandChildren; i++)
                                            {
                                                RowChildrenColl.Add(GridChildren[ElementIndexToSelect - 1 + i]);
                                            }
                                            foreach (AutomationElement RowChildObj in RowChildrenColl)
                                            {
                                                object valuePattern1 = null;
                                                if (RowChildObj.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern1))
                                                {
                                                    string ChildCurrValue = ((ValuePattern)valuePattern1).Current.Value;
                                                    //string UnCheckedStatus1 = "Unchecked";
                                                    if (Regex.IsMatch(ChildCurrValue, UnCheckedStatus2) || Regex.IsMatch(ChildCurrValue, UnCheckedStatus3))
                                                    {
                                                        //Get the bounding rectangle position & click
                                                        GuiObj.GetPositionFromBoundingRectangleAndClick(RowChildObj, LogFilePath, ClickMouseBtn);
                                                        break;

                                                    }
                                                }
                                            }

                                        }

                                    }
                                    else
                                    {
                                        NewLogObj.WriteLogFile(LogFilePath, ChildIndexToSelect + " is already checked", "info");
                                        return 1;
                                    }
                                }
                                else
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Check property is not supported for row in " + ChildIndexToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                                    return -1;
                                }
                            }
                            else
                            {
                                GuiObj.GetPositionFromBoundingRectangleAndClick(ChildObj, LogFilePath, ClickMouseBtn);
                                return 1;
                            }
                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for row  " + ChildIndexToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGridWithIndex", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  SelectItemFromDataGridWithIndex " + Ex.ToString(), "info");
                //return 0;
            }
            return 0;
        }


        
        public int SelectItemFromDataGridWithNameAndCheckCheckBoxIfReqd(AutomationElement DataGridObj, string ItemToSelect, int CheckBoxToBeChecked, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGridWithNameAndCheckCheckBoxIfReqd", "info");
            NewLogObj.WriteLogFile(LogFilePath, "================================================", "info");
            try
            {
                if (DataGridObj == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid -  AutoObj is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  AutoObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return -1;
                }

                DataGridObj.SetFocus();
                Thread.Sleep(1000);
                int ElementFoundFlag = 0;
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition GridChildrenCondition2 = GuiObj.SetPropertyCondition("ControlTypeProperty", "DataItem", 1, LogFilePath);
                AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition2);
                if (GridChildren.Count > 0)
                {
                    foreach (AutomationElement ChildObj in GridChildren)
                    {
                        if (string.Compare(ChildObj.Current.Name, ItemToSelect) == 0)
                        {
                            ElementFoundFlag = 1;
                            GetPositionFromBoundingRectangleAndClickBasedOnInput(ChildObj, LogFilePath, "Left", "left", 20);
                            break;
                            
                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for the row having " + ItemToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGridWithIndex", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                }
                //if grid children are not type of type data item
                else
                {
                    PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    //PropertyCondition GridNameCondition = GuiObj.SetPropertyCondition("NameProperty", "行 " + ItemIndex, 1, LogFilePath);
                    //string ItemToSelect = "T-36";
                    //AndCondition CombinedCondition = new AndCondition(GridChildrenCondition, GridNameCondition);
                    //AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, CombinedCondition);
                    GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);

                    //Assuming that the elemnt to be checked will be in the row ItemIndex
                    // AutomationElement ChildObj=GridChildren[0];
                    
                    foreach (AutomationElement ChildObj in GridChildren)
                    {
                        AutomationPattern[] patterns = ChildObj.GetSupportedPatterns();
                        object valuePattern = null;

                        if (ChildObj.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;
                            //Vlue pattern foramt - System.Drawing.Bitmap;Checked;xen-t8-sree2;6.1;Citrix XenServer 铂金版;从不;10.105.74.137:27000
                            string LookForPattern = ";" + ItemToSelect + ";";
                            if (Regex.IsMatch(CurrValue, LookForPattern))
                            {
                                ElementFoundFlag = 1;

                                if (CheckBoxToBeChecked == 1)
                                {
                                    //Check if the pattern to check is supported
                                    if (Regex.IsMatch(CurrValue, "checked", RegexOptions.IgnoreCase) || Regex.IsMatch(CurrValue, "true", RegexOptions.IgnoreCase) || Regex.IsMatch(CurrValue, "false", RegexOptions.IgnoreCase))
                                    {
                                        //Check if it is alreday checked
                                        string UnCheckedStatus2 = "Unchecked";
                                        string UnCheckedStatus3 = "False";
                                        if (Regex.IsMatch(CurrValue, UnCheckedStatus2) || Regex.IsMatch(CurrValue, UnCheckedStatus3))
                                        {
                                            //changing the value  property as checked is not checking the checkbox. 
                                            //CurrValue = Regex.Replace(CurrValue, UnCheckedStatus, ";checked;");
                                            //((ValuePattern)valuePattern).SetValue(CurrValue);

                                            //Workaround using bounding rectangle as setting value property is not checking the checkbox
                                            PropertyCondition RowChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                                            AutomationElementCollection RowChildren = ChildObj.FindAll(TreeScope.Children, RowChildrenCondition);
                                            foreach (AutomationElement RowChildObj in RowChildren)
                                            {
                                                object valuePattern1 = null;
                                                if (RowChildObj.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern1))
                                                {
                                                    string ChildCurrValue = ((ValuePattern)valuePattern1).Current.Value;
                                                    //string UnCheckedStatus1 = "Unchecked";
                                                    if (Regex.IsMatch(ChildCurrValue, UnCheckedStatus2) || Regex.IsMatch(ChildCurrValue, UnCheckedStatus3))
                                                    {
                                                        //Get the bounding rectangle position & click
                                                        GuiObj.GetPositionFromBoundingRectangleAndClick(RowChildObj, LogFilePath, "Left");
                                                       break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            NewLogObj.WriteLogFile(LogFilePath, ItemToSelect + " is already checked", "info");
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        NewLogObj.WriteLogFile(LogFilePath, "Check property is not supported for row in " + ItemToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                                        break;
                                    }
                                }
                                else
                                {
                                    GuiObj.GetPositionFromBoundingRectangleAndClick(ChildObj, LogFilePath, "Left");
                                    break;
                                }
                            }
                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for the row having " + ItemToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGridWithIndex", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  SelectItemFromDataGridWithIndex " + Ex.ToString(), "info");
                //return 0;
            }
            return 0;
        }

        public string SelectItemFromDataGridWithIndexAndExtractColumnValue(AutomationElement DataGridObj, int RowIndexToSelect, int ColumnNumToRetrieveValue,string ExpectedCoulmnValue, int ClickRequired,int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGridWithIndexAndExtractColumnValues", "info");
            NewLogObj.WriteLogFile(LogFilePath, "================================================", "info");
            try
            {
                if (DataGridObj == null)
                {
                   // NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid -  AutoObj is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  DataGridObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return null;
                }
                DataGridObj.SetFocus();
                Thread.Sleep(1000);
                int ElementFoundFlag = 0;
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition GridChildrenCondition2 = GuiObj.SetPropertyCondition("ControlTypeProperty", "DataItem", 1, LogFilePath);
                AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition2);
                if (GridChildren.Count > 0)
                {
                    if (GridChildren.Count > RowIndexToSelect)
                    {
                        AutomationElement ChildObj = GridChildren[RowIndexToSelect];
                        ElementFoundFlag = 1;
                        AutomationElementCollection ClumnsOftheRow = ChildObj.FindAll(TreeScope.Descendants, GridChildrenCondition2);
 
                        if (ClumnsOftheRow.Count < ColumnNumToRetrieveValue)
                        {
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Exiting from SelectItemFromDataGridWithIndexAndExtractColumnValues", "fail");
                                FileOperations FileObj = new FileOperations();
                                FileObj.ExitTestEnvironment();
                            }
                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Resuming as Terminate on Failure is no", "fail");
                                return null;
                            }
                        }
                        AutomationElement ColumnToSelect = ClumnsOftheRow[ColumnNumToRetrieveValue - 1];
                        object valuePattern = null;
                        if (ColumnToSelect.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;
                            if(string.Compare(CurrValue,ExpectedCoulmnValue)==0)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Value "+CurrValue+" found in CloumnNum " + ColumnNumToRetrieveValue + " is same as expected", "pass");
                            }
                            else
                            {
                                if (TerminateStatus == 1)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is not the same as expected value " + ExpectedCoulmnValue, "fail");
                                    FileOperations FileObj = new FileOperations();
                                    FileObj.ExitTestEnvironment();
                                }
                                else
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is not the same as expected value " + ExpectedCoulmnValue+" Resuming as TerminateOnFailure is no", "warn");
                                }
                            }
                            if (ClickRequired == 1)
                            {
                                GuiObj.GetPositionFromBoundingRectangleAndClick(ChildObj, LogFilePath, "Left");
                                return CurrValue;
                            }
                            else
                            {
                                return CurrValue;
                            }
                        }
                    }

                    if (ElementFoundFlag == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for the row  " + RowIndexToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGridWithIndexAndCheckCheckBoxIfReqd", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                }
                //if grid children are not type of type data item
                else
                {
                    Generic GenericObj = new Generic();
                    PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    //PropertyCondition GridNameCondition = GuiObj.SetPropertyCondition("NameProperty", "行 " + ItemIndex, 1, LogFilePath);
                    //string ItemToSelect = "T-36";
                    //AndCondition CombinedCondition = new AndCondition(GridChildrenCondition, GridNameCondition);
                    //AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, CombinedCondition);
                    GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);
                    int TotalGridChildren = GridChildren.Count;
                    //Each Grid child will have multiple childitems(like checkbox, label etc)   
                    //Finding the no of childern for each grid child
                    AutomationElement FirstChild = GridChildren[0];
                    string TopRowColumnTypes = "Custom";

                    string SystemLocale = GenericObj.GetSystemLocale();

                    NewLogObj.WriteLogFile(LogFilePath, "Grid first child name: " + FirstChild.Current.Name, "info");
                    String GridFirstChildName = FirstChild.Current.Name;
                    if (!Regex.IsMatch(SystemLocale, "en-US"))
                    {
                        //BingTranslator MyBingObj = new BingTranslator();
                        BingTranslator MyBingObj = new BingTranslator();
                        string Locale = MyBingObj.DecideLanguageBasedOnLocale(SystemLocale);

                        GridFirstChildName = MyBingObj.GenerateAccessTokenAndStartTranslation(GridFirstChildName, Locale, "en");
                    }
                    GridFirstChildName = GridFirstChildName.ToLower();
                    //1st row will be the header
                    //Detecting if header for chinese header is 首行 - translated as First line
                    // For JP トップの行 - translated as line of top
                    //For en Top row
                    //if (string.Compare(FirstChild.Current.Name, "Top Row") == 0)
                    
                    //if (string.Compare(FirstChild.Current.Name, "Top Row") == 0)
                    if (Regex.IsMatch(GridFirstChildName, "top") || Regex.IsMatch(GridFirstChildName, "first"))
                    {
                        // ChildIndexToSelect = ChildIndexToSelect + 1;
                        PropertyCondition HeaderCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Header", 1, LogFilePath);
                        AutomationElementCollection HeaderColl = FirstChild.FindAll(TreeScope.Descendants, HeaderCondition);
                        if (HeaderColl.Count > 0)
                        {
                            TopRowColumnTypes = "Header";
                        }
                        FirstChild = GridChildren[1];
                        //All the top row elements will be of type header
                       

                    }
                    PropertyCondition GrandChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    AutomationElementCollection GrandChildrenColl = FirstChild.FindAll(TreeScope.Descendants, GrandChildrenCondition);
                    int NoOfGrandChildren = GrandChildrenColl.Count;

                    if (NoOfGrandChildren < ColumnNumToRetrieveValue)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Exiting from SelectItemFromDataGridWithIndexAndExtractColumnValues", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Resuming as Terminate on Failure is no", "fail");
                        }
                    }
                    int NoOfGridDirectChildren = 0;
                    if (NoOfGrandChildren > 0)
                    {
                        NoOfGridDirectChildren = TotalGridChildren / NoOfGrandChildren;
                    }

                    //Assuming that the elemnt to be checked will be in the row ItemIndex
                    // AutomationElement ChildObj=GridChildren[0];
                    //if(GridChildren.Count>ChildIndexToSelect)
                   // int ElementIndexToSelect = (RowIndexToSelect * (NoOfGrandChildren + 1)) - 2;

                    //For top row coulun types will be of type "header"
                    int ElementIndexToSelect;
                    if (string.Compare(TopRowColumnTypes, "Header") == 0)
                    {
                        ElementIndexToSelect = (((RowIndexToSelect * (NoOfGrandChildren + 1)) + 1) - NoOfGrandChildren);
                    }
                    else
                    {
                        ElementIndexToSelect = ((RowIndexToSelect) * (NoOfGrandChildren + 1)) + 1;
                    }
                    //if (NoOfGridDirectChildren > ChildIndexToSelect && NoOfGridDirectChildren>ElementIndexToSelect)
                    if (NoOfGridDirectChildren > RowIndexToSelect)
                    {
                        AutomationElement ChildObj = GridChildren[ElementIndexToSelect-1];
                        //Now row element is obtained
                        //Coulumns will be the child elements of this row
                        PropertyCondition CoulumnCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                        AutomationElementCollection ClumnsOftheRow = ChildObj.FindAll(TreeScope.Descendants, CoulumnCondition);
                        if (ClumnsOftheRow.Count < ColumnNumToRetrieveValue)
                        {
                            if (TerminateStatus == 1)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Exiting from SelectItemFromDataGridWithIndexAndExtractColumnValues", "fail");
                                FileOperations FileObj = new FileOperations();
                                FileObj.ExitTestEnvironment();
                            }
                            else
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Resuming as Terminate on Failure is no", "fail");
                                return null;
                            }
                        }
                        AutomationElement ColumnToSelect = ClumnsOftheRow[ColumnNumToRetrieveValue - 1];

                        object valuePattern = null;

                        if (ColumnToSelect.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;
                            if (string.Compare(CurrValue, ExpectedCoulmnValue) == 0)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is same as expected", "pass");
                            }
                            else
                            {
                                if (TerminateStatus == 1)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is not the same as expected value " + ExpectedCoulmnValue, "fail");
                                    FileOperations FileObj = new FileOperations();
                                    FileObj.ExitTestEnvironment();
                                }
                                else
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is not the same as expected value " + ExpectedCoulmnValue + " Resuming as TerminateOnFailure is no", "warn");
                                }
                            }

                            if (ClickRequired == 1)
                            {
                                GuiObj.GetPositionFromBoundingRectangleAndClick(ChildObj, LogFilePath, "Left");
                                return CurrValue;
                            }
                            NewLogObj.WriteLogFile(LogFilePath, "Coulum value retrieved " + CurrValue, "info");
                            return CurrValue;
                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element for the row  " + RowIndexToSelect + " from SelectItemFromDataGridWithIndex", "fail");
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGridWithIndex", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  SelectItemFromDataGridWithIndex " + Ex.ToString(), "info");
                //return 0;
            }
            return null;
        }

        public string CheckDataGridColumnsForAnItemAndSelect(AutomationElement DataGridObj, int ColumnNumToRetrieveValue, string ExpectedCoulmnValue, int CheckCheckBox,int ClickRequired,string ClickMouseBtn, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckDataGridColumnsForAnItemAndSelect", "info");
            NewLogObj.WriteLogFile(LogFilePath, "================================================", "info");
            try
            {
                if (DataGridObj == null)
                {
                    // NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid -  AutoObj is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from CheckDataGridColumnsForAnItemAndSelect -  DataGridObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return null;
                }
                DataGridObj.SetFocus();
                Thread.Sleep(1000);
                int ElementFoundFlag = 0;
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition GridChildrenCondition2 = GuiObj.SetPropertyCondition("ControlTypeProperty", "DataItem", 1, LogFilePath);
                AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition2);
               
                if (GridChildren.Count > 0)
                {
                    foreach (AutomationElement GridChild in GridChildren)
                    {
                        object valuePattern = null;
                        if (GridChild.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;
                            if (string.Compare(CurrValue, ExpectedCoulmnValue) == 0)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is same as expected", "pass");
                            }
                            else
                            {
                                if (TerminateStatus == 1)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is not the same as expected value " + ExpectedCoulmnValue, "fail");
                                    FileOperations FileObj = new FileOperations();
                                    FileObj.ExitTestEnvironment();
                                }
                                else
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is not the same as expected value " + ExpectedCoulmnValue + " Resuming as TerminateOnFailure is no", "warn");
                                }
                            }
                            if (ClickRequired == 1)
                            {
                                //GuiObj.GetPositionFromBoundingRectangleAndClick(GridChild, LogFilePath, "Left");
                                GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(GridChild, LogFilePath, "Left", "Left", 20);
                                return CurrValue;
                            }
                            else
                            {
                                return CurrValue;
                            }
                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from CheckDataGridColumnsForAnItemAndSelect as no columns match the value " + ExpectedCoulmnValue, "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting application from CheckDataGridColumnsForAnItemAndSelect as no columns match the value " + ExpectedCoulmnValue + " Resuming as TerminateOnFailure is no", "warn");
                        }
                    }
                }
                
                //if grid children are not type of type data item
                else
                {
                    PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    //PropertyCondition GridNameCondition = GuiObj.SetPropertyCondition("NameProperty", "行 " + ItemIndex, 1, LogFilePath);
                    //string ItemToSelect = "T-36";
                    //AndCondition CombinedCondition = new AndCondition(GridChildrenCondition, GridNameCondition);
                    //AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, CombinedCondition);
                    GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);
                    int TotalGridChildren = GridChildren.Count;
                    //Each Grid child will have multiple childitems(like checkbox, label etc)   
                    //Finding the no of childern for each grid child
                    AutomationElement FirstChild = GridChildren[1];
            
                    PropertyCondition GrandChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    AutomationElementCollection GrandChildrenColl = FirstChild.FindAll(TreeScope.Descendants, GrandChildrenCondition);
                    int NoOfGrandChildren = GrandChildrenColl.Count;

                    if (NoOfGrandChildren < ColumnNumToRetrieveValue)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Exiting from SelectItemFromDataGridWithIndexAndExtractColumnValues", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Resuming as Terminate on Failure is no", "warn");
                        }
                    }
                    AutomationElement ContainerRow = null; //For holding the entire row value like 'Win7sp1_OVF_noVMtools;GoodVappDescriptionGoodVappDescriptionGoodVappDescription"
                    string ContainerRowValue = null;
                    foreach (AutomationElement GridChild in GridChildren)
                    {
                        object valuePattern = null;
                        if (GridChild.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;
                            
                            if (Regex.IsMatch(CurrValue, ";"))
                            {
                                ContainerRow = GridChild;
                                ContainerRowValue = CurrValue;
                            }
                            if (string.Compare(CurrValue, ExpectedCoulmnValue) == 0)
                            {
                                ElementFoundFlag = 1;
                                NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " is same as expected", "pass");
                                if (CheckCheckBox == 1)
                                {
                                    if (Regex.IsMatch(ContainerRowValue, "checked", RegexOptions.IgnoreCase) || Regex.IsMatch(ContainerRowValue, "true", RegexOptions.IgnoreCase) || Regex.IsMatch(ContainerRowValue, "false", RegexOptions.IgnoreCase))
                                    {
                                        //Check if it is alreday checked
                                        string UnCheckedStatus2 = "Unchecked";
                                        string UnCheckedStatus3 = "False";
                                        if (Regex.IsMatch(ContainerRowValue, UnCheckedStatus2) || Regex.IsMatch(ContainerRowValue, UnCheckedStatus3))
                                        {
                                            //changing the value  property as checked is not checking the checkbox. 
                                            //CurrValue = Regex.Replace(CurrValue, UnCheckedStatus, ";checked;");
                                            //((ValuePattern)valuePattern).SetValue(CurrValue);

                                            //Workaround using bounding rectangle as setting value property is not checking the checkbox
                                            PropertyCondition RowChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                                            AutomationElementCollection RowChildren = ContainerRow.FindAll(TreeScope.Children, RowChildrenCondition);
                                            foreach (AutomationElement RowChildObj in RowChildren)
                                            {
                                                object valuePattern1 = null;
                                                if (RowChildObj.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern1))
                                                {
                                                    string ChildCurrValue = ((ValuePattern)valuePattern1).Current.Value;
                                                    //string UnCheckedStatus1 = "Unchecked";
                                                    if (Regex.IsMatch(ChildCurrValue, UnCheckedStatus2) || Regex.IsMatch(ChildCurrValue, UnCheckedStatus3))
                                                    {
                                                        //Get the bounding rectangle position & click
                                                        GuiObj.GetPositionFromBoundingRectangleAndClick(RowChildObj, LogFilePath, "Left");
                                                        //GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(RowChildObj, LogFilePath, "Left", "Left", 20);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                else if (ClickRequired == 1)
                                {
                                   // GuiObj.GetPositionFromBoundingRectangleAndClick(GridChild, LogFilePath, ClickMouseBtn);
                                    GuiObj.GetPositionFromBoundingRectangleAndClickBasedOnInput(GridChild, LogFilePath, "Left", "Left", 20);
                                    return CurrValue;
                                }
                                NewLogObj.WriteLogFile(LogFilePath, "Coulum value retrieved " + CurrValue, "info");
                                return CurrValue;
                            }

                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the value  " + ExpectedCoulmnValue + "in the grid from CheckDataGridColumnsForAnItemAndSelect", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the value  " + ExpectedCoulmnValue + "in the grid from CheckDataGridColumnsForAnItemAndSelect. Resuming as TerminateOnFailurw is no", "warn");
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  CheckDataGridColumnsForAnItemAndSelect " + Ex.ToString(), "info");
                //return 0;
            }
            return null;
        }


        public string CheckDataGridColumnsForAPatternExtractAndSelectSame(AutomationElement DataGridObj, int ColumnNumToRetrieveValue, string SearchPattern, int CheckCheckBox, int ClickRequired, string ClickMouseBtn, int TerminateStatus)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckDataGridColumnsForAPatternExtractAndSelectSame", "info");
            NewLogObj.WriteLogFile(LogFilePath, "================================================", "info");
            try
            {
                if (DataGridObj == null)
                {
                    // NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid -  AutoObj is null", "fail");
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from CheckDataGridColumnsForAPatternExtractAndSelectSame -  DataGridObj is null", "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    return null;
                }
                DataGridObj.SetFocus();
                Thread.Sleep(1000);
                int ElementFoundFlag = 0;
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                PropertyCondition GridChildrenCondition2 = GuiObj.SetPropertyCondition("ControlTypeProperty", "DataItem", 1, LogFilePath);
                AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition2);

                Regex SearchPatternExp = new Regex(SearchPattern);
                if (GridChildren.Count > 0)
                {
                    foreach (AutomationElement GridChild in GridChildren)
                    {
                        object valuePattern = null;
                        if (GridChild.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;
                            Match PatternMatch = SearchPatternExp.Match(CurrValue);
                            if (PatternMatch.Success)
                            {
                                NewLogObj.WriteLogFile(LogFilePath, "SearchPattern " + SearchPattern + " found in CloumnNum " + ColumnNumToRetrieveValue + " Column value " + CurrValue, "pass");
                                if (ClickRequired == 1)
                                {
                                    GuiObj.GetPositionFromBoundingRectangleAndClick(GridChild, LogFilePath, "Left");
                                    return CurrValue;
                                }
                                else
                                {
                                    return CurrValue;
                                }
                            }
                        }
                    }
                    
                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from CheckDataGridColumnsForAPatternExtractAndSelectSame as no columns match the pattern " + SearchPattern, "fail");
                        FileOperations FileObj = new FileOperations();
                        FileObj.ExitTestEnvironment();
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Exiting application from CheckDataGridColumnsForAnItemAndSelect as no columns match the pattern " + SearchPattern + " Resuming as TerminateOnFailure is no", "warn");
                    }
                    
                }

                //if grid children are not type of type data item
                else
                {
                    PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    //PropertyCondition GridNameCondition = GuiObj.SetPropertyCondition("NameProperty", "行 " + ItemIndex, 1, LogFilePath);
                    //string ItemToSelect = "T-36";
                    //AndCondition CombinedCondition = new AndCondition(GridChildrenCondition, GridNameCondition);
                    //AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, CombinedCondition);
                    GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);
                    int TotalGridChildren = GridChildren.Count;
                    //Each Grid child will have multiple childitems(like checkbox, label etc)   
                    //Finding the no of childern for each grid child
                    AutomationElement FirstChild = GridChildren[1];

                    PropertyCondition GrandChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                    AutomationElementCollection GrandChildrenColl = FirstChild.FindAll(TreeScope.Descendants, GrandChildrenCondition);
                    int NoOfGrandChildren = GrandChildrenColl.Count;

                    if (NoOfGrandChildren < ColumnNumToRetrieveValue)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Exiting from SelectItemFromDataGridWithIndexAndExtractColumnValues", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "CloumnNum " + ColumnNumToRetrieveValue + " is not avialble. Resuming as Terminate on Failure is no", "warn");
                        }
                    }
                    AutomationElement ContainerRow = null; //For holding the entire row value like 'Win7sp1_OVF_noVMtools;GoodVappDescriptionGoodVappDescriptionGoodVappDescription"
                    string ContainerRowValue = null;
                    foreach (AutomationElement GridChild in GridChildren)
                    {
                        object valuePattern = null;
                        if (GridChild.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
                        {
                            string CurrValue = ((ValuePattern)valuePattern).Current.Value;

                            if (Regex.IsMatch(CurrValue, ";"))
                            {
                                ContainerRow = GridChild;
                                ContainerRowValue = CurrValue;
                            }
                            Match PatternMatch = SearchPatternExp.Match(CurrValue);
                            //Should not be container row
                            if (PatternMatch.Success && !(Regex.IsMatch(CurrValue, ";")))
                            //if (string.Compare(CurrValue, ExpectedCoulmnValue) == 0)
                            {
                                ElementFoundFlag = 1;
                                NewLogObj.WriteLogFile(LogFilePath, "Value " + CurrValue + " found in CloumnNum " + ColumnNumToRetrieveValue + " matches the pattern "+SearchPattern, "pass");
                                if (CheckCheckBox == 1)
                                {
                                    if (Regex.IsMatch(ContainerRowValue, "checked", RegexOptions.IgnoreCase) || Regex.IsMatch(ContainerRowValue, "true", RegexOptions.IgnoreCase) || Regex.IsMatch(ContainerRowValue, "false", RegexOptions.IgnoreCase))
                                    {
                                        //Check if it is alreday checked
                                        string UnCheckedStatus2 = "Unchecked";
                                        string UnCheckedStatus3 = "False";
                                        if (Regex.IsMatch(ContainerRowValue, UnCheckedStatus2) || Regex.IsMatch(ContainerRowValue, UnCheckedStatus3))
                                        {
                                            //changing the value  property as checked is not checking the checkbox. 
                                            //CurrValue = Regex.Replace(CurrValue, UnCheckedStatus, ";checked;");
                                            //((ValuePattern)valuePattern).SetValue(CurrValue);

                                            //Workaround using bounding rectangle as setting value property is not checking the checkbox
                                            PropertyCondition RowChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
                                            AutomationElementCollection RowChildren = ContainerRow.FindAll(TreeScope.Children, RowChildrenCondition);
                                            foreach (AutomationElement RowChildObj in RowChildren)
                                            {
                                                object valuePattern1 = null;
                                                if (RowChildObj.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern1))
                                                {
                                                    string ChildCurrValue = ((ValuePattern)valuePattern1).Current.Value;
                                                    //string UnCheckedStatus1 = "Unchecked";
                                                    if (Regex.IsMatch(ChildCurrValue, UnCheckedStatus2) || Regex.IsMatch(ChildCurrValue, UnCheckedStatus3))
                                                    {
                                                        //Get the bounding rectangle position & click
                                                        GuiObj.GetPositionFromBoundingRectangleAndClick(RowChildObj, LogFilePath, "Left");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                else if (ClickRequired == 1)
                                {
                                    GuiObj.GetPositionFromBoundingRectangleAndClick(GridChild, LogFilePath, ClickMouseBtn);
                                    return CurrValue;
                                }
                                NewLogObj.WriteLogFile(LogFilePath, "Coulum value retrieved " + CurrValue, "info");
                                return CurrValue;
                            }

                        }
                    }
                    if (ElementFoundFlag == 0)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the pattern  " + SearchPattern + "in the grid from CheckDataGridColumnsForAPatternExtractAndSelectSame", "fail");
                            FileOperations FileObj = new FileOperations();
                            FileObj.ExitTestEnvironment();
                        }
                        else
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the SearchPattern  " + SearchPattern + "in the grid from CheckDataGridColumnsForAnItemAndSelect. Resuming as TerminateOnFailurw is no", "warn");
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at  CheckDataGridColumnsForAPatternExtractAndSelectSame " + Ex.ToString(), "info");
                //return 0;
            }
            return null;
        }

        public int CloseChildWindowsUnderParent(AutomationElement ParentWindowObj,int TerminateStatus)
        {
            if (ParentWindowObj == null)
            {
                return -1;
            }
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CloseChildWindowsUnderParent", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================================", "info");
            Console.WriteLine("Closing Parent window");

            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition WindowCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Window", 1, LogFilePath);
            AutomationElementCollection ChildWndowCollection = ParentWindowObj.FindAll(TreeScope.Descendants, WindowCondition);

            PropertyCondition PropCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "Close", 0, LogFilePath);
            AutomationElement CloseBtnObj=null;
            //If any windows are opened close them one by one
            if (ChildWndowCollection.Count > 0)
            {
                foreach (AutomationElement ChldWind in ChildWndowCollection)
                {
                    ///Find the close button
                    CloseBtnObj = GuiObj.FindAutomationElement(ChldWind, PropCondition,TreeScope.Descendants, "Close Btn", 2, 0, LogFilePath);
                    if (CloseBtnObj != null)
                    {
                        GuiObj.ClickButton(CloseBtnObj, 0, "CloseBtn", 0, LogFilePath);
                    }
                }
            }
            //Close the parent window
            CloseBtnObj = GuiObj.FindAutomationElement(ParentWindowObj, PropCondition, TreeScope.Descendants, "Close Btn", 2, 0, LogFilePath);
            if (CloseBtnObj != null)
            {
                GuiObj.ClickButton(CloseBtnObj, 0, "CloseBtn", 0, LogFilePath);
            }
            return 1;
        }

        public AutomationElement CheckInRawViewIfExpectedElementIsPresent(AutomationElement ParentObj, string ExpectedVal,int ClickRequired,string ClickMouseBtn)
        {

            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckInRawViewIfExpectedElementIsPresent", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=================================", "info");
            if (string.Compare(ParentObj.Current.Name, ExpectedVal) == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Element with name " + ExpectedVal + " found ", "pass");
                return ParentObj;
            }
            //int ElementFoundFlag = 0;
            AutomationElement ChildObj = TreeWalker.RawViewWalker.GetFirstChild(ParentObj);
            if (ChildObj == null)
            {
                return null;
            }
            else if (string.Compare(ChildObj.Current.Name, ExpectedVal) == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Element with name " + ExpectedVal +" found ", "pass");
                if (ClickRequired == 1)
                {
                    GetPositionFromBoundingRectangleAndClick(ChildObj, LogFilePath,ClickMouseBtn);
                }
               // ElementFoundFlag = 1;
                return ChildObj;
            }
            else
            {
                ChildObj=CheckInRawViewIfExpectedElementIsPresent(ChildObj, ExpectedVal, ClickRequired, ClickMouseBtn);
                if (ChildObj!=null && string.Compare(ChildObj.Current.Name, ExpectedVal) == 0)
                {
                    return ChildObj;
                }
            }
            return null;
        }
        //public int SelectItemFromDataGridWithName(AutomationElement DataGridObj, string ItemName, int TerminateStatus)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGridWithName", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
        //    try
        //    {
        //        if (DataGridObj == null)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "SelectItemFromDataGrid -  AutoObj is null", "fail");
        //            if (TerminateStatus == 1)
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  AutoObj is null", "fail");
        //                FileOperations FileObj = new FileOperations();
        //                FileObj.ExitTestEnvironment();
        //            }
        //            return -1;
        //        }

                
        //        //Find the grid items
        //        AutomationElementIdentity GuiObj = new AutomationElementIdentity();
        //        PropertyCondition GridChildrenCondition = GuiObj.SetPropertyCondition("ControlTypeProperty", "Custom", 1, LogFilePath);
        //        AutomationElementCollection GridChildren = DataGridObj.FindAll(TreeScope.Descendants, GridChildrenCondition);
        //        if (GridChildren.Count <= 0)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Child element count of grid is 0 SelectItemFromDataGridWithName", "fail");
        //            if (TerminateStatus == 1)
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  AutoObj is null", "fail");
        //                FileOperations FileObj = new FileOperations();
        //                FileObj.ExitTestEnvironment();
        //            }
        //            return -1;
        //        }
        //        int DownArrowCount = 1;
        //        int ElementFoundFlag = 0;
        //        foreach (AutomationElement GridChildObj in GridChildren)
        //        {
        //            if (string.Compare(GridChildObj.Current.Name, ItemName) == 0)
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "Required element " + ItemName + " found in grid at SelectItemFromDataGridWithName " + GridChildObj.Current.Name, "info");
        //                ElementFoundFlag = 1;
        //            }
        //            else
        //            {
        //                DownArrowCount++;
        //            }
        //        }
        //        if (ElementFoundFlag == 0)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Required element " + ItemName + " not found in grid at  ", "fail");
        //            if (TerminateStatus == 1)
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "Exiting application from SelectItemFromDataGrid -  AutoObj is null", "fail");
        //                FileOperations FileObj = new FileOperations();
        //                FileObj.ExitTestEnvironment();
        //            }
        //            return -1;
        //        }
        //        //Send the arrow keys
        //        DataGridObj.SetFocus();
        //        for (int DownPress = 1; DownPress <= DownArrowCount; DownPress++)
        //        {
        //            System.Windows.Forms.SendKeys.SendWait("{DOWN}");
        //            Thread.Sleep(1000);
        //        }
        //     }
        //    catch (Exception Ex)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Exception at  SelectItemFromDataGridWithIndex " + Ex.ToString(), "info");
        //    }
        //    return 0;

        //}
        
    }

   
}

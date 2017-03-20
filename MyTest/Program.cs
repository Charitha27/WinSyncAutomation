using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test;
using System.Windows.Automation;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using XenCenterOperations;
using LoggerCollection;
using FileOperationsCollection;
using GuizardCollection;
using GUICollection;
using System.Globalization;

using GenericCollection;
using XenServerCollection;
using XenAPI;

namespace MyTest
{
    class Program
    {
        static void Main(string[] args)
        
        {
            XenCenter NewOprtorObj = new XenCenter();
            Logger NewLogObj = new Logger();
            NewLogObj.CreateLogFolder();
            string LogFilePath = NewLogObj.CreateLogFile();
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            Guizard NewGuiZardObj = new Guizard();
            FileOperations FileObj = new FileOperations();
            Generic GenericObj = new Generic();

            XenServer XenObj = new XenServer();
           // Session session =XenObj.Login("10.105.83.51", 80, "root", "citrix");
           // XenObj.GetVMParam(session, "GSEN-Win7x86_29aug2012_1", "IP");
            Generic NewGenericObj = new Generic();
            //int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
            FileObj.FinishCurrentTest("MyTest");

            //string cur = Thread.CurrentThread.CurrentCulture.Name;
            //CultureInfo us = new CultureInfo("zh-CN");
            //string shortUsDateFormatString = us.DateTimeFormat.ShortDatePattern;
           // string shortUsTimeFormatString = us.DateTimeFormat.ShortTimePattern;

           // string LabelPanel1 = "General";
           // string LabelPanel2 = "Custom Fields";
           // string InputFilePath = FileObj.GetInputFilePath(LogFilePath, "Inputs.txt");
           // string XenServerName = FileObj.GetInputPattern(InputFilePath, "AddedXenServerDisplayName");
           // string ChangeHyperlink = "Change...";
           // string EditTagsHyperlink = "Edit tags...";
           // string LocalizedFilePath = FileObj.GetLocalizedFilePath();
           // if (LocalizedFilePath != null)
           // {
           //     LabelPanel1 = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, LabelPanel1);
           //     LabelPanel2 = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, LabelPanel2);
           //     ChangeHyperlink = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, ChangeHyperlink);
           //     EditTagsHyperlink = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, EditTagsHyperlink);

           // }
           // AutomationElement XenCenterObj = NewOprtorObj.SetFocusOnWindow(AutomationElement.RootElement, "NameProperty", "XenCenter", TreeScope.Children, "XenCenter", 1, LogFilePath);

           // AutomationElement ServerObj = NewOprtorObj.CheckXenCenterServerNodetree(XenCenterObj, XenServerName, LogFilePath, 1, 1, "Right");

           // // Open properties
           // System.Windows.Forms.SendKeys.SendWait("r");
           // Thread.Sleep(2000);
           // AutomationElement PropertiesWindow=GenericObj.VerifyWindowOnScreen(XenCenterObj, "EditHostGeneralSettingsDialog", "TabTitle", LabelPanel1, 1);
           // PropertyCondition ParentListCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "verticalTabs", 1, LogFilePath);
           // AutomationElement ParentListObj = GuiObj.FindAutomationElement(PropertiesWindow, ParentListCondition, TreeScope.Descendants, "ParentListCondition", 1, LogFilePath);
           // //Select customfields tab
           // //GuiObj.SelectItemFromParent(ParentListObj, LabelPanel2, 1);
           //AutomationElement ListElement= GuiObj.GetListElementAndSelect(ParentListObj,1,3,LabelPanel2, 1);
           //System.Windows.Point ClickablePoint = ListElement.GetClickablePoint();

           //System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(ClickablePoint.X), Convert.ToInt32(ClickablePoint.Y));

           //Microsoft.Test.Input.Mouse.MoveTo(p);
           // Thread.Sleep(2000);
           //Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
           // NewGuiZardObj.VerifyGuiZardResults();
            //NewGuiZardObj.KillGuizard();
            System.Console.WriteLine("Finsihed");
            //string LogFilePath = NewLogObj.GetLogFilePath();
            //NewGuiZardObj.VerifyGuiZardHasFinishedCapturing(AddServerDialog, "AddServerDialog");
            //string AddServerDialog = "Add New Server";
            //string LocalizedFilePath = FileObj.GetLocalizedFilePath();
            //if (LocalizedFilePath != null)
            //{
            //    AddServerDialog = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, AddServerDialog);

            //}
            //NewGuiZardObj.VerifyGuiZardHasFinishedCapturing(AddServerDialog, "AddServerDialog");

            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Threading;
using System.IO;
//using MbUnit.Framework;

using GUICollection;
using LoggerCollection;
using XenCenterOperations;
using FileOperationsCollection;
using GenericCollection;


namespace Installer
{
    class Program
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        static void Main(string[] args)
        {
            string XenCenterInstallStatusFile="C:\\XenCenterInstalled.txt";

            String TestStatus = "Pass";
            Logger NewLogObj = new Logger();
            NewLogObj.CreateLogFolder();
            string LogFilePath = NewLogObj.CreateLogFile();
            NewLogObj.WriteLogFile(LogFilePath, "Test : Installer", "info");
            NewLogObj.WriteLogFile(LogFilePath, "======================", "info");

            if(File.Exists(XenCenterInstallStatusFile))
            {
                File.Delete(XenCenterInstallStatusFile);
            }
            FileOperations FileObj = new FileOperations();
            string InputFilePath = FileObj.GetInputFilePath(LogFilePath, "Inputs.txt");

            string InstallerLocation = FileObj.GetInputPattern(InputFilePath, "InstallerLocation");
            string InstallerProcessName = FileObj.GetInputPattern(InputFilePath, "InstallerProcessName");
            string LocationToInstallXenCenter=FileObj.GetInputPattern(InputFilePath, "LocationToInstallXenCenter");
            string XenCenterVersion = FileObj.GetInputPattern(InputFilePath, "XenCenterVersion");

            string SetUpWindowTitle="Citrix XenCenter Setup";
            string InterruptedDialog = "Citrix XenCenter Setup Wizard was interrupted";
            string HelpMenu = "&Help";
            string AboutXenCenterMenu = "&About XenCenter";
            
            string LocalizedFilePath = FileObj.GetLocalizedFilePath();
            //if (LocalizedFilePath != null)
            //{
            SetUpWindowTitle = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, SetUpWindowTitle);
            InterruptedDialog = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, InterruptedDialog);
            HelpMenu = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, HelpMenu);
            AboutXenCenterMenu = FileObj.GetLocalizedMappedPattern(LocalizedFilePath, AboutXenCenterMenu);
            //}
            //else
            //{
            //    //Substitue the &
            //    HelpMenu = FileObj.RemovePatternFromString(HelpMenu, "&");
            //    AboutXenCenterMenu = FileObj.RemovePatternFromString(AboutXenCenterMenu, "&");

            //}

            //string InputFilePath1 = FileObj.GetInputFilePath(LogFilePath, "Mapped_CH.txt");
            //string SetupWindowTitle = FileObj.SearchFileForPattern(InputFilePath1, "Citrix XenCenter Setup", 1, LogFilePath);
            //int IndexEqual2 = SetupWindowTitle.IndexOf("=");
            //SetupWindowTitle = SetupWindowTitle.Substring(IndexEqual2 + 1);
            Console.WriteLine("InstallerLocation " + InstallerLocation);
            Console.WriteLine("InstallerProcessName " + InstallerProcessName);
            
            Generic GenricObj = new Generic();
            GenricObj.StartProcess(InstallerLocation, InstallerProcessName,LogFilePath,1);
            Thread.Sleep(5000);
            //Console.WriteLine("handling UAC");
            ////Assuming that UAC prompt is on the screen
            //System.Windows.Forms.SendKeys.SendWait("{LEFT}");
            //Thread.Sleep(1000);
            //System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            //Thread.Sleep(3000);
           // Console.WriteLine("Click send to UAC");
           
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition WindowReturnCondition = GuiObj.SetPropertyCondition("NameProperty", SetUpWindowTitle, 1, LogFilePath);
            AutomationElement SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
            Console.WriteLine("Checking for SetupDialog ");
            NewLogObj.WriteLogFile(LogFilePath, "Checking for SetupDialog", "info");

            int WaitTimeOut = 240000;//3 mins
            int timer = 0;
            while (SetupDialogObj == null && timer < WaitTimeOut)
            {
                SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
                Thread.Sleep(3000);
                timer = timer + 3000;
                NewLogObj.WriteLogFile(LogFilePath, "Waiting for SetupDialog", "info");
                Console.WriteLine("Waiting for SetupDialog ");
            }
            if (SetupDialogObj == null && timer < WaitTimeOut)
            {
                Console.WriteLine("Timeout waiting for setup dialog obj.Exiting.. ");
                Console.WriteLine("Timeout waiting for SetupDialog obj.Exiting.. ");
                FileObj.ExitTestEnvironment();

            }
            NewLogObj.WriteLogFile(LogFilePath, "SetupDialog obj found", "info");
            Console.WriteLine("SetupDialog obj found ");
            //Click cancel btn & verify actions
            Program PgmObj = new Program();
            PgmObj.ClickCancel(SetupDialogObj, SetUpWindowTitle,"yes");
            PgmObj.CheckInterruptedDialog(WindowReturnCondition, InterruptedDialog);
            PgmObj.ClickFinishBtn(WindowReturnCondition); ;

            //Start installer again
            GenricObj.StartProcess(InstallerLocation, InstallerProcessName, LogFilePath, 1);
            Thread.Sleep(3000);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);

            timer = 0;
            while (SetupDialogObj == null && timer < WaitTimeOut)
            {
                SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
                Thread.Sleep(3000);
                timer = timer + 3000;
                NewLogObj.WriteLogFile(LogFilePath, "Waiting for SetupDialog", "info");
                Console.WriteLine("Waiting for SetupDialog ");
            }
            if (SetupDialogObj == null && timer < WaitTimeOut)
            {
                Console.WriteLine("Timeout waiting for setup dialog obj.Exiting.. ");
                Console.WriteLine("Timeout waiting for SetupDialog obj.Exiting.. ");
                FileObj.ExitTestEnvironment();

            }

            //Click cancel & no
            PgmObj.ClickCancel(SetupDialogObj, SetUpWindowTitle, "no");
            //Verify SetupDialogObj is still there
            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
            if (SetupDialogObj != null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "SetupDialogObj is there after pressing on no btn", "info");
            }

            PropertyCondition NextBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "613", 1, LogFilePath);
            AutomationElement NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Descendants, "Next Btn", 1, LogFilePath);
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
            ////Destination text
            //PropertyCondition DestinationTextReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "679", 1, LogFilePath);
            //AutomationElement DestinationTextObj = GuiObj.FindAutomationElement(SetupDialogObj, DestinationTextReturnCondition, TreeScope.Descendants, "Xencenter install DestinationTextObj", 1, LogFilePath);
            //Click on browse btn
            PropertyCondition BrowseBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "683", 1, LogFilePath);
            AutomationElement BrowseBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, BrowseBtnReturnCondition, TreeScope.Descendants, "Next Btn", 1, LogFilePath);
            GuiObj.ClickButton(BrowseBtnObj, 1, "Browse Button", 1, LogFilePath);
            //Verify if child winodw with xencenter location has apperaed
            AutomationElement LocationChildWndObj = GuiObj.FindAutomationElement(SetupDialogObj, WindowReturnCondition, TreeScope.Descendants, "Xencenter install location browse", 1, LogFilePath);
            //LOcate the textbox having location
            PropertyCondition LocationEditCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "547", 1, LogFilePath);
            AutomationElement LocationObj = GuiObj.FindAutomationElement(LocationChildWndObj, LocationEditCondition, TreeScope.Descendants, "Xencenter install location edit box", 1, LogFilePath);
            GuiObj.SetTextBoxText(LocationObj, LocationToInstallXenCenter, "Xencenter Install location textbox",1, LogFilePath);

            PropertyCondition OKBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "549", 1, LogFilePath);
            AutomationElement OKbtnObj = GuiObj.FindAutomationElement(LocationChildWndObj, OKBtnCondition, TreeScope.Descendants, "Xencenter install location edit box", 1, LogFilePath);
            GuiObj.ClickButton(OKbtnObj, 1, "OK Button", 1, LogFilePath);
            Thread.Sleep(2000);
            //find the radio btns
            PropertyCondition ForAllRadioBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "507", 1, LogFilePath);
            AutomationElement ForAllradioBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, ForAllRadioBtnCondition, TreeScope.Descendants, "Xencenter install for all radiobtn", 1, LogFilePath);
            GuiObj.SetRadioButton(ForAllradioBtnObj, "Install for all radioBytn", 1, LogFilePath);
            Thread.Sleep(1000);

            PropertyCondition ForMeRadioBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "781", 1, LogFilePath);
            AutomationElement ForMeradioBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, ForMeRadioBtnCondition, TreeScope.Descendants, "Xencenter install for all radiobtn", 1, LogFilePath);
            GuiObj.SetRadioButton(ForMeradioBtnObj, "Install for me radioBytn", 1, LogFilePath);
            Thread.Sleep(1000);

            //Click next btn
            PropertyCondition Next1BtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "613", 1, LogFilePath);
            AutomationElement Next1BtnObj = GuiObj.FindAutomationElement(SetupDialogObj, Next1BtnCondition, TreeScope.Descendants, "Xencenter install for all radiobtn", 1, LogFilePath);
            GuiObj.ClickButton(Next1BtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
            //Click install btn
            PropertyCondition InstallBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "648", 1, LogFilePath);
            AutomationElement InstallBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, InstallBtnCondition, TreeScope.Descendants, "Xencenter install for all radiobtn", 1, LogFilePath);
            GuiObj.ClickButton(InstallBtnObj, 1, "Install Button", 1, LogFilePath);
            AutomationElement FinishBtnObj = PgmObj.WaitTillFinishButtonIsActive(WindowReturnCondition);
            if (FinishBtnObj == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Xencenter installation timeout", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                FileObj.ExitTestEnvironment();
            }
            
            GuiObj.ClickButton(FinishBtnObj, 0, "Finish Button", 1, LogFilePath);
            File.Create(XenCenterInstallStatusFile);
            PgmObj.CheckXenCenterInstalledVersion(HelpMenu, AboutXenCenterMenu, XenCenterVersion);

            FileObj.FinishCurrentTest("Installer");

            
        }

        public AutomationElement WaitTillFinishButtonIsActive(PropertyCondition WindowReturnCondition)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ClickCancWaitTillFinishButtonIsActiveel", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===============================", "info");
            
            int Timer = 0;
            int Timeout = 600000; //10 min
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition FinishBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "589", 1, LogFilePath);
            AutomationElement FinishBnObj = null;
            while (FinishBnObj == null && Timer<Timeout)
            {
                AutomationElement SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
                FinishBnObj = GuiObj.FindAutomationElement(SetupDialogObj, FinishBtnReturnCondition, TreeScope.Descendants, "FinishBtn", 1, LogFilePath);
                if (FinishBnObj != null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Finish btn is active aftre Xencenter install", "info");
                    return FinishBnObj;

                }
                Timer = Timer + 3000;
                Thread.Sleep(3000);
            }
            NewLogObj.WriteLogFile(LogFilePath, "Finish btn is not active even aftre Xencenter install timeout", "fail");
            return null;

        }

        public void ClickCancel(AutomationElement SetupDialogObj, string SetUpWindowTitle,string BtnToClick)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ClickCancel", "info");
            NewLogObj.WriteLogFile(LogFilePath, "============", "info");
            
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition CancelBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "551", 1, LogFilePath);
            AutomationElement CancelBnObj = GuiObj.FindAutomationElement(SetupDialogObj, CancelBtnReturnCondition, TreeScope.Descendants, "Next Btn", 1, LogFilePath);
            GuiObj.ClickButton(CancelBnObj, 1, "Cancel Button", 1, LogFilePath);
            Thread.Sleep(2000);
            //Check if the child iwnodw with "yes"/'No" btns has appeared\
            PropertyCondition ChildWndReturnCondition = GuiObj.SetPropertyCondition("NameProperty", SetUpWindowTitle, 1, LogFilePath);
            AutomationElement ChildWindowObj = GuiObj.FindAutomationElement(SetupDialogObj, ChildWndReturnCondition, TreeScope.Descendants, "Yes No Window", 1, LogFilePath);
            if(string.Compare(BtnToClick,"yes")==0)
            {

                PropertyCondition YesBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "618", 1, LogFilePath);
                AutomationElement YesBtnwObj = GuiObj.FindAutomationElement(ChildWindowObj, YesBtnReturnCondition, TreeScope.Descendants, "Yes Btn", 1, LogFilePath);
                GuiObj.ClickButton(YesBtnwObj, 1, "Yes Button", 1, LogFilePath);
            }
            else if (string.Compare(BtnToClick,"no")==0)
            {
                    PropertyCondition NoBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "617", 1, LogFilePath);
                    AutomationElement NoBtnwObj = GuiObj.FindAutomationElement(ChildWindowObj, NoBtnReturnCondition, TreeScope.Descendants, "No Btn", 1, LogFilePath);
                    GuiObj.ClickButton(NoBtnwObj, 1, "No Button", 1, LogFilePath);
            }

            Thread.Sleep(3000);


        }

        public void CheckInterruptedDialog(PropertyCondition SetupDialogObjCondition, string InterruptedDialog)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckInterruptedDialog", "info");
            NewLogObj.WriteLogFile(LogFilePath, "============", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            AutomationElement SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, SetupDialogObjCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
            
            PropertyCondition InterruptedDialogReturnCondition = GuiObj.SetPropertyCondition("NameProperty", InterruptedDialog, 1, LogFilePath);
            AutomationElement InterruptedDialogObj = GuiObj.FindAutomationElement(SetupDialogObj, InterruptedDialogReturnCondition, TreeScope.Descendants, "Finish Btn", 1, LogFilePath);
            
        }

        //public void ClickFinishBtn(AutomationElement SetupDialogObj)
        public void ClickFinishBtn(PropertyCondition SetupDialogObjCondition)
        {

            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ClickFinishBtn", "info");
            NewLogObj.WriteLogFile(LogFilePath, "============", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            AutomationElement SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, SetupDialogObjCondition, TreeScope.Children, "Xencenter Setup window", 1, LogFilePath);
            PropertyCondition FinishBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "589", 1, LogFilePath);
            AutomationElement FinishBtnwObj = GuiObj.FindAutomationElement(SetupDialogObj, FinishBtnReturnCondition, TreeScope.Descendants, "Finish Btn", 1, LogFilePath);
            GuiObj.ClickButton(FinishBtnwObj, 1, "FinishBtnwObj", 1, LogFilePath);
        }

        public void VerifySetupWindowClosed(string SetUpWindowTitle)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "VerifySetupWindowClosed", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=====================", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            
            PropertyCondition WindowReturnCondition = GuiObj.SetPropertyCondition("NameProperty", SetUpWindowTitle, 1, LogFilePath);
            AutomationElement SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "Xencenter Setup window", 0, LogFilePath);
            if (SetupDialogObj == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "SetupDialog not closed even after clicking on finish btn", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting test", "fail");
                FileObj.ExitTestEnvironment();
                

            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "SetupDialog closed after clicking on finish btn", "pass");
            }

        }

        public void CheckXenCenterInstalledVersion(string HelpMenu, string AboutXenCenterMenu, string XenCenterVersion)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckXenCenterInstalledVersion", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==============================", "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();

            XenCenter NewOprtorObj = new XenCenter();
            FileOperations FileObj = new FileOperations();
            NewOprtorObj.OpenXenCenter(LogFilePath);
            Thread.Sleep(2000);
            AutomationElement NewAutoObj = NewOprtorObj.SetFocusOnWindow(AutomationElement.RootElement, "NameProperty", "XenCenter", TreeScope.Children, "XenCenter", 1, LogFilePath);
            int WaitTimeOut = 300000;//5 mins
            int timer = 0;
            while (NewAutoObj == null && timer < WaitTimeOut)
            {
                NewAutoObj = NewOprtorObj.SetFocusOnWindow(AutomationElement.RootElement, "NameProperty", "XenCenter", TreeScope.Children, "XenCenter", 1, LogFilePath);
                Thread.Sleep(2000);
                timer = timer + 2000;
                NewLogObj.WriteLogFile(LogFilePath, "Waiting for Xencenter to open", "info");
                Console.WriteLine("Waiting for Xencenter to open ");
            }
            if (NewAutoObj == null && timer >= WaitTimeOut)
            {
                Console.WriteLine("Timeout waiting for Xencenter to open  after installation.Exiting.. ");
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for Xencenter to open  after installation.Exiting.. ","fail");
                FileObj.ExitTestEnvironment();

            }
            NewLogObj.WriteLogFile(LogFilePath, "Xencenter has opened  after installation", "info");
            Console.WriteLine("Xencenter has opened  after installation ");

            
            //Check if periodic update cjheck window has come
            //Commenting for demo.. Remmber to uncomment *****************************************
            //PropertyCondition AllowUpdateWindowCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "AllowUpdatesDialog", 1, LogFilePath);
            //AutomationElement AllowUpdatesDialog = GuiObj.FindAutomationElement(NewAutoObj, AllowUpdateWindowCondition, TreeScope.Children, "AllowUpdatesDialog", 0, LogFilePath);
            //WaitTimeOut = 60000;//1 mins
            //timer = 0;
            //while (AllowUpdatesDialog == null && timer < WaitTimeOut)
            //{
            //    AllowUpdatesDialog = GuiObj.FindAutomationElement(NewAutoObj, AllowUpdateWindowCondition, TreeScope.Children, "AllowUpdatesDialog", 0, LogFilePath);
            //    Thread.Sleep(3000);
            //    timer = timer + 3000;
            //    NewLogObj.WriteLogFile(LogFilePath, "Waiting for AllowUpdatesDialog to open", "info");
            //    Console.WriteLine("Waiting for AllowUpdatesDialog to open ");
            //}
            //if (AllowUpdatesDialog == null && timer >= WaitTimeOut)
            //{
            //    Console.WriteLine("Timeout waiting for AllowUpdatesDialog to open  after installation. ");
            //    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for AllowUpdatesDialog to open  after installation. ", "fail");
            //    //Environment.Exit(1);

            //}
            //if (AllowUpdatesDialog != null)
            //{
            //    NewLogObj.WriteLogFile(LogFilePath, "AllowUpdatesDialog has opened  after installation", "info");
            //    Console.WriteLine("AllowUpdatesDialog has opened  after installation ");

            //    NewLogObj.WriteLogFile(LogFilePath, "AllowUpdatesDialog  found after installation", "info");
            //    PropertyCondition YesBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "YesButton", 1, LogFilePath);
            //    AutomationElement YesBtnObj = GuiObj.FindAutomationElement(AllowUpdatesDialog, YesBtnCondition, TreeScope.Children, "YesBtn", 1, LogFilePath);
            //    GuiObj.ClickButton(YesBtnObj, 1, "YesBtnObj", 1, LogFilePath);
            //    Thread.Sleep(1000);
            //}
            //Commenting for demo.. Remmber to uncomment *****************************************
            NewOprtorObj.InvokeXenCenterSubMenuItem(NewAutoObj, HelpMenu, AboutXenCenterMenu, LogFilePath, 1);
            Thread.Sleep(3000);

            PropertyCondition AboutDialogCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "AboutDialog", 1, LogFilePath);
            AutomationElement AboutDialogObj = GuiObj.FindAutomationElement(NewAutoObj, AboutDialogCondition, TreeScope.Children, "AboutDialog", 1, LogFilePath);
            PropertyCondition TblLayoutCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "tableLayoutPanel1", 1, LogFilePath);
            AutomationElement TblLayoutObj = GuiObj.FindAutomationElement(AboutDialogObj, TblLayoutCondition, TreeScope.Children, "AboutDialog", 1, LogFilePath);

            //Get the version
            PropertyCondition VersionLabelCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "VersionLabel", 1, LogFilePath);
            AutomationElement VersionLabelObj = GuiObj.FindAutomationElement(TblLayoutObj, VersionLabelCondition, TreeScope.Children, "AboutDialog", 1, LogFilePath);
            string VersionNumber = VersionLabelObj.Current.Name.ToString();
            NewLogObj.WriteLogFile(LogFilePath, "VersionNumber found after install *" + VersionNumber+"*", "info");
            NewLogObj.WriteLogFile(LogFilePath, "Expected VersionNumber XenCenterVersion *" + XenCenterVersion + "*", "info");

            if (string.Compare(XenCenterVersion, VersionNumber) == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Right version of Xencenter installed " + VersionNumber, "pass");
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Wrong version of Xencenter installed. Version found from Xenvcenter " + VersionNumber + "Version from inputs file " + XenCenterVersion, "fail");
            }
            PropertyCondition OKBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "OkButton", 1, LogFilePath);
            AutomationElement OKBtnObj = GuiObj.FindAutomationElement(TblLayoutObj, OKBtnCondition, TreeScope.Children, "OKBtnObj", 1, LogFilePath);
            GuiObj.ClickButton(OKBtnObj, 1, "OKBtnObj", 1, LogFilePath);

            

        }
    }
}

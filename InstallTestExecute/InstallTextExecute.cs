using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Automation;
using System.Diagnostics;
using System.Threading;
//using MbUnit;

using GUICollection;
using LoggerCollection;
using XenCenterOperations;
using FileOperationsCollection;
using TestAPICollection;

namespace InstallTestExecute
{
    class InstallTextExecute
    {
        static void Main(string[] args)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.CreateLogFolder();
            string LogFilePath = NewLogObj.CreateLogFile();
            NewLogObj.WriteLogFile(LogFilePath, "Test : InstallTextExecute", "info");
            NewLogObj.WriteLogFile(LogFilePath, "======================", "info");
            Console.WriteLine("Starting testexceute installation");
            //This file will be used by another scripts to verify that test complete installed successfully
            string StatusFilePath = "C:\\TestCompleteInstalledSuccess.txt";
            if(File.Exists(StatusFilePath))
            {
                File.Delete(StatusFilePath);
            }
            Console.WriteLine("Starting the process testexceute ");
            System.Diagnostics.Process.Start("C:\\testexecute80.exe");
            Thread.Sleep(10000);
            Console.WriteLine("process started ");
            //Process[] pname = Process.GetProcessesByName("testexecute80.exe");

            //if (pname.Length == 0)
            //{
            //    NewLogObj.WriteLogFile(LogFilePath, "Uable to start testexecute", "fail");
            //    Environment.Exit(1);
            //}
            //Will take some time for the installer to launch & unzip file
            Thread.Sleep(15000);
            int WindowWaitTimeOut = 3000000;
            int timer = 0;
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition WindowReturnCondition = GuiObj.SetPropertyCondition("NameProperty", "AutomatedQA TestExecute 8 - InstallShield Wizard", 1, LogFilePath);
            AutomationElement SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            Console.WriteLine("Looking for setupdailaog ");
            while (SetupDialogObj == null && timer < WindowWaitTimeOut)
            {
                SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
                timer=timer+3000;
                Thread.Sleep(3000);
            }

            if (SetupDialogObj == null && timer >= WindowWaitTimeOut)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Uable to find testexecute window even after timeout. Exiting ..", "fail");
                Environment.Exit(1);
            }

            int WaitForNextBtn = 300000;
            timer = 0;
            Console.WriteLine("Looking for nexr btn ");
            PropertyCondition NextBtnReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "1", 1, LogFilePath);
            AutomationElement NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            while (NextBtnObj == null && timer < WaitForNextBtn)
            {
                SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
                if (SetupDialogObj != null)
                {
                    NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
                }
                Thread.Sleep(3000);
                timer = timer + 3000;
            }
            if (NextBtnObj == null && timer >= WaitForNextBtn)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Uable to find Next btn  even after timeout. Exiting ..", "fail");
                Environment.Exit(1);
            }
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            PropertyCondition AcceptReturnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", "1000", 1, LogFilePath);
            AutomationElement AcceptObj = GuiObj.FindAutomationElement(SetupDialogObj, AcceptReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            GuiObj.SetRadioButton(AcceptObj, "Accept license", 1, LogFilePath);

            NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
            GuiObj.ClickButton(NextBtnObj, 1, "Next Button", 1, LogFilePath);
            
            //Wait for Installation complete window
            int InstallationTimeout = 1200000; // 20 mins
            int InstallationTimer = 0;

            SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
            PropertyCondition InstallationCompleteLabelCondition = GuiObj.SetPropertyCondition("NameProperty", "TestExecute Installation Complete", 1, LogFilePath);
            AutomationElement InstallCompleteLabelObj = GuiObj.FindAutomationElement(SetupDialogObj, InstallationCompleteLabelCondition, TreeScope.Children, "TestExecute install complete label", 0, LogFilePath);

            while (InstallCompleteLabelObj == null && InstallationTimer < InstallationTimeout)
            {
                Thread.Sleep(5000);
                InstallationTimer = InstallationTimer + 5000;
                SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
                if (SetupDialogObj != null)
                {
                    InstallCompleteLabelObj = GuiObj.FindAutomationElement(SetupDialogObj, InstallationCompleteLabelCondition, TreeScope.Children, "TestExecute install complete label", 0, LogFilePath);
                }

            }

            if (InstallCompleteLabelObj == null && InstallationTimer >= InstallationTimeout)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Testexecute installation timed out.. Exiting", "fail");
                Environment.Exit(1);
            }
            if (InstallCompleteLabelObj != null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Testexecute installaticompleted", "info");
                
                SetupDialogObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, WindowReturnCondition, TreeScope.Children, "TestExecute Setup window", 0, LogFilePath);
                NextBtnObj = GuiObj.FindAutomationElement(SetupDialogObj, NextBtnReturnCondition, TreeScope.Children, "Next Btn", 0, LogFilePath);
                GuiObj.ClickButton(NextBtnObj, 1, "Finsih Button", 1, LogFilePath);

                //Create status file
                File.Create(StatusFilePath);
        
            }
        }

        
    }
}

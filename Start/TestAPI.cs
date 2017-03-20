using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

using Microsoft.Test;

using GUICollection;
using LoggerCollection;

namespace TestAPICollection
{
    public class TestAPI
    {

        public void ClickLeftBtnOnAutomationElement(AutomationElement AutoEle, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                System.Windows.Point clickablePoint = AutoEle.GetClickablePoint();
                if (clickablePoint != null)
                {
                    System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(clickablePoint.X), Convert.ToInt32(clickablePoint.Y));
                    Microsoft.Test.Input.Mouse.MoveTo(p);
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                }
                else
                {
                     AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                    GuiObj.GetPositionFromBoundingRectangleAndClick(AutoEle,LogFilePath,"Left");
                    
                }
                //else
                //{
                //    NewLogObj.WriteLogFile(LogFilePath, "Unable tp get the clickable point", "fail");
                //    if (TerminateStatus == 1)
                //    {
                //        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                //        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnAutomationElement**", "fail");
                //        Environment.Exit(1);
                //    }
                //}

            }

            catch (Exception Ex)
            {
                //NewLogObj.WriteLogFile(LogFilePath, "Exception at ClickOnAutomationElement" + Ex.ToString(), "fail");
                //if (TerminateStatus == 1)
                //{
                //    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                //    NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnAutomationElement**", "fail");
                //    Environment.Exit(1);
                //}
                //else
                //{
                //    NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                //}
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                GuiObj.GetPositionFromBoundingRectangleAndClick(AutoEle, LogFilePath, "Left");
            }
        }

        public void ClickRightBtnOnAutomationElement(AutomationElement AutoEle, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            try
            {
                System.Windows.Point clickablePoint = AutoEle.GetClickablePoint();
                if (clickablePoint != null)
                {
                    System.Drawing.Point p = new System.Drawing.Point(Convert.ToInt32(clickablePoint.X), Convert.ToInt32(clickablePoint.Y));
                    Microsoft.Test.Input.Mouse.MoveTo(p);
                    Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Right);
                }
                else
                {
                    AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                    GuiObj.GetPositionFromBoundingRectangleAndClick(AutoEle, LogFilePath, "Right");

                }
                //else
                //{
                //    NewLogObj.WriteLogFile(LogFilePath, "Unable tp get the clickable point", "fail");
                //    if (TerminateStatus == 1)
                //    {
                //        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                //        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnAutomationElement**", "fail");
                //        Environment.Exit(1);
                //    }
                //}

            }

            catch (Exception Ex)
            {
                //    NewLogObj.WriteLogFile(LogFilePath, "Exception at ClickOnAutomationElement" + Ex.ToString(), "fail");
                //    if (TerminateStatus == 1)
                //    {
                //        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 1.", "info");
                //        NewLogObj.WriteLogFile(LogFilePath, "***Exiting application from ClickOnAutomationElement**", "fail");
                //        Environment.Exit(1);
                //    }
                //    else
                //    {
                //        NewLogObj.WriteLogFile(LogFilePath, TerminateStatus + "is 0.", "info");
                //    }

                AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                GuiObj.GetPositionFromBoundingRectangleAndClick(AutoEle, LogFilePath, "Left");
            }
        }
    }
}

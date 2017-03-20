using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using LoggerCollection;
using GUICollection;

namespace VerifyTests
{
    public class Verify
    {
        public int CheckTextBoxText(AutomationElement TextBoxObj,string ExpectedText,int TerminateStatus,string TBName)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath=NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "CheckTextBoxText", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==================", "info");
            NewLogObj.WriteLogFile(LogFilePath, "Checking "+TBName+" textbox for text "+ExpectedText, "info");
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            string ActaulText=GuiObj.GetTextBoxText(TextBoxObj, TBName, 0, LogFilePath);
            NewLogObj.WriteLogFile(LogFilePath, "ActaulText on " + TBName + "  " + ActaulText, "info");
            if (ActaulText != null)
            {
                if (string.Compare(ActaulText, ExpectedText) == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "ExpectedText found", "info");
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                NewLogObj.WriteLogFile(TBName, "Textbox value is null", "info");
                return 0;
            }
            
            return 0;



        }
    }
}

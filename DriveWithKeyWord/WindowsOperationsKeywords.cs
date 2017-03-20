using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LoggerCollection;
using WindowsOperationsCollection;

namespace WindowsOperationsKeywordsCollection
{
    public class WindowsOperationsKeywords
    {

        public void ProcessWindowsOperationsKeywords(string CommandName, Dictionary<string, string> ParamValueMapDict)
        {
            //Remove the XS: part from command name
            int IndexOfColon = CommandName.IndexOf(":");
            CommandName = CommandName.Substring(IndexOfColon + 2);
            CommandName = CommandName.ToLower();
            switch (CommandName)
            {
                case "waittillsystemisup":
                    int WaitTimeOut=1;
                    if (ParamValueMapDict.ContainsKey("waittimeoutinmins"))
                    {
                        Int32.TryParse(ParamValueMapDict["waittimeoutinmins"], out WaitTimeOut);
                    }
                    WaitTillSystemIsUp(ParamValueMapDict["systemip"], WaitTimeOut);
                    break;
                case "authenticatetoremotemachinepath":

                    AuthenticateToRemoteSystem(ParamValueMapDict["systempathwithip"], ParamValueMapDict["systemusername"], ParamValueMapDict["systempassword"]);
                    break;
                
            }

        }
        public void WaitTillSystemIsUp(string SystemIP,int WaitTimeout)
        {
            WindowsOperations WindowsOpsObj = new WindowsOperations();
            WindowsOpsObj.WaitTillSystemIsUp(SystemIP, WaitTimeout);
        }
        public void AuthenticateToRemoteSystem(string SystemIP, string USerName,string Password)
        {
            WindowsOperations WindowsOpsObj = new WindowsOperations();
            WindowsOpsObj.AuthenticateToRemoteMachine(SystemIP, USerName, Password);
        }
        
    }
}

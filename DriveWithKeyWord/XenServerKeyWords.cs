using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LoggerCollection;
using XenServerCollection;

namespace XenServerKeyWordsCollection
{
    public class XenServerKeyWords
    {

        public void ProcessXenserverKeywords(string CommandName,Dictionary<string, string> ParamValueMapDict)
        {
            //Remove the XS: part from command name
            int IndexOfColon = CommandName.IndexOf(":");
            CommandName = CommandName.Substring(IndexOfColon + 2);
            CommandName = CommandName.ToLower();
            switch (CommandName)
            {
                case "waittillvmipisobtained":
                    WaitTillVMIPIsObtained(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["vmnamelabel"]);
                break;
                case "importtemplatefromcifsshare":
                    ImportTemplateFromCIFSShare(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["cifssharepath"], ParamValueMapDict["cifsusername"], ParamValueMapDict["cifspassword"], ParamValueMapDict["vmnametoimport"]);
                break;
                case "importtemplatefromcifsshareandcreatevm":
                    ImportTemplateFromCIFSShareAndCreateVM(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["cifssharepath"], ParamValueMapDict["cifsusername"], ParamValueMapDict["cifspassword"], ParamValueMapDict["templatenametoimport"], ParamValueMapDict["newvmname"]);
                break;
                case "startvm":
                    StartVM(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["vmparamtype"], ParamValueMapDict["vmidentifier"]);
                break;
                case "shutdownvm":
                    ShutdownVM(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["vmparamtype"], ParamValueMapDict["vmidentifier"]);
                break;
                case "startvmfromtemplate":
                   StartVMFromTemplate(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["newvmname"], ParamValueMapDict["templateidentifier"]);
                break;
                case "starthostbackup":
                    StartHostBackUp(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["backuplocationwithfilename"]);
                break;
                case "storeserverkeyinputtycache":
                    StoreServerKeyInPuttyCache(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"]);
                break;
                case "movevdi":
                    MoveVDI(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["vmnamelabel"], ParamValueMapDict["srnamelabel"]);
                break;
                case "movevm":
                    MoveVM(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["vmnamelabel"], ParamValueMapDict["srnamelabel"], ParamValueMapDict["newvmnamelabel"]);
                break;
                case "shutdownallvms":
                    ShutDownAllVMs(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"]);
                break;
                case "deleteallvms":
                    DeleteAllVMs(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"]);
                break;
                case "waittillvmisavailable":
                    int TimeoutInMins;
                    Int32.TryParse(ParamValueMapDict["waittimeoutinmins"], out TimeoutInMins);
                    WaitTillVMIsAvailable(ParamValueMapDict["serverip"], ParamValueMapDict["serverusername"], ParamValueMapDict["serverpassword"], ParamValueMapDict["vmnamelabel"], TimeoutInMins);
                break;
            }

        }
        public void WaitTillVMIPIsObtained(string ServerIP, string ServerUserName, string ServerPassword, string VMNameLabel)
        {
            XenServer XenObj = new XenServer();
            XenObj.GetVMIP(ServerIP, ServerUserName, ServerPassword, VMNameLabel, 0);

        }
        public void ImportTemplateFromCIFSShare(string ServerIP, string ServerUserName, string ServerPassword, string CIFSSharePath, string CIFSUserName, string CIFSPassword, string VMNameLabel)
        {
            XenServer XenObj = new XenServer();
            XenObj.ImportVM(ServerIP, ServerUserName, ServerPassword, CIFSSharePath, CIFSUserName, CIFSPassword, VMNameLabel);
        }
        public void ImportTemplateFromCIFSShareAndCreateVM(string ServerIP, string ServerUserName, string ServerPassword, string CIFSSharePath, string CIFSUserName, string CIFSPassword, string TemplateNameLabel,string NewVMName)
        {
            XenServer XenObj = new XenServer();
            XenObj.ImportTemplateFromCIFSShareAndStartVM(ServerIP, ServerUserName, ServerPassword, CIFSSharePath, CIFSUserName, CIFSPassword, TemplateNameLabel, NewVMName);
        }

        public void StartVM(string ServerIP, string ServerUserName, string ServerPassword,string VMParamType,string VMIdentifier)
        {
            XenServer XenObj = new XenServer();
            XenObj.StartVM(ServerIP, ServerUserName, ServerPassword, VMParamType, VMIdentifier);
        }
        public void ShutdownVM(string ServerIP, string ServerUserName, string ServerPassword, string VMParamType, string VMIdentifier)
        {
            XenServer XenObj = new XenServer();
            XenObj.ShutdownVM(ServerIP, ServerUserName, ServerPassword, VMParamType, VMIdentifier,1);
        }
        public void StartVMFromTemplate(string ServerIP, string ServerUserName, string ServerPassword,string VMName,string TemplateIdentifier)
        {
            XenServer XenObj = new XenServer();
            XenObj.InstallVMFromTemplate(ServerIP, ServerUserName, ServerPassword, VMName, TemplateIdentifier);
            XenObj.StartVM(ServerIP, ServerUserName, ServerPassword, "name-label", VMName);
        }
        public void StartHostBackUp(string ServerIP, string ServerUserName, string ServerPassword, string BackUpLocationWithFileName)
        {
            XenServer XenObj = new XenServer();
            XenObj.StartHostBackup(ServerIP, ServerUserName, ServerPassword, BackUpLocationWithFileName);
        }
        public void StoreServerKeyInPuttyCache(string ServerIP, string ServerUserName, string ServerPassword)
        {
            XenServer XenObj = new XenServer();
            XenObj.StoreServerKeyInPuttyCache(ServerIP, ServerUserName, ServerPassword);
        }

        public void MoveVDI(string ServerIP, string ServerUserName, string ServerPassword, string VMNameLabel, string SRNameLabel)
        {
            XenServer XenObj = new XenServer();
            XenObj.MoveVDI(ServerIP, ServerUserName, ServerPassword, VMNameLabel, SRNameLabel);

        }
        public void MoveVM(string ServerIP, string ServerUserName, string ServerPassword, string VMNameLabel, string SRNameLabel,string NewVMNameLabel)
        {
            XenServer XenObj = new XenServer();
            XenObj.MoveVM(ServerIP, ServerUserName, ServerPassword, VMNameLabel, SRNameLabel, NewVMNameLabel);

        }
        public void ShutDownAllVMs(string ServerIP, string ServerUserName, string ServerPassword)
        {
            XenServer XenObj = new XenServer();
            XenObj.ShutdownAllVMs(ServerIP, ServerUserName, ServerPassword);
        }
        public void DeleteAllVMs(string ServerIP, string ServerUserName, string ServerPassword)
        {
            XenServer XenObj = new XenServer();
            XenObj.DeleteAllVMs(ServerIP, ServerUserName, ServerPassword);
        }
        public void WaitTillVMIsAvailable(string ServerIP, string ServerUserName, string ServerPassword, string VMNameLabel,int WaitTimeOutInMins)
        {
            XenServer XenObj = new XenServer();
            XenObj.WaitTillVMIsAvailable(ServerIP, ServerUserName, ServerPassword, VMNameLabel, WaitTimeOutInMins);
        }
    }
}

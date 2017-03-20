using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

using XenAPI;
using LoggerCollection;
using FileOperationsCollection;
using GenericCollection;

namespace XenServerCollection
{
    public class XenServer
    {

        public Session Login(string HostName, int Port, string UserName, string Password)
        {
            Session session = new Session(HostName, Port);

            // Authenticate with username and password. 
            session.login_with_password(UserName, Password, API_Version.API_1_3);
            return session;

        }

        public void GetVMParam(Session session, string VMName, string Parameter)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GetVMParam", "info");
            NewLogObj.WriteLogFile(LogFilePath, "==============", "info");
            NewLogObj.WriteLogFile(LogFilePath, "VMName " + VMName, "info");
            List<XenRef<VM>> vmRefs = VM.get_all(session);
            foreach (XenRef<VM> vmRef in vmRefs)
            {
                VM vm = VM.get_record(session, vmRef);
                //if (string.Compare(VMName, vm.name_label) == 0)
                //{
                    NewLogObj.WriteLogFile(LogFilePath, "Required VM "+VMName+ " found", "info");
                    
                    XenRef<VM_guest_metrics> GuestMetrics = vm.guest_metrics;
                    //GuestMetrics.
                    //List<XenRef<Network>> NWRefs=XenAPI.Network.get_all(session);
                    XenRef<VM_guest_metrics> gmsref = vm.guest_metrics;
                    //VM_guest_metrics gms = VM_guest_metrics.get_record(session, gmsref);
                    Dictionary<String, String> dict = null;
                    dict = VM_guest_metrics.get_networks(session, gmsref);
                    
                   // dict = gms.networks;
                    String vmIP = null;
                    foreach (String keyStr in dict.Keys)
                    {
                        vmIP = (String)(dict[keyStr]);
                        System.Console.WriteLine(vmIP);
                    }


               // }

                System.Console.WriteLine("Name: {0}\nvCPUs: {1}\nDescription: {2}\n-", vm.name_label, vm.VCPUs_at_startup, vm.name_description);
            }
        }

        public int GetVMIP(string ServerIP, string username, string password,string VMNameLabel,int Timer)
        {

            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            FileOperations FileObj = new FileOperations();
            string hostname = ServerIP;
            int port = 80; // default
            int ExceptionHandlingnotReqdFlag = 0;
            
            try
            {
                Session session = new Session(hostname, port);
                session.login_with_password(username, password, API_Version.API_1_3);
                //MessageBox.Show("Conection Successfully Estabilished to the XenServer: " +Connection_XS_IP +"");
                List<XenRef<VM>> vmRefs = VM.get_all(session);

                StartSearch:
                foreach (XenRef<VM> vmRef in vmRefs)
                { 
                    //http://forums.citrix.com/thread.jspa?threadID=244784
                    
                    VM vm = VM.get_record(session, vmRef);

                    if (!vm.is_a_template && !vm.is_control_domain && !vm.is_a_snapshot && !vm.is_snapshot_from_vmpp)
                    {
                        //System.Console.Write("\nVM Name: {0} -> IP Address: ", vm.name_label);                                                
                        if (string.Compare(vm.name_label, VMNameLabel) == 0)
                        {
                            try
                            {
                                NewLogObj.WriteLogFile(LogFilePath, VMNameLabel+" found. Fetching records", "info");
                                bool dict_key = true;
                                int WaitTimeout = 2400000; //40 mins
                                
                                //Wait till VM starts running
                                while (!(string.Compare(vm.power_state.ToString(), "Running") == 0) && Timer < WaitTimeout)
                                {
                                    System.Console.WriteLine("Waiting for VM " + VMNameLabel + " to poweron");
                                    Thread.Sleep(1000);
                                    Timer = Timer + 1000;
                                    goto StartSearch;
                                }
                                if (!(string.Compare(vm.power_state.ToString(), "Running") == 0) && Timer >= WaitTimeout)
                                {
                                    System.Console.WriteLine("Timeout waiting for VM " + VMNameLabel + " to poweron.");
                                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for VM " + VMNameLabel + " to poweron.", "fail");
                                    return -1;
                                }
                                
                                while(dict_key==true && Timer<WaitTimeout )
                                {
                                    try
                                    {
                                        int IPFound = 0;
                                       
                                        XenRef<VM_guest_metrics> gmsref = vm.guest_metrics;
                                        VM_guest_metrics gms = VM_guest_metrics.get_record(session, gmsref);
                                        Dictionary<String, String> dict = null;
                                        dict = gms.networks;

                                        String vmIP = null;
                                        foreach (String keyStr in dict.Keys)
                                        {
                                            dict_key = false;
                                            vmIP = (String)(dict[keyStr]);
                                            //excluding IPv6 address which contains the character :
                                            if (!vmIP.Contains(':'))
                                            {
                                                if (!vmIP.StartsWith("169"))
                                                {
                                                    System.Console.WriteLine(vmIP);
                                                    NewLogObj.WriteLogFile(LogFilePath, "IP address obtained for VM " + VMNameLabel + " is " + vmIP, "info");
                                                    IPFound = 1;
                                                    return 1;
                                                }
                                            }

                                        } //for each keyStr
                                        if (dict_key == false || IPFound==0)
                                        {
                                            System.Console.WriteLine("No valid IP addressfound for "+VMNameLabel);
                                            NewLogObj.WriteLogFile(LogFilePath, "No valid IP addressfound for " + VMNameLabel, "fail");
                                            ExceptionHandlingnotReqdFlag = 1;
                                            return -1;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                       //To avoid reduntant calls to function
                                        if (ExceptionHandlingnotReqdFlag == 1)
                                        {
                                            return -1;
                                        }
                                        else
                                        {
                                            //System.Console.WriteLine(e.ToString() + " Waiting ..");
                                            System.Console.WriteLine(" Waiting for VM " + VMNameLabel + "to aquire IP address..");
                                            Thread.Sleep(1000);
                                            Timer = Timer + 1000;
                                            goto StartSearch;
                                            //GetVMIP(ServerIP, username, password, VMNameLabel, Timer);
                                        }
                                    }
                                }
                                if (dict_key == true && Timer > WaitTimeout)
                                {
                                    NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for the VM " + VMNameLabel + " to obtain IP address", "fail");
                                    FileObj.ExitTestEnvironment();
                                    return -1;
                                }
                                
                            } //try loop
                            catch (Exception e)
                            {
                                System.Console.WriteLine(e.ToString());
                                return -1;
                            }
                        }
                    } //check and allow only VMs there by excluding templates/controldomain/snapshots/etc...
                  
                }  //For each...                     
            } // try to connect XenServer host. 

            catch (Exception e1)
            {
                string exceptionText = "Server \"" + hostname + "\" cannot be contacted." + "\n\nError: " + e1.ToString();
                System.Console.WriteLine(exceptionText);
            }
            //System.Console.WriteLine("Press a key to continue...");
            //System.Console.ReadLine();
            return -1;
        }

        public void StoreServerKeyInPuttyCache(string ServerIp, string ServerUserName, string ServerPassword)
        {
            //string Cmd = "plink -l root -pw citrix 10.105.83.51 mount -t cifs -o username=srini,password=srini //10.105.74.63/iso /mnt";
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
           // `echo y | plink -ssh $XenServerIP -l root -pw citrix exit`;
            String StoreServerKeyInPuttyCacheCmd = "echo y | plink -ssh " + ServerIp + " -l " + ServerUserName + " -pw " + ServerPassword + " exit";
            NewLogObj.WriteLogFile(LogFilePath, "StoreServerKeyInPuttyCacheCmd : " + StoreServerKeyInPuttyCacheCmd, "info");
            GenericObj.StartCmdExecutionAndWaitForCompletion(StoreServerKeyInPuttyCacheCmd);
        }
        public void MountCIFS(string ServerIp, string ServerUserName, string ServerPassword, string CIFSPathToImportFrom, string CIFSUserName, string CIFSPassword)
        {
            //string Cmd = "plink -l root -pw citrix 10.105.83.51 mount -t cifs -o username=srini,password=srini //10.105.74.63/iso /mnt";
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            String MountCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " mount -t cifs -o username=" + CIFSUserName + ",password=" + CIFSPassword + " " + CIFSPathToImportFrom+ " /mnt";
            NewLogObj.WriteLogFile(LogFilePath, "CIFS mount cmd : " + MountCmd, "info");
            GenericObj.StartCmdExecutionAndWaitForCompletion(MountCmd);
        }

        public void UnMountCIFS(string ServerIp, string ServerUserName, string ServerPassword)
        {
            //plink -l root -pw citrix 10.105.83.51 umount -f /mnt
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            String UMountCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " umount -f /mnt";
            NewLogObj.WriteLogFile(LogFilePath, "CIFS unmount cmd : " + UMountCmd, "info");
            GenericObj.StartCmdExecutionAndWaitForCompletion(UMountCmd);
        }
        public string ImportVM(string ServerIp, string ServerUserName, string ServerPassword, string CIFSPathToImportFrom, string CIFSUserName, string CIFSPassword,string TemplateName)
        {
            //plink -l root -pw citrix 10.105.83.51 umount -f /mnt
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            StoreServerKeyInPuttyCache(ServerIp, ServerUserName, ServerPassword);

            //Mount the CIFS path having the template
            MountCIFS(ServerIp, ServerUserName, ServerPassword, CIFSPathToImportFrom, CIFSUserName, CIFSPassword);

            //plink -l root -pw citrix 10.105.83.51 xe vm-import filename=/mnt/GSEN-WIN8x86.xva
            string ImportVMcmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-import filename=/mnt/" + TemplateName;
            System.Console.WriteLine(ImportVMcmd);
            NewLogObj.WriteLogFile(LogFilePath, "ImportVMcmd : " + ImportVMcmd, "info");
            //GenericObj.StartCmdExecutionAndExit(ImportVMcmd); 
            string result=GenericObj.StartCmdExecutionAndWaitForCompletion(ImportVMcmd); // ********************************************Comment
            System.Console.WriteLine(result);
            NewLogObj.WriteLogFile(LogFilePath, "ImportVMcmd result: " + result, "info");
            //Remove whitespaces
            result = result.Trim();
            NewLogObj.WriteLogFile(LogFilePath, "ImportVMcmd result after format: *" + result+"*", "info");
            
            //unmount Cifs after import
            UnMountCIFS(ServerIp, ServerUserName, ServerPassword);
            return result;
            
        }
        public string InstallVMFromTemplate(string ServerIp, string ServerUserName, string ServerPassword,string VMName, string TemplateIdentifer)
        {
            //plink -l root -pw citrix 10.105.83.51 umount -f /mnt
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

             //plink -l $XenServerUserName -pw $XenServerPasswd $XenServerIP xe vm-install new-name-label=$NewVMName template=$InputTemplateUUID
            string InstallVMFromtemplatecmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-install new-name-label=" + VMName + " template=" + TemplateIdentifer;
            //System.Console.WriteLine(ImportVMcmd);
            NewLogObj.WriteLogFile(LogFilePath, "InstallVMFromtemplatecmd : " + InstallVMFromtemplatecmd, "info");
            //GenericObj.StartCmdExecutionAndExit(ImportVMcmd);  //--> ************************************Uncomment
            string result = GenericObj.StartCmdExecutionAndWaitForCompletion(InstallVMFromtemplatecmd); // ********************************************Comment
            System.Console.WriteLine(result);
            NewLogObj.WriteLogFile(LogFilePath, "InstallVMFromtemplatecmd result: " + result, "info");
            result = result.Trim();
            NewLogObj.WriteLogFile(LogFilePath, "InstallVMFromtemplatecmd result after format: *" + result + "*", "info");
            return result;
            //unmount Cifs after import
            //UnMountCIFS(ServerIp, ServerUserName, ServerPassword);

        }

        public void StartHostBackup(string ServerIp, string ServerUserName, string ServerPassword,string BackUpLocationWithFileName)
        {
             //xe host-backup file-name="\\10.105.95.1\GSData (E)\People\Srinivas\ISOs\TesthostBackup"
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            String StartHostBackupCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe host-backup file-name=" + "\"" + BackUpLocationWithFileName + "\"";
            NewLogObj.WriteLogFile(LogFilePath, "StartHostBackupCmd : " + StartHostBackupCmd, "info");
            GenericObj.StartCmdExecutionAndWaitForCompletion(StartHostBackupCmd);
        }

        public void WaitTillVMIsAvailable(string ServerIp, string ServerUserName, string ServerPassword, string VMNamelabel,int WaitTimeOutInMins)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string VMQueryCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-list name-label=" + VMNamelabel;
            string result = GenericObj.StartCmdExecutionAndWaitForCompletion(VMQueryCmd);
            System.Console.WriteLine(result);
            NewLogObj.WriteLogFile(LogFilePath, "VMQueryCmd result: " + result, "info");
            int WaitTimer = 0;
            int WaitTimeOutInSecs=WaitTimeOutInMins*60*1000;
            while (string.IsNullOrWhiteSpace(result) && WaitTimer < WaitTimeOutInSecs)
            {
                Thread.Sleep(10000);
                WaitTimer = WaitTimer + 10000;
                result = GenericObj.StartCmdExecutionAndWaitForCompletion(VMQueryCmd);
            }
            if (string.IsNullOrWhiteSpace(result) && WaitTimer >= WaitTimeOutInSecs)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Timeout waiting for VM "+ VMNamelabel +"  to be available in server", "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();
            }
        }
        public string StartVM(string ServerIp, string ServerUserName, string ServerPassword, string VMParamType, string VMIdentifier)
        {
            
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            string VMPowerState = ReturnVMPowerState(ServerIp, ServerUserName, ServerPassword, VMParamType, VMIdentifier,0);
            VMPowerState = VMPowerState.ToLower();
            if (Regex.IsMatch(VMPowerState, "halted") || Regex.IsMatch(VMPowerState, "suspended"))
            {
                //plink -l $XenServerUserName -pw $XenServerPasswd $XenServerIP xe vm-start vm=$InputUUID
                string StartVMCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-start vm=" + VMIdentifier;

                NewLogObj.WriteLogFile(LogFilePath, "InstallVMFromtemplatecmd : " + StartVMCmd, "info");

                string result = GenericObj.StartCmdExecutionAndWaitForCompletion(StartVMCmd);
                System.Console.WriteLine(result);
                NewLogObj.WriteLogFile(LogFilePath, "StartVMCmd result: " + result, "info");
                result = result.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "StartVMCmd result after format: *" + result + "*", "info");
                return result;
                //unmount Cifs after import
                //UnMountCIFS(ServerIp, ServerUserName, ServerPassword);
            }
            else if (Regex.IsMatch(VMPowerState, "running"))
            {
                NewLogObj.WriteLogFile(LogFilePath, "StartVM: Vm with " + VMParamType + " " + VMIdentifier + " is already in running state.", "warn");
                return null;
            }
            return null;
        }

        public string ShutdownVM(string ServerIp, string ServerUserName, string ServerPassword, string VMParamType, string VMIdentifier,int TerminateStatus)
        {
            
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            //plink -l $XenServerUserName -pw $XenServerPasswd $XenServerIP xe vm-shutdown --force vm=$InputUUID
            string ShutdownVMCmd = null;
            //plink -l $XenServerUserName -pw $XenServerPasswd $XenServerIP xe vm-list uuid=$UUID
            string VMPowerState = ReturnVMPowerState(ServerIp, ServerUserName, ServerPassword, VMParamType, VMIdentifier, TerminateStatus);
            VMPowerState = VMPowerState.ToLower();
            if (Regex.IsMatch(VMPowerState, "running") || Regex.IsMatch(VMPowerState, "suspended"))
            {
                ShutdownVMCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-shutdown --force --multiple vm=\'" + VMIdentifier+"\'";
                //if (Regex.IsMatch(VMParamType, "uuid"))
                //{
                //    ShutdownVMCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-shutdown --force vm=" + VMIdentifier;
                //}
                //else if (Regex.IsMatch(VMParamType, "label"))
                //{
                //    ShutdownVMCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-shutdown --force vm=" + VMIdentifier;
                //}

                NewLogObj.WriteLogFile(LogFilePath, "InstallVMFromtemplatecmd : " + ShutdownVMCmd, "info");

                string result = GenericObj.StartCmdExecutionAndWaitForCompletion(ShutdownVMCmd);
                System.Console.WriteLine(result);
                NewLogObj.WriteLogFile(LogFilePath, "ShutdownVMCmd result: " + result, "info");
                result = result.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "ShutdownVMCmd result after format: *" + result + "*", "info");
                return result;
            }
            else if (Regex.IsMatch(VMPowerState, "halted"))
            {
                NewLogObj.WriteLogFile(LogFilePath, "ShutdownVM: Vm with " + VMParamType + " " + VMIdentifier + " is already in halted state.", "warn");
                return null;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "ShutdownVM: Invalid powerstate retrieved for Vm with " + VMParamType + " " + VMIdentifier, "warn");
                return null;
            }
        }

        public string ReturnVMPowerState(string ServerIp, string ServerUserName, string ServerPassword, string VMParamType, string VMIdentifier,int TerminateStatus)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string PowerStateCmd = null;
            //plink -l $XenServerUserName -pw $XenServerPasswd $XenServerIP xe vm-list uuid=$UUID
            VMParamType = VMParamType.ToLower();
            if (Regex.IsMatch(VMParamType, "uuid"))
            {
                PowerStateCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-list uuid=" + VMIdentifier;
            }
            else if(Regex.IsMatch(VMParamType, "label"))
            {
                PowerStateCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-list name-label=\'" + VMIdentifier+"\'";
            }

            NewLogObj.WriteLogFile(LogFilePath, "PowerStateCmd : " + PowerStateCmd, "info");

            string result = GenericObj.StartCmdExecutionAndWaitForCompletion(PowerStateCmd);
            System.Console.WriteLine(result);
            NewLogObj.WriteLogFile(LogFilePath, "PowerStateCmd result: " + result, "info");
            result = result.Trim();
            NewLogObj.WriteLogFile(LogFilePath, "PowerStateCmd result after format: *" + result + "*", "info");
            if (result.Length <= 1)
            {
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "VM of " + VMIdentifier + "does not exist" + "in server " + ServerIp, "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "VM of " + VMIdentifier + "does not exist" + "in server " + ServerIp, "warn");
                }
            }
            if (Regex.IsMatch(result, "halted", RegexOptions.IgnoreCase))
            {
                NewLogObj.WriteLogFile(LogFilePath, "PowerStateCmd is halted ", "info");
                return "halted";
            }
            else if (Regex.IsMatch(result, "running", RegexOptions.IgnoreCase))
            {
                NewLogObj.WriteLogFile(LogFilePath, "PowerStateCmd is running ", "info");
                return "running";
            }
            else if (Regex.IsMatch(result, "suspended", RegexOptions.IgnoreCase))
            {
                NewLogObj.WriteLogFile(LogFilePath, "PowerStateCmd is suspended ", "info");
                return "suspended";
            }

            return result;
        }

        public void ImportTemplateFromCIFSShareAndStartVM(string ServerIp, string ServerUserName, string ServerPassword, string CIFSPathToImportFrom, string CIFSUserName, string CIFSPassword, string TemplateName,string NewVMName)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string ImportedTemplateUUID= ImportVM(ServerIp, ServerUserName, ServerPassword, CIFSPathToImportFrom, CIFSUserName, CIFSPassword, TemplateName);
            NewLogObj.WriteLogFile(LogFilePath, "ImportedTemplateUUID *" + ImportedTemplateUUID + "*", "info");
            string VMUUID=InstallVMFromTemplate(ServerIp, ServerUserName, ServerPassword, NewVMName, ImportedTemplateUUID);
            //string VMUUID = InstallVMFromTemplate(ServerIp, ServerUserName, ServerPassword, NewVMName, TemplateName);
            NewLogObj.WriteLogFile(LogFilePath, "VMUUID *" + VMUUID + "*", "info");
           StartVM(ServerIp, ServerUserName, ServerPassword, "uuid",VMUUID);
            //StartVM(ServerIp, ServerUserName, ServerPassword, "name-label", NewVMName);
        }

        public string FindSRUUID(string ServerIp, string ServerUserName, string ServerPassword, string SRNamelabel)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            try
            {
                string FindSRUUIDCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe sr-list name-label=" + SRNamelabel;
                NewLogObj.WriteLogFile(LogFilePath, "FindSRUUIDCmd " + FindSRUUIDCmd, "info");
                string result = GenericObj.StartCmdExecutionAndWaitForCompletion(FindSRUUIDCmd);
                System.Console.WriteLine(result);
                NewLogObj.WriteLogFile(LogFilePath, "FindSRUUIDCmd result: " + result, "info");
                result = result.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "FindSRUUIDCmd result after format: *" + result + "*", "info");
                string UUID = ReturnCorrespondingParamvalue("uuid", result);
                UUID.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "UUID: *" + UUID + "*", "info");
                if (string.IsNullOrEmpty(UUID) || UUID.Length <= 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invalid SR UUID *" + UUID + "* obtained for SR " + SRNamelabel, "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }

                return UUID;
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at MoveVM " + Ex.ToString(), "warn");
                return null;
            }
        }

        public string FindVDIUUID(string ServerIp, string ServerUserName, string ServerPassword, string VMUUID)
        {
            //xe vbd-list vm-uuid=1f73dd61-64e7-8b60-4c11-a8626795c205
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
                string FindVDIUUIDCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vbd-list vm-uuid=" + VMUUID;
                NewLogObj.WriteLogFile(LogFilePath, "FindSRUUIDCmd " + FindVDIUUIDCmd, "info");
                string result = GenericObj.StartCmdExecutionAndWaitForCompletion(FindVDIUUIDCmd);
                System.Console.WriteLine(result);
                NewLogObj.WriteLogFile(LogFilePath, "FindVDIUUIDCmd result: " + result, "info");
                result = result.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "FindVDIUUIDCmd result after format: *" + result + "*", "info");
                string UUID = ReturnCorrespondingParamvalue("vdi-uuid", result);
                UUID.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "UUID: *" + UUID + "*", "info");
                if (string.IsNullOrEmpty(UUID)||UUID.Length <= 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invalid VDI UUID *" + UUID + "* obtained for VM " + VMUUID, "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                return UUID;
            }
            catch(Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at MoveVM "+Ex.ToString(), "warn");
                return null;
            }
        }

        public string FindVMUUID(string ServerIp, string ServerUserName, string ServerPassword, string VMNamelabel)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            try
            {
                string FindVMUUIDCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-list name-label=" + VMNamelabel;
                NewLogObj.WriteLogFile(LogFilePath, "FindVMUUIDCmd " + FindVMUUIDCmd, "info");
                string result = GenericObj.StartCmdExecutionAndWaitForCompletion(FindVMUUIDCmd);
                System.Console.WriteLine(result);
                NewLogObj.WriteLogFile(LogFilePath, "FindSRUUIDCmd result: " + result, "info");
                result = result.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "FindSRUUIDCmd result after format: *" + result + "*", "info");
                string UUID = ReturnCorrespondingParamvalue("uuid", result);
                UUID.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "UUID: *" + UUID + "*", "info");
                if (string.IsNullOrEmpty(UUID) || UUID.Length <= 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invalid VM UUID *" + UUID + "* obtained for VM " + VMNamelabel, "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                return UUID;
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at MoveVM " + Ex.ToString(), "warn");
                return null;
            }
        }

        public string MoveVDI(string ServerIp, string ServerUserName, string ServerPassword, string VMNameLabel, string SRNameLabel)
        {
            //xe vdi-copy uuid=957188cf-ea77-45e0-b8e0-7a9e80132aa6 sr-uuid=4f7a973a-c699-9937-f8bf-11a645738078
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
                //find UUID of sr
                string SRUUID = FindSRUUID(ServerIp, ServerUserName, ServerPassword, SRNameLabel);
                string VMUUID = FindVMUUID(ServerIp, ServerUserName, ServerPassword, VMNameLabel);
                string VDIUUID = FindVDIUUID(ServerIp, ServerUserName, ServerPassword, VMUUID);

                string FindVDIUUIDCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vdi-copy uuid=" + VDIUUID + " sr-uuid=" + SRUUID;
                NewLogObj.WriteLogFile(LogFilePath, "FindVDIUUIDCmd " + FindVDIUUIDCmd, "info");
                string result = GenericObj.StartCmdExecutionAndWaitForCompletion(FindVDIUUIDCmd);
                System.Console.WriteLine(result);
                NewLogObj.WriteLogFile(LogFilePath, "FindVDIUUIDCmd result: " + result, "info");
                result = result.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "FindVDIUUIDCmd result after format: *" + result + "*", "info");
                //string UUID = ReturnCorrespondingParamvalue("uuid", result);
                //NewLogObj.WriteLogFile(LogFilePath, "UUID: *" + UUID + "*", "info");
                if (string.IsNullOrEmpty(result) || result.Length <= 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invalid result *" + result + "* obtained for MoveVDI ", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                return result;
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at MoveVM " + Ex.ToString(), "warn");
                return null;
            }
        }

        public string MoveVM(string ServerIp, string ServerUserName, string ServerPassword, string VMNameLabel, string SRNameLabel,string NewVMName)
        {
            //xe vm-copy vm=GudVM sr-uuid=4f7a973a-c699-9937-f8bf-11a645738078 new-name-label=DisasterRecTest
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
                //find UUID of sr
                string SRUUID = FindSRUUID(ServerIp, ServerUserName, ServerPassword, SRNameLabel);
                //string VMUUID = FindVMUUID(ServerIp, ServerUserName, ServerPassword, VMNameLabel);
                //string VDIUUID = FindVDIUUID(ServerIp, ServerUserName, ServerPassword, VMUUID);

                string MoveVM = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-copy vm=" + VMNameLabel + " sr-uuid=" + SRUUID + " new-name-label=" + NewVMName;
                NewLogObj.WriteLogFile(LogFilePath, "MoveVM " + MoveVM, "info");
                string result = GenericObj.StartCmdExecutionAndWaitForCompletion(MoveVM);
                System.Console.WriteLine(result);
                NewLogObj.WriteLogFile(LogFilePath, "FindVDIUUIDCmd result: " + result, "info");
                result = result.Trim();
                NewLogObj.WriteLogFile(LogFilePath, "FindVDIUUIDCmd result after format: *" + result + "*", "info");
                //string UUID = ReturnCorrespondingParamvalue("uuid", result);
                //NewLogObj.WriteLogFile(LogFilePath, "UUID: *" + UUID + "*", "info");
                if (string.IsNullOrEmpty(result) || result.Length <= 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invalid result *" + result + "* obtained for MoveVM ", "fail");
                    FileOperations FileObj = new FileOperations();
                    FileObj.ExitTestEnvironment();
                }
                return result;
            }
            catch(Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at MoveVM "+Ex.ToString(), "warn");
                return null;
            }
        }

        public void DeleteVM(string ServerIp, string ServerUserName, string ServerPassword,string VMNameLabel)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string DeleteVMCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + "  xe vm-uninstall --multiple --force vm=\'" + VMNameLabel+"\'";
            NewLogObj.WriteLogFile(LogFilePath, "DeleteVMCmd " + DeleteVMCmd, "info");
            string result = GenericObj.StartCmdExecutionAndWaitForCompletion(DeleteVMCmd);
            System.Console.WriteLine(result);
            
        }

        public List<string> GetAllVMNames(string ServerIp, string ServerUserName, string ServerPassword)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string GetAllVMNamesCmd = "plink -l " + ServerUserName + " -pw " + ServerPassword + " " + ServerIp + " xe vm-list";
            NewLogObj.WriteLogFile(LogFilePath, "MoveVM " + GetAllVMNamesCmd, "info");
            string result = GenericObj.StartCmdExecutionAndWaitForCompletion(GetAllVMNamesCmd);
            System.Console.WriteLine(result);
            List<string> VMNameList = new List<string>();
            VMNameList=ReturnCorrespondingParamValueList("name-label",result);
            return VMNameList;
        }

        public void ShutdownAllVMs(string ServerIp, string ServerUserName, string ServerPassword)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            List<string> VMNameList = new List<string>();
            VMNameList = GetAllVMNames(ServerIp, ServerUserName, ServerPassword);
            foreach (string VMName in VMNameList)
            {
                if (!Regex.IsMatch(VMName, "Control domain"))
                {
                    //Check if VMname has unidoce chars
                    bool UnicodeStatus = GenericObj.IsUnicode(VMName);
                    if (UnicodeStatus == false)
                    {
                        ShutdownVM(ServerIp, ServerUserName, ServerPassword, "name-label", VMName,0);
                    }
                }
            } 
        }
        public void DeleteAllVMs(string ServerIp, string ServerUserName, string ServerPassword)
        {
            Generic GenericObj = new Generic();
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            List<string> VMNameList = new List<string>();
            ShutdownAllVMs(ServerIp, ServerUserName, ServerPassword);
            VMNameList = GetAllVMNames(ServerIp, ServerUserName, ServerPassword);
            foreach (string VMName in VMNameList)
            {
                if (!Regex.IsMatch(VMName, "Control domain"))
                {
                    bool UnicodeStatus = GenericObj.IsUnicode(VMName);
                    if (UnicodeStatus == false)
                    {
                        DeleteVM(ServerIp, ServerUserName, ServerPassword, VMName);
                    }
                }
            }
        }
       
        //Return a single value
        public string ReturnCorrespondingParamvalue(string Param,string Input)
        {
            string[] TempArrLines = Regex.Split(Input, "\n");
            string Value = null;
            foreach (string Line in TempArrLines)
            {
                if (Regex.IsMatch(Line, Param))
                {
                    string[] Paramvalmap = Regex.Split(Line, ": ");
                    Value = Paramvalmap[1];
                    return Value;
                }
            }
            return Value;
        }

        //Return a list of values
        public List<string> ReturnCorrespondingParamValueList(string Param, string Input)
        {
            string[] TempArrLines = Regex.Split(Input, "\n");
            List<string> ValueList = new List <string>();
            foreach (string Line in TempArrLines)
            {
                if (Regex.IsMatch(Line, Param))
                {
                    string[] Paramvalmap = Regex.Split(Line, ": ");
                    Paramvalmap[1].Trim();
                    ValueList.Add(Paramvalmap[1]);
                }
            }
            return ValueList;
        }

       
    }
}

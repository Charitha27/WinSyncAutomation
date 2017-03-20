start /WAIT InitiateSetup.exe
timeout 5
start /WAIT RunTest.exe Installer.txt
timeout 5
start /WAIT RunTest.exe AddServer.txt
timeout 5
start /WAIT RunTest.exe AssignLicense.txt
timeout 5
start /WAIT RunTest.exe XenServerProperties.txt
timeout 5
start /WAIT RunTest.exe NewPool.txt
timeout 5
start /WAIT RunTest.exe AddCIFS.txt
timeout 5
start /WAIT RunTest.exe AddNFSVHD.txt
timeout 5
start /WAIT RunTest.exe IScsi.txt
timeout 5
start /WAIT RunTest.exe RenameIScsi.txt
timeout 5
start RunTest.exe ImportTemplateAndStartVMUsingCLI.txt
timeout 5
start /WAIT RunTest.exe AddNFSISO.txt
timeout 5
start /WAIT RunTest.exe NewVM.txt
timeout 5
start /WAIT RunTest.exe XenServerTabs.txt
timeout 5
start /WAIT RunTest.exe VmProperties.txt
timeout 5
start /WAIT RunTest.exe ChangeServerPassword.txt
timeout 5
start /WAIT RunTest.exe ConfigureGraph.txt
timeout 5
start /WAIT RunTest.exe MainUI.txt
timeout 5
start /WAIT RunTest.exe ImportVMXVAFormat.txt
timeout 5
start /WAIT RunTest.exe ImportVMVmdkFormat.txt
timeout 5
start /WAIT RunTest.exe ImportVMOVFFormat.txt
timeout 5
start /WAIT RunTest.exe About.txt
timeout 5
start /WAIT RunTest.exe PoolProperties.txt
timeout 5
start /WAIT RunTest.exe Snapshot.txt
timeout 5
start /WAIT RunTest.exe ADIntegration.txt
timeout 5
start /WAIT RunTest.exe HA.txt
timeout 5
start /WAIT RunTest.exe CopyVM.txt
timeout 5
start /WAIT RunTest.exe MoveVM.txt
timeout 5
start /WAIT RunTest.exe DeleteVM.txt
timeout 5
start /WAIT RunTest.exe ManageVApps.txt
timeout 5
start /WAIT RunTest.exe ShutDownVM.txt
timeout 5
start /WAIT RunTest.exe ShutDownAllVMs.txt
timeout 5
start /WAIT RunTest.exe ShutDownServer.txt
timeout 5
start /WAIT RunTest.exe DisableHA.txt
timeout 5
start /WAIT RunTest.exe RemoveAD.txt
timeout 5
start /WAIT RunTest.exe RebootServer.txt
timeout 5
start /WAIT RunTest.exe RemoveServerFromPool.txt
timeout 5
start /WAIT RunTest.exe MaintanenceMode.txt
timeout 5
start /WAIT RunTest.exe DynamicMemoryController.txt
timeout 5
start /WAIT RunTest.exe Disconnect.txt
timeout 5
start /WAIT RunTest.exe AddToPool.txt
timeout 5
start /WAIT RunTest.exe DisasterRecovery.txt
timeout 5
start /WAIT RunTest.exe ManagementInterfaces.txt
timeout 5
start /WAIT RunTest.exe NIC.txt
timeout 5
start /WAIT RunTest.exe Networking.txt
timeout 5
start /WAIT RunTest.exe Alerts.txt
timeout 5
start /WAIT RunTest.exe Storage.txt
timeout 5
start /WAIT RunTest.exe Template.txt
timeout 5
start /WAIT RunTest.exe ShutDownAllVMs.txt
timeout 5
start /WAIT RunTest.exe DeleteLocalizedVMs.txt
timeout 5
start /WAIT RunTest.exe DeleteAllVMs.txt
timeout 5
start /WAIT RunTest.exe DeleteVApps.txt
timeout 5
start /WAIT RunTest.exe DeleteTemplates.txt
timeout 5
start /WAIT RunTest.exe RemoveCIFS.txt
timeout 5
start /WAIT RunTest.exe RemoveNFSISO.txt
timeout 5
start /WAIT RunTest.exe RemoveISCSI.txt
timeout 5
start /WAIT RunTest.exe RemoveNFSVHD.txt
timeout 5
start /WAIT RunTest.exe ShutDownAllVMsSecondPool.txt
timeout 5
start /WAIT RunTest.exe DeleteAllVMsSecondPool.txt
timeout 5
start /WAIT RunTest.exe RemovePool.txt
timeout 5
start /WAIT RunTest.exe Repair.txt
timeout 5
start /WAIT RunTest.exe Uninstallation.txt
timeout 5
start /WAIT EndTest.exe
start /WAIT InitiateSetup.exe
timeout 5
start /WAIT RunTest.exe Installer_SF.txt
timeout 5
start /WAIT RunTest.exe InitialSetup.txt
timeout 5
start /WAIT RunTest.exe CreateStore.txt
timeout 5
start /WAIT RunTest.exe  IntegrateWithCitrixOnline.txt
timeout 5
start /WAIT RunTest.exe  ManageReceiverUpdates.txt
timeout 5
start /WAIT RunTest.exe  ExportMultiStoreProvisioningFile.txt
timeout 5
start /WAIT RunTest.exe  HideAndAdvertiseStore.txt
timeout 5
start /WAIT RunTest.exe ManageDeliveryControllers.txt
timeout 5
start /WAIT RunTest.exe EnableRemoteAccess.txt
timeout 5
start /WAIT RunTest.exe ExportProvisioningFile.txt
timeout 5
start /WAIT RunTest.exe ConfigureLegacySupport.txt
timeout 5
start /WAIT RunTest.exe GenerateSecurityKeys.txt
timeout 5
start /WAIT RunTest.exe AddServer.txt
timeout 5
start /WAIT RunTest.exe ChangeBaseURL.txt
timeout 5
start /WAIT RunTest.exe AddRemoveAuthenticationMethods.txt
timeout 5
start /WAIT RunTest.exe ConfigureTrustedDomains.txt
timeout 5
start /WAIT RunTest.exe ManagePasswordOptions.txt
timeout 5
start /WAIT RunTest.exe DisableAuthenticationMethods.txt
timeout 5
start /WAIT RunTest.exe ConfigureDelegatedAuthentication.txt
timeout 5
start /WAIT RunTest.exe CreateWebSite.txt
timeout 5
start /WAIT RunTest.exe AddShortCutsToWebSite.txt
timeout 5
start /WAIT RunTest.exe ChangeStoreURL.txt
timeout 5
start /WAIT RunTest.exe DeployCitrixReceiver.txt
timeout 5
start /WAIT RunTest.exe RemoveWebSite.txt
timeout 5
start /WAIT RunTest.exe RemoveNetScalarGatewayAppliance.txt
timeout 5
start /WAIT RunTest.exe AddNetScalarGatewayAppliance.txt
timeout 5
start /WAIT RunTest.exe ChangeGeneralSettings.txt
timeout 5
start /WAIT RunTest.exe SecureTicketAuthority.txt
timeout 5
start /WAIT RunTest.exe ManageBeacons.txt
timeout 5
start /WAIT RunTest.exe RemoveStore.txt
timeout 5
start /WAIT RunTest.exe AboutMMC.txt
timeout 5
start /WAIT RunTest.exe AboutSF.txt
timeout 5
start /WAIT RunTest.exe TechCenterWebSite.txt
timeout 5
start /WAIT RunTest.exe HelpTopics.txt
timeout 5
start /WAIT RunTest.exe DiskCleanup.txt
timeout 5
start /WAIT RunTest.exe ShowHideActionPane.txt
timeout 5
start /WAIT RunTest.exe ShowHideConsoleTree.txt
timeout 5
start /WAIT RunTest.exe Help.txt
timeout 5
start /WAIT RunTest.exe  CustomizeView.txt
timeout 5
start /WAIT EndTest.exe

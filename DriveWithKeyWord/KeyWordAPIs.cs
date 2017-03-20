using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Automation;
using System.Text.RegularExpressions;
using System.Threading;

using LoggerCollection;
using TestInputFileParsingCollection;
using GUIControlCollection;
using FileOperationsCollection;
using MapperCollection;
using GuizardCollection;
using GUICollection;
using GenericCollection;
using XenServerKeyWordsCollection;
using WindowsOperationsKeywordsCollection;


namespace KeyWordAPIsCollection
{
    class KeyWordAPIs
    {
        public AutomationElement SuperParentObj = AutomationElement.RootElement;
        public AutomationElement ParentObj;
        public AutomationElement CurrentActiveWindowObj;
        public Dictionary<string, string> ParamValueMapDict = new Dictionary<string, string>();
        public Dictionary<string, string>  RequiredValueMapDict = new Dictionary<string, string>();
        
        //For handling partiially localized products
        public static Dictionary<String, String> LocEnStringMappingForPartialLocalization = new Dictionary<string, string>();
        public static int PartiallyLocalized = 0;
        public static int ReRunInitiated = 0;

        public string GuizardLocation = null;
        public int FindParentFirstCall = 1;
        int GuizardStarted = 0;

        //To handle same string having multipke mappings
        public int SelectIndexForMultipleMappingString = 0;

        public void InterpretInputLine(string InputLine)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            //NewLogObj.WriteLogFile(LogFilePath, "InterpretInputLine", "info");
           // NewLogObj.WriteLogFile(LogFilePath, "================================", "info");
            //Extract the keyword
            int InputLineIndex = InputLine.IndexOf("(");
            string InputLineCommandName = InputLine.Substring(0, InputLineIndex);
            InputLineCommandName=InputLineCommandName.ToLower();
           NewLogObj.WriteLogFile(LogFilePath, "InputLineCommandName " + InputLineCommandName + " found in InputLine " + InputLine, "info");
            Console.WriteLine("\n" + InputLine + "\n\n");
            TestInputFileParsing TestInputParseObj = new TestInputFileParsing();
            GUIControl GuiControlObj=new GUIControl();
            FileOperations FileObj = new FileOperations();
            

            //Get the list of params
           // List<string> ParamList = TestInputParseObj.GetParamValueList(InputLine);
            string ParamType = TestInputParseObj.GetFirstParameterName(InputLine);
            ParamType=ParamType.ToLower();
            //Replace the read and mapped keywors if any with none
            ParamType = Regex.Replace(ParamType, "mappednamenotinttk", "");
            ParamType = Regex.Replace(ParamType, "readrufield", "");
            ParamType = Regex.Replace(ParamType, "read", "");
            ParamType = Regex.Replace(ParamType, "Read", "");
            ParamType = Regex.Replace(ParamType, "mapped", "");
            ParamType = Regex.Replace(ParamType, "Mapped", "");
            
            string ParamType1=null;
            if(Regex.IsMatch(ParamType,"automationid"))
            {
                ParamType1="automationid";
            }
            else if (Regex.IsMatch(ParamType, "class"))
            {
                ParamType1 = "class";
            }
            else if (Regex.IsMatch(ParamType, "name"))
            {
                ParamType1 = "name";
            }
            else if (Regex.IsMatch(ParamType, "controltype"))
            {
                ParamType1 = "controltype";
            }

            try
            {

                ParamValueMapDict = TestInputParseObj.GetLocalizedMappedPatternList(InputLine);

            }
            catch (Exception ex)
            {
            }
            //To handle partially localized product
            Mapper MapObj=new Mapper();
            LocEnStringMappingForPartialLocalization = MapObj.GetLocEnStringMappingForPartialLocalization();
            PartiallyLocalized = MapObj.GetPartialLocalizationStatus();

            int Timeout = -1; int TerminateStatus = -1; int Index = 0; int ChildSelectIndex = 0; int ParentSelectIndex = 0; int SkipGuizard = 0;

            
            //if (ParamValueMapDict["timeout"]!=null)
            if(ParamValueMapDict.ContainsKey("timeoutinsec"))
            {
                Int32.TryParse(ParamValueMapDict["timeoutinsec"], out Timeout);
                Timeout = Timeout * 1000;
            }
           // if (ParamValueMapDict.ContainsKey("terminatestatus"))
            if (ParamValueMapDict.ContainsKey("terminateonfailure"))
            {
                ParamValueMapDict["terminateonfailure"] = ParamValueMapDict["terminateonfailure"].ToLower();
                if(string.Compare(ParamValueMapDict["terminateonfailure"],"yes")==0)
                {
                    TerminateStatus = 1;
                }
                else if (string.Compare(ParamValueMapDict["terminateonfailure"], "no") == 0)
                {
                    TerminateStatus = 0;
                }
            }
            if (ParamValueMapDict.ContainsKey("skipguizard"))
            {
                ParamValueMapDict["skipguizard"] = ParamValueMapDict["skipguizard"].ToLower();
                if (string.Compare(ParamValueMapDict["skipguizard"], "yes") == 0)
                {
                    SkipGuizard = 1;
                }
                else if (string.Compare(ParamValueMapDict["skipguizard"], "no") == 0)
                {
                    SkipGuizard = 0;
                }
            }
            if (ParamValueMapDict.ContainsKey("selectitemindexifduplicatesexist"))
            {
                Int32.TryParse(ParamValueMapDict["selectitemindexifduplicatesexist"], out Index);
            }
            if (ParamValueMapDict.ContainsKey("containerselectitemindexifduplicatesexist"))
            {
                Int32.TryParse(ParamValueMapDict["containerselectitemindexifduplicatesexist"], out ParentSelectIndex);
            }
            if (ParamValueMapDict.ContainsKey("childselectitemindexifduplicatesexist"))
            {
                Int32.TryParse(ParamValueMapDict["childselectitemindexifduplicatesexist"], out ChildSelectIndex);
            }
           
            
            Dictionary<string, string>.KeyCollection InputLineParams = ParamValueMapDict.Keys;

            //Specifying the input file
            Generic NewGenericObj = new Generic();
            string SystemLocale = NewGenericObj.GetSystemLocale();
            //NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
            string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
            string GuizardLocation = FileObj.GetInputPattern(InputFileName, "GuiZardLocation");

            string ProductSpecifcKeywordPrefix = FileObj.GetInputPattern(InputFileName, "ProductSpecificKeywordPrefix");

            //Regex ProductSpecificReg=new Regex("^xs:",RegexOptions.IgnoreCase);
            Regex ProductSpecificReg = new Regex("^" + ProductSpecifcKeywordPrefix, RegexOptions.IgnoreCase);
            //See if there are any product specific call
            if (ProductSpecificReg.IsMatch(InputLineCommandName))
            {
                if (string.Compare(ProductSpecifcKeywordPrefix.ToLower(), "xs") == 0)
                {
                    XenServerKeyWords XenServerKeywordsObj = new XenServerKeyWords();
                    XenServerKeywordsObj.ProcessXenserverKeywords(InputLineCommandName, ParamValueMapDict);
                }
            }

            if(GuizardLocation.ToLower().Equals("no"))
            {
                SkipGuizard = 1;
            }

            //Check if any windows operations
            Regex WindowsSpecificReg = new Regex("^ws:", RegexOptions.IgnoreCase);
            if (WindowsSpecificReg.IsMatch(InputLineCommandName))
            {
                WindowsOperationsKeywords WindowsOpsKeywordObj = new WindowsOperationsKeywords();
                WindowsOpsKeywordObj.ProcessWindowsOperationsKeywords(InputLineCommandName, ParamValueMapDict);
            }

            switch (InputLineCommandName)
            {
                case "capturescreenshot":
                    NewLogObj.CaptureScreenShotsAtEveryStage();
                    break;
                case "starttest":
                    GuiControlObj.StartTest(ParamValueMapDict["testname"]);
                    break;
                case "startprocess":
                    string KillExistingRunningProcess = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "killifprocessrunning");

                    if (KillExistingRunningProcess != null)
                    {
                        GuiControlObj.StartProcess(ParamValueMapDict["exelocation"], ParamValueMapDict["processname"], ParamValueMapDict["killifprocessrunning"]);
                    }
                    else
                    {
                        GuiControlObj.StartProcess(ParamValueMapDict["exelocation"], ParamValueMapDict["processname"], null);
                    }
                    
                    break;

                case "sleep":
                    int SleepTimeInSecs=1;
                    if (ParamValueMapDict.ContainsKey("sleeptimeinsecs"))
                    {
                        Int32.TryParse(ParamValueMapDict["sleeptimeinsecs"], out SleepTimeInSecs);
                        SleepTimeInSecs = SleepTimeInSecs * 1000;
                    }
                    Thread.Sleep(SleepTimeInSecs);
                    break;

                case "sendkeystotype":
                    GuiControlObj.SendKeysToType(ParamValueMapDict["keystosend"]);
                    break;
                case "sendkeys":
                    GuiControlObj.SendKeys(ParamValueMapDict["keystosend"]);
                    break;

                case "findparentwindow":

                    if (!GuizardLocation.ToLower().Equals("no"))
                    {
                        string GuiZardExeName = FileObj.GetInputPattern(InputFileName, "GuiZardExeName");
                        string GuiZardFullPath = GuizardLocation + "\\" + GuiZardExeName;
                        if (!File.Exists(GuiZardFullPath))
                        {
                            Console.WriteLine("GuiZardFullPath " + GuiZardFullPath + " Does not exist");
                            NewLogObj.WriteLogFile(LogFilePath, "GuiZardFullPath " + GuiZardFullPath + " Does not exist", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                    }


                    //Storing the credentials of current window
                    RequiredValueMapDict["CurrWindowIdentifier"] = ParamValueMapDict[ParamType];
                    if (TerminateStatus == 0 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        break;
                    }
                    if (TerminateStatus == 1 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                   // if (string.Equals(ParamType, "automationid", StringComparison.OrdinalIgnoreCase))
                    if(Regex.IsMatch(ParamType,"automationid",RegexOptions.CultureInvariant))
                    {
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "automationid";
                        //Assuming that the 1st call to findparentwindow will be the "Real Parent Window of the product" on which the further operations will have to be carried out
                        //SuperParentObj will be initialized during the 1st call, with base of search as "root"
                        //For subsequent calls to findparent, base of serach will be "SuperParent"
                         ParentObj = GuiControlObj.WaitWindow(AutomationElement.RootElement, "Root", "automationid", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                         RequiredValueMapDict["CurrWindowIdentifierType"] = "automationid";
                    }
                    else if (Regex.IsMatch(ParamType, "mappednamenotinttk", RegexOptions.CultureInvariant))
                    {
                        ParentObj = GuiControlObj.WaitWindow(AutomationElement.RootElement, "Root", "name", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation, SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    //else if (string.Equals(ParamType, "name", StringComparison.OrdinalIgnoreCase))
                    else if (Regex.IsMatch(ParamType, "mappedname", RegexOptions.CultureInvariant))
                    {
                         ParentObj = GuiControlObj.WaitWindow(AutomationElement.RootElement, "Root", "name", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                         RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    else if (Regex.IsMatch(ParamType, "classname", RegexOptions.CultureInvariant))
                    {
                        ParentObj = GuiControlObj.WaitWindow(AutomationElement.RootElement, "Root", "class", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "class";
                    }
                    //else if (string.Equals(ParamType, "mappedname", StringComparison.OrdinalIgnoreCase))
                    else if (Regex.IsMatch(ParamType, "name", RegexOptions.CultureInvariant))
                    {
                        ParentObj = GuiControlObj.WaitWindow(AutomationElement.RootElement, "Root", "name", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine " + InputLine, "fail");
                        FileObj.FinishCurrentTest(ParentObj);
                    }
                    if (ParentObj == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parent obj is null " + InputLine + "Exiting ..", "fail");
                        FileObj.FinishCurrentTest(ParentObj);
                    }
                    else
                    {
                        CurrentActiveWindowObj = ParentObj;
                        AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                        PropertyCondition MaximizedBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ParamValueMapDict["maximizebtnautomationid"], 1, LogFilePath);
                        
                        //MaximizedBtn is optional cntrl
                        AutomationElement MaximizedBtn = GuiObj.FindAutomationElement(ParentObj, MaximizedBtnCondition, TreeScope.Descendants, "Maximize Btn", LogFilePath);
                        if (MaximizedBtn != null)
                        {
                            GuiObj.ClickButton(MaximizedBtn, 0, "MaximizedBtn", 1, LogFilePath);
                        }
                        if (GuizardStarted == 0)
                        {
                            Guizard NewGuiZardObj = new Guizard();

                            NewGuiZardObj.StartNewTest(GuizardLocation);
                            GuizardStarted = 1;
                           
                        }
                    }

                    FindParentFirstCall = 0;
                    break;

                case "findparentwindowwithpatterninname":

                //Storing the credentials of current window
                    RequiredValueMapDict["CurrWindowIdentifier"] = ParamValueMapDict[ParamType];
                    if (TerminateStatus == 0 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        break;
                    }
                    if (TerminateStatus == 1 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (Regex.IsMatch(ParamType, "mappednamenotinttk", RegexOptions.CultureInvariant))
                    {
                        ParentObj = GuiControlObj.WaitWindowWithPattern(AutomationElement.RootElement, "Root", ParamValueMapDict["searchelementcontroltype"], ParamValueMapDict["patternname"], Timeout, TerminateStatus, GuizardLocation, SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    //else if (string.Equals(ParamType, "name", StringComparison.OrdinalIgnoreCase))
                    else if (Regex.IsMatch(ParamType, "mappedname", RegexOptions.CultureInvariant))
                    {
                        ParentObj = GuiControlObj.WaitWindowWithPattern(AutomationElement.RootElement, "Root", ParamValueMapDict["searchelementcontroltype"], ParamValueMapDict["patternname"], Timeout, TerminateStatus, GuizardLocation, SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                   
                    //else if (string.Equals(ParamType, "mappedname", StringComparison.OrdinalIgnoreCase))
                    else if (Regex.IsMatch(ParamType, "name", RegexOptions.CultureInvariant))
                    {
                        ParentObj = GuiControlObj.WaitWindowWithPattern(AutomationElement.RootElement, "Root", ParamValueMapDict["searchelementcontroltype"], ParamValueMapDict["patternname"], Timeout, TerminateStatus, GuizardLocation, SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine " + InputLine, "fail");
                        FileObj.FinishCurrentTest(ParentObj);
                    }
                    if (ParentObj == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parent obj is null " + InputLine + "Exiting ..", "fail");
                        FileObj.FinishCurrentTest(ParentObj);
                    }
                    else
                    {
                        CurrentActiveWindowObj = ParentObj;
                        AutomationElementIdentity GuiObj = new AutomationElementIdentity();
                        PropertyCondition MaximizedBtnCondition = GuiObj.SetPropertyCondition("AutomationIdProperty", ParamValueMapDict["maximizebtnautomationid"], 1, LogFilePath);

                        //MaximizedBtn is optional cntrl
                        AutomationElement MaximizedBtn = GuiObj.FindAutomationElement(ParentObj, MaximizedBtnCondition, TreeScope.Descendants, "Maximize Btn", LogFilePath);
                        if (MaximizedBtn != null)
                        {
                            GuiObj.ClickButton(MaximizedBtn, 0, "MaximizedBtn", 1, LogFilePath);
                        }
                        if (GuizardStarted == 0)
                        {
                            Guizard NewGuiZardObj = new Guizard();

                            NewGuiZardObj.StartNewTest(GuizardLocation);
                            GuizardStarted = 1;

                        }
                    }

                    FindParentFirstCall = 0;
                    break;

                case "waitwindow":

                    //Storing the credentials of current window
                    if (TerminateStatus == 0 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        break;
                    }
                    if (TerminateStatus == 1 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    RequiredValueMapDict["CurrWindowIdentifier"] = ParamValueMapDict[ParamType];
                    
                    //if (string.Equals(ParamType, "automationid", StringComparison.OrdinalIgnoreCase))
                    if (Regex.IsMatch(ParamType, "automationid", RegexOptions.CultureInvariant))
                    {
                        CurrentActiveWindowObj = GuiControlObj.WaitWindow(ParentObj, "Child", "automationid", ParamValueMapDict[ParamType],Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "automationid";
                    }
                    //else if (string.Equals(ParamType, "name", StringComparison.OrdinalIgnoreCase))
                    else if (Regex.IsMatch(ParamType, "mappedname", RegexOptions.CultureInvariant))
                    {
                        CurrentActiveWindowObj = GuiControlObj.WaitWindow(ParentObj, "Child", "name", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    else if (Regex.IsMatch(ParamType, "classname", RegexOptions.CultureInvariant))
                    {
                        CurrentActiveWindowObj = GuiControlObj.WaitWindow(ParentObj, "Child", "class", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "classname";
                    }
                    //else if (string.Equals(ParamType, "mappedname", StringComparison.OrdinalIgnoreCase))
                    else if (Regex.IsMatch(ParamType, "name", RegexOptions.CultureInvariant))
                    {
                        CurrentActiveWindowObj = GuiControlObj.WaitWindow(ParentObj, "Child", "name", ParamValueMapDict[ParamType], Timeout, TerminateStatus, GuizardLocation,SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();                    
                    }
                    break;

                case "waitwindowwithpatterninname":

                    //Storing the credentials of current window
                    if (TerminateStatus == 0 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        break;
                    }
                    if (TerminateStatus == 1 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType])))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    RequiredValueMapDict["CurrWindowIdentifier"] = ParamValueMapDict[ParamType];

                    if (Regex.IsMatch(ParamType, "mappedname", RegexOptions.CultureInvariant))
                    {   
                        CurrentActiveWindowObj=GuiControlObj.WaitWindowWithPattern(ParentObj, "Child", ParamValueMapDict["searchelementcontroltype"], ParamValueMapDict["patternname"], Timeout, TerminateStatus, GuizardLocation, SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    else if (Regex.IsMatch(ParamType, "name", RegexOptions.CultureInvariant))
                    {
                        CurrentActiveWindowObj = GuiControlObj.WaitWindowWithPattern(ParentObj, "Child", ParamValueMapDict["searchelementcontroltype"], ParamValueMapDict["patternname"], Timeout, TerminateStatus, GuizardLocation, SkipGuizard);
                        RequiredValueMapDict["CurrWindowIdentifierType"] = "name";
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "invokemenutree":
                    string SubMenuParamName1 = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "SubMenuName1");
                    string SubMenuParamName2 = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "SubMenuName2");
                    if (((TerminateStatus == 0) && ( string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[SubMenuParamName1]) || string.IsNullOrEmpty(ParamValueMapDict[SubMenuParamName2]) )))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[SubMenuParamName1]) || string.IsNullOrEmpty(ParamValueMapDict[SubMenuParamName2]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    GuiControlObj.InvokeMenuTree(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParamValueMapDict[SubMenuParamName1], ParamValueMapDict[SubMenuParamName2], Index,Timeout, TerminateStatus);
                 
                    break;
                case "invokemenu":

                    string SubMenuParamName = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "submenu");
                    if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[SubMenuParamName]) )))
                    {
                        break;
                    }
                    if (SubMenuParamName != null)
                    {
                        if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || (ParamValueMapDict.Keys.Contains(SubMenuParamName) && string.IsNullOrEmpty(ParamValueMapDict[SubMenuParamName])))))
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                            FileObj.ExitTestEnvironment();
                        }
                    }
                    if (SubMenuParamName != null)
                    {
                        GuiControlObj.InvokeMenu(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParamValueMapDict[SubMenuParamName], Index, Timeout, TerminateStatus);
                    }
                    else
                    {
                        GuiControlObj.InvokeMenu(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, Timeout, TerminateStatus);
                    }
                    break;

                case "capturepopupunderdesktop":
                    string PopUpMenuParamName = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "PopupMenu");
                    if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName]))))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (Regex.IsMatch(PopUpMenuParamName, "automationid", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null,ParamType1, ParamValueMapDict[ParamType], "automationid", ParamValueMapDict[PopUpMenuParamName], Index, 1,Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "mappedname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null,ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index, Timeout,1, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "classname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null,ParamType1, ParamValueMapDict[ParamType], "classname", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 1,TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "name", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 1,TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "controltype", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null, ParamType1, ParamValueMapDict[ParamType], "controltype", ParamValueMapDict[PopUpMenuParamName], Index,1, Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine to capturepopup " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();                    
                    }
                    break;
                case "capturepopupundercurrentactivewindow":
                    PopUpMenuParamName = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "PopupMenu");
                    if (TerminateStatus == 0 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName])))
                    {
                        break;
                    }
                    if (TerminateStatus == 1 && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName])))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (Regex.IsMatch(PopUpMenuParamName, "automationid", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj,ParamType1, ParamValueMapDict[ParamType], "automationid", ParamValueMapDict[PopUpMenuParamName], Index,1, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "mappedname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index,1, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "classname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "classname", ParamValueMapDict[PopUpMenuParamName], Index,1, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "name", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index,1, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "controltype", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "controltype", ParamValueMapDict[PopUpMenuParamName], Index, 1,Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine to capturepopup " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;

                case "checkifpopupmenuitemunderactivewindowexists":
                    PopUpMenuParamName = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "PopupMenu");
                    if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName]))))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (Regex.IsMatch(PopUpMenuParamName, "automationid", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "automationid", ParamValueMapDict[PopUpMenuParamName], Index, 0, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "mappedname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 0, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "classname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "classname", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 0, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "name", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 0, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "controltype", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "controltype", ParamValueMapDict[PopUpMenuParamName], Index, 0, Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine to capturepopup " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "checkifpopupmenuitemunderdesktopwindowexists":
                    PopUpMenuParamName = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "PopupMenu");
                    if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName]))))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict[PopUpMenuParamName]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (Regex.IsMatch(PopUpMenuParamName, "automationid", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null, ParamType1, ParamValueMapDict[ParamType], "automationid", ParamValueMapDict[PopUpMenuParamName], Index, 0, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "mappedname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 0, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "classname", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null, ParamType1, ParamValueMapDict[ParamType], "classname", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 0, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "name", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[PopUpMenuParamName], Index, Timeout, 0, TerminateStatus);
                    }
                    else if (Regex.IsMatch(PopUpMenuParamName, "controltype", RegexOptions.CultureInvariant))
                    {
                        GuiControlObj.CapturePopUp(null, ParamType1, ParamValueMapDict[ParamType], "controltype", ParamValueMapDict[PopUpMenuParamName], Index, 0, Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid parameter " + ParamType + " found in InputLine to capturepopup " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "clickoniconfromsystemtraynotificationarea":
                    if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict["clickmousebtn"]))))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]) || string.IsNullOrEmpty(ParamValueMapDict["clickmousebtn"]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    GuiControlObj.ClickOnIconFromSystemTrayNotificationArea(ParamType1, ParamValueMapDict[ParamType],ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    break;

                case "expandcombobox":
                     if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    GuiControlObj.ExpandCmbBox(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType],Index, Timeout, TerminateStatus);
                    break;

              
                case "typeintextbox":
                    if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    GuiControlObj.TypeInTextBox(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParamValueMapDict["texttotype"],Index, Timeout, TerminateStatus);
                   break;

                case "checkiftextboxhasexpectedvalue":
                   if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                   {
                       break;
                   }
                   if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                   {
                       NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                       FileObj.ExitTestEnvironment();
                   }
                   GuiControlObj.CheckIfTextBoxHasExpectedValue(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParamValueMapDict["expectedvalue"], Index, Timeout, TerminateStatus);
                   break;
                case "typeindocumentobject":

                     if (((TerminateStatus == 0) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                    {
                        break;
                    }
                    if (((TerminateStatus == 1) && (string.IsNullOrEmpty(ParamValueMapDict[ParamType]))))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Parameter values are found to be null in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                   GuiControlObj.SetDocObjText(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParamValueMapDict["texttotype"], Timeout, TerminateStatus);
                   break;

                case "clickbtn":
                    GuiControlObj.ClickBtn(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, Timeout, TerminateStatus);
                    break;
                case "setradiobutton":
                    
                    GuiControlObj.SetRdBtn(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index,Timeout, TerminateStatus);    
                    break;
                 case "uncheckcheckbox":
                     
                    GuiControlObj.UnCheckCheckBox(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType],Index, Timeout, TerminateStatus);
                    break;
                case "checkcheckbox":

                    GuiControlObj.CheckCheckBox(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType],Index, Timeout, TerminateStatus);
                    break;

                case "clickhyperlink":
                    GuiControlObj.ClickHyperLinkObj(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, Timeout, TerminateStatus);
                    break;
                case "selectitemfromlist":
                    
                    if (InputLineParams.Contains("itemindextoselect"))
                    {
                        int IndexToSelect;
                        Int32.TryParse(ParamValueMapDict["itemindextoselect"], out IndexToSelect);
                        //List has to be selected based on index
                        //GuiControlObj.GtListElementAndSelect(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], null, 1, IndexToSelect, Index,Timeout, TerminateStatus);
                        GuiControlObj.GtListElementAndSelect(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], null, 1, Index, IndexToSelect, Timeout, TerminateStatus);
                    }
                        
                    else if (InputLineParams.Contains("nameitemtoselect"))
                    {
                        //List has to be selected based on element name
                        GuiControlObj.GtListElementAndSelect(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParamValueMapDict["nameitemtoselect"], 0, 0, Index, Timeout, TerminateStatus);
                    }
                    break;
                case "selectitemfromgridwithrowindex":
                    if (InputLineParams.Contains("itemrowindextoselect"))
                    {
                        //List has to be selected based on index
                        GuiControlObj.SelectItemFromDataGridView(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], 1, ParamValueMapDict["itemrowindextoselect"], 0,0,0 ,Index,null,0,Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "selectitemfromdatagridwithrowindexandcheckcheckbox":
                    if (InputLineParams.Contains("itemrowindextoselect"))
                    {
                        //List has to be selected based on index
                        GuiControlObj.SelectItemFromDataGridView(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], 1, ParamValueMapDict["itemrowindextoselect"], 1, Index, 0,0,null,0,Timeout, TerminateStatus);                                     
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "selectitemfromdatagridwithrowindexandclickonfirstcolumn":
                    if (InputLineParams.Contains("itemrowindextoselect"))
                    {
                        //List has to be selected based on index
                        GuiControlObj.SelectItemFromDataGridView(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], 1, ParamValueMapDict["itemrowindextoselect"], 1, Index, 0, 0, null,1, Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "clickongridwithrowindex":
                    if (InputLineParams.Contains("itemrowindextoselect"))
                    {
                        //List has to be selected based on index
                        GuiControlObj.SelectItemFromDataGridView(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], 1, ParamValueMapDict["itemrowindextoselect"], Index, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);                       
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "selectitemfromgridwithitemname":
                    GuiControlObj.SelectItemFromDataGridView(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], 0, ParamValueMapDict["itemnametoselect"], 0, Index,0,0,null,0,Timeout, TerminateStatus);
                    break;
                case "selectitemfromgridwithitemnameandcheckcheckbox":
                    GuiControlObj.SelectItemFromDataGridView(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], 0, ParamValueMapDict["itemnametoselect"], 1, Index,0,0,null,0,Timeout, TerminateStatus);
                    break;
                case "selectitemfromdatagridwithrowindexandverifycolumnvalue":
                    string ExpectedColumnValue = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ExpectedColumnValue");
                    if (!InputLineParams.Contains(ExpectedColumnValue))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "ExpectedColumnValue param not found in selectitemfromdatagridwithrowindexandverifycolumnvalue", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (InputLineParams.Contains("itemrowindextoselect") && InputLineParams.Contains("columnnumtoretrievevalue"))
                    {
                        int ColumnNumToExtractValue = 0;
                        Int32.TryParse(ParamValueMapDict["columnnumtoretrievevalue"], out ColumnNumToExtractValue);
                        GuiControlObj.SelectItemFromDataGridView(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], 1, ParamValueMapDict["itemrowindextoselect"], 1, Index, 1, ColumnNumToExtractValue, ParamValueMapDict[ExpectedColumnValue], 0,Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect or columnnumtoretrievevalue param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "verifycolumnvalueindatagridandclickonit":
                    ExpectedColumnValue = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ExpectedColumnValue");
                    if (!InputLineParams.Contains(ExpectedColumnValue))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "ExpectedColumnValue param not found in selectitemfromdatagridwithrowindexandverifycolumnvalue", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (InputLineParams.Contains("columnnumtoretrievevalue"))
                    {
                        int ColumnNumToExtractValue = 0;
                        Int32.TryParse(ParamValueMapDict["columnnumtoretrievevalue"], out ColumnNumToExtractValue);
                        GuiControlObj.CheckItemPresentInDataGridview(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, ColumnNumToExtractValue, ParamValueMapDict[ExpectedColumnValue],0, 1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect or columnnumtoretrievevalue param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "verifycolumnvalueindatagridandcheckcorrespondingcheckbox":
                    ExpectedColumnValue = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ExpectedColumnValue");
                    
                    if (!InputLineParams.Contains(ExpectedColumnValue))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "ExpectedColumnValue param not found in selectitemfromdatagridwithrowindexandverifycolumnvalue", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (InputLineParams.Contains("columnnumtoretrievevalue"))
                    {
                        int ColumnNumToExtractValue = 0;
                        Int32.TryParse(ParamValueMapDict["columnnumtoretrievevalue"], out ColumnNumToExtractValue);
                        GuiControlObj.CheckItemPresentInDataGridview(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, ColumnNumToExtractValue, ParamValueMapDict[ExpectedColumnValue], 1,1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect or columnnumtoretrievevalue param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;

                case "verifydatagridcolumnforapatternextractandclickthesame":
                    string SearchPattern = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "SearchPattern");
                    if (!InputLineParams.Contains(SearchPattern))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "SearchPattern param not found in verifyandextractcolumnpatternindatagridandclickonit", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    if (InputLineParams.Contains("columnnumtoretrievevalue"))
                    {
                        int ColumnNumToExtractValue = 0;
                        Int32.TryParse(ParamValueMapDict["columnnumtoretrievevalue"], out ColumnNumToExtractValue);
                       // GuiControlObj.CheckItemPresentInDataGridview(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, ColumnNumToExtractValue, ParamValueMapDict[ExpectedColumnValue], 0, 1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        GuiControlObj.CheckPatternPresentInDataGridviewAndExtract(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, ColumnNumToExtractValue, ParamValueMapDict[SearchPattern], 0, 1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "itemrowindextoselect or columnnumtoretrievevalue param not found", "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "checkcontainerrawviewifelementpresent":
                    string containerdirectchildparam = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "containerdirectchild");
                    if (Regex.IsMatch(containerdirectchildparam, "automationid"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "automationid", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 0, null, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(containerdirectchildparam, "controltype"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "controltype", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 0,null,Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(containerdirectchildparam, "classname"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "classname", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 0, null, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(containerdirectchildparam, "name"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "name", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 0, null, Timeout, TerminateStatus);
                    }
                    
                    break;
                case "checkcontainerrawviewifelementpresentandclickonit":
                    containerdirectchildparam = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "containerdirectchild");
                    if (Regex.IsMatch(containerdirectchildparam, "automationid"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "automationid", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(containerdirectchildparam, "controltype"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "controltype", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(containerdirectchildparam, "classname"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "classname", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(containerdirectchildparam, "name"))
                    {
                        GuiControlObj.CheckContainerRawViewIfElementPresent(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, "name", ParamValueMapDict[containerdirectchildparam], ParamValueMapDict["searchelementexpectedname"], 1, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                    }

                    break;
                case "selecttabitemfromtabcontrol":
                    string tabitemtoselect = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "tabitemtoselect");
                    if (Regex.IsMatch(tabitemtoselect, "automationid"))
                    {
                        GuiControlObj.SelectTabItemFromTabControl(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "automationid",ParamValueMapDict[tabitemtoselect], Index, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(tabitemtoselect, "classname"))
                    {
                        GuiControlObj.SelectTabItemFromTabControl(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "classname", ParamValueMapDict[tabitemtoselect], Index, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(tabitemtoselect, "name"))
                    {
                        GuiControlObj.SelectTabItemFromTabControl(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict[tabitemtoselect], Index, Timeout, TerminateStatus);
                    }
                    else if (Regex.IsMatch(tabitemtoselect, "controltype"))
                    {
                        GuiControlObj.SelectTabItemFromTabControl(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "controltype", ParamValueMapDict[tabitemtoselect], Index, Timeout, TerminateStatus);
                    }
                    int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
                    if (ProcessStatus == 1)
                    {
                        Guizard NewGuiZardObj = new Guizard();
                        NewGuiZardObj.VerifyTestCompletedFilePresent(GuizardLocation, ParamValueMapDict[tabitemtoselect]);
                        //#############################################################################
                        //while GUIzard is capturing the windows, it is filling up the controls in the window with a value 1.
                        //Slep added to make sure that Guizard finish captuing the window
                        //Thread.Sleep(1000);
                        //#############################################################################
                    }
                    break;

                case "waittillcontrolisactive":
                    //CurrentActiveWindowObj = GuiControlObj.WaitTillContrilIsActive(ParamType1, ParamValueMapDict[ParamType], RequiredValueMapDict["CurrWindowIdentifierType"], RequiredValueMapDict["CurrWindowIdentifier"], Timeout, TerminateStatus);
                    CurrentActiveWindowObj = GuiControlObj.WaitTillContrilIsActive(CurrentActiveWindowObj, ParamType1,ParamValueMapDict[ParamType],Index, Timeout, TerminateStatus);
                    break;
                case "clickontreeelement":
                    AutomationElement TreeElement = null;
                    string treeIdentifier=null;
                    if(ParamValueMapDict.ContainsKey("elementtoclickname"))
                    {
                        TreeElement=GuiControlObj.ChkTreeForElementAndClickIfReqd(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict["elementtoclickname"], 1, ParamValueMapDict["clickmousebtn"], TerminateStatus);
                        treeIdentifier="name - "+ParamValueMapDict["elementtoclickname"];
                    }
                    else if (ParamValueMapDict.ContainsKey("elementtoclickautomationid"))
                    {
                        TreeElement=GuiControlObj.ChkTreeForElementAndClickIfReqd(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "automationid", ParamValueMapDict["elementtoclickautomationid"], 1, ParamValueMapDict["clickmousebtn"], TerminateStatus);
                        treeIdentifier="automationid - "+ParamValueMapDict["elementtoclickautomationid"];
                    }
                    if (TreeElement == null)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element in tree " + treeIdentifier + " Exiting..", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element in tree  " + treeIdentifier + " Resuming as Terminateonfailure is no", "warn");
                    }
                    break;

                case "checkifelementexistintree":
                    TreeElement = null;
                    treeIdentifier = null;
                    if (ParamValueMapDict.ContainsKey("elementtocheckname"))
                    {
                        TreeElement=GuiControlObj.ChkTreeForElementAndClickIfReqd(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict["elementtocheckname"], 0, "0", TerminateStatus);
                        treeIdentifier = "name - " + ParamValueMapDict["elementtocheckname"];
                    }
                    else if (ParamValueMapDict.ContainsKey("elementtocheckautomationid"))
                    {
                        TreeElement=GuiControlObj.ChkTreeForElementAndClickIfReqd(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "automationid", ParamValueMapDict["elementtocheckautomationid"], 0, "0", TerminateStatus);
                        treeIdentifier = "automationid - " + ParamValueMapDict["elementtocheckautomationid"];
                    }
                    if (TreeElement == null)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element in tree  " + treeIdentifier + " Exiting..", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element in tree  " + treeIdentifier + " Resuming as Terminateonfailure is no", "warn");
                    }
                    break;
                case "expandtree":
                    TreeElement = null;
                    treeIdentifier = null;
                    if (ParamValueMapDict.ContainsKey("elementtoexpandname"))
                    {
                        TreeElement = GuiControlObj.ChkTreeForElementAndExpandIfReqd(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "name", ParamValueMapDict["elementtoexpandname"], 1, TerminateStatus);
                        treeIdentifier = "name - " + ParamValueMapDict["elementtoexpandname"];
                    }
                    else if (ParamValueMapDict.ContainsKey("elementtoexpandautomationid"))
                    {
                        TreeElement = GuiControlObj.ChkTreeForElementAndExpandIfReqd(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "automationid", ParamValueMapDict["elementtoexpandautomationid"], 1, TerminateStatus);
                        treeIdentifier = "automationid - " + ParamValueMapDict["elementtoexpandautomationid"];
                    }
                    if (TreeElement == null)
                    {
                        if (TerminateStatus == 1)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element in tree " + treeIdentifier + " Exiting..", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find the automation element in tree " + treeIdentifier + " Resuming as Terminateonfailure is no", "warn");
                    }
                    break;

                case "terminateiftreeelementisleafnode":
                    GuiControlObj.ChkTreeIfElementIsLeafNodeAndTerminateIfReqd(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParamValueMapDict["elementtocheckname"], TerminateStatus);
                    break;

                case "selectitemfromtreebasedonindex":
                    int ParentIndexIfDuplicatesExist=0;
                    if (InputLineParams.Contains("parentselectitemindexifduplicatesexist"))
                    {
                        Int32.TryParse(ParamValueMapDict["parentselectitemindexifduplicatesexist"], out ParentIndexIfDuplicatesExist);
                    }
                    
                    if (InputLineParams.Contains("childitemindex"))
                    {
                        int ChildIndexToSelect;
                        Int32.TryParse(ParamValueMapDict["childitemindex"], out ChildIndexToSelect);
                        GuiControlObj.SelectItemFromTreeBasedOnIndex(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentIndexIfDuplicatesExist,ChildIndexToSelect, 1, ParamValueMapDict["clickmousebtn"], TerminateStatus);
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "selectitemfromtreebasedOnindex - Invalid parameter " + ParamType + " found in InputLine " + InputLine, "fail");
                        FileObj.ExitTestEnvironment();
                    }
                    break;
                case "clickonelementatbeginning":
                    //if (string.Equals(ParamType, "automationid", StringComparison.OrdinalIgnoreCase))
                    //string ParentContainerParam = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ParentCntr");
                    string ElementToClickParam=TestInputParseObj.SearchParamListForPattern(ParamValueMapDict,"ElementToClick");
                    ElementToClickParam = ElementToClickParam.ToLower();
                    
                    int SearchWithInContainerFlag=0;
                    //Check if click has to be done inside a parent container obj or not. Sam keyword defined for both the conditions
                    foreach(string Key1 in ParamValueMapDict.Keys)
                    {
                        if (Regex.IsMatch(Key1, "Parent", RegexOptions.IgnoreCase))
                        {
                            SearchWithInContainerFlag = 1;
                            break;
                        }
                    }

                    if (SearchWithInContainerFlag == 1)
                    {
                        if (Regex.IsMatch(ElementToClickParam, "automationid", RegexOptions.IgnoreCase))
                        {
                            //GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid", ParamValueMapDict[ElementToClickParam], "Left", ChildSelectIndex, Timeout, TerminateStatus);
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid", ParamValueMapDict[ElementToClickParam], "Left", ChildSelectIndex, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);

                        }
                        else if (Regex.IsMatch(ElementToClickParam, "name", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "name", ParamValueMapDict[ElementToClickParam], "Left", ChildSelectIndex, ParamValueMapDict["clickmousebtn"],Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementToClickParam, "controltype", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "controltype", ParamValueMapDict[ElementToClickParam], "Left", ChildSelectIndex, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        }
                    }
                    else
                    {
                        GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "Left", Index, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        
                    }
                    break;
                case "clickonelementatend":

                    ElementToClickParam=TestInputParseObj.SearchParamListForPattern(ParamValueMapDict,"ElementToClick");
                    ElementToClickParam = ElementToClickParam.ToLower();
                    SearchWithInContainerFlag=0;
                    //Check if click has to be done inside a parent container obj or not. Sam keyword defined for both the conditions
                    foreach(string Key1 in ParamValueMapDict.Keys)
                    {
                        if (Regex.IsMatch(Key1, "Parent", RegexOptions.IgnoreCase))
                        {
                            SearchWithInContainerFlag = 1;
                            break;
                        }
                    }

                    if (SearchWithInContainerFlag == 1)
                    {
                    
                        if (Regex.IsMatch(ElementToClickParam, "automationid", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid", ParamValueMapDict[ElementToClickParam], "Right", ChildSelectIndex, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementToClickParam, "name", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "name", ParamValueMapDict[ElementToClickParam], "Right", ChildSelectIndex, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementToClickParam, "controltype", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "controltype", ParamValueMapDict[ElementToClickParam], "Right", ChildSelectIndex, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        }
                     }
                    else
                    {
                        GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "Right", Index, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        
                    }
                    break;
                case "clickonelementatmiddle":
                    ElementToClickParam=TestInputParseObj.SearchParamListForPattern(ParamValueMapDict,"ElementToClick");
                    ElementToClickParam = ElementToClickParam.ToLower();
                    SearchWithInContainerFlag=0;
                    //Check if click has to be done inside a parent container obj or not. Sam keyword defined for both the conditions
                    foreach(string Key1 in ParamValueMapDict.Keys)
                    {
                        if (Regex.IsMatch(Key1, "Parent", RegexOptions.IgnoreCase))
                        {
                            SearchWithInContainerFlag = 1;
                            break;
                        }
                    }

                    if (SearchWithInContainerFlag == 1)
                    {
                        if (Regex.IsMatch(ElementToClickParam, "automationid", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid", ParamValueMapDict[ElementToClickParam], "Middle", ChildSelectIndex, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementToClickParam, "name", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "name", ParamValueMapDict[ElementToClickParam], "Middle", ChildSelectIndex, ParamValueMapDict["clickmousebtn"],Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementToClickParam, "controltype", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "controltype", ParamValueMapDict[ElementToClickParam], "Middle", ChildSelectIndex, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);
                        }
                    }
                    else
                    {
                        GuiControlObj.ClickOnElementAtGivenPosition(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], "Left", Index, ParamValueMapDict["clickmousebtn"], Timeout, TerminateStatus);

                    }
                    break;

                case "checkifelementdoesnotexists":
                    string parentCntrElement = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ParentCntr");
                    string ElementIdentifier = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "Element");
                    if (parentCntrElement != null)
                    {
                        if (Regex.IsMatch(ElementIdentifier, "automationid", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementDoesNotExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "classname", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementDoesNotExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "classname", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "controltype", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementDoesNotExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "controltype", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                    }
                    else
                    {
                        GuiControlObj.CheckIfElementDoesNotExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, Timeout, TerminateStatus);
                    }
                    
                    break;
                case "checkifelementexists":
                    parentCntrElement = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ParentCntr");
                    ElementIdentifier = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "Element");
                    if (parentCntrElement != null)
                    {
                        if (Regex.IsMatch(ElementIdentifier, "automationid", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid",ParamValueMapDict[ElementIdentifier],ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "classname", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "classname", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "controltype", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "controltype", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                    }
                    else
                    {
                        GuiControlObj.CheckIfElementExists(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, Timeout, TerminateStatus);
                    }
                    break;
                case "checkifelementisactive":
                    parentCntrElement = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ParentCntr");
                    ElementIdentifier = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "Element");
                    if (parentCntrElement != null)
                    {
                        if (Regex.IsMatch(ElementIdentifier, "automationid", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementIsActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "classname", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementIsActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "classname", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "controltype", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementIsActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "controltype", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                    }
                    else
                    {
                        GuiControlObj.CheckIfElementIsActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, Timeout, TerminateStatus);
                    }
                    break;
                case "checkifelementisnotactive":
                    parentCntrElement = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "ParentCntr");
                    ElementIdentifier = TestInputParseObj.SearchParamListForPattern(ParamValueMapDict, "Element");
                    if (parentCntrElement != null)
                    {
                        if (Regex.IsMatch(ElementIdentifier, "automationid", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementIsNotActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "automationid", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "classname", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementIsNotActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "classname", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                        else if (Regex.IsMatch(ElementIdentifier, "controltype", RegexOptions.IgnoreCase))
                        {
                            GuiControlObj.CheckIfElementIsNotActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], ParentSelectIndex, "controltype", ParamValueMapDict[ElementIdentifier], ChildSelectIndex, Timeout, TerminateStatus);
                        }
                    }
                    else
                    {
                        GuiControlObj.CheckIfElementIsNotActive(CurrentActiveWindowObj, ParamType1, ParamValueMapDict[ParamType], Index, Timeout, TerminateStatus);
                    }
                    break;
                case "lookforelementwithpattern":
                    GuiControlObj.LookForElementWithPattern(CurrentActiveWindowObj, ParamValueMapDict["elementtosearchcontroltype"], ParamValueMapDict["searchpattern"],0,null, TerminateStatus);
                   
                    break;
                case "lookforelementwithpatternandclickonit":
                    GuiControlObj.LookForElementWithPattern(CurrentActiveWindowObj, ParamValueMapDict["elementtosearchcontroltype"], ParamValueMapDict["searchpattern"], 1, ParamValueMapDict["clickmousebtn"], TerminateStatus);

                    break;
                case "waitforguizardtofinishcapture":

                    GuiControlObj.ExplicitWaitFotGuizardToFinishCapturing(GuizardLocation, ParamValueMapDict["windowname"]);
                    break;

                case "xs:":
                case "finishtest":
                    //FileObj.FinishCurrentTest(ParamValueMapDict["testname"],GuizardLocation);
                    FileObj.FinishCurrentTest(ParentObj);
                    break;
                case "finishclitest":
                    FileObj.FinishCurrentCLITest();
                    break;
            }

        }

    }
}

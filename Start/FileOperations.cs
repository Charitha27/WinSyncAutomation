using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Automation;

using LoggerCollection;
using GenericCollection;
using GuizardCollection;
using GUICollection;

namespace FileOperationsCollection
{
    public class FileOperations
    {
        public string InputFilePath;
        
        public string GetInputFilePath(string LogFilePath,string FileName)
        {
            Generic NewGenObj = new Generic();
            Logger NewLogObj = new Logger();
            FileOperations NewFileObj = new FileOperations();
            string LogFilePath1 = NewLogObj.LogFileFullPath;
            //NewLogObj.WriteLogFile(LogFilePath, "GetInputFilePath", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            string CurrentDirPath = Directory.GetCurrentDirectory();
            string CurrentLocale = NewGenObj.GetSystemLocale();
            string MappedFileName;
            if (Regex.IsMatch(FileName, "Inputs"))
            {
                if (Regex.IsMatch(CurrentLocale, "CN", RegexOptions.CultureInvariant))
                {
                    MappedFileName = "Inputs_CH.txt";
                    InputFilePath = CurrentDirPath + "\\..\\Inputs\\" + MappedFileName;
                    if (File.Exists(InputFilePath))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Input file path" + InputFilePath + " exist", "info");
                        return InputFilePath;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Input file path" + InputFilePath + "Does not exist", "fail");
                        NewLogObj.WriteLogFile(LogFilePath, "****Exiting application from GetInputFilePath***", "fail");
                        return null;
                    }
                    
                }
                else if (Regex.IsMatch(CurrentLocale, "JP", RegexOptions.CultureInvariant))
                {
                    MappedFileName = "Inputs_JP.txt";
                    InputFilePath = CurrentDirPath + "\\..\\Inputs\\" + MappedFileName;
                    if (File.Exists(InputFilePath))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Input file path" + InputFilePath + " exist", "info");
                        return InputFilePath;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Input file path " + InputFilePath + " Does not exist", "fail");
                        NewLogObj.WriteLogFile(LogFilePath, "****Exiting application from GetInputFilePath***", "fail");
                        return null;
                    }
                    
                }
                else 
                {
                    MappedFileName = "Inputs.txt";
                    InputFilePath = CurrentDirPath + "\\..\\Inputs\\" + MappedFileName;
                    if (File.Exists(InputFilePath))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Input file path" + InputFilePath + " exist", "info");
                        return InputFilePath;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Input file path " + InputFilePath + " Does not exist", "fail");
                        NewLogObj.WriteLogFile(LogFilePath, "****Exiting application from GetInputFilePath***", "fail");
                        return null;
                    }

                }
                
            }
            
            else
            {
                InputFilePath = CurrentDirPath + "\\..\\Inputs\\" + FileName;
                NewLogObj.WriteLogFile(LogFilePath, "Input file path " + InputFilePath, "info");
                if (File.Exists(InputFilePath))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Input file path " + InputFilePath + " exist", "info");
                    return InputFilePath;
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Input file path " + InputFilePath + " Does not exist", "fail");
                    NewLogObj.WriteLogFile(LogFilePath, "****Exiting application from GetInputFilePath***", "fail");
                    return null;
                }
                    
             }
            
        }

        public string SearchFileForPattern(String FilePath, string Pattern, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            if(File.Exists(FilePath))
            {
                StreamReader reader = new StreamReader(FilePath);
                string line = string.Empty;
                //Replace special chars
                Regex SpecialCharLookup = new Regex(@"(\(|\)|\*|\+|\?)");
                MatchCollection SpecailCharMatchColl = SpecialCharLookup.Matches(Pattern);

                if (SpecailCharMatchColl.Count > 0)
                {
                    foreach (Match SpecialCharMatch in SpecailCharMatchColl)
                    {
                        string MatchedString = SpecialCharMatch.ToString();
                        // string ReplaceWithString = @"\" + MatchedString;
                        string ReplaceWithString = "\\" + MatchedString;
                        Pattern = Pattern.Replace(MatchedString, ReplaceWithString);
                    }
                }
               // Pattern = "^" + Pattern;
                Pattern = "^" + Pattern+"\\s*=";
                while ((line = reader.ReadLine()) != null)
                {
                    //Match m = Regex.Match(line, Pattern, RegexOptions.CultureInvariant);
                    if (Regex.IsMatch(line, Pattern, RegexOptions.CultureInvariant))
                    {
                        reader.Close();
                        return line;
                    }
                }
                NewLogObj.WriteLogFile(LogFilePath,"Unable to find "+Pattern+" in "+FilePath,"fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath,"***Exiting Application from SearchFileForPattern***", "fail");
                    //Application.Exit();

                }
                reader.Close();
                return null;

            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath,FilePath + " Does not exist", "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath,"***Exiting Application from SearchFileForPattern***", "fail");
                    //Application.Exit();
                    return null;
                }
                return null;
            }
        }

        //if the pattern appear multiple times, will return the pattern based on index
        // If index is 2, will return the second appearing similar pattern in the file
        public string SearchFileForSamePatternAppearinMultipleTimes(String FilePath, string Pattern,int MultipleOccurancePatternIndex, int TerminateStatus, string LogFilePath)
        {
            Logger NewLogObj = new Logger();
            List<string> MatchedLines = new List<string>();
            if (File.Exists(FilePath))
            {
                StreamReader reader = new StreamReader(FilePath);
                string line = string.Empty;
                //Replace special chars
                Regex SpecialCharLookup = new Regex(@"(\(|\)|\*|\+|\?)");
                MatchCollection SpecailCharMatchColl = SpecialCharLookup.Matches(Pattern);

                if (SpecailCharMatchColl.Count > 0)
                {
                    foreach (Match SpecialCharMatch in SpecailCharMatchColl)
                    {
                        string MatchedString = SpecialCharMatch.ToString();
                        // string ReplaceWithString = @"\" + MatchedString;
                        string ReplaceWithString = "\\" + MatchedString;
                        Pattern = Pattern.Replace(MatchedString, ReplaceWithString);
                    }
                }
                // Pattern = "^" + Pattern;
                Pattern = "^" + Pattern + "\\s*=";
                int MatchFoundFlag = 0;
                int IndexCounter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    //Match m = Regex.Match(line, Pattern, RegexOptions.CultureInvariant);
                    if (Regex.IsMatch(line, Pattern, RegexOptions.CultureInvariant))
                    {
                        IndexCounter++;
                        MatchedLines.Add(line);
                        MatchFoundFlag = 1;
                        if (IndexCounter == MultipleOccurancePatternIndex)
                        {
                            reader.Close();
                            return line;
                        }
                        //reader.Close();
                        //return line;
                    }
                }
                if (MatchFoundFlag == 0)
                {

                    if (TerminateStatus == 1)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find " + Pattern + " with index " + MultipleOccurancePatternIndex + " in " + FilePath, "fail");
                        //Application.Exit();
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Unable to find " + Pattern + " with index " + MultipleOccurancePatternIndex + " in " + FilePath, "warn");
                    }
                    reader.Close();
                    return null;
                }

                return null;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, FilePath + " Does not exist", "fail");
                if (TerminateStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "***Exiting Application from SearchFileForSamePatternAppearinMultipleTimes***", "fail");
                    //Application.Exit();
                    return null;
                }
                return null;
            }
        }

        //SerachPositionFlag - indicates if serach has to be done at begin or end or anywhere
        // value - Begin, End,null
        public int CheckStringForPattern(string InputString,string Pattern,string SerachPositionFlag)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.LogFileFullPath;
            Regex PatternObj = null;
            if (SerachPositionFlag != null)
            {
                if (string.Compare(SerachPositionFlag.ToLower(), "begin") == 0)
                {
                    PatternObj = new Regex("^" + Pattern, RegexOptions.IgnoreCase);
                }
                else if (string.Compare(SerachPositionFlag.ToLower(), "end") == 0)
                {
                    PatternObj = new Regex(Pattern + "$", RegexOptions.IgnoreCase);
                }
            }
            else
            {
                PatternObj = new Regex(Pattern,RegexOptions.IgnoreCase);
            }
            Match MatchResult = PatternObj.Match(InputString);
            if (MatchResult.Success)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Pattern " + Pattern + " found in string " + InputString, "info");
                return 1;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Pattern " + Pattern + " not found in string " + InputString, "info");
                return - 1;
            }

        }
        //public int CheckUnicodeStringForPattern(string InputString, string Pattern, string SerachPositionFlag)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.LogFileFullPath;
        //    Regex PatternObj = null;
        //    if (SerachPositionFlag != null)
        //    {
        //        if (string.Compare(SerachPositionFlag.ToLower(), "begin") == 0)
        //        {
        //            PatternObj = new Regex("^" + Pattern);
        //        }
        //        else if (string.Compare(SerachPositionFlag.ToLower(), "end") == 0)
        //        {
        //            PatternObj = new Regex(Pattern + "$");
        //        }
        //    }
        //    else
        //    {
        //        PatternObj = new Regex(Pattern, RegexOptions.CultureInvariant);
        //    }
        //    Match MatchResult = PatternObj.Match(InputString);
        //    if (MatchResult.Success)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Pattern " + Pattern + " found in string " + InputString, "info");
        //        return 1;
        //    }
        //    else
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Pattern " + Pattern + " not found in string " + InputString, "info");
        //        return -1;
        //    }

        //}
        public string GetLocalizedFilePath()
        {
            Generic NewGenObj = new Generic();
            Logger NewLogObj=new Logger();
            FileOperations NewFileObj = new FileOperations();
            string LogFilePath=NewLogObj.LogFileFullPath;
            NewLogObj.WriteLogFile(LogFilePath, "GetLocalizedFilePath", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            
            string CurrentLocale = NewGenObj.GetSystemLocale();
            string MappedFileName;
            if (Regex.IsMatch(CurrentLocale, "CN", RegexOptions.CultureInvariant))
            {
                MappedFileName = "Mapped_CH.txt";
                string LocalizedFilePath = NewFileObj.GetInputFilePath(LogFilePath, MappedFileName);
                NewLogObj.WriteLogFile(LogFilePath, "LocalizedFilePath " + LocalizedFilePath, "info");
                return LocalizedFilePath;
             }
            else if (Regex.IsMatch(CurrentLocale, "JP", RegexOptions.CultureInvariant))
            {
                MappedFileName = "Mapped_JP.txt";
                string LocalizedFilePath = NewFileObj.GetInputFilePath(LogFilePath, MappedFileName);
                NewLogObj.WriteLogFile(LogFilePath, "LocalizedFilePath " + LocalizedFilePath, "info");
                return LocalizedFilePath;
            }
            return null;
            
        }

        public string RemovePatternFromString(string input,string Pattern)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.LogFileFullPath;
            NewLogObj.WriteLogFile(LogFilePath, "RemovePatternFromString", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            if (Regex.IsMatch(input, Pattern, RegexOptions.CultureInvariant))
            {
                input = Regex.Replace(input, Pattern, "");
             }
            return input;

        }

        //Check the english - localized string mapped file
        public string GetLocalizedMappedPattern(string FilePath,string Pattern)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.LogFileFullPath;
            NewLogObj.WriteLogFile(LogFilePath, "GetLocalizedPattern", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            Generic NewGenObj = new Generic();
            string Localized=NewGenObj.CheckIfProductLocalizedForCurrentLocale();
            if (Localized != null)
            {
                //Mappiing is required only for localized pltforms
                FileOperations FileObj = new FileOperations();
                string LocalizedPttern = FileObj.SearchFileForPattern(FilePath, Pattern, 1, LogFilePath);
                int IndexEqual = LocalizedPttern.IndexOf("=");
                LocalizedPttern = LocalizedPttern.Substring(IndexEqual + 1);
                NewLogObj.WriteLogFile(LogFilePath, "Localized string for  " + Pattern + " " + LocalizedPttern, "info");
                return LocalizedPttern;
            }
                //If product is not localized return the english pattern after removing the &
            else
            {
                //Check if pattern has any & sign
                if (Regex.IsMatch(Pattern, "&", RegexOptions.CultureInvariant))
                {
                    Pattern = Regex.Replace(Pattern, "&","");
                    NewLogObj.WriteLogFile(LogFilePath, "Pattern  " + Pattern + " " + Pattern, "info");
                }
                return Pattern;
            }

        }

        public string GetInputPattern(string FilePath, string Pattern)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            //NewLogObj.WriteLogFile(LogFilePath, "GetInputPattern", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            FileOperations FileObj = new FileOperations();
            string InputPttern = FileObj.SearchFileForPattern(FilePath, Pattern, 1, LogFilePath);
            if (InputPttern == null)
            {
                return null;
            }
            int IndexEqual = InputPttern.IndexOf("=");
            InputPttern = InputPttern.Substring(IndexEqual + 1);
            NewLogObj.WriteLogFile(LogFilePath, "InputPttern string for  " + Pattern + " " + InputPttern, "info");
            return InputPttern;
            
        }

        public string ReadLogFileForStatus(string TestName)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath =  NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ReadLogFileForStatus", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

            int ElementMissingFlag = 0;
            if (!File.Exists(LogFilePath))
            {
                NewLogObj.WriteLogFile(LogFilePath, LogFilePath+ "does not exist", "fail");
                return null;
            }
            try
            {
                string[] lines = System.IO.File.ReadAllLines(LogFilePath);
                int Failcounter = 0;
                string FailSummary = null;
                foreach (string line in lines)
                {
                    try
                    {
                        //Extract the last 4 chars in the line, since it shows the status
                        int length = line.Length;
                        if (length > 1)
                        {
                            string status = line.Substring(length - 14);
                            //if (string.Compare(status, "FAIL")==0)
                            if (Regex.IsMatch(status, "FAIL"))
                            {
                                if (Failcounter == 0)
                                {
                                    FailSummary = line;
                                }
                                Failcounter = Failcounter + 1;
                                
                                //check if failure is due to automation element missing. For rerun
                                if (Regex.IsMatch(line, "Unable to find the automation element", RegexOptions.IgnoreCase) || Regex.IsMatch(line, "Timeout waiting", RegexOptions.IgnoreCase))
                                {
                                    ElementMissingFlag = 1;
                                }
                            }
                        }
                    }
                    catch
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Result verifictaion :Exception while processing *" +line+"*", "warn");
                    }

                }
                if (Failcounter == 0)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "No fails found in " + LogFilePath, "pass");
                    //NewLogObj.WriteLogFile(LogFilePath, "============================================== ", "info");
                    //NewLogObj.WriteLogFile(LogFilePath, "Test  " + TestName + "PASS", "info");
                    //NewLogObj.WriteLogFile(LogFilePath, "============================================== ", "info");
                    return "1";
                }
                else 
                {
                    NewLogObj.WriteLogFile(LogFilePath, Failcounter + " fails found in " + LogFilePath, "fail");
                    string returnVal = null;
                    if (ElementMissingFlag == 0)
                    {
                        returnVal = "-1:" + FailSummary;
                        //return -1;
                        return returnVal;
                    }
                    else
                    {
                        returnVal = "0:" + FailSummary;
                       // return 0;
                        return returnVal;
                    }
                }
            }
            catch
            {
                return null;
            }

        }

        
        public int FinishCurrentTest(AutomationElement ParentObj)
        {
            
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "FinishCurrentTest", "info");
            NewLogObj.WriteLogFile(LogFilePath, "====================", "info");
            FileOperations FileObj = new FileOperations();
            //string GuizardPathFile = Directory.GetCurrentDirectory() + "//GuizardPathFile.txt";
            //string GuiZardLocation = FileObj.GetInputPattern(GuizardPathFile, "GuizardLocation");
            Generic NewGenericObj = new Generic();
            try
            {
                string SystemLocale = NewGenericObj.GetSystemLocale();
                //NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";

                string GuiZardLocation = FileObj.GetInputPattern(InputFileName, "GuiZardLocation");
                string CurrentFileName = Environment.GetCommandLineArgs()[1];
                //Remove the .txt part
                string Testname = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
                //Check if guizard is running
                string GuizardResultsFolder = null;

                int ProcessStatus = NewGenericObj.CheckIfProcessIsRunning("GUIzard_3");
                if (ProcessStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Guizard is running. Verifying Guizard results", "info");
                    Guizard NewGuiZardObj = new Guizard();
                    //Verify results of GUizard
                    GuizardResultsFolder = NewGuiZardObj.VerifyGuiZardResults(Testname);
                    //NewGuiZardObj.VerifyGuiZardResults(Testname);
                    //GuizardResultsFolder = "C:\\TestResults\\" + Testname;
                    NewGuiZardObj.CreateGuizardStopTestFile(GuiZardLocation);
                    //Giving enough time for guizard to close
                    NewGuiZardObj.WaitTillGuizardClose();

                    
                }
                else
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Guizard is not running.", "info");
                }
                //Close the XenCenter
                AutomationElementIdentity GuiObj = new AutomationElementIdentity();

                GuiObj.CloseChildWindowsUnderParent(ParentObj, 0);

                //check if process is still running
                string ParentProcessName = FileObj.GetInputPattern(InputFileName, "ParentProcessName");
                int ParentProcessStatus = NewGenericObj.CheckIfProcessIsRunning(ParentProcessName);
                if (ParentProcessStatus == 1)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Process " + ParentProcessName + "still running even after close. Killing the parent process", "fail");
                    NewGenericObj.KillProcess(ParentProcessName, LogFilePath);
                }
                //int Status = ReadLogFileForStatus(Testname);
                string TempStatus = ReadLogFileForStatus(Testname);
                string[] Temp=null;
                string TempStr = null;
                string Status = null;
                 string FailureSummary = null;
                 if (string.Compare(TempStatus, "1") == 0)
                 {
                     Status = "1";
                 }
                 else
                 {
                     Temp = Regex.Split(TempStatus, ":");
                     Status = Temp[0];
                     //if(Status==1)
                     //Extracting the fail summary
                     int IndexFontColor = Temp[4].IndexOf("=");
                     TempStr = Temp[4].Substring(IndexFontColor);

                     if (Regex.IsMatch(TempStr, "guizard", RegexOptions.IgnoreCase))
                     {
                         FailureSummary = "GUIZard failures";
                     }
                     else
                     {
                         int MarkerIndex1 = TempStr.IndexOf(">");
                         int MarkerIndex2 = TempStr.IndexOf("<");
                         FailureSummary = TempStr.Substring(MarkerIndex1 + 1, MarkerIndex2 - (MarkerIndex1 + 1));
                     }
                 }
                if (string.Compare(Status, "1") == 0)
                {
                    //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "Test " + Testname + "  PASS", "PASS");
                    //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                    NewLogObj.WriteToTestSummaryFile(Testname, "PASS", GuizardResultsFolder, "NA");


                }
                //else if(Status==-1)
                else if (string.Compare(Status, "-1") == 0)
                {
                    //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "Test " + Testname + "  FAIL", "FAIL");
                    //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                    NewLogObj.WriteToTestSummaryFile(Testname, "FAIL", GuizardResultsFolder, FailureSummary);

                }
                //else if (Status == 0)
                else if (string.Compare(Status, "0") == 0)
                {
                    //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                    NewLogObj.WriteLogFile(LogFilePath, "Test " + Testname + "  FAIL", "FAIL");
                    //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                    NewLogObj.WriteToTestSummaryFile(Testname, "FAIL", GuizardResultsFolder, FailureSummary);

                    string RerunFilePattern = "^ReRun";
                    if (Regex.IsMatch(CurrentFileName, RerunFilePattern))
                    {
                        Console.WriteLine("Rerun pattern found in filename. Exiting");
                        Environment.Exit(1);
                        Console.WriteLine("Exiting");
                    }
                    else
                    {
                        //Check if rerun is required
                        CheckIfRerunRequired();
                    }

                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at FinsihCurrentTest " + Ex.ToString() , "warn");
            }
            return 1;
        }
        //For testcases done thorugh CLI, which does not product & guizard windows to be closed
        public void FinishCurrentCLITest()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "FinishCurrentCLITest", "info");
            NewLogObj.WriteLogFile(LogFilePath, "====================", "info");
            FileOperations FileObj = new FileOperations();
            
            Generic NewGenericObj = new Generic();
            
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            //Remove the .txt part
            string Testname = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
            //int Status = ReadLogFileForStatus(Testname);
            string TempStatus = ReadLogFileForStatus(Testname);
            string[] TempArr = Regex.Split(TempStatus, ":");
            string Status = TempArr[0];
            //if (Status == 1)
            if(string.Compare(Status,"1")==0)
            {
                //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                NewLogObj.WriteLogFile(LogFilePath, "Test " + Testname + "  PASS", "PASS");
                //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                NewLogObj.WriteToTestSummaryFile(Testname, "PASS", "",null);

            }
            //else if (Status == -1)
            else if (string.Compare(Status, "-1") == 0)
            {
                //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                NewLogObj.WriteLogFile(LogFilePath, "Test " + Testname + "  FAIL", "FAIL");
                //NewLogObj.WriteLogFile(LogFilePath, "*******************************************", "info");
                NewLogObj.WriteToTestSummaryFile(Testname, "FAIL", "", TempArr[1]);
            }
        }

        public string GetLatestFileInDirectory(string DirectoryPath)
        {
            var directory = new DirectoryInfo(DirectoryPath);
           // System.IO.FileInfo FileInfoObj=(from f in directory.GetFiles()orderby f.LastWriteTime descending select f).First();
            FileInfo[] files = directory.GetFiles();
            DateTime lastWrite = DateTime.MinValue;
            string File1=null;
            DateTime lastHigh = new DateTime(1900, 1, 1);
            string highDir=null;
            foreach (string subdir in Directory.GetDirectories(DirectoryPath))
            {
                DirectoryInfo fi1 = new DirectoryInfo(subdir);
                DateTime created = fi1.LastWriteTime;
                
                if (created > lastHigh)
                {
                    highDir = subdir;
                    lastHigh = created;
                    
                }
            }
            return highDir;           

        }

        public void CopyContentsOfAFolder(string SourceFolderPath, string DesiatinationPath)
        {
            try
            {
                string[] files = System.IO.Directory.GetFiles(SourceFolderPath);
                if (Directory.Exists(DesiatinationPath))
                {
                    Directory.CreateDirectory(DesiatinationPath);
                }

                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(DesiatinationPath, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            catch
            {

            }
        }

        public void CopyFileToAFolder(string SourceFilePath, string DesiatinationFolderPath)
        {
            try
            {
                string fileName = System.IO.Path.GetFileName(SourceFilePath);
                string destFile = System.IO.Path.Combine(DesiatinationFolderPath, fileName);
                System.IO.File.Copy(SourceFilePath, destFile, true);
            }
            catch
            {

            }
        }
        public void MakeACopyOfFile(string SourceFilePath, string DesiatinationFolderPath,string NewFileName)
        {
            try
            {
                string fileName = System.IO.Path.GetFileName(SourceFilePath);
                string destFile = System.IO.Path.Combine(DesiatinationFolderPath, NewFileName);
                System.IO.File.Copy(SourceFilePath, destFile, true);
            }
            catch
            {

            }
        }
        public void RenameFile(string SourceFilePath, string RenamedFilePath)
        {
            try
            {
                string fileName = System.IO.Path.GetFileName(SourceFilePath);

                System.IO.File.Move(SourceFilePath, RenamedFilePath);
            }
            catch
            {

            }

        }
        public int ExitTestEnvironment()
        {
            Logger NewLogObj = new Logger();
            NewLogObj.CaptureScreenShot();
            string CurrentExeName = Environment.GetCommandLineArgs()[1];
            //AutomationElementIdentity GuiObj=new AutomationElementIdentity();
            AutomationElement SuperparentWindowOj = AutomationElementIdentity.SuperParentWindowObj;

            //string CurrentFileName = Environment.GetCommandLineArgs()[1];
            
            string LogFilePath = NewLogObj.GetLogFilePath();
            int Length = CurrentExeName.Length;
            CurrentExeName = CurrentExeName.Substring(0, Length - 4);
            //NewLogObj.WriteLogFile(LogFilePath, "=============================================", "fail");
            NewLogObj.WriteLogFile(LogFilePath, "Exiting Test Envt for "+CurrentExeName, "fail");
            FileOperations FileObj = new FileOperations();
            //FileObj.FinishCurrentTest(CurrentExeName,"0");
            int Status=FileObj.FinishCurrentTest(SuperparentWindowOj);
            Environment.Exit(1);
            return 1;
        }

        //Exit envt without verifying guizard
        public void ExitTestEnvironment(int NoGuizardVerification)
        {
            Logger NewLogObj = new Logger();
            NewLogObj.CaptureScreenShot();
            string CurrentExeName = Environment.GetCommandLineArgs()[1];
            //AutomationElementIdentity GuiObj=new AutomationElementIdentity();
            AutomationElement SuperparentWindowOj = AutomationElementIdentity.SuperParentWindowObj;

            //string CurrentFileName = Environment.GetCommandLineArgs()[1];

            string LogFilePath = NewLogObj.GetLogFilePath();
            int Length = CurrentExeName.Length;
            CurrentExeName = CurrentExeName.Substring(0, Length - 4);
            //NewLogObj.WriteLogFile(LogFilePath, "=============================================", "fail");
            NewLogObj.WriteLogFile(LogFilePath, "Exiting Test Envt for " + CurrentExeName, "fail");
            FileOperations FileObj = new FileOperations();
            //FileObj.FinishCurrentTest(CurrentExeName,"0");
            FileObj.FinishCurrentCLITest();
            //NewLogObj.WriteLogFile(LogFilePath, "=============================================", "fail");
            Environment.Exit(1);
        }

        public void CreateFileAndWriteContents(string FileNameWithPath, string StringToWrite)
        {
            if(File.Exists(FileNameWithPath))
            {
                File.Delete(FileNameWithPath);
            }
            FileStream LF = File.Create(FileNameWithPath);
            LF.Close();
            StreamWriter SW = File.AppendText(FileNameWithPath);
            SW.WriteLine(StringToWrite);
            SW.Flush();
            SW.Close();

        }

        public int CheckIfRerunRequired()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
                string CurrentLocale = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
                string InputFilePath = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + CurrentLocale + ".txt";
                FileOperations FileObj = new FileOperations();
                string RerunValuestr = FileObj.GetInputPattern(InputFilePath, "ReRunOnFailure");
                int ReRunVal = 0;
                Int32.TryParse(RerunValuestr, out ReRunVal);
                if (ReRunVal == 1)
                {
                    //Check if rerun is already initiated. If rerun is initiated there will be a filename with Rerun tag already created
                   
                    string CurrentFileName = Environment.GetCommandLineArgs()[1];
                    string RerunFilePattern = "^ReRun";
                    if (Regex.IsMatch(CurrentFileName, RerunFilePattern))
                    {
                        Console.WriteLine("CheckIfRerunRequired : Rerun pattern found in filename. Exiting");
                        FileObj.ExitTestEnvironment();
                        Console.WriteLine("Exiting");
                    }
                    
                    //Remove the .txt part
                    string Testname = CurrentFileName.Substring(0, CurrentFileName.Length - 4);
                    Console.WriteLine(" ReRunning the  test " + Testname);
                    NewLogObj.WriteLogFile(LogFilePath, "**************ReRun Initiated on test" + Testname + " ****************", "warn");


                    //Create a copy of the existing file with a tag Rerun ;e ReRun_AddServer.txt
                    string CurrentTestFilePath = Directory.GetCurrentDirectory() + "\\TestFiles\\" + CurrentFileName;
                    string DestinationFilePath = Directory.GetCurrentDirectory() + "\\TestFiles";
                    string DestinationFileName = "ReRun_" + CurrentFileName;

                    string ReRunFilePath = DestinationFilePath + "\\" + DestinationFileName;
                   
                    FileObj.MakeACopyOfFile(CurrentTestFilePath, DestinationFilePath, DestinationFileName);
                    if (File.Exists(DestinationFilePath + "\\" + DestinationFileName))
                    {
                        string Command = "start /WAIT RunTest.exe " + DestinationFileName;
                        Generic GenericObj = new Generic();
                        GenericObj.StartCmdExecutionAndWaitForCompletion(Command);

                        //Delte the rerun file
                        return 1;
                    }
                    else
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "ReRun for " + Testname + "cannot be started as " + DestinationFilePath + "\\" + DestinationFileName + " is missing", "fail");
                        return -1;
                    }
                    

                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception while rerunnig " + Ex.ToString(), "warn");
            }
            return 1;
        }

    }
}

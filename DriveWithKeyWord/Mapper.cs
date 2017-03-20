using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading;

using LoggerCollection;
using FileOperationsCollection;
using GenericCollection;
using KeyWordAPIsCollection;


namespace MapperCollection
{
    public class Mapper
    {
        
        Dictionary<string, string> LocalizedTTKFileMap = new Dictionary<string, string>();
        //To handle partially localized product
        public static Dictionary<String, String> LocEnStringMappingForPartialLocalization = new Dictionary<string, string>();
        public static int PartiallyLocalized = 0;
        //string TTKFileName = null;
        //public void GenerateMapping()
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    string TTKFileName=CheckIfProductLocalizedForCurrentLocale();
        //    if (TTKFileName == null)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, TTKFileName + " TTKFileName is null, Cannot genertae mapping", "fail");
        //    }
        //    string[] Temp = TTKFileName.Split('-');
        //    TTKFileName = Temp[0];
        //    string CurrentDir = Directory.GetCurrentDirectory();
        //    string MappedFile = CurrentDir + "//Inputs//Mapped.txt";
        //    //if (File.Exists(MappedFile))
        //    //{
        //    //    File.Delete(MappedFile);
        //    //}
        //   // File.Create(MappedFile);
        //    //Introducing timeout of 1 min for attempting multiple times for perl script to start
        //    int timer = 0; int PerlStartTimeout=60000; // 1 min
        //    while (timer < PerlStartTimeout)
        //    {
        //        try
        //        {
        //            string PerlFilePath = CurrentDir + "//Inputs//" + TTKFileName;
        //            Process myProcess = new Process();
        //            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo("perl.exe");
        //            //myProcessStartInfo.Arguments = "GenerateMapping.pl " + TTKFileName;
        //            NewLogObj.WriteLogFile(LogFilePath, "GenerateMapping.pl \"" + PerlFilePath + "\"", "info");
        //            myProcessStartInfo.Arguments = "GenerateMapping.pl \"" + PerlFilePath + "\"";
        //            myProcessStartInfo.UseShellExecute = false;
        //            myProcessStartInfo.RedirectStandardOutput = true;
        //            myProcess.StartInfo = myProcessStartInfo;
        //            myProcess.Start();
        //            myProcess.WaitForExit();
        //            break;
        //        }
        //        catch (Exception Ex)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Exception while calling GenerateMapping.pl " + Ex.ToString(), "fail");
        //            timer = timer + 2000;
        //            Thread.Sleep(2000);
                    
        //        }
        //    }
        //    if (timer >= PerlStartTimeout)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, "Unable to start GenerateMapping.pl even after retry attempts " , "fail");
        //        FileOperations FileObj = new FileOperations();
        //        FileObj.ExitTestEnvironment();
        //    }


        //}

        public void GenerateMapping(string[] TTKFiles)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            Generic NewGenericObj = new Generic();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GenerateMapping ", "info");
            NewLogObj.WriteLogFile(LogFilePath, "============= ", "info");
            string[] WindowTitlesInEnglish = null;
            string FileToConvert = Directory.GetCurrentDirectory() + "\\Inputs\\In.txt";
            if (File.Exists(FileToConvert))
            {
                WindowTitlesInEnglish = System.IO.File.ReadAllLines(FileToConvert);
            }
            string[] TempWindowTitlesInEnglish = WindowTitlesInEnglish;

            string SystemLocale = NewGenericObj.GetSystemLocale();
            string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
            string TTKHotKeyPatternSpecifier = FileObj.GetInputPattern(InputFileName, "TTKHotKeyPatternSpecifier");

            //Replacing special chars
            for (int i = 0; i < TempWindowTitlesInEnglish.Length; i++)
            {
                //if (Regex.IsMatch(TempWindowTitlesInEnglish[i], TTKHotKeyPatternSpecifier, RegexOptions.CultureInvariant))
                //{
                //    TempWindowTitlesInEnglish[i] = Regex.Replace(TempWindowTitlesInEnglish[i], TTKHotKeyPatternSpecifier, "");
                //}
                Regex SpecialCharLookup = new Regex(@"(\(|\)|\*|\+|\?|\.)");
                MatchCollection SpecailCharMatchColl = SpecialCharLookup.Matches(TempWindowTitlesInEnglish[i]);


                if (SpecailCharMatchColl.Count > 0)
                {
                    string PreviousSpecialCharMatch = null;

                    foreach (Match SpecialCharMatch in SpecailCharMatchColl)
                    {

                        string MatchedString = SpecialCharMatch.ToString();

                        // string ReplaceWithString = @"\" + MatchedString;
                        if (PreviousSpecialCharMatch != MatchedString)
                        {
                            string ReplaceWithString = "\\" + MatchedString;
                            TempWindowTitlesInEnglish[i] = TempWindowTitlesInEnglish[i].Replace(MatchedString, ReplaceWithString);
                        }
                        PreviousSpecialCharMatch = MatchedString;
                    }
                }

            }

            string OutPutFile = Directory.GetCurrentDirectory() + "\\Inputs\\Mapped.txt";
            if (File.Exists(OutPutFile))
            {
                File.Delete(OutPutFile);
            }

            StreamWriter outfile = new StreamWriter(@OutPutFile);

            //string[] TTKFiles=Environment.GetCommandLineArgs();

            for (int y = 0; y < TTKFiles.Length; y++)
            {
                string TTKFileFullPath = Directory.GetCurrentDirectory() + "\\Inputs\\" + TTKFiles[y];
                if (!File.Exists(TTKFileFullPath))
                {
                    Console.WriteLine("TTKfile " + TTKFileFullPath + " does not exist");
                    NewLogObj.WriteLogFile(LogFilePath, TTKFiles[y] + " does not exist ", "fail");

                    FileObj.ExitTestEnvironment();
                }
            }

            for (int y = 0; y < TTKFiles.Length; y++)
            {
                string TTKFile = TTKFiles[y];
                string TTKFileFullPath = Directory.GetCurrentDirectory() + "\\Inputs\\" + TTKFile;
                string[] InputLines = System.IO.File.ReadAllLines(TTKFileFullPath);

                foreach (string WindowTitle in TempWindowTitlesInEnglish)
                {
                    int FirstIterationFlag = 0;
                    foreach (string line in InputLines)
                    {
                        string pattern = "(?<ENStringMatched>^" + WindowTitle + ")(?<UnicodeStringMatched>\t+.*)";
                        Regex r = new Regex(pattern);
                        Match match = r.Match(line);
                        if (match.Success)
                        {
                            //string EnString = match.Groups[1].Value;
                            //string UnicodeString = match.Groups[2].Value;
                            string EnString = match.Groups["ENStringMatched"].Value;
                            string UnicodeString = match.Groups["UnicodeStringMatched"].Value;
                            string[] TempArray = Regex.Split(UnicodeString, "\t+");
                            outfile.Write(EnString + "=" + TempArray[1] + "\n");
                            if (FirstIterationFlag == 0)
                            {
                                //Splice the element
                                List<String> Temp = TempWindowTitlesInEnglish.ToList();
                                Temp.Remove(WindowTitle);
                                TempWindowTitlesInEnglish = Temp.ToArray();
                            }
                            FirstIterationFlag = 1;
                        }
                    }

                }
            }

            if (TempWindowTitlesInEnglish.Length > 0)
            {
                foreach (string Wnd in TempWindowTitlesInEnglish)
                {
                    if (!Wnd.StartsWith("#"))
                    {
                        //outfile.Write(Wnd+"\n");
                        outfile.Write(Wnd + "=" + "" + "\n");
                    }
                }

            }
            outfile.Close();
        }

        //will take care even if hot keys changes based on language
        public void GenerateMappingOld2(string[] TTKFiles)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            Generic NewGenericObj = new Generic();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GenerateMapping ", "info");
            NewLogObj.WriteLogFile(LogFilePath, "============= ", "info");
            string[] WindowTitlesInEnglish=null;
            string FileToConvert = Directory.GetCurrentDirectory()+"\\Inputs\\In.txt";
            if (File.Exists(FileToConvert))
            {
                WindowTitlesInEnglish = System.IO.File.ReadAllLines(FileToConvert);
            }
            string[] TempWindowTitlesInEnglish = WindowTitlesInEnglish;

            string SystemLocale = NewGenericObj.GetSystemLocale();
            string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
            string TTKHotKeyPatternSpecifier = FileObj.GetInputPattern(InputFileName, "TTKHotKeyPatternSpecifier");
                
            //Replacing special chars
            for (int i = 0; i < TempWindowTitlesInEnglish.Length; i++)
            {
                //if (Regex.IsMatch(TempWindowTitlesInEnglish[i], TTKHotKeyPatternSpecifier, RegexOptions.CultureInvariant))
                //{
                //    TempWindowTitlesInEnglish[i] = Regex.Replace(TempWindowTitlesInEnglish[i], TTKHotKeyPatternSpecifier, "");
                //}
                Regex SpecialCharLookup = new Regex(@"(\(|\)|\*|\+|\?|\.)");
                MatchCollection SpecailCharMatchColl = SpecialCharLookup.Matches(TempWindowTitlesInEnglish[i]);

               
                if (SpecailCharMatchColl.Count > 0)
                {
                    string PreviousSpecialCharMatch = null;
                    
                    foreach (Match SpecialCharMatch in SpecailCharMatchColl)
                    {
                        
                        string MatchedString = SpecialCharMatch.ToString();
                       
                        // string ReplaceWithString = @"\" + MatchedString;
                        if (PreviousSpecialCharMatch != MatchedString)
                        {
                            string ReplaceWithString = "\\" + MatchedString;
                            TempWindowTitlesInEnglish[i] = TempWindowTitlesInEnglish[i].Replace(MatchedString, ReplaceWithString);
                        }
                        PreviousSpecialCharMatch = MatchedString;
                    }
                }
                
            }

            string OutPutFile=Directory.GetCurrentDirectory()+"\\Inputs\\Mapped.txt";
            if (File.Exists(OutPutFile))
            {
                File.Delete(OutPutFile);
            }
     
            StreamWriter outfile = new StreamWriter(@OutPutFile);

            //string[] TTKFiles=Environment.GetCommandLineArgs();
            
            for (int y = 0; y < TTKFiles.Length; y++)
            {
                string TTKFileFullPath = Directory.GetCurrentDirectory() + "\\Inputs\\" + TTKFiles[y];
                if (!File.Exists(TTKFileFullPath))
                {
                    Console.WriteLine("TTKfile " + TTKFileFullPath + " does not exist");
                    NewLogObj.WriteLogFile(LogFilePath, TTKFiles[y]+" does not exist ", "fail");
                    
                    FileObj.ExitTestEnvironment();
                }
            }
             
            for (int y=0; y<TTKFiles.Length;y++)
            {
                string TTKFile = TTKFiles[y];
                string TTKFileFullPath = Directory.GetCurrentDirectory() + "\\Inputs\\" + TTKFile;
                string[] InputLines = System.IO.File.ReadAllLines(TTKFileFullPath);
                foreach (string line in InputLines)
                {
                    string TTKFileLineWithHotKeyPatternExemped = null;
                    //if (Regex.IsMatch(line, TTKHotKeyPatternSpecifier, RegexOptions.CultureInvariant))
                    //{
                    //    TTKFileLineWithHotKeyPatternExemped = Regex.Replace(line, TTKHotKeyPatternSpecifier, "");
                    //}
                    if(TTKFileLineWithHotKeyPatternExemped==null)
                    {
                        TTKFileLineWithHotKeyPatternExemped = line;
                    }
                    //TTKFileLineWithHotKeyPatternExemped = TTKFileLineWithHotKeyPatternExemped.ToLower();
                    //line.ToLower();
                    if (TempWindowTitlesInEnglish.Length > 0)
                    {
                        for (int i = 0; i < TempWindowTitlesInEnglish.Length; i++)
                        {
                            //string pattern = "(^" + TempWindowTitlesInEnglish[i] + ")(\t+.*)";
                            string pattern = "(?<ENStringMatched>^" + TempWindowTitlesInEnglish[i] + ")(?<UnicodeStringMatched>\t+.*)";
                            Regex r = new Regex(pattern);
                            Match match = r.Match(TTKFileLineWithHotKeyPatternExemped);
                            if (match.Success)
                            {
                                //string EnString = match.Groups[1].Value;
                                //string UnicodeString = match.Groups[2].Value;
                                string EnString = match.Groups["ENStringMatched"].Value;
                                string UnicodeString = match.Groups["UnicodeStringMatched"].Value;
                                string[] TempArray = Regex.Split(UnicodeString, "\t+");
                                outfile.Write(EnString + "=" + TempArray[1] + "\n");
                                //Splice the element
                                List<String> Temp = TempWindowTitlesInEnglish.ToList();
                                Temp.Remove(TempWindowTitlesInEnglish[i]);
                                TempWindowTitlesInEnglish = Temp.ToArray();

                                break;

                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (TempWindowTitlesInEnglish.Length > 0)
            {
                foreach (string Wnd in TempWindowTitlesInEnglish)
                {
                    if(!Wnd.StartsWith("#"))
                    {
                        //outfile.Write(Wnd+"\n");
                        outfile.Write(Wnd +"="+""+ "\n");
                    }
                }

            }
            outfile.Close();
        }

        //Will serach for exact hot key pattern
        // Will fail when the hot keys differ based on locale.
        public void GenerateMappingOld(string[] TTKFiles)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "GenerateMapping ", "info");
            NewLogObj.WriteLogFile(LogFilePath, "============= ", "info");
            string[] WindowTitlesInEnglish = null;
            string FileToConvert = Directory.GetCurrentDirectory() + "\\Inputs\\In.txt";
            if (File.Exists(FileToConvert))
            {
                WindowTitlesInEnglish = System.IO.File.ReadAllLines(FileToConvert);
            }
            string[] TempWindowTitlesInEnglish = WindowTitlesInEnglish;

            //Replacing special chars
            for (int i = 0; i < TempWindowTitlesInEnglish.Length; i++)
            {
                Regex SpecialCharLookup = new Regex(@"(\(|\)|\*|\+|\?)");
                MatchCollection SpecailCharMatchColl = SpecialCharLookup.Matches(TempWindowTitlesInEnglish[i]);

                if (SpecailCharMatchColl.Count > 0)
                {
                    foreach (Match SpecialCharMatch in SpecailCharMatchColl)
                    {
                        string MatchedString = SpecialCharMatch.ToString();
                        // string ReplaceWithString = @"\" + MatchedString;
                        string ReplaceWithString = "\\" + MatchedString;
                        TempWindowTitlesInEnglish[i] = TempWindowTitlesInEnglish[i].Replace(MatchedString, ReplaceWithString);
                    }
                }

            }

            string OutPutFile = Directory.GetCurrentDirectory() + "\\Inputs\\Mapped.txt";
            if (File.Exists(OutPutFile))
            {
                File.Delete(OutPutFile);
            }

            StreamWriter outfile = new StreamWriter(@OutPutFile);

            //string[] TTKFiles=Environment.GetCommandLineArgs();
            FileOperations FileObj = new FileOperations();
            for (int y = 0; y < TTKFiles.Length; y++)
            {
                string TTKFileFullPath = Directory.GetCurrentDirectory() + "\\Inputs\\" + TTKFiles[y];
                if (!File.Exists(TTKFileFullPath))
                {
                    Console.WriteLine("TTKfile " + TTKFileFullPath + " does not exist");
                    NewLogObj.WriteLogFile(LogFilePath, TTKFiles[y] + " does not exist ", "fail");
                   
                    FileObj.ExitTestEnvironment();
                }
            }
            for (int y = 0; y < TTKFiles.Length; y++)
            {
                string TTKFile = TTKFiles[y];
                string TTKFileFullPath = Directory.GetCurrentDirectory() + "\\Inputs\\" + TTKFile;
                string[] InputLines = System.IO.File.ReadAllLines(TTKFileFullPath);
                foreach (string line in InputLines)
                {
                    if (TempWindowTitlesInEnglish.Length > 0)
                    {
                        for (int i = 0; i < TempWindowTitlesInEnglish.Length; i++)
                        {
                            //string pattern = "(^" + TempWindowTitlesInEnglish[i] + ")(\t+.*)";

                            string pattern = "(?<ENStringMatched>^" + TempWindowTitlesInEnglish[i] + ")(?<UnicodeStringMatched>\t+.*)";
                            Regex r = new Regex(pattern);
                            Match match = r.Match(line);
                            if (match.Success)
                            {
                                //string EnString = match.Groups[1].Value;
                                //string UnicodeString = match.Groups[2].Value;
                                string EnString = match.Groups["ENStringMatched"].Value;
                                string UnicodeString = match.Groups["UnicodeStringMatched"].Value;
                                string[] TempArray = Regex.Split(UnicodeString, "\t+");
                                outfile.Write(EnString + "=" + TempArray[1] + "\n");
                                //Splice the element
                                List<String> Temp = TempWindowTitlesInEnglish.ToList();
                                Temp.Remove(TempWindowTitlesInEnglish[i]);
                                TempWindowTitlesInEnglish = Temp.ToArray();

                                break;

                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (TempWindowTitlesInEnglish.Length > 0)
            {
                foreach (string Wnd in TempWindowTitlesInEnglish)
                {
                    if (!Wnd.StartsWith("#"))
                    {
                        outfile.Write(Wnd + "\n");
                    }
                }

            }
            outfile.Close();
        }

        
        public void WriteMapperInputFile(List<string> Input)
        {
            string CurrentDirPath = Directory.GetCurrentDirectory();
            string INFilePath = CurrentDirPath + "\\Inputs\\In.txt";
            FileStream LF=null;
            StreamWriter SW=null;
            if(File.Exists(INFilePath))
            {
                File.Delete(INFilePath);
            }
            LF = File.Create(INFilePath);
            LF.Close();
            SW = File.AppendText(INFilePath);
           // foreach (string Value in Input)
            for(int i=0;i<Input.Count;i++)
            {
                SW.WriteLine(Input[i]);
            }
            
            SW.Flush();
            SW.Close();
            

        }

        public void FindLocalizedLanguages()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            string TTKFilePath=CurrentDirPath+"\\Inputs\\TTKFileList.txt";
            if (!File.Exists(TTKFilePath))
            {
                NewLogObj.WriteLogFile(LogFilePath, TTKFilePath + " file does not exist ", "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();
            }

            StreamReader reader = new StreamReader(TTKFilePath);
            string line = string.Empty;
            while ((line = reader.ReadLine()) != null)
            {
                string[] Temp = line.Split('-');
                string Language = Temp[0];
                string TTKFile = Temp[1];
                string LocalizationStatus = Temp[2];

                //To handle partially localized products
                string TempStr = TTKFile + "-" + LocalizationStatus;
                //LocalizedTTKFileMap.Add(Language,TTKFile);
                LocalizedTTKFileMap.Add(Language, TempStr);
            }
            NewLogObj.WriteLogFile(LogFilePath, "Product localized on below languages", "info");
            
            foreach (KeyValuePair<string, string> TTKLangMap in LocalizedTTKFileMap)
            {
                NewLogObj.WriteLogFile(LogFilePath, TTKLangMap.Key + " " +TTKLangMap.Value, "info");
                
            }

        }

        public string CheckIfProductLocalizedForCurrentLocale()
        {
            Logger NewLogObj = new Logger();
            FileOperations NewFileObj = new FileOperations();
            string LogFilePath = NewLogObj.LogFileFullPath;
            NewLogObj.WriteLogFile(LogFilePath, "CheckIfProductLocalizedForCurrentLocale", "info");
            NewLogObj.WriteLogFile(LogFilePath, "====================================", "info");
            if (!(LocalizedTTKFileMap.Count > 0))
            {
                FindLocalizedLanguages();
            }

            string CurrentLocale = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            foreach (KeyValuePair<string, string> TTKLangMap in LocalizedTTKFileMap)
            {
                if (Regex.IsMatch(CurrentLocale, TTKLangMap.Key,RegexOptions.IgnoreCase))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Product is localized for currentlocale " + CurrentLocale, "info");
                    return TTKLangMap.Value;
                }
                
            }
            return null;

        }

        public List<string> FindMappedPattern(List<string> Inputpattern)
        {
            Logger NewLogObj = new Logger();
            FileOperations NewFileObj = new FileOperations();
            string LogFilePath = NewLogObj.LogFileFullPath;
            List<string> LocalizedMappedPatternList=new List <string>();
            Generic NewGenericObj = new Generic();
            FileOperations FileObj = new FileOperations();

             //In case of partially localized products, there will be 2 return lists
            //one having the localized values
            
            string LocalizationStatus;

            //Assuming that if 1st aparemeter has "mapped" word in it, parameter following it will be the EN string name
            Mapper MapObj = new Mapper();
           

            string TTKFileName = MapObj.CheckIfProductLocalizedForCurrentLocale();

            string LocalizedPttern=null;
            if (TTKFileName == null)
            {
                NewLogObj.WriteLogFile(LogFilePath, "IProduct is not localized for current locale", "info");
                //LocalizedPttern = Inputpattern;
                //foreach (string Value in Inputpattern)
                //Generic NewGenericObj = new Generic();
                
                string SystemLocale = NewGenericObj.GetSystemLocale();
                //NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
                string TTKHotKeyPatternSpecifier = FileObj.GetInputPattern(InputFileName, "TTKHotKeyPatternSpecifier");
                for(int i=0;i<Inputpattern.Count;i++)
                {
                    // if (Regex.IsMatch(Inputpattern[i], "&", RegexOptions.CultureInvariant))
                    if (Regex.IsMatch(Inputpattern[i], TTKHotKeyPatternSpecifier, RegexOptions.CultureInvariant))
                    {
                        Inputpattern[i] = Regex.Replace(Inputpattern[i], TTKHotKeyPatternSpecifier, "");
                        NewLogObj.WriteLogFile(LogFilePath, "Inputpattern[i]  " + Inputpattern[i], "info");
                    }
                    LocalizedMappedPatternList.Add(Inputpattern[i]);
                    
                }
                
                //return LocalizedPttern;
                return LocalizedMappedPatternList;
            }
          else
            {
               //For capturing strings that appear multiple times in files
                string MappingIndexCaptureFile = Directory.GetCurrentDirectory() + "//SelectIndexForMultipleMappingString.txt";
                int SelectIndexForMultipleMappingString = 0;
                if(File.Exists(MappingIndexCaptureFile))
                {
                String SelectIndexForMultipleMappingStringLine = FileObj.SearchFileForPattern(MappingIndexCaptureFile, "SelectIndexForMultipleMappingString", 1, LogFilePath);
                int IndexEqual1 = SelectIndexForMultipleMappingStringLine.IndexOf("=");
                

                //For patterns which are localised, get the localised vals else get the EN vals
                if (IndexEqual1 != -1)
                {
                    string TempIndex = SelectIndexForMultipleMappingStringLine.Substring(IndexEqual1 + 1);
                    Int32.TryParse(TempIndex, out SelectIndexForMultipleMappingString);
                }
                }

                string[] Temp = TTKFileName.Split('-');
                TTKFileName = Temp[0];

                //To handle more than 1 ttk file
                string[] TTKFileList = TTKFileName.Split(',');

                LocalizationStatus = Temp[1];

                NewLogObj.WriteLogFile(LogFilePath, "TTKFileName  " + TTKFileName, "info");
                MapObj.WriteMapperInputFile(Inputpattern);
                MapObj.GenerateMapping(TTKFileList);
               // MapObj.GenerateMappingModified(TTKFileList);
                string FilePath = Directory.GetCurrentDirectory() + "//Inputs//Mapped.txt";
                if(!File.Exists(FilePath))
                {
                    NewLogObj.WriteLogFile(LogFilePath, FilePath+" does not exits", "fail");
                   // NewFileObj.ExitTestEnvironment();
                }
                
                string SystemLocale = NewGenericObj.GetSystemLocale();
                string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
                string TTKHotKeyPatternSpecifier = FileObj.GetInputPattern(InputFileName, "TTKHotKeyPatternSpecifier");
                for (int i = 0; i < Inputpattern.Count; i++)
                {
                    //if (Regex.IsMatch(Inputpattern[i], TTKHotKeyPatternSpecifier, RegexOptions.CultureInvariant))
                    //{
                    //    Inputpattern[i] = Regex.Replace(Inputpattern[i], TTKHotKeyPatternSpecifier, "");
                    //}
                    if (SelectIndexForMultipleMappingString == 0)
                    {
                        LocalizedPttern = NewFileObj.SearchFileForPattern(FilePath, Inputpattern[i], 1, LogFilePath);
                    }
                    else if (SelectIndexForMultipleMappingString > 1)
                    {
                        LocalizedPttern = NewFileObj.SearchFileForSamePatternAppearinMultipleTimes(FilePath, Inputpattern[i], SelectIndexForMultipleMappingString, 1, LogFilePath);
                    }
                    if (LocalizedPttern == null)
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "LocalizedPttern is null correspondng to " + Inputpattern[i], "warn");

                       // FileObj.ExitTestEnvironment();
                    }
                    else
                    {
                        int IndexEqual = LocalizedPttern.IndexOf("=");
                        //For patterns which are localised, get the localised vals else get the EN vals
                        if (IndexEqual != -1)
                        {
                            LocalizedPttern = LocalizedPttern.Substring(IndexEqual + 1);
                        }

                        ////Removing the hotkey pattern if it exists
                        //Generic NewGenericObj = new Generic();

                        //string SystemLocale = NewGenericObj.GetSystemLocale();
                        //string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
                        //string TTKHotKeyPatternSpecifier = FileObj.GetInputPattern(InputFileName, "TTKHotKeyPatternSpecifier");
                        if (Regex.IsMatch(LocalizedPttern, TTKHotKeyPatternSpecifier, RegexOptions.CultureInvariant))
                        {
                            LocalizedPttern = Regex.Replace(LocalizedPttern, TTKHotKeyPatternSpecifier, "");
                            NewLogObj.WriteLogFile(LogFilePath, "LocalizedPttern  " + LocalizedPttern, "info");
                        }

                        NewLogObj.WriteLogFile(LogFilePath, "Localized string for  " + Inputpattern + " " + LocalizedPttern, "info");
                        LocalizedMappedPatternList.Add(LocalizedPttern);

                        if (Regex.IsMatch(LocalizationStatus, "Partial", RegexOptions.IgnoreCase))
                        {
                            LocEnStringMappingForPartialLocalization[LocalizedPttern] = Inputpattern[i];
                        }
                    }
                   
                }

                if (Regex.IsMatch(LocalizationStatus, "partial", RegexOptions.IgnoreCase))
                {
                    KeyWordAPIs KeywordApiObj = new KeyWordAPIs();
                   
                    PartiallyLocalized = 1;
                }
                return LocalizedMappedPatternList;
            }
            return null;
        }

        public Dictionary<String, string> GetLocEnStringMappingForPartialLocalization()
        {
            return LocEnStringMappingForPartialLocalization;
        }
        public int GetPartialLocalizationStatus()
        {
            return PartiallyLocalized;
        }
    }
}

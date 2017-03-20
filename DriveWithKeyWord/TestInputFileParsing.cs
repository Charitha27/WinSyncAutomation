using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

using LoggerCollection;
using FileOperationsCollection;
using KeyWordAPIsCollection;
using MapperCollection;
using GenericCollection;

namespace TestInputFileParsingCollection
{
    public class TestInputFileParsing
    {
       // public string[] KeyWordList = {};
        List<string> KeyWordList = new List<string>();



        //Reads the test input file at the start of the test
        //Checks all the lines and will through an error if format of any of the line is incorrect
        public void ValidateTestInputFile(string TestFileName)
        {

            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            //NewLogObj.WriteLogFile(LogFilePath, "ValidateTestInputFile", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "===================", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "TestFileName " + TestFileName, "info");
            string TestFilePath = CurrentDirPath + "\\TestFiles\\" + TestFileName;
            //NewLogObj.WriteLogFile(LogFilePath, "TestFilePath " + TestFilePath, "info");

            if (!File.Exists(TestFilePath))
            {
                NewLogObj.WriteLogFile(LogFilePath, "Test input File " + TestFileName +" does not exist", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();

            }

            string[] InputLines = System.IO.File.ReadAllLines(TestFilePath);
            //foreach (string line in InputLines)
            for(int LineNum=0;LineNum<InputLines.Count();LineNum++)
            {
                // if(!line.StartsWith("#"))
                if(!InputLines[LineNum].StartsWith("#")&& !string.IsNullOrEmpty(InputLines[LineNum]))
                {
                    ValidateInputLineFormat(InputLines[LineNum]);
                    ValidateInputLinesWithKeyWordFormat(InputLines[LineNum], LineNum);
                }
            }
            

        }

        public int ValidateInputLinesWithKeyWordFormat(string InputLine,int LineNum)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            //NewLogObj.WriteLogFile(LogFilePath, "ValidateInputLinesWithKeyWordFormat", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "================================", "info");

            FileOperations FileObj = new FileOperations();
            //Prepare the list of keywords
            PrepareKeywordList();
            Dictionary<string, string> InputLineParamValueMapDict = new Dictionary<string, string>();
            Dictionary<string, string> KeywordLineParamValueMapDict = new Dictionary<string, string>(); 
            int Matched = 0;

            foreach (string Keyword in KeyWordList)
            {
                if (Keyword.StartsWith("#")|| string.IsNullOrEmpty(Keyword))
                {
                    continue;
                }

                int InputLineIndex = InputLine.IndexOf("(");
                string InputLineCommandName = InputLine.Substring(0, InputLineIndex);
                
                int KeywordCmdNameIndex = Keyword.IndexOf("(");
                string KeywordCmdName = Keyword.Substring(0, KeywordCmdNameIndex);

                if (string.Compare(InputLineCommandName, KeywordCmdName) == 0)
                {
                    //NewLogObj.WriteLogFile(LogFilePath, "Comparing Keywordline " + Keyword + " with InputLine " + InputLine, "info");
                    int KeywordLineNumParams = CountStringOccurrences(Keyword, ",");
                   // KeywordLineNumParams = KeywordLineNumParams + 1;

                    int InputNumParams = CountStringOccurrences(InputLine, ",");
                    //InputNumParams = InputNumParams + 1;
                    //NewLogObj.WriteLogFile(LogFilePath, "InputNumParams - " + InputNumParams + " KeywordLineNumParams " + KeywordLineNumParams, "info");
                    int Unmatched = 0;
                    if (InputNumParams == KeywordLineNumParams)
                    {
                        InputLineParamValueMapDict = GetParamValueMap(InputLine);
                        KeywordLineParamValueMapDict = GetParamValueMap(Keyword);
                        Dictionary<string, string>.KeyCollection InputLineParams = InputLineParamValueMapDict.Keys;
                        Dictionary<string, string>.KeyCollection KeywordLineParams = KeywordLineParamValueMapDict.Keys;
                        Dictionary<string, string>.ValueCollection InputValueCollection = InputLineParamValueMapDict.Values;
                        //Check if any of the values have null values
                       if (InputValueCollection.Contains(""))
                        //if (InputValueCollection.Contains(null))
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "One of the Value is found as null in InputLine " + InputLine+" line num "+LineNum, "fail");
                            FileObj.ExitTestEnvironment();
                            return 0;
                        }
                        if(InputLineParams.Count()==KeywordLineParams.Count())
                        {
                            foreach (string InputKey in InputLineParams)
                            {
                                if (!KeywordLineParams.Contains(InputKey))
                                {
                                    Unmatched = 1;
                                    break;

                                }
                                
                            }
                            if (Unmatched == 0)
                            {
                                Matched = 1;
                                return 1;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                       // NewLogObj.WriteLogFile(LogFilePath, "KeywordLineNumParams " + KeywordLineNumParams + " InputNumParams " + InputNumParams + " does not match", "info");
                        continue;
                    }
                }
              
            }
            if (Matched == 0)
            {
                NewLogObj.WriteLogFile(LogFilePath, " InputLine " + InputLine + "at linenum "+LineNum+" is not  in proper format", "fail");
                //FileObj.FinishCurrentTest("test");
                FileObj.ExitTestEnvironment();
                return 0;

            }
            return 0;

       }

        public void ValidateInputLineFormat(String InputLine)
        {
            //CheckIfElementExists(ReadName="EN Name",TimeOutInSec="Secs",TerminateOnFailure="yes or no")
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            //NewLogObj.WriteLogFile(LogFilePath, "ValidateInputLineFormat", "info");
            //NewLogObj.WriteLogFile(LogFilePath, "========================", "info");

            FileOperations FileObj = new FileOperations();
            try
            {
                //if (InputLine.Trim().Contains("capturescreenshot")) return;

                //Check if the synatx is proper
                InputLine = InputLine.Trim();
                string LastChar = InputLine.Substring(InputLine.Length - 1, 1);
                if (!(string.Compare(LastChar, ")") == 0))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Line Not ending with braces " + InputLine, "fail");
                    //FileObj.ExitTestEnvironment();
                    FileObj.ExitTestEnvironment(1);
                }
                int ParamStartBracesIndex = InputLine.IndexOf("(");
                int ParamEqualIndex = InputLine.IndexOf("=");
                string Temp = InputLine.Substring(ParamStartBracesIndex + 1, ParamEqualIndex - ParamStartBracesIndex - 1);
                //Shud be alphanumeric after "(" till "="
                Regex AlphaNumericExp = new Regex(@"[a-z]|[A-Z]|[0-9]");
                if (!AlphaNumericExp.IsMatch(Temp))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invalid chars in 1st param " + Temp + " of line " + InputLine, "fail");
                    FileObj.ExitTestEnvironment(1);
                }
                //After "=" shud be """
                string CharafterEqual = InputLine.Substring(ParamEqualIndex + 1, 1);
                if (!(string.Compare(CharafterEqual, "\"") == 0))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Invalid char found after = in 1st param of line " + InputLine, "fail");
                    FileObj.ExitTestEnvironment(1);
                }
                //Splice the string to find position of next quotes
                string TempinputLine = InputLine;
                Temp = TempinputLine.Substring(ParamEqualIndex + 2);

                int QuotesPosition = Temp.IndexOf("\"");

                //find no of params

                int InputNumParams = CountStringOccurrences(InputLine, ",");
                if (InputNumParams == 0)
                {
                    int ClosingBracesPosition = Temp.IndexOf(")");
                    String CharBforClosingBraces = Temp.Substring(ClosingBracesPosition - 1, 1);
                    if (!(string.Compare(CharBforClosingBraces, "\"") == 0))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Values are not properly quoted in " + InputLine, "fail");
                        FileObj.ExitTestEnvironment(1);
                    }

                }

                for (int i = 0; i < InputNumParams; i++)
                {
                    int ParamCommaIndex = TempinputLine.IndexOf(",");
                    if (ParamCommaIndex == -1)
                    {
                        ParamCommaIndex = TempinputLine.IndexOf(")");
                    }
                    string CharBforComma = TempinputLine.Substring(ParamCommaIndex - 1, 1);
                    //Bfore "," shud be ""
                    if (!(string.Compare(CharBforComma, "\"") == 0))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Values are not properly quoted in  " + InputLine, "fail");
                        FileObj.ExitTestEnvironment(1);
                    }
                    TempinputLine = TempinputLine.Substring(ParamCommaIndex + 1);
                    int ParamEqualIndex1 = TempinputLine.IndexOf("=");
                    String ParamName = TempinputLine.Substring(0, ParamEqualIndex1);
                    if (!AlphaNumericExp.IsMatch(Temp))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid chars  paramname " + ParamName + " of line " + InputLine, "fail");
                        FileObj.ExitTestEnvironment(1);
                    }
                    string CharAfterEqual = TempinputLine.Substring(ParamEqualIndex1 + 1, 1);
                    if (!(string.Compare(CharAfterEqual, "\"") == 0))
                    {
                        NewLogObj.WriteLogFile(LogFilePath, "Invalid char found after = of line " + InputLine, "fail");
                        FileObj.ExitTestEnvironment(1);
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception "+ Ex.ToString()+" while processing line " + InputLine, "fail");
                FileObj.ExitTestEnvironment(1);
            }

            
        }
        //public void ValidateInputLinesWithKeyWordFormatOld(string InputLine)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();
        //    string CurrentDirPath = Directory.GetCurrentDirectory();
        //    NewLogObj.WriteLogFile(LogFilePath, "ValidateInputLinesWithKeyWordFormat", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "================================", "info");

        //    FileOperations FileObj = new FileOperations();
        //    //Prepare the list of keywords
        //    PrepareKeywordList();
        //    Dictionary<string, string> InputLineParamValueMapDict = new Dictionary<string, string>();
        //    Dictionary<string, string> KeywordLineParamValueMapDict = new Dictionary<string, string>();
        //    int Matched = 0;
        //    foreach (string Keyword in KeyWordList)
        //    {
        //        if (Keyword.StartsWith("#") || string.IsNullOrEmpty(Keyword))
        //        {
        //            continue;
        //        }
        //        int KeywordCmdNameIndex = Keyword.IndexOf("(");
        //        string KeywordCmdName = Keyword.Substring(0, KeywordCmdNameIndex);

        //        int InputLineIndex = InputLine.IndexOf("(");
        //        string InputLineCommandName = InputLine.Substring(0, InputLineIndex);

        //        if (string.Compare(InputLineCommandName, KeywordCmdName) == 0)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, "Comparing Keywordline " + Keyword + " with InputLine " + InputLine, "info");
        //            int KeywordLineNumParams = CountStringOccurrences(Keyword, ",");
        //            // KeywordLineNumParams = KeywordLineNumParams + 1;

        //            int InputNumParams = CountStringOccurrences(InputLine, ",");
        //            //InputNumParams = InputNumParams + 1;
        //            NewLogObj.WriteLogFile(LogFilePath, "InputNumParams - " + InputNumParams + " KeywordLineNumParams " + KeywordLineNumParams, "info");

        //            if (InputNumParams == KeywordLineNumParams)
        //            {
        //                //Split and check for each argmt
        //                //WaitWindow(WindowName="NameoftheWindow",TimeOutInSec="Secs",TerminateStatus="Status 1 or 0")
        //                int Unmatched = 0;
        //                int IndexOfEqual = InputLine.IndexOf("=");
        //                string InputLineParam1 = InputLine.Substring(InputLineIndex + 1, IndexOfEqual - InputLineIndex);
        //                int IndexOfEqual1 = Keyword.IndexOf("=");
        //                string KeyWordLineParam1 = Keyword.Substring(KeywordCmdNameIndex + 1, IndexOfEqual1 - KeywordCmdNameIndex);
        //                NewLogObj.WriteLogFile(LogFilePath, "KeyWordLineParam1 - " + KeyWordLineParam1 + " InputLineParam1 " + InputLineParam1, "info");
        //                if (string.Compare(InputLineParam1, KeyWordLineParam1) == 0)
        //                {
        //                    int InputEqualIndex = IndexOfEqual;
        //                    if (InputNumParams != 0)
        //                    {
        //                        InputLine = InputLine.Substring(InputEqualIndex + 1);
        //                        int ParamCounter = 0;
        //                        string[] InputLineParamList = { };
        //                        string[] KeywordLineParamList = { };

        //                        for (ParamCounter = 1; ParamCounter <= InputNumParams; ParamCounter++)
        //                        {
        //                            //Params will be b/w , & =
        //                            int InputCommaIndex = InputLine.IndexOf(",");
        //                            InputEqualIndex = InputLine.IndexOf("=");
        //                            string InputParam = InputLine.Substring(InputCommaIndex + 1, InputEqualIndex - InputCommaIndex);
        //                            int KeywordLineCommaIndex = InputLine.IndexOf(",");
        //                            int KeywordLineEqualIndex = InputLine.IndexOf("=");
        //                            string KeywordLineParam = InputLine.Substring(InputCommaIndex + 1, KeywordLineEqualIndex - KeywordLineCommaIndex);
        //                            if (string.Compare(KeywordLineParam, InputParam) != 0)
        //                            {
        //                                NewLogObj.WriteLogFile(LogFilePath, "Params does not match for lines.Keyword line - " + Keyword + " with InputLine " + InputLine, "info");
        //                                Unmatched = 1;
        //                                break;
        //                            }
        //                            InputLine = InputLine.Substring(InputEqualIndex + 1);

        //                        }
        //                        //Go to the next lie
        //                        if (Unmatched == 1)
        //                        {
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            NewLogObj.WriteLogFile(LogFilePath, "InputLine " + InputLine + " is in proper format", "info");
        //                            Matched = 1;
        //                            break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        NewLogObj.WriteLogFile(LogFilePath, "InputLine " + InputLine + " is in proper format", "info");
        //                        Matched = 1;
        //                        break;
        //                    }
        //                }
        //                else
        //                {
        //                    NewLogObj.WriteLogFile(LogFilePath, "No of Params does not match for lines.Keyword line - " + Keyword + " with InputLine " + InputLine, "info");
        //                    continue;

        //                }
        //            }
        //            else
        //            {
        //                NewLogObj.WriteLogFile(LogFilePath, "KeywordLineNumParams " + KeywordLineNumParams + " InputNumParams " + InputNumParams + " does not match", "info");
        //                continue;
        //            }
        //        }

        //    }
        //    if (Matched == 0)
        //    {
        //        NewLogObj.WriteLogFile(LogFilePath, " InputLine " + InputLine + " is not  in proper format", "fail");
        //        FileObj.FinishCurrentTest("test");

        //    }

        //}

        public void ReadTestInputFileForExecution(string TestFileName)
        {

            Logger NewLogObj = new Logger();
           
            string CurrentDirPath = Directory.GetCurrentDirectory();
            
            string TestFilePath = CurrentDirPath + "\\TestFiles\\" + TestFileName;

            if (!File.Exists(TestFilePath))
            {
                NewLogObj.CreateLogFolder();
                string LogFilePath = NewLogObj.CreateLogFile();
                NewLogObj.WriteLogFile(LogFilePath, "Test input File " + TestFilePath + " does not exist", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();

            }

            string[] InputLines = System.IO.File.ReadAllLines(TestFilePath);
            KeyWordAPIs KeyWordApiObj = new KeyWordAPIs();
            foreach (string line in InputLines)
            {
                if ((!(line.StartsWith("#"))) && (!(string.IsNullOrEmpty(line)) && (!(string.IsNullOrWhiteSpace(line)) && line.Length>0)))
                {
                   string MappingIndexCaptureFile = Directory.GetCurrentDirectory() + "//SelectIndexForMultipleMappingString.txt";
                   if (File.Exists(MappingIndexCaptureFile))
                   {
                       File.Delete(MappingIndexCaptureFile);;
                   }
                    ///For handling strings that occur multiple times in ttk files
                    if(Regex.IsMatch(line,"selectindexformultiplemappingstring",RegexOptions.IgnoreCase))
                    {
                        string tempLine = line.ToLower();
                        int TempIndex = tempLine.IndexOf("selectindexformultiplemappingstring");
                        tempLine = tempLine.Substring(TempIndex);
                        TempIndex = tempLine.IndexOf("\"");
                        int tempIndex2 = tempLine.IndexOf(",");
                        string TempSelectIndexForMultipleMappingString = tempLine.Substring(TempIndex + 1, tempIndex2 - (TempIndex+2));
                        int SelectIndexForMultipleMappingString;
                        Int32.TryParse(TempSelectIndexForMultipleMappingString, out SelectIndexForMultipleMappingString);
                        
                        FileStream LF = File.Create(MappingIndexCaptureFile);
                        LF.Close();
                        StreamWriter SW = File.AppendText(MappingIndexCaptureFile);
                        SW.WriteLine(line);

                        SW.WriteLine("SelectIndexForMultipleMappingString =" + SelectIndexForMultipleMappingString);
                        SW.Close();
                        
                    }
                    KeyWordApiObj.InterpretInputLine(line);
                }
            }
        }
        public void PrepareKeywordList()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            //NewLogObj.WriteLogFile(LogFilePath, "PrepareKeywordList", "info");
           // NewLogObj.WriteLogFile(LogFilePath, "======================", "info");
            string KeywordListFileName = "KeyWordList.txt";
            string KeywordListFilePath = CurrentDirPath + "\\Inputs\\" + KeywordListFileName;
            //NewLogObj.WriteLogFile(LogFilePath, "KeywordListFilePath " + KeywordListFilePath, "info");
            if (!File.Exists(KeywordListFilePath))
            {
                NewLogObj.WriteLogFile(LogFilePath, "KeywordListFilePath " + KeywordListFilePath + " does not exist", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();

            }
            string[] KeyWordLines = System.IO.File.ReadAllLines(KeywordListFilePath);
            
            foreach (string KeywordLine in KeyWordLines)
            {
                //Check if it is a commented line
                if(!KeywordLine.StartsWith("#"))
                {
                    //KeyWordList[ListIndex] = KeywordLine;
                    KeyWordList.Add(KeywordLine);
                   // ListIndex++;
                }

            }


        }

        public int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        //First parameter will say if detect based on mapped name or autoid or cntrl type
        public string GetFirstParameterName(string InputLine)
        {
            //if (InputLine.Contains("capturescreenshot")) return string.Empty;
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            NewLogObj.WriteLogFile(LogFilePath, "GetFirstParameterName", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===================", "info");
            int IndexOfEqual = InputLine.IndexOf("=");
            int InputLineIndex = InputLine.IndexOf("(");
            string InputLineParam1 = InputLine.Substring(InputLineIndex + 1, IndexOfEqual - (InputLineIndex+1));
            NewLogObj.WriteLogFile(LogFilePath, "InputLine " + InputLine + " Param1 " + InputLineParam1, "info");
            return InputLineParam1;

        }

        public List<String> GetParamValueList(string InputLine)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            List<string> ParamList = new List<string>();

            NewLogObj.WriteLogFile(LogFilePath, "GetParamValueList", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===================", "info");

            NewLogObj.WriteLogFile(LogFilePath, "Param Values in line " + InputLine, "info");
            //Get the 1st parameter
            int IndexOfEqual = InputLine.IndexOf("=");
            int InputCommaIndex = InputLine.IndexOf(",");
            string InputLineParam1 = InputLine.Substring(IndexOfEqual + 1, InputCommaIndex - IndexOfEqual);
            //Value will be quoted. Remove the quotes
            InputLineParam1 = InputLineParam1.Substring(1, InputLineParam1.Length - 3);

            ParamList.Add(InputLineParam1);
            NewLogObj.WriteLogFile(LogFilePath, InputLineParam1, "info");
            //Check if line has only 1 parameter or more., by checking for num of commas
            int InputNumParams = CountStringOccurrences(InputLine, ",");
            if (InputNumParams != 0)
            {
                int ParamCounter = 0;
                for (ParamCounter = 1; ParamCounter <= InputNumParams; ParamCounter++)
                {
                    InputLine = InputLine.Substring(InputCommaIndex + 1);
                    //Params will be b/w , & =
                    IndexOfEqual = InputLine.IndexOf("=");
                    //For last parameter there wont be comma seperation
                    if (ParamCounter == InputNumParams)
                    {
                        InputCommaIndex = InputLine.IndexOf(")");
                    }
                    else
                    {
                        InputCommaIndex = InputLine.IndexOf(",");
                    }
                   
                    string InputParam = InputLine.Substring(IndexOfEqual + 1, InputCommaIndex - IndexOfEqual);
                    //Value will be quoted. Remove the quotes
                    InputParam = InputParam.Substring(1, InputParam.Length - 3);

                    ParamList.Add(InputParam);
                    NewLogObj.WriteLogFile(LogFilePath, InputParam, "info");
                }
            }
            return ParamList;

        }

        public Dictionary<string, string> GetLocalizedMappedPatternList(string InputLine)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            List<string> ParamList = new List<string>();
            List<string> ToBeMappedValues = new List<string>();
            List<string> ToBeMappedParam = new List<string>();
            List<string> MappedValues = new List<string>();
            List <string> ParamValueList=new List<string>();
            
            Dictionary<string, string> ParamValueMapDict = new Dictionary<string, string>();

            //To handle partially localized products
            Dictionary<string, string> LocStrToENMapDict = new Dictionary<string, string>(); 

            //string[] ParamValueList = { };
            List<int> ToBeMappedParamPosition = new List<int>();
            int ParamPosition = 0;

            NewLogObj.WriteLogFile(LogFilePath, "GetLocalizedMappedPatternList", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===========================", "info");
            //FindParentWindow(Name="XenCenter",TimeOutInSec="30",TerminateStatus="1") 
            NewLogObj.WriteLogFile(LogFilePath, "Param  in line " + InputLine, "info");
            //Get the 1st parameter
            int InputNumParams = CountStringOccurrences(InputLine, ",");
            //Initializing the param value list to null, so that can be filled later on
            for (int i = 0; i <= InputNumParams; i++)
            {
                ParamValueList.Add(null);
            }

            int InputCommaIndex = InputLine.IndexOf("(");
            int IndexOfEqual = InputLine.IndexOf("=");
            string InputLineParam1 = InputLine.Substring(InputCommaIndex + 1, IndexOfEqual - (InputCommaIndex+1));
            InputLineParam1= InputLineParam1.ToLower();

            //Get the corresponding value
            int IndexOfQuote1 = InputLine.IndexOf("\"");
            string TempInputLine = InputLine.Substring(IndexOfQuote1 + 1);
            int IndexOfQuote2 = TempInputLine.IndexOf("\"");
            IndexOfQuote2 = IndexOfQuote2 + IndexOfQuote1;
            string Value = InputLine.Substring(IndexOfQuote1 + 1, IndexOfQuote2 - IndexOfQuote1);
            if (Regex.IsMatch(InputLineParam1, "MappedNameNotInTTK", RegexOptions.IgnoreCase))
            {
                Mapper MapObj = new Mapper();
                string TTKFileName = MapObj.CheckIfProductLocalizedForCurrentLocale();
                if (TTKFileName == null)
                {
                    //Product is not localized
                    InputLineParam1 = Regex.Replace(InputLineParam1, "mappednamenotinttk", "");
                    InputLineParam1 = Regex.Replace(InputLineParam1, "MappedNameNotInTTK", "");
                    ParamValueMapDict[InputLineParam1] = Value;
                }
                else
                {
                    //Will haev to read the correspondong locale file, MappedNameNotInTTK_<Locale>.txt, get the value from file
                    Generic NewGenericObj = new Generic();
                    string SystemLocale = NewGenericObj.GetSystemLocale();
                    //NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                    string CurrentDir = Directory.GetCurrentDirectory();
                    string MappedNameNotInTTKFile = CurrentDir + "\\Inputs\\MappedNameNotInTTK_" + SystemLocale + ".txt";

                    //Read the file to get the value corresponding to the param supplied as MappedNameNotInTTK_
                    FileOperations FileObj = new FileOperations();
                    string MappedPatternNotInTTK = FileObj.GetInputPattern(MappedNameNotInTTKFile, Value);
                    NewLogObj.WriteLogFile(LogFilePath, "MappedPatternNotInTTK corresponding to value  " + Value + " is " + MappedPatternNotInTTK, "info");
                    ParamValueList[0] = (MappedPatternNotInTTK);
                    InputLineParam1 = Regex.Replace(InputLineParam1, "mappednamenotinttk", "");
                    InputLineParam1 = Regex.Replace(InputLineParam1, "MappedNameNotInTTK", "");
                    ParamValueMapDict[InputLineParam1] = MappedPatternNotInTTK;
                }

            }
            else if (Regex.IsMatch(InputLineParam1, "mapped", RegexOptions.CultureInvariant))
            {
                InputLineParam1=Regex.Replace(InputLineParam1, "mapped", "");
                InputLineParam1 = Regex.Replace(InputLineParam1, "Mapped", "");
                //Value has to be mapped before adding
                ToBeMappedValues.Add(Value);
                //Initializing ToBeMappedParamPosition
                ToBeMappedParamPosition.Add(-1);
                ToBeMappedParamPosition[ParamPosition] = 0;
                ToBeMappedParam.Add(null);
                ToBeMappedParam[ParamPosition] = InputLineParam1;
                ParamPosition++;
                
            }
            else if (Regex.IsMatch(InputLineParam1, "readrufield", RegexOptions.CultureInvariant))
            {
                InputLineParam1 = Regex.Replace(InputLineParam1, "readrufield", "");
                InputLineParam1 = Regex.Replace(InputLineParam1, "readrufield", "");
                Mapper MapObj = new Mapper();
                FileOperations FileObj = new FileOperations();
                string InputFileName;
                string TTKFileName = MapObj.CheckIfProductLocalizedForCurrentLocale();
                
                Generic NewGenericObj = new Generic();

                string SystemLocale = NewGenericObj.GetSystemLocale();
               // NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".xml";
                //NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName, "info");
                //}
                if (!File.Exists(InputFileName))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName + " does not exist. Exiting", "fail");

                    FileObj.ExitTestEnvironment();
                }
                //Get the actal param name exempting the keyword "readrufield"
                string InputLineParamToRead = InputLineParam1.Substring(11);
                //Value = FileObj.GetInputPattern(InputFileName, InputLineParamToRead);
                Value = ReadRandomUnicodeGeneratorFile(InputFileName, Value);

                if (Value == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to read the value of InputLineParamToRead " + InputLineParamToRead + " from InputFileName." + InputFileName + " Exiting", "fail");
                    FileObj.ExitTestEnvironment();
                }
                //else
                //{
                //    if (Value.StartsWith("~"))
                //    {
                //        Value = Value.Replace("~", "");
                //    }
                //    if (Value.EndsWith("~"))
                //    {
                //        Value = Value.Replace("~", "");
                //    }
            
                //}
                ParamValueList[0] = (Value);
                //ParamValueMapDict[InputLineParam1] = Value;
                ParamValueMapDict[InputLineParam1] = Value;


            }
                //Where inputs have to be read from inputs file
            //Input param format = 'ReadTextToType="PoolName"', prefixed with keyword "Read" will make to read from input file
            else if (Regex.IsMatch(InputLineParam1, "read", RegexOptions.CultureInvariant))
            {
                InputLineParam1 = Regex.Replace(InputLineParam1, "read", "");
                InputLineParam1 = Regex.Replace(InputLineParam1, "Read", "");
                Mapper MapObj = new Mapper();
                FileOperations FileObj = new FileOperations();
                string InputFileName;
                string TTKFileName = MapObj.CheckIfProductLocalizedForCurrentLocale();
                //if (TTKFileName == null)
                //{
                //    //Product is not localized
                //    InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs.txt";
                //    NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName, "info");
                //}
                //else
                //{
                    //Check if input file is there
                Generic NewGenericObj = new Generic();
                    
                string SystemLocale = NewGenericObj.GetSystemLocale();
                //NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
               // NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName, "info");
                //}
                if (!File.Exists(InputFileName))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName + " does not exist. Exiting", "fail");

                    FileObj.ExitTestEnvironment();
                }
                //Get the actal param name exempting the keyword "read"
                string InputLineParamToRead = InputLineParam1.Substring(4);
                //Value = FileObj.GetInputPattern(InputFileName, InputLineParamToRead);
                Value = FileObj.GetInputPattern(InputFileName, Value);
                if (Value == null)
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Unable to read the value of InputLineParamToRead " + InputLineParamToRead + " from InputFileName." + InputFileName + " Exiting", "fail");
                    FileObj.ExitTestEnvironment();
                }
                ParamValueList[0] = (Value);
                //ParamValueMapDict[InputLineParam1] = Value;
                ParamValueMapDict[InputLineParam1] = Value;
                

            }
            else
            {
                ParamValueList[0]=(Value);
                ParamValueMapDict[InputLineParam1] = Value;
            }

            //ParamList.Add(InputLineParam1);
            NewLogObj.WriteLogFile(LogFilePath, InputLineParam1, "info");
            //Check if line has only 1 parameter or more., by checking for num of commas
           // FindParentWindow(Name="XenCenter",TimeOutInSec="30",TerminateStatus="1") 
            if (InputNumParams != 0)
            {
                int ParamCounter = 0;
                for (ParamCounter = 1; ParamCounter <= InputNumParams; ParamCounter++)
                {
                    InputCommaIndex = InputLine.IndexOf(",");
                    InputLine = InputLine.Substring(InputCommaIndex + 1);
                    //Params will be b/w , & =
                    IndexOfEqual = InputLine.IndexOf("=");
                    //For last parameter there wont be comma seperation
                    
                    //InputCommaIndex = InputLine.IndexOf(",");
                    
                    //string InputParam = InputLine.Substring(InputCommaIndex + 1, IndexOfEqual - (InputCommaIndex+1));
                    string InputParam = InputLine.Substring(0, IndexOfEqual);
                    InputParam = InputParam.ToLower();
                    IndexOfQuote1 = InputLine.IndexOf("\"");
                    TempInputLine = InputLine.Substring(IndexOfQuote1 + 1);
                    IndexOfQuote2 = TempInputLine.IndexOf("\"");
                    IndexOfQuote2 = IndexOfQuote2 + IndexOfQuote1;
                    Value = InputLine.Substring(IndexOfQuote1 + 1, IndexOfQuote2 - IndexOfQuote1);

                    if (Regex.IsMatch(InputParam, "MappedNameNotInTTK", RegexOptions.IgnoreCase))
                    {
                        Mapper MapObj = new Mapper();
                        string TTKFileName = MapObj.CheckIfProductLocalizedForCurrentLocale();
                        if (TTKFileName == null)
                        {
                            InputLineParam1 = Regex.Replace(InputLineParam1, "mappednamenotinttk", "");
                            InputLineParam1 = Regex.Replace(InputLineParam1, "MappedNameNotInTTK", "");
                            //Product is not localized
                            ParamValueMapDict[InputParam] = Value;
                        }
                        else
                        {
                            //Will haev to read the correspondong locale file, MappedNameNotInTTK_<Locale>.txt, get the value from file
                            Generic NewGenericObj = new Generic();
                            string SystemLocale = NewGenericObj.GetSystemLocale();
                            NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                            string CurrentDir = Directory.GetCurrentDirectory();
                            string MappedNameNotInTTKFile = CurrentDir + "\\Inputs\\MappedNameNotInTTK_" + SystemLocale + ".txt";

                            //Read the file to get the value corresponding to the param supplied as MappedNameNotInTTK_
                            FileOperations FileObj = new FileOperations();
                            string MappedPatternNotInTTK = FileObj.GetInputPattern(MappedNameNotInTTKFile, Value);
                            NewLogObj.WriteLogFile(LogFilePath, "MappedPatternNotInTTK corresponding to value  " + Value + " is " + MappedPatternNotInTTK, "info");
                            ParamValueList[0] = (MappedPatternNotInTTK);
                            InputParam = Regex.Replace(InputParam, "mappednamenotinttk", "");
                            InputParam = Regex.Replace(InputParam, "mappednamenotinttk", "");
                            ParamValueMapDict[InputParam] = MappedPatternNotInTTK;
                        }

                    }
                    //Check if input param has the name mapped in it and write to in file
                    else if (Regex.IsMatch(InputParam, "mapped", RegexOptions.CultureInvariant))
                    {
                        InputParam = Regex.Replace(InputParam, "mapped", "");
                        InputParam = Regex.Replace(InputParam, "Mapped", "");
                        ToBeMappedValues.Add(Value);
                        //Initializing ToBeMappedParamPosition
                        ToBeMappedParamPosition.Add(-1);
                        ToBeMappedParamPosition[ParamPosition] = ParamCounter;
                        ToBeMappedParam.Add(null);
                        ToBeMappedParam[ParamPosition] = InputParam;
                        ParamPosition++;

                    }
                    else if (Regex.IsMatch(InputParam, "readrufield", RegexOptions.CultureInvariant))
                    {
                        InputParam = Regex.Replace(InputParam, "readrufield", "",RegexOptions.IgnoreCase);
                        //InputParam = Regex.Replace(InputParam, "Readrufield", "");
                        Mapper MapObj = new Mapper();
                        FileOperations FileObj = new FileOperations();
                        string InputFileName;
                        string TTKFileName = MapObj.CheckIfProductLocalizedForCurrentLocale();
                        
                        //Check if input file is there
                        Generic NewGenericObj = new Generic();
                        string SystemLocale = NewGenericObj.GetSystemLocale();
                        //NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                        InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".xml";
                        //NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName, "info");
                        //}
                        if (!File.Exists(InputFileName))
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName + " does not exist. Exiting", "fail");

                            FileObj.ExitTestEnvironment();
                        }

                        string InputLineParamToRead = Value;
                        //Value = FileObj.GetInputPattern(InputFileName, InputLineParamToRead);
                        Value = ReadRandomUnicodeGeneratorFile(InputFileName, InputLineParamToRead);
                        if (Value == null)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to read the value of InputLineParamToRead " + InputLineParamToRead + " from InputFileName." + InputFileName + " Exiting", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                        //else
                        //{
                        //    if (Value.StartsWith("~"))
                        //    {
                        //        Value = Value.Replace("~", "");
                        //    }
                        //    if (Value.EndsWith("~"))
                        //    {
                        //        Value = Value.Replace("~", "");
                        //    }
            
                        //}
                        //Removing the "read"
                        ParamValueList[ParamCounter] = (Value);
                        ParamValueMapDict[InputParam] = Value;

                    }
                    //Where inputs have to be read from inputs file
                    //Input param format = "ReadTextToType="PoolName"", prefixed with keyword "Read" will make to read from input file
                    else if (Regex.IsMatch(InputParam, "read", RegexOptions.CultureInvariant))
                    {
                        InputParam = Regex.Replace(InputParam, "read", "");
                        InputParam = Regex.Replace(InputParam, "Read", "");
                        Mapper MapObj = new Mapper();
                        FileOperations FileObj = new FileOperations();
                        string InputFileName;
                        string TTKFileName = MapObj.CheckIfProductLocalizedForCurrentLocale();
                        //if (TTKFileName == null)
                        //{
                        //    //Product is not localized
                        //    InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs.txt";
                        //    NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName, "info");
                        ////}
                        //else
                        //{
                            //Check if input file is there
                        Generic NewGenericObj = new Generic();
                        string SystemLocale = NewGenericObj.GetSystemLocale();
                       // NewLogObj.WriteLogFile(LogFilePath, "SystemLocale " + SystemLocale, "info");
                        InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + SystemLocale + ".txt";
                       // NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName, "info");
                        //}
                        if (!File.Exists(InputFileName))
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "InputFileName " + InputFileName + " does not exist. Exiting", "fail");

                            FileObj.ExitTestEnvironment();
                        }
                        
                        string InputLineParamToRead = Value;
                        Value = FileObj.GetInputPattern(InputFileName, InputLineParamToRead);
                        if (Value == null)
                        {
                            NewLogObj.WriteLogFile(LogFilePath, "Unable to read the value of InputLineParamToRead " + InputLineParamToRead + " from InputFileName." + InputFileName + " Exiting", "fail");
                            FileObj.ExitTestEnvironment();
                        }
                        //Removing the "read"
                        ParamValueList[ParamCounter] = (Value);
                        ParamValueMapDict[InputParam] = Value;

                    }
                    else
                    {
                        ParamValueList[ParamCounter] = Value;
                        ParamValueMapDict[InputParam] = Value;
                    }
                    //ParamList.Add(InputParam);
                    NewLogObj.WriteLogFile(LogFilePath, InputParam, "info");
                }
            }
            //Genertae mapping for the value in ToBeMappedValues
            if (ToBeMappedValues.Count > 0)
            {
                Mapper MapObj = new Mapper();
                MappedValues = MapObj.FindMappedPattern(ToBeMappedValues);
                NewLogObj.WriteLogFile(LogFilePath, "printing MappedValues", "info");
                foreach (string Val in MappedValues)
                {
                    NewLogObj.WriteLogFile(LogFilePath, Val, "info");
                    //foreach (int Position in ToBeMappedParamPosition)
                    //{
                    //    ParamValueList[Position] = Val;
                    //}
                }
                //Inserting the mapped values in right position
                for (int i = 0; i < MappedValues.Count; i++)
                {
                    ParamValueList[ToBeMappedParamPosition[i]] = MappedValues[i];
                    ParamValueMapDict[ToBeMappedParam[i]] = MappedValues[i];
                }
                if (ToBeMappedParam.Count > 0 && MappedValues.Count==0)
                {
                    for (int i = 0; i < ToBeMappedParam.Count; i++)
                    {
                        ParamValueList[ToBeMappedParamPosition[i]] = null;
                        ParamValueMapDict[ToBeMappedParam[i]] = null;
                    }
                }
                //return MappedValues;
            }
            return ParamValueMapDict;

        }

        public Dictionary<string, string> GetParamValueMap(string InputLine)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();

            List<string> ParamList = new List<string>();
            List<string> ParamValueList = new List<string>();

            Dictionary<string, string> ParamValueMapDict = new Dictionary<string, string>();

           // NewLogObj.WriteLogFile(LogFilePath, "GetParamValueMap", "info");
           // NewLogObj.WriteLogFile(LogFilePath, "===============", "info");
            //FindParentWindow(Name="XenCenter",TimeOutInSec="30",TerminateStatus="1") 
            //NewLogObj.WriteLogFile(LogFilePath, "Param  in line " + InputLine, "info");
            //Get the 1st parameter

            try
            {
                
                int InputNumParams = CountStringOccurrences(InputLine, ",");
                //Initializing the param value list to null, so that can be filled later on
                for (int i = 0; i <= InputNumParams; i++)
                {
                    ParamValueList.Add(null);
                }

                //if (InputLine.Contains("capturescreenshot"))
                //{
                //    return ParamValueMapDict;
                //}

                int InputCommaIndex = InputLine.IndexOf("(");
                int IndexOfEqual = InputLine.IndexOf("=");
                string InputLineParam1 = InputLine.Substring(InputCommaIndex + 1, IndexOfEqual - (InputCommaIndex + 1));
                InputLineParam1 = InputLineParam1.ToLower();

                //Get the corresponding value
                int IndexOfQuote1 = InputLine.IndexOf("\"");
                string TempInputLine = InputLine.Substring(IndexOfQuote1 + 1);
                int IndexOfQuote2 = TempInputLine.IndexOf("\"");
                IndexOfQuote2 = IndexOfQuote2 + IndexOfQuote1;
                string Value = InputLine.Substring(IndexOfQuote1 + 1, IndexOfQuote2 - IndexOfQuote1);
                ParamValueMapDict[InputLineParam1] = Value;


                //ParamList.Add(InputLineParam1);
               // NewLogObj.WriteLogFile(LogFilePath, InputLineParam1, "info");
                //Check if line has only 1 parameter or more., by checking for num of commas
                // FindParentWindow(Name="XenCenter",TimeOutInSec="30",TerminateStatus="1") 
                if (InputNumParams != 0)
                {
                    int ParamCounter = 0;

                    for (ParamCounter = 1; ParamCounter <= InputNumParams; ParamCounter++)
                    {

                        InputCommaIndex = InputLine.IndexOf(",");
                        InputLine = InputLine.Substring(InputCommaIndex + 1);
                        //Params will be b/w , & =
                        IndexOfEqual = InputLine.IndexOf("=");
                        //For last parameter there wont be comma seperation

                        //InputCommaIndex = InputLine.IndexOf(",");

                        //string InputParam = InputLine.Substring(InputCommaIndex + 1, IndexOfEqual - (InputCommaIndex+1));
                        string InputParam = InputLine.Substring(0, IndexOfEqual);
                        InputParam = InputParam.ToLower();
                        IndexOfQuote1 = InputLine.IndexOf("\"");
                        TempInputLine = InputLine.Substring(IndexOfQuote1 + 1);
                        IndexOfQuote2 = TempInputLine.IndexOf("\"");
                        IndexOfQuote2 = IndexOfQuote2 + IndexOfQuote1;
                        Value = InputLine.Substring(IndexOfQuote1 + 1, IndexOfQuote2 - IndexOfQuote1);
                        ParamValueMapDict[InputParam] = Value;
                    }
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Input line " + InputLine + "not in proper format. Exception - "+Ex.ToString(), "fail");

            }
            return ParamValueMapDict;

        }

        //public List<String> GetLocalizedMappedPatternListOld(string InputLine)
        //{
        //    Logger NewLogObj = new Logger();
        //    string LogFilePath = NewLogObj.GetLogFilePath();

        //    List<string> ParamList = new List<string>();
        //    List<string> ToBeMappedValues = new List<string>();
        //    List<string> MappedValues = new List<string>();
        //    List<string> ParamValueList = new List<string>();
        //    //string[] ParamValueList = { };
        //    List<int> ToBeMappedParamPosition = new List<int>();
        //    int ParamPosition = 0;

        //    NewLogObj.WriteLogFile(LogFilePath, "GetLocalizedMappedPatternList", "info");
        //    NewLogObj.WriteLogFile(LogFilePath, "===========================", "info");
        //    //FindParentWindow(Name="XenCenter",TimeOutInSec="30",TerminateStatus="1") 
        //    NewLogObj.WriteLogFile(LogFilePath, "Param  in line " + InputLine, "info");
        //    //Get the 1st parameter
        //    int InputNumParams = CountStringOccurrences(InputLine, ",");
        //    //Initializing the param value list to null, so that can be filled later on
        //    for (int i = 0; i <= InputNumParams; i++)
        //    {
        //        ParamValueList.Add(null);
        //    }

        //    int InputCommaIndex = InputLine.IndexOf("(");
        //    int IndexOfEqual = InputLine.IndexOf("=");
        //    string InputLineParam1 = InputLine.Substring(InputCommaIndex + 1, IndexOfEqual - (InputCommaIndex + 1));
        //    InputLineParam1 = InputLineParam1.ToLower();

        //    //Get the corresponding value
        //    int IndexOfQuote1 = InputLine.IndexOf("\"");
        //    string TempInputLine = InputLine.Substring(IndexOfQuote1 + 1);
        //    int IndexOfQuote2 = TempInputLine.IndexOf("\"");
        //    IndexOfQuote2 = IndexOfQuote2 + IndexOfQuote1;
        //    string Value = InputLine.Substring(IndexOfQuote1 + 1, IndexOfQuote2 - IndexOfQuote1);
        //    if (Regex.IsMatch(InputLineParam1, "mapped", RegexOptions.CultureInvariant))
        //    {
        //        //Value has to be mapped before adding
        //        ToBeMappedValues.Add(Value);
        //        //Initializing ToBeMappedParamPosition
        //        ToBeMappedParamPosition.Add(-1);
        //        ToBeMappedParamPosition[ParamPosition] = 0;
        //        ParamPosition++;
        //    }
        //    else
        //    {
        //        ParamValueList[0] = (Value);
        //    }

        //    //ParamList.Add(InputLineParam1);
        //    NewLogObj.WriteLogFile(LogFilePath, InputLineParam1, "info");
        //    //Check if line has only 1 parameter or more., by checking for num of commas

        //    if (InputNumParams != 0)
        //    {
        //        int ParamCounter = 0;
        //        for (ParamCounter = 1; ParamCounter <= InputNumParams; ParamCounter++)
        //        {
        //            InputCommaIndex = InputLine.IndexOf(",");
        //            InputLine = InputLine.Substring(InputCommaIndex + 1);
        //            //Params will be b/w , & =
        //            IndexOfEqual = InputLine.IndexOf("=");
        //            //For last parameter there wont be comma seperation

        //            //InputCommaIndex = InputLine.IndexOf(",");

        //            //string InputParam = InputLine.Substring(InputCommaIndex + 1, IndexOfEqual - (InputCommaIndex+1));
        //            string InputParam = InputLine.Substring(0, IndexOfEqual);
        //            InputParam = InputParam.ToLower();
        //            IndexOfQuote1 = InputLine.IndexOf("\"");
        //            TempInputLine = InputLine.Substring(IndexOfQuote1 + 1);
        //            IndexOfQuote2 = TempInputLine.IndexOf("\"");
        //            IndexOfQuote2 = IndexOfQuote2 + IndexOfQuote1;
        //            Value = InputLine.Substring(IndexOfQuote1 + 1, IndexOfQuote2 - IndexOfQuote1);
        //            //Check if input param has the name mapped in it and write to in file
        //            if (Regex.IsMatch(InputParam, "mapped", RegexOptions.CultureInvariant))
        //            {
        //                ToBeMappedValues.Add(Value);
        //                //Initializing ToBeMappedParamPosition
        //                ToBeMappedParamPosition.Add(-1);
        //                ToBeMappedParamPosition[ParamPosition] = ParamCounter;

        //            }
        //            else
        //            {
        //                ParamValueList[ParamCounter] = Value;
        //            }
        //            //ParamList.Add(InputParam);
        //            NewLogObj.WriteLogFile(LogFilePath, InputParam, "info");
        //        }
        //    }
        //    //Genertae mapping for the value in ToBeMappedValues
        //    if (ToBeMappedValues.Count > 0)
        //    {
        //        Mapper MapObj = new Mapper();
        //        MappedValues = MapObj.FindMappedPattern(ToBeMappedValues);
        //        NewLogObj.WriteLogFile(LogFilePath, "printing MappedValues", "info");
        //        foreach (string Val in MappedValues)
        //        {
        //            NewLogObj.WriteLogFile(LogFilePath, Val, "info");
        //            //foreach (int Position in ToBeMappedParamPosition)
        //            //{
        //            //    ParamValueList[Position] = Val;
        //            //}
        //        }
        //        //Inserting the mapped values in right position
        //        for (int i = 0; i < MappedValues.Count; i++)
        //        {
        //            ParamValueList[ToBeMappedParamPosition[i]] = MappedValues[i];
        //        }
        //        //return MappedValues;
        //    }
        //    return ParamValueList;

        //}


        public void ExecuteTestInputFile(string TestFileName)
        {

            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            string CurrentDirPath = Directory.GetCurrentDirectory();
            NewLogObj.WriteLogFile(LogFilePath, "ExecuteTestInputFile", "info");
            NewLogObj.WriteLogFile(LogFilePath, "===================", "info");
            NewLogObj.WriteLogFile(LogFilePath, "TestFileName " + TestFileName, "info");
            string TestFilePath = CurrentDirPath + "\\" + TestFileName;
            NewLogObj.WriteLogFile(LogFilePath, "TestFilePath " + TestFilePath, "info");

            if (!File.Exists(TestFilePath))
            {
                NewLogObj.WriteLogFile(LogFilePath, "Test input File " + TestFileName + " does not exist", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                FileOperations FileObj = new FileOperations();
                FileObj.ExitTestEnvironment();

            }

            string[] InputLines = System.IO.File.ReadAllLines(TestFilePath);
            foreach (string line in InputLines)
            {
                if (!line.StartsWith("#"))
                {
                    
                }
            }


        }

        public string SearchParamListForPattern(Dictionary<string, string> ParamValueMapDict,string Pattern)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            
            NewLogObj.WriteLogFile(LogFilePath, "SearchParamListForPattern", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=======================", "info");
            Dictionary<string, string>.KeyCollection keys = ParamValueMapDict.Keys;
            foreach (string key in keys)
            {
                if (Regex.IsMatch(key, Pattern,RegexOptions.IgnoreCase))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Pattern " + Pattern + " matched in key" + key, "info");
                    return key; ;
                }
            }
            NewLogObj.WriteLogFile(LogFilePath, "No key matched the Pattern " + Pattern , "warn");
            return null;

        }


        public void PrerequisitesValidation()
        {
            Logger NewLogObj = new Logger();
            string CurrentLocale = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            string InputFilePath = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + CurrentLocale+".txt";
            FileOperations FileObj = new FileOperations();

            //Check if input file exists
            if (!File.Exists(InputFilePath))
            {
                Console.WriteLine(InputFilePath+ " does not exist. Exiting");
                NewLogObj.CreateLogFolder();
                string LogFilePath = NewLogObj.CreateLogFile();
                NewLogObj.WriteLogFile(LogFilePath, "Test input File " + InputFilePath + " does not exist", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                
                FileObj.ExitTestEnvironment();
            }

            //Check if ttk file exists
            string TTKFilePath = Directory.GetCurrentDirectory() + "\\Inputs\\TTKFileList.txt";
            if (!File.Exists(TTKFilePath))
            {
                NewLogObj.CreateLogFolder();
                string LogFilePath = NewLogObj.CreateLogFile();
                NewLogObj.WriteLogFile(LogFilePath, "TTKFilePath " + TTKFilePath + " does not exist", "fail");
                NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                
                FileObj.ExitTestEnvironment();
            }
            StreamReader reader = new StreamReader(TTKFilePath);
            string line = string.Empty;
            while ((line = reader.ReadLine()) != null)
            {
                string[] Temp = line.Split('-');
                string Language = Temp[0];
                string TTKFiles = Temp[1]; ;
                if (Regex.IsMatch(CurrentLocale, Language, RegexOptions.IgnoreCase))
                {
                    string[] TTKFilesList = TTKFiles.Split(',');
                    //Check if all ttk fies exist
                    for (int i = 0; i < TTKFilesList.Length; i++)
                    {
                        string EachTTKFilePath = Directory.GetCurrentDirectory() + "\\Inputs\\"+TTKFilesList[i];
                        if(!File.Exists(EachTTKFilePath))
                        {
                            NewLogObj.CreateLogFolder();
                            string LogFilePath = NewLogObj.CreateLogFile();
                            NewLogObj.WriteLogFile(LogFilePath, "TTKFilePath " + EachTTKFilePath + " does not exist", "fail");
                            NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");
                            
                            FileObj.ExitTestEnvironment();

                        }

                    }
                }

            }

            //Check if guizard is present
            string InputFileName = Directory.GetCurrentDirectory() + "\\Inputs\\Inputs_" + CurrentLocale + ".txt";

            string GuizardLocation = FileObj.GetInputPattern(InputFileName, "GuiZardLocation");
            string GuiZardExeName = FileObj.GetInputPattern(InputFileName, "GuiZardExeName");

            if (!GuizardLocation.ToLower().Equals("no"))
            {
                if (!File.Exists(GuizardLocation + "\\" + GuiZardExeName))
                {
                    NewLogObj.CreateLogFolder();
                    string LogFilePath = NewLogObj.CreateLogFile();
                    NewLogObj.WriteLogFile(LogFilePath, "Guizard " + GuizardLocation + "\\" + GuiZardExeName + " does not exist", "fail");
                    NewLogObj.WriteLogFile(LogFilePath, "Exiting ...", "fail");

                    FileObj.ExitTestEnvironment();
                }
            }
        }

        public string ReadRandomUnicodeGeneratorFile(string XMLFilePath, string GetFieldName)
        {
            Logger NewLogObj = new Logger();
            FileOperations FileObj = new FileOperations();
            string LogFilePath = NewLogObj.GetLogFilePath();

            if (!File.Exists(XMLFilePath))
            {
                NewLogObj.WriteLogFile(LogFilePath, XMLFilePath + " does not exist. Exiting", "fail");
                FileObj.ExitTestEnvironment();
            }

            XmlReader reader = XmlReader.Create(XMLFilePath);
            while (reader.Read())
            {
                if (reader.IsStartElement() && string.Compare(reader.Name, "TestId") == 0)
                {
                    string TestID = reader["id"];
                    string FieldName;
                    string TestData;
                    reader.Read();
                    if (reader.IsStartElement() && string.Compare(reader.Name, "Field") == 0)
                    {
                        FieldName = reader.ReadString();

                        if (string.Compare(FieldName, GetFieldName) == 0)
                        {
                            while (string.Compare(reader.Name, "TestData") != 0)
                            {
                                reader.Read();
                            }
                            TestData = reader.ReadString();
                            NewLogObj.WriteLogFile(LogFilePath, "RUTestData coresponding to filedname "+GetFieldName +"is "+ TestData , "info");
                            return TestData;
                        }
                    }

                }
            }
            return null;
        }

        
    }
}

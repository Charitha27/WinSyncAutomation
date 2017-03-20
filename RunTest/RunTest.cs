using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TestInputFileParsingCollection;
using System.Windows.Automation;

namespace RunTest
{
    class RunTest
    {
        
        static void Main(string[] args)
        {
            TestInputFileParsing FileParseObj = new TestInputFileParsing();
         
            string CurrentFileName = Environment.GetCommandLineArgs()[1];
            Console.WriteLine("\n***Executing test " + CurrentFileName + "***\n");
            //FileParseObj.ValidateTestInputFile("AddServer.txt");
            FileParseObj.ReadTestInputFileForExecution(CurrentFileName);

            
        }
    }
}

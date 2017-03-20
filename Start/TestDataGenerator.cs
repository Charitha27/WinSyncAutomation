using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

using LoggerCollection;
using FileOperationsCollection;

namespace TestDataGeneratorCollection
{
    public class TestDataGenerator
    {
        public int GenerateRUFile()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            FileOperations FileObj = new FileOperations();

            Console.WriteLine("\n \n Generating Test Data Generator files. Please wait ...\n ");

            string CurrentLocale = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
            string OutPutFilePath = Directory.GetCurrentDirectory() + "\\TestGeneratorFiles\\Output_Inputs_" + CurrentLocale + ".xml";
            string OutPutFilePath1 = "C:\\Output_Inputs_" + CurrentLocale + ".xml";
            string FileNameAfterRename = "Inputs_" + CurrentLocale + ".xml";
            string DestinationFilePath = Directory.GetCurrentDirectory() + "\\Inputs\\" + FileNameAfterRename;
            if (File.Exists(OutPutFilePath))
            {
                File.Delete(OutPutFilePath);
            }
            if (File.Exists(OutPutFilePath1))
            {
                File.Delete(OutPutFilePath1);
            }
            if (File.Exists(DestinationFilePath))
            {
                File.Delete(DestinationFilePath);
            }
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            string InputFileName = "Inputs_" + CurrentLocale + ".xlsx";
            string InputFilePath = Directory.GetCurrentDirectory() + "\\TestGeneratorFiles\\Inputs_" + CurrentLocale + ".xlsx";

           
             string FileName = Directory.GetCurrentDirectory() + "\\TestGeneratorFiles\\TestDataGenerator.exe";
             string Command = FileName + " " + InputFilePath;
            
             System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + Command);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            Thread.Sleep(20000);
           // proc.WaitForExit();
            string result = proc.StandardOutput.ReadToEnd();
            NewLogObj.WriteLogFile(LogFilePath, "Result of execution of cmd " + Command + " : *" + result + "*", "info");
                       
            if (File.Exists(OutPutFilePath))
            {
                NewLogObj.WriteLogFile(LogFilePath, "Test generator file "+OutPutFilePath+" generated successfully", "info");
                FileObj.RenameFile(OutPutFilePath, DestinationFilePath);
                //FileObj.CopyFileToAFolder(OutPutFilePath, DestinationFilePath);
                return 1;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Test generator file " + OutPutFilePath + " generation failed", "warn");
            }

            if (File.Exists(OutPutFilePath1))
            {
                NewLogObj.WriteLogFile(LogFilePath, "Test generator file " + OutPutFilePath1 + " generated successfully", "info");
                FileObj.CopyFileToAFolder(OutPutFilePath1, DestinationFilePath);
                return 1;
            }
            else
            {
                NewLogObj.WriteLogFile(LogFilePath, "Test generator file " + OutPutFilePath1 + " generation failed", "warn");
            }
            NewLogObj.WriteLogFile(LogFilePath, "Test generator file generation failed. Trying to copy the default test data generator files", "warn");

            //Using the default test data generator files
            string DefaultTestDataGenFilePath = Directory.GetCurrentDirectory() + "\\TestGeneratorFiles\\DefaultTestdataGeneratorFiles" + InputFileName;
            if (File.Exists(DefaultTestDataGenFilePath))
            {
               
                FileObj.CopyFileToAFolder(DefaultTestDataGenFilePath, DestinationFilePath);
                if (File.Exists(DestinationFilePath))
                {
                    NewLogObj.WriteLogFile(LogFilePath, "Default Test generator file copied successfully", "warn");
                }
                else
                {
                    Console.WriteLine("****Test generator file generation failed. Exiting ****");
                    FileObj.ExitTestEnvironment();
                    return -1;
                }

            }
            else
            {
                Console.WriteLine("****Test generator file generation failed. Exiting ****");
                FileObj.ExitTestEnvironment();
                return -1;
            }
            return -1;
        }
        
    }
}

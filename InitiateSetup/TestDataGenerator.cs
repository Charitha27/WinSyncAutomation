using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace TestDataGeneratorCollection
{
    public class TestDataGenerator
    {
        public int GenerateRUFile()
        {
            
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
            //NewLogObj.WriteLogFile(LogFilePath, "Result of execution of cmd " + Command + " : *" + result + "*", "info");
                       
            if (File.Exists(OutPutFilePath))
            {
                Console.WriteLine("Test generator file "+OutPutFilePath+" generated successfully");
                RenameFile(OutPutFilePath, DestinationFilePath);
                //FileObj.CopyFileToAFolder(OutPutFilePath, DestinationFilePath);
                return 1;
            }
            else
            {
                Console.WriteLine("Test generator file " + OutPutFilePath + " generation failed");
            }

            if (File.Exists(OutPutFilePath1))
            {   
                Console.WriteLine("Test generator file " + OutPutFilePath1 + " generated successfully");
                CopyFileToAFolder(OutPutFilePath1, DestinationFilePath);
                return 1;
            }
            else
            {   
                Console.WriteLine("Test generator file " + OutPutFilePath1 + " generation failed");
            }
            
            Console.WriteLine("Test generator file generation failed. Trying to copy the default test data generator files");

            //Using the default test data generator files
            string DefaultTestDataGenFilePath = Directory.GetCurrentDirectory() + "\\TestGeneratorFiles\\DefaultTestdataGeneratorFiles" + InputFileName;
            if (File.Exists(DefaultTestDataGenFilePath))
            {
               
                CopyFileToAFolder(DefaultTestDataGenFilePath, DestinationFilePath);
                if (File.Exists(DestinationFilePath))
                {   
                    Console.WriteLine("Default Test generator file copied successfully");
                }
                else
                {
                    Console.WriteLine("****Test generator file generation failed. Exiting ****");
                   
                    return -1;
                }

            }
            else
            {
                Console.WriteLine("****Test generator file generation failed. Exiting ****");
                KillProcess("cmd.exe", null);
                Console.ReadLine();
                return -1;
            }
            return -1;
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
         public void KillProcess(string ProcessName, string LogFilePath)
         {
             
             try
             {
                 int IndexOfDot = ProcessName.IndexOf(".");
                 if (IndexOfDot != -1)
                 {
                     ProcessName = ProcessName.Substring(0, IndexOfDot);
                 }
                 Process[] pname = Process.GetProcessesByName(ProcessName);

                 if (pname.Length != 0)
                 {
                     Console.WriteLine("Killing process " + ProcessName);
                    
                     pname[0].Kill();
                 }
                 else
                 {
                     
                 }
             }
             catch
             {

             }
         }
        
    }
}

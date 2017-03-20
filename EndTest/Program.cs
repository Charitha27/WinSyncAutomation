using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace EndTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //Remove the rerun files
            string TestFolderPath = Directory.GetCurrentDirectory() + "//TestFiles";
            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(TestFolderPath);
            string ReRunFileNamePattern="^ReRun";
            foreach (System.IO.FileInfo file in directory.GetFiles())
            {
                if (Regex.IsMatch(file.Name, ReRunFileNamePattern))
                {
                    file.Delete();
                }
            }
        }
    }
}

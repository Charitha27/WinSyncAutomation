using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

using LoggerCollection;

namespace GlobalizationCollection
{
   public class Globalization
    {

        public string ReturnLocale()
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "ReturnLocale", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            string CurrentLocale = Thread.CurrentThread.CurrentCulture.Name;
            NewLogObj.WriteLogFile(LogFilePath, "CurrentLocale " + CurrentLocale, "info");
            return CurrentLocale;
        }

        public string LocaleBasedDateFormat(string Locale)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "LocaleBasedDateFormat", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            CultureInfo Culture = new CultureInfo(Locale);
            string shortDateFormatString = Culture.DateTimeFormat.LongDatePattern;
            NewLogObj.WriteLogFile(LogFilePath, "shortDateFormatString " + shortDateFormatString, "info");
            return shortDateFormatString;

        }
        public string LocaleBasedTimeFormat(string Locale)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            NewLogObj.WriteLogFile(LogFilePath, "LocaleBasedTimeFormat", "info");
            NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");
            CultureInfo Culture = new CultureInfo(Locale);
            string shortTimeFormatString = Culture.DateTimeFormat.LongTimePattern;
            NewLogObj.WriteLogFile(LogFilePath, "shortTimeFormatString " + shortTimeFormatString, "info");
            return shortTimeFormatString;

        }

       public int CompareDateFormat(string ExpectedDateFormat,string ActualDateFormat)
       {
           Logger NewLogObj = new Logger();
           string LogFilePath = NewLogObj.GetLogFilePath();
           NewLogObj.WriteLogFile(LogFilePath, "CompareDateFormat", "info");
           NewLogObj.WriteLogFile(LogFilePath, "=============================", "info");

           NewLogObj.WriteLogFile(LogFilePath, "Comapring ExpectedDateFormat " + ExpectedDateFormat + "ActualDateFormat " + ActualDateFormat, "info");
           if (ExpectedDateFormat.Length != ActualDateFormat.Length)
           {
               NewLogObj.WriteLogFile(LogFilePath, "Length of strings does not match", "fail");
               return -1;
           }
           //Find the delimiters
           //Gte the no of digits before ecah delimiter

           int ExpectedFirstDelimterIndex = -1;
           int ExpectedSecondDelimterIndex = -1;
           int ExpectedThirdDelimterIndex = -1;
           for (int i = 0; i < ExpectedDateFormat.Length; i++)
           {
               if (!(Char.IsDigit(ExpectedDateFormat[i])))
               {
                   if (ExpectedFirstDelimterIndex == -1)
                   {
                       ExpectedFirstDelimterIndex = i;
                   }
                   else if (ExpectedSecondDelimterIndex == -1)
                   {
                       ExpectedSecondDelimterIndex = i;
                   }
                   else if (ExpectedThirdDelimterIndex == -1)
                   {
                       ExpectedThirdDelimterIndex = i;
                   }
               }
           }

           NewLogObj.WriteLogFile(LogFilePath, "ExpectedDateFormat " + ExpectedDateFormat + "ExpectedFirstDelimterIndex " + ExpectedFirstDelimterIndex + "ExpectedSecondDelimterIndex " + ExpectedSecondDelimterIndex + "ExpectedThirdDelimterIndex " + ExpectedThirdDelimterIndex, "info");

           int ActualFirstDelimterIndex = -1;
           int ActualSecondDelimterIndex = -1;
           int ActualThirdDelimterIndex = -1;
           for (int i = 0; i < ActualDateFormat.Length; i++)
           {
               if (!(Char.IsDigit(ActualDateFormat[i])))
               {
                   if (ActualFirstDelimterIndex == -1)
                   {
                       ActualFirstDelimterIndex = i;
                   }
                   else if (ActualSecondDelimterIndex == -1)
                   {
                       ActualSecondDelimterIndex = i;
                   }
                   else if (ActualThirdDelimterIndex == -1)
                   {
                       ActualThirdDelimterIndex = i;
                   }
               }
           }
           NewLogObj.WriteLogFile(LogFilePath, "ExpectedDateFormat " + ExpectedDateFormat + "ActualFirstDelimterIndex " + ActualFirstDelimterIndex + "ActualSecondDelimterIndex " + ActualSecondDelimterIndex + "ActualThirdDelimterIndex " + ActualThirdDelimterIndex, "info");

           if (ExpectedFirstDelimterIndex != ActualFirstDelimterIndex)
           {
               NewLogObj.WriteLogFile(LogFilePath, "ActualFirstDelimterIndex " + ActualFirstDelimterIndex + "ExpectedFirstDelimterIndex " + ExpectedFirstDelimterIndex + "does not macth " , "info");
               return -1;
           }
           if (ExpectedSecondDelimterIndex != ActualSecondDelimterIndex)
           {
               NewLogObj.WriteLogFile(LogFilePath, "ActualSecondDelimterIndex " + ActualSecondDelimterIndex + "ExpectedSecondDelimterIndex " + ExpectedSecondDelimterIndex + "does not macth ", "info");
               return -1;
           }
           if (ExpectedThirdDelimterIndex != ActualThirdDelimterIndex)
           {
               NewLogObj.WriteLogFile(LogFilePath, "ActualThirdDelimterIndex " + ActualThirdDelimterIndex + "ExpectedThirdDelimterIndex " + ExpectedThirdDelimterIndex + "does not macth ", "info");
               return -1;
           }

           NewLogObj.WriteLogFile(LogFilePath, "Strings are of samepattern " + ExpectedDateFormat + "ActualDateFormat " + ActualDateFormat, "info");
           return 1;
       }
    }
}

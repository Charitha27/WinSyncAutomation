using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevoPoint.Framework.Extensions.BingTranslator;
using System.Text.RegularExpressions;
using LoggerCollection;

namespace BingTranslatorold
{
    class MyBingTranslator
    {
        public string TranslateString(string ToBeTranslatedString, string Language)
        {
            Logger NewLogObj = new Logger();
            
            string LogFilePath = NewLogObj.GetLogFilePath();

            BingTranslationExtension.BingAppID = "CFD4E2AA752DCD5C2BE1558330A1DFC220541848";
            BingLanguage BingSupportedLanguage = new BingLanguage();
            if (Regex.IsMatch(Language, "japan", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.Japanese;
            }
            else if (Regex.IsMatch(Language, "english", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.English;
            }
            else if (Regex.IsMatch(Language, "simplifiedchinese", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.ChineseSimplified;
            }
            else if (Regex.IsMatch(Language, "traditionalchinese", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.ChineseTraditional;
            }
            else if (Regex.IsMatch(Language, "french", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.French;
            }
            else if (Regex.IsMatch(Language, "german", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.German;
            }
            else if (Regex.IsMatch(Language, "spanish", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.Spanish;
            }
            else if (Regex.IsMatch(Language, "russian", RegexOptions.IgnoreCase))
            {
                BingSupportedLanguage = BingLanguage.Russian;
            }

            String TranslatedString = ToBeTranslatedString.BingTranslate(BingSupportedLanguage);
            NewLogObj.WriteLogFile(LogFilePath, "Transating input string " + ToBeTranslatedString + " to " + BingSupportedLanguage.ToString() + " .TranslatedString " + TranslatedString, "info");
            return TranslatedString;
        }


        public string DecideLanguageBasedOnLocale(string SystemLocale)
        {
           string Language=null;
            if (Regex.IsMatch(SystemLocale, "ja-JP", RegexOptions.IgnoreCase))
            {
                Language = "Japanese";
            }
            else if (Regex.IsMatch(SystemLocale, "ko-KR", RegexOptions.IgnoreCase))
            {
                Language = "Korean";
            }
            else if (Regex.IsMatch(SystemLocale, "en-US", RegexOptions.IgnoreCase))
            {
                 Language = "English";
            }
            else if (Regex.IsMatch(SystemLocale, "zh-CN", RegexOptions.IgnoreCase))
            {
                Language = "simplifiedchinese";
            }
            else if (Regex.IsMatch(SystemLocale, "tc-TC", RegexOptions.IgnoreCase))
            {
                Language = "traditionalchinese";
            }
            else if (Regex.IsMatch(SystemLocale, "fr-FR", RegexOptions.IgnoreCase))
            {
                Language = "French";
            }
            else if (Regex.IsMatch(SystemLocale, "de-DE", RegexOptions.IgnoreCase))
            {
                Language = "german";
            }
            else if (Regex.IsMatch(SystemLocale, "es-ES", RegexOptions.IgnoreCase))
            {
                Language = "spanish";
            }
            else if (Regex.IsMatch(SystemLocale, "ru-RU", RegexOptions.IgnoreCase))
            {
               Language = "Russian";
            }
            return Language;
        }
    }
}

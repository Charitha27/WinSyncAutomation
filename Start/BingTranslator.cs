using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;


using LoggerCollection;
using FileOperationsCollection;

namespace Start
{
    class BingTranslator
    {

        public string GenerateAccessTokenAndStartTranslation(string TextToTranslate, string SourceLanguage, string TargetLanguage)
        {
            AdmAccessToken admToken;
            string headerValue;
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            //AdmAuthentication admAuth = new AdmAuthentication("MyCuteGudApp", "fIPVXUdw0fXOWZyshtx+bC2RbI3yvvAGc/AvkVxZq6s=");
            FileOperations FileObj=new FileOperations();
            string BingCredentialsFilePath=Directory.GetCurrentDirectory()+"\\BingCredentials.txt";

            string clientId = FileObj.SearchFileForPattern(BingCredentialsFilePath, "clientId", 1, LogFilePath);
            string clientSecret = FileObj.SearchFileForPattern(BingCredentialsFilePath, "clientSecret", 1, LogFilePath);
            clientId.Trim();
            clientSecret.Trim();
            int IndexEqual = clientId.IndexOf("=");
            clientId = clientId.Substring(IndexEqual + 1);
            IndexEqual = clientSecret.IndexOf("=");
            clientSecret = clientSecret.Substring(IndexEqual + 1);
            AdmAuthentication admAuth = new AdmAuthentication(clientId, clientSecret);
            try
            {
                admToken = admAuth.GetAccessToken();
                DateTime tokenReceived = DateTime.Now;
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                //TranslateMethod(headerValue);
                BingTranslator PgmObj = new BingTranslator();
                string TranslatedText = PgmObj.TranslateMethod(headerValue, TextToTranslate, SourceLanguage, TargetLanguage);
                return TranslatedText;
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
               // Console.WriteLine("Press any key to continue...");
               // Console.ReadKey(true);
                NewLogObj.WriteLogFile(LogFilePath, "Exception at BingTranslate "+ex.ToString() , "warn");
                return null;
            }
        }

        public string TranslateMethod(string authToken, string TextToTranslate, string SourceLanguage, string TargetLanguage)
        {
            Logger NewLogObj = new Logger();
            string LogFilePath = NewLogObj.GetLogFilePath();
            try
            {
               
                // Add TranslatorService as a service reference, Address://http://api.microsofttranslator.com/V2/Soap.svc
                TranslatorService.LanguageServiceClient client = new TranslatorService.LanguageServiceClient();
                
                
                //Set Authorization header before sending the request
                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Method = "POST";
                httpRequestProperty.Headers.Add("Authorization", authToken);

                // Creates a block within which an OperationContext object is in scope.
                using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                    //string sourceText = "<UL><LI>Use generic class names. <LI>Use pixels to express measurements for padding and margins. <LI>Use percentages to specify font size and line height. <LI>Use either percentages or pixels to specify table and container width.   <LI>When selecting font families, choose browser-independent alternatives.   </LI></UL>";
                    string translationResult;
                    //Keep appId parameter blank as we are sending access token in authorization header.
                    //translationResult = client.Translate("", sourceText, "en", "de", "text/html", "general");
                    translationResult = client.Translate("", TextToTranslate, SourceLanguage, TargetLanguage, "text/html", "general");
                    NewLogObj.WriteLogFile(LogFilePath, "Translation for " + TextToTranslate + " from " + SourceLanguage + " to " + TargetLanguage + " is " + translationResult, "info");
                    //Console.WriteLine("Translation for source {0} from {1} to {2} is", TextToTranslate, "en", "de");
                    //Console.WriteLine(translationResult);
                    // Console.WriteLine("Press any key to continue...");
                    //Console.ReadKey(true);
                    return translationResult;
                }
            }
            catch (Exception Ex)
            {
                NewLogObj.WriteLogFile(LogFilePath, "Exception at BingTranslate " + Ex.ToString(), "warn");
                return null;
            }
        }
        public string DecideLanguageBasedOnLocale(string SystemLocale)
        {
            string Language = null;
            if (Regex.IsMatch(SystemLocale, "ja-JP", RegexOptions.IgnoreCase))
            {
                Language = "ja";
            }
            else if (Regex.IsMatch(SystemLocale, "ko-KR", RegexOptions.IgnoreCase))
            {
                Language = "ko";
            }
            else if (Regex.IsMatch(SystemLocale, "en-US", RegexOptions.IgnoreCase))
            {
                Language = "en";
            }
            else if (Regex.IsMatch(SystemLocale, "zh-CN", RegexOptions.IgnoreCase))
            {
                Language = "zh-CHS";
            }
            else if (Regex.IsMatch(SystemLocale, "tc-TC", RegexOptions.IgnoreCase))
            {
                Language = "zh-CHT";
            }
            else if (Regex.IsMatch(SystemLocale, "fr-FR", RegexOptions.IgnoreCase))
            {
                Language = "fr";
            }
            else if (Regex.IsMatch(SystemLocale, "de-DE", RegexOptions.IgnoreCase))
            {
                Language = "de";
            }
            else if (Regex.IsMatch(SystemLocale, "es-ES", RegexOptions.IgnoreCase))
            {
                Language = "es";
            }
            else if (Regex.IsMatch(SystemLocale, "ru-RU", RegexOptions.IgnoreCase))
            {
                Language = "ru";
            }
            return Language;
        }
        [DataContract]
        public class AdmAccessToken
        {
            [DataMember]
            public string access_token { get; set; }
            [DataMember]
            public string token_type { get; set; }
            [DataMember]
            public string expires_in { get; set; }
            [DataMember]
            public string scope { get; set; }
        }

        public class AdmAuthentication
        {
            public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            private string clientId;
            private string cientSecret;
            private string request;

            public AdmAuthentication(string clientId, string clientSecret)
            {
                this.clientId = clientId;
                this.cientSecret = clientSecret;
                //If clientid or client secret has special characters, encode before sending request
                this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
            }

            public AdmAccessToken GetAccessToken()
            {
                return HttpPost(DatamarketAccessUri, this.request);
            }

            private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
            {
                //Prepare OAuth request 
                WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
                webRequest.ContentLength = bytes.Length;
                using (Stream outputStream = webRequest.GetRequestStream())
                {
                    outputStream.Write(bytes, 0, bytes.Length);
                }
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                    //Get deserialized object from JSON stream
                    AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                    return token;
                }
            }
        }
    }
}

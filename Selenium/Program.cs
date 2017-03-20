using System;

using System.Text;

using System.Text.RegularExpressions;

using System.Threading;

using Selenium;



namespace SeleniumTests

{
    public class sel
    {
        private ISelenium selenium;


        //[SetUp]
        public void SetupTest()
        {
        selenium = new DefaultSelenium("localhost", 4444, "*chrome", "http://localhost:1947/");
        selenium.Start();
        //verificationErrors = new StringBuilder();
        }

    

    
        public void TheTectscompleteTest()
        {
			        selenium.Open("/_int_/config_to.html");
			        selenium.Click("id=aggressive");
			        selenium.Type("id=serverlist", "10.105.74.54");
			        selenium.Click("link=Submit");
        }

        static void Main(string[] args)
        {
            sel obj = new sel();
            obj.SetupTest();
            obj.TheTectscompleteTest();
            Console.WriteLine("finsihed");
        }
    }

	
}

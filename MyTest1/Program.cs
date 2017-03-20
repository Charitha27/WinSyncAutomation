using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Threading;
using System.Windows.Forms;
using GUICollection;
using LoggerCollection;
using MapperCollection;
using System.IO;
using System.Text.RegularExpressions;
using GuizardCollection;
using GenericCollection;
using FileOperationsCollection;
using XenServerCollection;
using WindowsOperationsCollection;
using TestDataGeneratorCollection;


namespace MyTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            AutomationElementIdentity GuiObj = new AutomationElementIdentity();
            PropertyCondition NameCond=GuiObj.SetPropertyCondition("NameProperty","Citrix StoreFront",1,null);
            AutomationElement SFObj = GuiObj.FindAutomationElement(AutomationElement.RootElement, NameCond, TreeScope.Children, "parent", null);
            PropertyCondition GridCond = GuiObj.SetPropertyCondition("AutomationIdProperty", "siteListView", 1, null);
            AutomationElement GridObj = GuiObj.FindAutomationElement(SFObj, GridCond, TreeScope.Descendants, "parent", null);
            PropertyCondition DataItemCond = GuiObj.SetPropertyCondition("ControlTypeProperty", "DataItem", 1, null);
            AutomationElementCollection GridChildren = GridObj.FindAll(TreeScope.Descendants, DataItemCond);
            Program PgmObj = new Program();
            foreach (AutomationElement GridChild in GridChildren)
            {
                int Status=PgmObj.CheckRawView(GridChild, "Pass-through from NetScaler Gateway");
                if (Status == 1)
                {
                    Console.WriteLine("Element Found.. Exiting");
                }
            }
           //AutomationElement DataItem = GuiObj.FindAutomationElement(GridObj, DataItemCond, TreeScope.Descendants, "parent", null);
           // Program PgmObj = new Program();
           // PgmObj.CheckRawView(DataItem, "User name and password");
        }

        public int CheckRawView(AutomationElement ParentObj, string ExpectedVal)
        {
            //AutomationElement ChildObj = TreeWalker.RawViewWalker.GetNextSibling(ParentObj);
            AutomationElement ChildObj = TreeWalker.RawViewWalker.GetFirstChild(ParentObj);
            if (ChildObj == null)
            {
                return -1;
            }
            else if (string.Compare(ChildObj.Current.Name, ExpectedVal) == 0)
            {
                Console.WriteLine("Element Found");
                return 1;
            }
            else
            {
                CheckRawView(ChildObj, ExpectedVal);
            
            }
            return 0;
        }
       
    }
}

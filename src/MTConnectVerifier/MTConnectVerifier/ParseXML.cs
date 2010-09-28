/* 
 * Project:    	MT Connect - MTConnectVerifier
 * Company:    	Georgia Tech - MARC - FIS
 * Author:     	Douglas A. Furbush
 * Contact:     404-385-4881  cell 770-316-1636
 * Date:     	February - June 2009
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;
using System.Reflection;

namespace MTConnectVerifier
{
    class ParseXML
    {
        public static Boolean debug = true;
        public static String filename = "docout.xml";
        public static DateTime datetm = DateTime.Now;
        static readonly int BUFSIZE = 16384;
        //private static bool LOAD_DEFAULT_CONFIGURATION = true;
        //static private System.Diagnostics.EventLog eventLog1;	//true to load default.ini	
        public static string serverURI = @"http://ldap.mtconnect.org";
        public static string deviceURI = @"http://banyan.marc.gatech.edu";
        public static string company = "GeorgiaTech";
        public static string descript = "GT Devices";
        public static string deviceName = "GT4MTC";
        public static string serialno = "323235";
        public string version2 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        static void Main(string[] args)
        {

            bool loopit = true;
            string server = ""; 
            try
            {
                server = deviceURI + "/current"; // MT Sim Device
                LogToFile("HTTP request = " + server);
                Console.WriteLine("HTTP request = " + server);

                while (loopit)
                {
                    datetm = DateTime.Now;
                    Console.WriteLine("MTCVerifier: Started at " + datetm.ToString() );
                    Thread.Sleep(1000);
                    string result = DoCmd(server);
                    //LogToFile("HTTP Returns = " + result);
                    //Console.Write(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Main Error = :" + e.Message);
                LogToFile("Main Error = :" + e.Message + ":");
            }
        }

        public static string DoProbe(string cmdStr)
        {
            string result = "</Error>";
            string server = ""; // MT Workshops
            try
            {
                server = deviceURI + "/" + cmdStr; // MT Sim Device
                //Console.WriteLine("HTTP request = " + server);

                result = DoCmd(server);
            }
            catch (Exception e)
            {
                Console.WriteLine("Main Error = :" + e.Message);
                LogToFile("Main Error = :" + e.Message + ":");
            }
            return result;
        }

        public static string DoCurrent(string cmdStr)
        {
            string result = "</Error>";
            string server = ""; 
            try
            {
                server = deviceURI + "/" + cmdStr; // MT Sim Device
                //LogToFile("HTTP request = " + server);
                //Console.WriteLine("HTTP request = " + server);
                result = DoCmd(server);
            }
            catch (Exception e)
            {
                Console.WriteLine("Main Error = :" + e.Message);
                LogToFile("Main Error = :" + e.Message + ":");
            }
            return result;
        }

        public static string DoSample(string cmdStr)
        {
            string result = "</Error>";
            string server = ""; // MT Workshops
            try
            {
                server = deviceURI + "/" + cmdStr; // MT Sim Device
                //Console.WriteLine("HTTP request = " + server);
                result = DoCmd(server);
            }
            catch (Exception e)
            {
                Console.WriteLine("Main Error = :" + e.Message);
                LogToFile("Main Error = :" + e.Message + ":");
            }
            return result;
        }

        public static string DoCmd(string cmdStr)
        {
            //LogToFile("***********************************************************");
            LogToFile("*** MTConnectVerifier Started at " + datetm.ToString() + " ***");
            StringBuilder result = null;
            HttpWebRequest cmd = (HttpWebRequest)WebRequest.Create(cmdStr);
            try
            {
                HttpWebResponse resp = (HttpWebResponse)cmd.GetResponse();
                Stream recvStream = resp.GetResponseStream();
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                //if (debug)
                //    LogToFile("Sending cmd = :" + cmdStr + ":");
                StreamReader rStream = new StreamReader(recvStream, enc);
                result = new System.Text.StringBuilder();
                char[] buf = new char[BUFSIZE];
                int count;
                do
                {
                    count = rStream.Read(buf, 0, BUFSIZE);
                    result.Append(buf, 0, count);
                } while (count > 0);

                resp.Close();
                //Added sleep
                Thread.Sleep(100);

            }
            //A first chance exception of type 'System.IO.DirectoryNotFoundException' occurred in mscorlib.dll
            catch (DirectoryNotFoundException ed)
            {
                System.Console.Write("DirectoryNotFoundException:" + ed.ToString());
                LogToFile("*** DirectoryNotFoundException:" + ed.ToString());
            }
            catch (Exception ex)
            {
                System.Console.Write("ErrorEventArgs:" + ex.ToString());
                LogToFile("*** ErrorEventArgs:" + ex.ToString());
            }
            return result.ToString();
        }

        public static XmlDocument BuildDom(string result)
        {
            //Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(result);
                //Save the document to a file.
                //doc.Save(filename);
                //LogToFile("DoCmd:DisplayStream = :" + filename + ":");
                //DisplayStream(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine("BuildDom:Error = :" + e.Message);
                LogToFile("BuildDom:Error = :" + e.Message + ":");
            }
            //Console.WriteLine("MTCVerifier: Finished Build XML Dom Test");
            return doc;
        }

        public static void LogToFile(string msg)
        {
            System.IO.StreamWriter sw = System.IO.File.AppendText(
                "MTConnectVerifier.log");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        public static string GetHeader(XmlDocument xmldoc, string value)
        {
            string component = "", name = "", subtype = "", statid = "";
            string ctype = "", values = "", code = "", statname = "";
            string valuefound = "";
            XmlNodeList xmlnoda = xmldoc.GetElementsByTagName("Header");
            for (int i = 0; i < xmlnoda.Count; i++)
            {
                XmlAttributeCollection xmlattrd = xmlnoda[i].Attributes;
                for (int k = 0; k < xmlnoda[i].Attributes.Count; k++)
                {
                    if (xmlattrd[k].Name.Equals(value)) valuefound = xmlattrd[k].Value;

                }
            }
            Console.WriteLine("GetHeader:result = :" + valuefound);
            Thread.Sleep(100);
            //Console.WriteLine("MTCVerifier: Finished GetHeader Test");
            //LogToFile("MTCVerifier: Finished");

            return valuefound;
        }


    } // End Class Program
}

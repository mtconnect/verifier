using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;
using System.Threading;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Schema;



namespace MTConnectVerifier
{
    class ParseXML
    {
        //private static System.Diagnostics.EventLog eventLog1;
        private static bool LOAD_DEFAULT_CONFIGURATION = true;  //true to load <default>.ini	
        private static Configuration config = new Configuration(LOAD_DEFAULT_CONFIGURATION);
        static readonly int BUFSIZE = 16384;
        public static Boolean debug = true;
        public static String filename = "docout.xml";
        public static DateTime datetm = DateTime.Now;
        //public static string serverURI = @"http://ldap.mtconnect.org";
        public static string company = config.Company;
        public static string descript = config.Description;
        public static string deviceName = config.DeviceName;
        //public static string serialno = "323235";
        public static string deviceURI = config.DeviceURI;
        public string version2 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static bool validationSuccess = true;


        static void Main(string[] args)
        {
        }
               

        public static bool HttpProtocalVersionTest()
        {
            LogToFile("*** MTConnectVerifier Started at " + datetm.ToString() + " ***");            
            bool IsSupported1 = true;
            bool IsSupported2 = true;
            //StringBuilder result = null;
            Console.WriteLine("\nUsing Http version 1.0.");
            Console.WriteLine("HTTP request = " + deviceURI + "/");            
            HttpWebRequest cmd1 = (HttpWebRequest)WebRequest.Create(deviceURI + "/");
            cmd1.ProtocolVersion = HttpVersion.Version10;
            

            HttpWebResponse resp1 = (HttpWebResponse)cmd1.GetResponse();
            Console.WriteLine("\nThe HttpHeaders of the request are \n"+cmd1.Headers.ToString());
            Console.WriteLine("HttpVersion:"+cmd1.ProtocolVersion.ToString());
            Console.WriteLine("\nThe HttpHeaders of the response are \n"+ resp1.Headers.ToString());
            Console.WriteLine("StatusCode:" + resp1.StatusCode.ToString());

            //Stream recvStream = resp1.GetResponseStream();
           /* Encoding enc = System.Text.Encoding.GetEncoding("utf-8");            
            StreamReader rStream = new StreamReader(recvStream, enc);
            result = new System.Text.StringBuilder();
            char[] buf = new char[BUFSIZE];
            int count;
            do
            {
                count = rStream.Read(buf, 0, BUFSIZE);
                result.Append(buf, 0, count);
            } while (count > 0);*/

            resp1.Close();           
           // Thread.Sleep(100);
           // Console.WriteLine(result.ToString());
           // XElement xmlresult = XElement.Parse(result.ToString());*/
            if (resp1.StatusCode.ToString()!="OK")
            {
                IsSupported1 = IsSupported1 & false;
                Console.WriteLine("The Agent doesn't support Http version 1.0.");
            }
            else
            {
                Console.WriteLine("The Agent support Http version 1.0.");
            }

            Console.WriteLine("\nUsing Http version 1.1.");
            Console.WriteLine("HTTP request = " + deviceURI + "/");            
            HttpWebRequest cmd2 = (HttpWebRequest)WebRequest.Create(deviceURI + "/");
            cmd2.ProtocolVersion = HttpVersion.Version11;
            

            HttpWebResponse resp2 = (HttpWebResponse)cmd2.GetResponse();
            Console.WriteLine("\nThe HttpHeaders of the request are \n"+cmd2.Headers.ToString());
            Console.WriteLine("HttpVersion:" + cmd2.ProtocolVersion.ToString());
            Console.WriteLine("\nThe HttpHeaders of the response are \n"+resp2.Headers.ToString());
            Console.WriteLine("StatusCode:" + resp2.StatusCode.ToString());

            /* recvStream = resp2.GetResponseStream();
            
             rStream = new StreamReader(recvStream, enc);
             result = new System.Text.StringBuilder();
             char[] buf2 = new char[BUFSIZE];
            
             do
             {
                 count = rStream.Read(buf2, 0, BUFSIZE);
                 result.Append(buf2, 0, count);
             } while (count > 0);*/

            resp2.Close();
            //Thread.Sleep(100);
            //Console.WriteLine(result.ToString());
            //xmlresult = XElement.Parse(result.ToString());
            if (resp2.StatusCode.ToString() != "OK")
            {
                IsSupported2 = IsSupported2 & false;
                Console.WriteLine("The Agent doesn't support Http version 1.1.");
            }
            else
            {
                Console.WriteLine("The Agent support Http version 1.1.");
            }
            bool IsSupported = true;
            if (IsSupported1 == false & IsSupported2 == false)
                IsSupported = false;
            return IsSupported;

        }

        public static string HttpGetVerbTest()
        {           
            //StringBuilder result = null;
            Console.WriteLine("\nUsing Http Verb:GET.");
            Console.WriteLine("HTTP request = " + deviceURI + "/");
            HttpWebRequest cmd1 = (HttpWebRequest)WebRequest.Create(deviceURI + "/");
            cmd1.Method = "GET";
            //cmd1.ContentLength = 1;


            HttpWebResponse resp1 = (HttpWebResponse)cmd1.GetResponse();
           
            Console.WriteLine("Http Request Verb:" + cmd1.Method);
            
            Console.WriteLine("Http Response StatusCode:" + resp1.StatusCode.ToString());

           /* Stream recvStream = resp1.GetResponseStream();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader rStream = new StreamReader(recvStream, enc);
            result = new System.Text.StringBuilder();
            char[] buf = new char[BUFSIZE];
            int count;
            do
            {
                count = rStream.Read(buf, 0, BUFSIZE);
                result.Append(buf, 0, count);
            } while (count > 0);
            rStream.Close();*/
            resp1.Close();
            Thread.Sleep(100);
            //Console.WriteLine(result.ToString());
            return resp1.StatusCode.ToString();

        }

        public static string HttpPostVerbTest()
        {
            //StringBuilder result = null;
            Console.WriteLine("\nUsing Http Verb:POST.");
            Console.WriteLine("HTTP request = " + deviceURI + "/");
            HttpWebRequest cmd1 = (HttpWebRequest)WebRequest.Create(deviceURI + "/");
            cmd1.Method = "POST";
            string postdata = "Testing";
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postdata);
            cmd1.ContentType = "application/x-www-form-urlencoded";
            cmd1.ContentLength = byte1.Length;
            Stream newstream = cmd1.GetRequestStream();
            newstream.Write(byte1,0,byte1.Length);
            newstream.Close();


            HttpWebResponse resp1 = (HttpWebResponse)cmd1.GetResponse();

            Console.WriteLine("Http Request Verb:" + cmd1.Method);

            Console.WriteLine("Http Response StatusCode:" + resp1.StatusCode.ToString());

            
            resp1.Close();
            Thread.Sleep(100);
            
            return resp1.StatusCode.ToString();

        }


        public static string DoProbe(string cmdStr)
        {
            string result = "</Error>";
            string server = ""; // MT Workshops
            try
            {
                server = deviceURI + "/" + cmdStr; // MT Sim Device
                Console.WriteLine("\nHTTP request = " + server+"\n");

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
                Console.WriteLine("\nHTTP request = " + server + "\n");
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
                Console.WriteLine("\nHTTP request = " + server+"\n");
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
            
           
            StringBuilder result = null;
            HttpWebRequest cmd = (HttpWebRequest)WebRequest.Create(cmdStr);
            //cmd.Method="GET";
           
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
                
            }
            catch (Exception e)
            {
                Console.WriteLine("BuildDom:Error = :" + e.Message);
                LogToFile("BuildDom:Error = :" + e.Message + ":");
            }
            
            return doc;
        }
        /// <summary>
        /// Save input to the log file.
        /// </summary>
        /// <param name="msg"></param>
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
        /// <summary>
        /// Get the value of Attributes in the Header of MTConnectDevices Xml file.
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="attributename"></param>
        /// <returns></returns>
        public static string GetMTConnectDevicesAttribute(XElement xmldoc, string attributename)
        {
            
            
            XElement header = xmldoc.Element(MTConnectNameSpace.mtDevices+"Header");
            string valuefound = header.Attribute(attributename).Value;            
            Console.WriteLine("GetHeader: "+attributename+" value = " + valuefound);
           // Thread.Sleep(100);          
            return valuefound;
        }
        /// <summary>
        /// Get the value of Attributes in the Header of MTConnectStreams Xml file.
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="attributename"></param>
        /// <returns></returns>
        public static string GetMTConnectStreamsAttribute(XElement xmldoc, string attributename)
        {


            XElement header = xmldoc.Element(MTConnectNameSpace.mtStreams + "Header");
            string valuefound = header.Attribute(attributename).Value;
            Console.WriteLine("GetHeader: " + attributename + " value = " + valuefound);
            // Thread.Sleep(100);          
            return valuefound;
        }
        /// <summary>
        /// Validate the responded MTConnectDevices Xml file against "MTConnectDevices.xsd".
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public static bool ValidateMTConnectDevices(string xmldoc)
        {
                
            Console.WriteLine("Validate response against MTConnectDevices.xsd.\r\n");
            validationSuccess = true;

            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add("urn:mtconnect.org:MTConnectDevices:1.1", "MTConnectDevices.xsd");
            
            
            
      
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);


            byte[] byteArray = Encoding.ASCII.GetBytes( xmldoc );
            MemoryStream stream = new MemoryStream( byteArray ); 

            XmlReader reader = XmlReader.Create(stream, settings);
            while (reader.Read()) ;

           

            if (validationSuccess)
            { Console.WriteLine("XML validation Against MTConnectDevices.xsd succeeded.\r\n"); }
            else
            {
                Console.WriteLine("XML validation Against MTConnectDevices.xsd failed.\r\n");
            }    

            return validationSuccess;          
        }
        /// <summary>
        /// Validate the responded MTConnectError Xml file against "MTConnectError.xsd".
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public static bool ValidateMTConnectError(string xmldoc)
        {
                       
            Console.WriteLine("Validate response against MTConnectError.xsd.\r\n");
            validationSuccess = true;

            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add("urn:mtconnect.org:MTConnectError:1.1", "MTConnectError.xsd");




            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);


            byte[] byteArray = Encoding.ASCII.GetBytes(xmldoc);
            MemoryStream stream = new MemoryStream(byteArray);

            XmlReader reader = XmlReader.Create(stream, settings);
            while (reader.Read()) ;



            if (validationSuccess)
            { Console.WriteLine("XML validation Against MTConnectError.xsd succeeded.\r\n"); }
            else
            {
                Console.WriteLine("XML validation Against MTConnectError.xsd failed.\r\n");
            }

            return validationSuccess;          
            
            
        }
        /// <summary>
        /// Validate the responded MTConnectStreams Xml file against "MTConnectStreams.xsd".
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public static bool ValidateMTConnectStreams(string xmldoc)
        {
            Console.WriteLine("Validate response against MTConnectStreams.xsd.\r\n");
            validationSuccess = true;

            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add("urn:mtconnect.org:MTConnectStreams:1.1", "MTConnectStreams.xsd");




            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);


            byte[] byteArray = Encoding.ASCII.GetBytes(xmldoc);
            MemoryStream stream = new MemoryStream(byteArray);

            XmlReader reader = XmlReader.Create(stream, settings);
            while (reader.Read()) ;



            if (validationSuccess)
            { Console.WriteLine("XML validation Against MTConnectStreams.xsd succeeded.\r\n"); }
            else
            {
                Console.WriteLine("XML validation Against MTConnectStreams.xsd failed.\r\n");
            }

            return validationSuccess;

        }
        /// <summary>
        /// Check if the xml response is well formed. 
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool XmlWellFormed(string xmldoc, string name)
        { 
            bool IsWellFormed=true;
            try
            {

                // Try to load the value into a document
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(xmldoc);

                // If we managed with no exception then this is valid XML!


            }
            catch (System.Xml.XmlException)
            {
                IsWellFormed = false;
            }

            finally
            {
                if (IsWellFormed)
                {
                    Console.WriteLine(name + " is well-formed xml file.\r\n");
                }
                else
                {
                    Console.WriteLine(name + "is not well-formed xml file.\r\n");
                }
            }

                return IsWellFormed;
            
        }

        public static void ValidationHandler(object sender,ValidationEventArgs args)
        {
            validationSuccess = false; //Validation failed

            Console.WriteLine("XML validation failed." + "\r\n" +"Validation error: " + args.Message + "\r\n");

        }


        /// <summary>
        /// Compare two MTConnctStreams Response Xml files except for the Header.  
        /// </summary>
        /// <param name="xmldoc1"></param>
        /// <param name="xmldoc2"></param>
        /// <returns></returns>
        public static bool MTConnectStreamsAreEqual(XElement xmldoc1, XElement xmldoc2)
        {
            bool t1 = (xmldoc1.Name.LocalName == xmldoc2.Name.LocalName);
            bool t2 = (xmldoc1.Element(MTConnectNameSpace.mtStreams + "Streams").ToString() == xmldoc2.Element(MTConnectNameSpace.mtStreams + "Streams").ToString());
            //Console.WriteLine(xmldoc1.Name.LocalName.ToString());
            //Console.WriteLine(xmldoc2.Name.LocalName.ToString());
            //Console.WriteLine(xmldoc1.Element(MTConnectNameSpace.mtStreams + "Streams").ToString());
            //Console.WriteLine(xmldoc2.Element(MTConnectNameSpace.mtStreams + "Streams").ToString());
            return (t1 & t2);
        
        }
        /// <summary>
        /// Compare two MTConnctDevices Response Xml files except for the Header.
        /// </summary>
        /// <param name="xmldoc1"></param>
        /// <param name="xmldoc2"></param>
        /// <returns></returns>
        public static bool MTConnectDevicesAreEqual(XElement xmldoc1, XElement xmldoc2)
        {
            bool t1 = (xmldoc1.Name.LocalName == xmldoc2.Name.LocalName);
            bool t2 = (xmldoc1.Element(MTConnectNameSpace.mtDevices + "Devices").ToString() == xmldoc2.Element(MTConnectNameSpace.mtDevices + "Devices").ToString());
            //Console.WriteLine(xmldoc1.Name.LocalName.ToString());
            //Console.WriteLine(xmldoc2.Name.LocalName.ToString());
            //Console.WriteLine(xmldoc1.Element(MTConnectNameSpace.mtDevices + "Devices").ToString());
            //Console.WriteLine(xmldoc2.Element(MTConnectNameSpace.mtDevices + "Devices").ToString());
            return (t1 & t2);

        }

        public static Stream convert_string_to_stream(string input)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return stream;
        
        }

        public static String getErrorCode(XElement doc)
        {
            XElement error = doc.Descendants(MTConnectNameSpace.mtError + "Error").ElementAt(0);
            String errorcode = error.Attribute("errorCode").Value;
            return errorcode;
        
        }

        public static String[] getDiffenrentDataItemIds(XElement doc)
        {
            String[] IdArray=new string[] { "null"};
            int i=1;
            foreach (XElement data in doc.Elements())
            {
                String dataItemId = data.Attribute("dataItemId").Value;
                if (!IdArray.Contains(dataItemId))
                {
                    IdArray[i] = dataItemId;
                    i++;
                }
            
            }
            return IdArray;
        }

       /* public static String getDataElementName(String _type)
        {
            //POSITION_XXX to Position_Xxx
            String name = "";
            String[] parts = _type.Split('_');
            for (int i = 0; i < parts.Length; i++)
            {
                name += (parts[i].Substring(0, 1).ToUpper() + parts[i].Substring(1).ToLower());
            }
            return name;
        }*/

        /*public static IEnumerable<XElement> orderBySequence(IEnumerable<XElement> datas)
        { 
           IEnumerable<XElement> ordered=datas;
            for(int i=0;i<ordered.Count();i++)
            {
                for(int j=i;j<ordered.Count();j++)
                {
                    if(Int64.Parse(ordered.ElementAt(j).Attribute("sequence").Value)>Int64.Parse(ordered.ElementAt(j+1).Attribute("sequence").Value))
                    {XElement temp=ordered.ElementAt(j);
                    ordered.OrderBy=ordered.ElementAt(j+1);

                    }
                }
            }
            return ordered;
        }*/

        public static bool Equal2datas(XElement data1, XElement data2)
        {
            bool t=true;
            XElement temp1 = new XElement(data1);
            XElement temp2 = new XElement(data2);
            t=t&(temp1.Name.LocalName==temp2.Name.LocalName);
            t=t&(temp1.Value==temp2.Value);
            if (temp1.Attribute("nativeCode") != null & temp2.Attribute("nativeCode")!=null)
                t = t & (temp1.Attribute("nativeCode").Value == temp2.Attribute("nativeCode").Value);
            if (temp1.Attribute("code") != null & temp2.Attribute("code") != null)
                t = t & (temp1.Attribute("code").Value == temp2.Attribute("code").Value);

            return t;

        }

        public static string GetContainer(XElement dataitem)
        {
            String container=null;
            String category;
            category = dataitem.Attribute("category").Value;
            if (category == "CONDITION")
                container = "Condition";
            else if (category == "EVENT")
                container = "Events";
            else if (category == "SAMPLE")
                container = "Samples";

            return container;

        }


        public static String getDataElementName(String _type)
        {
            //POSITION_XXX to PositionXxx
            String name = "";
            String[] parts = _type.Split('_');
            for (int i = 0; i < parts.Length; i++)
            {
                name += (parts[i].Substring(0, 1).ToUpper() + parts[i].Substring(1).ToLower());
            }
            return name;
        }  
    } // End Class Program
}
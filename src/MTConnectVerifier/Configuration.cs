using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace MTConnectVerifier
{
    [XmlRoot("Configuration")]
    public class Configuration
    {
        #region Local Variables

        private string debug = "true";
        private const int DEFAULT_WAIT_TIME = 150;        /*default wait time in msecs*/
        private string description = "MTConnect Verifier";
        private string company = "TechSolve";
        private string deviceName = "GT4MTC";
        private string deviceURI = @"http://pine.marc.gatech.edu";
        private int waitTime = DEFAULT_WAIT_TIME;
        private int waitTimeMsg = 0;
        private const string DEFAULT_FILE_NAME = "mtcverifier.ini";
        static public string defaultDirectory = @"";
        static public string logDirectory = @"log\";
        static public string confDirectory = @"conf\";
        static public string textFilePath = @"data\";
        private bool createNewLogfile = false;
        private bool useLogFile = true;

        
        #endregion

        #region Configuration

        public Configuration(Configuration source)
        {
            this.company = source.company;
            this.description = source.description;
            this.deviceName = source.deviceName;
            this.deviceURI = source.deviceURI;
            this.debug = source.debug;
            this.WaitTime = source.WaitTime;
            this.WaitTimeMsg = source.WaitTimeMsg;
            this.DefaultDirectory = source.DefaultDirectory;
            this.CreateNewLogfile = source.CreateNewLogfile;
            this.UseLogFile = source.UseLogFile;
        }

        private void CloneConfiguration(Configuration source)
        {
            this.company = source.company;
            this.description = source.description;
            this.deviceName = source.deviceName;
            this.deviceURI = source.deviceURI;
            this.debug = source.debug;
            this.WaitTime = source.WaitTime;
            this.WaitTimeMsg = source.WaitTimeMsg;
            this.DefaultDirectory = source.DefaultDirectory;
            this.CreateNewLogfile = source.CreateNewLogfile;
            this.UseLogFile = source.UseLogFile;
        }

        
        public Configuration()
        {
            
        }

        public Configuration(bool LoadDefault)
        {
            System.Console.Out.WriteLine("Configuration:LoadDefault = " + LoadDefault);
            if (LoadDefault)
            {
                TextReader xmlIn = null;

                try
                {
                    //DAF Display
                   //String dirStr =Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + DEFAULT_FILE_NAME;
                    String dirStr = defaultDirectory + confDirectory + DEFAULT_FILE_NAME;

                   // Stream mtcverifierinitial = Assembly.GetExecutingAssembly().GetManifestResourceStream("verifier test.mtcverifier.ini");
                   System.Console.Out.WriteLine("Configuration: file=" + dirStr);

                   xmlIn = new StreamReader(dirStr);
                   // xmlIn = new StreamReader(mtcverifierinitial);
                    XmlSerializer s = new XmlSerializer(typeof(Configuration));
                    CloneConfiguration((Configuration)s.Deserialize(xmlIn));
                    Console.WriteLine("Setup Check: Using deviceURI = " + deviceURI);
                }
                catch (Exception ec)
                {
                    System.Console.Out.WriteLine("*** Configuration:Exception: Conf Directory = " + ec);
                }
                finally
                {
                    if (xmlIn != null)
                        xmlIn.Close();
                }
            }
        }
        #endregion

        #region MTConnectAgentCore Connection Configuration
        //properties 

        [XmlElement("Debug")]
        public string Debug
        {
            set
            {
                debug = value;
            }
            get
            {
                return debug;
            }
        }

        [XmlElement("Company")]
        public string Company
        {
            set
            {
                company = value;
            }
            get
            {
                return company;
            }
        }

        [XmlElement("Description")]
        public string Description
        {
            set
            {
                description = value;
            }
            get
            {
                return description;
            }
        }

        [XmlElement("DeviceName")]
        public string DeviceName
        {
            set
            {
                deviceName = value;
            }
            get
            {
                return deviceName;
            }
        }

        [XmlElement("DeviceURI")]
        public string DeviceURI
        {
            set
            {
                deviceURI = value;
            }
            get
            {
                return deviceURI;
            }
        }

        #endregion

        #region CAMXAdapter Configuration

        [XmlElement("WaitTime")]
        public int WaitTime
        {
            set
            {
                waitTime = value;
            }
            get
            {
                return waitTime;
            }
        }

        [XmlElement("WaitTimeMsg")]
        public int WaitTimeMsg
        {
            set
            {
                waitTimeMsg = value;
            }
            get
            {
                return waitTimeMsg;
            }
        }

        [XmlElement("DefaultDirectory")]
        public string DefaultDirectory
        {
            set
            {
                defaultDirectory = value;
            }
            get
            {
                return defaultDirectory;
            }
        }

        [XmlElement("CreateNewLogfile")]
        public bool CreateNewLogfile
        {
            set
            {
                createNewLogfile = value;
            }
            get
            {
                return createNewLogfile;
            }
        }

        [XmlElement("UseLogFile")]
        public bool UseLogFile
        {
            set { useLogFile = value; }
            get { return useLogFile; }
        }

        #endregion

    }
}
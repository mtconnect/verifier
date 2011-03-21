/* 
 * Project:    	MT Connect - MTConnectVerifier
 * Company:    	Georgia Tech - MARC - FIS
 * Author:     	Douglas A. Furbush
 * Contact:     404-385-4881  cell 770-316-1636
 * Date:     	February - June 2009
 */

using System;
using System.Xml.Serialization;
using System.IO;

namespace MTConnectVerifier
{
	[XmlRoot("Configuration")]
	public class Configuration
	{
        private string debug = "true";
        private const int DEFAULT_WAIT_TIME = 150;        /*default wait time in msecs*/
        private string serverName = "fis-banyan";
        private string serverURL = "192.168.0.201";
        private string portId = "389";
        private string deviceName = "GT4MTC";
        private string adminDN = "dc=mtconnect,dc=org";
        private string container = "ou=Devices,dc=mtconnect,dc=org";
        private string company = "GeorgiaTech";
        private string description = "GT MTConnect Devices";
        private string deviceURI = @"http://pine.marc.gatech.edu";
        private string serialno = "444555";
        private string loginDN = "cn=Agent,ou=accounts,dc=mtconnect,dc=org";
        private string passwd = "mtc0nnect";
        private int waitTime = DEFAULT_WAIT_TIME;
		private int waitTimeMsg=0;
        private const string DEFAULT_FILE_NAME = "MTCVerifier.ini";
//        static public string defaultDirectory = Environment.GetEnvironmentVariable(SERVICEVAR);
//        static public string defaultDirectory = "c:";
        static public string defaultDirectory = @"C:\";	
		static public string logDirectory = @"log\";	
		static public string confDirectory= @"conf\";
        static public string textFilePath = @"data\";
		private bool createNewLogfile=false;
		private bool useLogFile=true;
		
		private bool sqlServerEnabled=false;
		private bool mySQLEnabled=false;
		private bool oracleEnabled=false;
		private bool textFileEnabled=false;
        //ParseXML act = new ParseXML();

		public Configuration(Configuration source)
		{
            this.serverName = source.serverName;
            this.serverURL = source.serverURL;
            this.portId = source.portId;
            this.deviceName = source.deviceName;

            this.adminDN = source.adminDN;
            this.container = source.container;
            this.company = source.company;
            this.description = source.description;
            this.deviceURI = source.deviceURI;
            this.serialno = source.serialno;
            this.loginDN = source.loginDN;
            this.passwd = source.passwd;
            this.debug = source.debug;

			this.WaitTime=source.WaitTime;
			this.WaitTimeMsg=source.WaitTimeMsg;
			this.DefaultDirectory=source.DefaultDirectory;
			this.CreateNewLogfile=source.CreateNewLogfile;

            this.UseLogFile = source.UseLogFile;
		}
		
		private void CloneConfiguration(Configuration source)
		{
            this.serverName = source.serverName;
            this.serverURL = source.serverURL;
            this.portId = source.portId;
            this.deviceName = source.deviceName;

            this.adminDN = source.adminDN;
            this.container = source.container;
            this.company = source.company;
            this.description = source.description;
            this.deviceURI = source.deviceURI;
            this.serialno = source.serialno;
            this.loginDN = source.loginDN;
            this.passwd = source.passwd;
            this.debug = source.debug;

			this.WaitTime=source.WaitTime;
			this.WaitTimeMsg=source.WaitTimeMsg;
			this.DefaultDirectory=source.DefaultDirectory;
			this.CreateNewLogfile=source.CreateNewLogfile;

			this.UseLogFile=source.UseLogFile;
		}

		public Configuration(bool LoadDefault)
		{
			if(LoadDefault)
			{
				TextReader xmlIn=null;
			
				try
				{
                    //DAF Display
                    String dirStr = defaultDirectory + confDirectory + DEFAULT_FILE_NAME;
                    System.Console.Out.WriteLine("log Directory = " + dirStr);
                    //xmlIn = new StreamReader(defaultDirectory + confDirectory + DEFAULT_FILE_NAME);
                    //xmlIn = new StreamReader(dirStr);
                    xmlIn = new StreamReader(DEFAULT_FILE_NAME);
					XmlSerializer s = new XmlSerializer( typeof( Configuration ) );
					CloneConfiguration((Configuration) s.Deserialize( xmlIn ));
				}
				catch(Exception ec)
				{
                    //act.LogToFile("***Exception: log Directory = " + ec);
                    //LogToFile("***Exception: log Directory = " + ec);
                    System.Console.Out.WriteLine("***Exception: log Directory = " + ec);
                }
				finally
				{
					if (xmlIn!=null)
						xmlIn.Close();
				}
			}
		}


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

        [XmlElement("ServerName")]
        public string ServerName
        {
            set
            {
                serverName = value;
            }
            get
            {
                return serverName;
            }
        }

        [XmlElement("ServerURL")]
        public string ServerURL
        {
            set
            {
                serverURL = value;
            }
            get
            {
                return serverURL;
            }
        }

        [XmlElement("PortId")]
        public string PortId
        {
            set
            {
                portId = value;
            }
            get
            {
                return portId;
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

        [XmlElement("AdminDN")]
        public string AdminDN
        {
            set
            {
                adminDN = value;
            }
            get
            {
                return adminDN;
            }
        }

        [XmlElement("Container")]
        public string Container
        {
            set
            {
                container = value;
            }
            get
            {
                return container;
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

        [XmlElement("Serialno")]
        public string Serialno
        {
            set
            {
                serialno = value;
            }
            get
            {
                return serialno;
            }
        }

        [XmlElement("LoginDN")]
        public string LoginDN
        {
            set
            {
                loginDN = value;
            }
            get
            {
                return loginDN;
            }
        }

        [XmlElement("Passwd")]
        public string Passwd
        {
            set
            {
                passwd = value;
            }
            get
            {
                return passwd;
            }
        }

        #endregion
			
		#region Verifier Configuration
		
		[XmlElement("WaitTime")]
		public int WaitTime
		{
			set
			{
				waitTime=value;
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
				waitTimeMsg=value;
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
				defaultDirectory=value;
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
				createNewLogfile=value;
			}
			get
			{
				return  createNewLogfile;
			}
		}
								
		[XmlElement("UseLogFile")]
		public bool UseLogFile
		{
			set{useLogFile=value;}
			get{return useLogFile;}
		}
				
		#endregion



    }
}
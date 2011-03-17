using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using NUnit.Framework;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Net;


namespace MTConnectVerifier
{
    [TestFixture, Description("MTConnectAgent_HTTP")]
    public class S1_MTConnectAgent_HTTP
    {
        [Test, Description("MTConnect Agent MUST support HTTP version 1.0 or greater.")]
        public void T027_Agent_Supports_HTTP10_Or_HTTP11()
        {
            bool success = ParseXML.HttpProtocalVersionTest();
            Assert.True(success);
            
        }

        [Test, Description("MTConnect Agent MUST support HTTP verb GET.")]
        public void T028_Agent_Supports_HTTP_GET()
        {
            String statusCode = ParseXML.HttpGetVerbTest();
            Assert.True(statusCode=="OK");

        }

        [Test, Description("If the HTTP GET verb is not used, the Agent must respond with a HTTP 400 Bad Request indicating that the client issued a bad request.")]
        public void T030_Agent_Not_Supports_HTTP_Verbs_Other_Than_GET()
        {
            String statusCode = ParseXML.HttpPostVerbTest();
            Assert.True(statusCode == "BadRequest");

        }


        [Test, Description("Agent supports HTTP protocol to receive a request")]
        public void T001_Agent_Supports_HTTP_Protocol_to_Receive()
        {
            //string result = ParseXML.DoProbe("probe");
            Console.WriteLine("HTTP request = " + ParseXML.deviceURI + "/");
            HttpWebRequest cmd = (HttpWebRequest)WebRequest.Create(ParseXML.deviceURI + "/");
            HttpWebResponse resp = (HttpWebResponse)cmd.GetResponse();
            Console.WriteLine("StatusCode:" + resp.StatusCode.ToString());
            //XElement xmlresult = XElement.Parse(result);

            // Regex r = new Regex("<mt:MTConnectDevices ");
            //Match m = r.Match(result);
            Assert.True(resp.StatusCode.ToString() == "OK");
        }

        
      
    }

    [TestFixture, Description("MTConnectAgent_Protocal")]
    public class S2_MTConnectAgent_Protocal
    {

        [Test, Description("MTConnectAgent MUST support three types of requests:probe,current,sample")]
        public void T031_MTConnectAgent_Support_Probe_Current_Sample()
        {
            string proberesult = ParseXML.DoProbe("probe");
            bool t1 = ParseXML.XmlWellFormed(proberesult, "Probe result");
            string currentresult = ParseXML.DoProbe("current");
            bool t2 = ParseXML.XmlWellFormed(currentresult, "Current result");
            string sampleresult = ParseXML.DoProbe("sample");
            bool t3 = ParseXML.XmlWellFormed(sampleresult, "Sample result");
            Assert.True(t1 & t2 & t3);
        }


        [Test, Description("NextSequence MUST have a maximum value of 2^63-1 and MUST be stored in an signed 64 bit integer")]
        public void T016_NextSequence_Signed_64_Integer()
        {
            string result = ParseXML.DoProbe("current");
            XElement xmlresult = XElement.Parse(result);
            //Console.WriteLine(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("nextSequence").Value);
            long nextsequence = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("nextSequence").Value);
            Assert.True(nextsequence < 9223372036854775807);
        }

        [Test, Description("InstanceId MUST have a maximum value of 2^63-1 and MUST be stored in an signed 64 bit integer.")]
        public void T017_InstanceId_Signed_64_Integer()
        {
            string result = ParseXML.DoProbe("current");
            XElement xmlresult = XElement.Parse(result);
            long instanceId = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("instanceId").Value);
            Assert.True(instanceId < 9223372036854775807);
        }

        [Test, Description("The bufferSize MUST be a positive integer value with a maximum value of 2^31-1")]
        public void T018_BufferSize_Signed_32_Integer()
        {
            string result = ParseXML.DoProbe("current");
            XElement xmlresult = XElement.Parse(result);
            long bufferSize = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("bufferSize").Value);
            Assert.True(bufferSize < 2147483647);
        }

        [Test, Description("FirstSequence MUST have a maximum value of 2^63-1 and MUST be stored in an signed 64 bit integer")]
        public void T019_FirstSequence_Signed_64_Integer()
        {
            string result = ParseXML.DoProbe("current");
            XElement xmlresult = XElement.Parse(result);
            long firstsequence = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("firstSequence").Value);
            Assert.True(firstsequence < 9223372036854775807);
        }

        [Test, Description("LastSequence MUST have a maximum value of 2^63-1 and MUST be stored in an signed 64 bit integer")]
        public void T020_LastSequence_Signed_64_Integer()
        {
            string result = ParseXML.DoProbe("current");
            XElement xmlresult = XElement.Parse(result);
            long lastsequence = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("lastSequence").Value);
            Assert.True(lastsequence < 9223372036854775807);
        }

        [Test, Description("The bufferSize MUST contain the maximum number of results that can be stored in the Agent at any one instant")]
        public void T026_BufferSize_Represents_Maximum_Number_Of_Results_That_Can_Be_Stored()
        {
            string result = ParseXML.DoProbe("current");
            XElement xmlresult = XElement.Parse(result);
            long firstsequence = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("firstSequence").Value);
            long lastsequence = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("lastSequence").Value);
            long bufferSize = Int64.Parse(xmlresult.Element(MTConnectNameSpace.mtStreams + "Header").Attribute("bufferSize").Value);
            Assert.True((lastsequence - firstsequence + 1) < bufferSize + 1);
        }

        [Test, Description("There MUST NEVER be two identical adjacent values for the same data item in the same component.")]
        public void T079_No_Two_Identical_Adjacent_Data_Values()
        {
            bool t = true;
            IEnumerable<XElement> datas;
            String result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");
            //result = ParseXML.DoCurrent("current");
            //XElement currentresult = XElement.Parse(result);
            //long firstsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult,"firstSequence"));
            //long lastsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence"));
            //long count = lastsequence - firstsequence + 1;
            foreach (XElement dataitem in dataitems)
            {
                result = ParseXML.DoSample("sample?" + @"&path=//DataItem[@id=""" + dataitem.Attribute("id").Value + @"""]");
                XElement sampleresult = XElement.Parse(result);
                Console.WriteLine(result);
                if (dataitem.Attribute("category").Value != "CONDITION")
                {
                    datas = sampleresult.Descendants(MTConnectNameSpace.mtStreams + ParseXML.getDataElementName(dataitem.Attribute("type").Value));
                    for (int i = 0; i < datas.Count() - 1; i++)
                    {
                        t = t & (!ParseXML.Equal2datas(datas.ElementAt(i), datas.ElementAt(i + 1)));
                    }
                }
                else
                {
                    if (sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition").Count() > 0)
                    {
                        datas = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition").First().Elements();
                        for (int i = 0; i < datas.Count() - 1; i++)
                        {
                            t = t & (!ParseXML.Equal2datas(datas.ElementAt(i), datas.ElementAt(i + 1)));
                        }
                    }
                }
                Console.WriteLine(t);

            }

            Assert.True(t);
        }

        [Test, Description("The sequence number MUST be unique for this instance of the MTConnect Agent, regardless of the device or component the data came from.")]
        public void T080_Sequence_Unique()
        {

            long i = 0;
            bool t = true;
            //string result = ParseXML.DoCurrent("current");
            //XElement currentresult = XElement.Parse(result);
            //long firstsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "firstSequence"));
            //long lastsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence"));
            //long count = lastsequence - firstsequence + 1;
            string result = ParseXML.DoSample("sample");
            XElement sampleresult = XElement.Parse(result);
            String[] sequencArray = new String[100];
            IEnumerable<XElement> samples = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            foreach (XElement sample in samples)
            {
                foreach (XElement data in sample.Elements())
                {
                    if (i != 0)
                    {
                        if (sequencArray.Contains(data.Attribute("sequence").Value))
                        {
                            t = false;
                            Console.WriteLine("Sequence Number:" + data.Attribute("sequence").Value + "is not unique.");


                        }

                    }

                    sequencArray[i] = data.Attribute("sequence").Value;
                    // sequencArray.SetValue(data.Attribute("sequence").Value, i);
                    i++;

                }
            }
            foreach (XElement event2 in events)
            {
                foreach (XElement data in event2.Elements())
                {
                    if (1 != 0)
                    {
                        if (sequencArray.Contains(data.Attribute("sequence").Value))
                        {
                            t = false;
                            Console.WriteLine("Sequence Number:" + data.Attribute("sequence").Value + "is not unique.");

                        }

                    }

                    sequencArray[i] = data.Attribute("sequence").Value;
                    // sequencArray.SetValue(data.Attribute("sequence").Value, i);
                    i++;
                }

            }
            foreach (XElement condition2 in condition)
            {
                foreach (XElement data in condition2.Elements())
                {
                    if (i != 0)
                    {
                        if (sequencArray.Contains(data.Attribute("sequence").Value))
                        {
                            t = false;
                            Console.WriteLine("Sequence Number:" + data.Attribute("sequence").Value + "is not unique.");

                        }

                    }

                    sequencArray[i] = data.Attribute("sequence").Value;
                    //sequencArray.SetValue(data.Attribute("sequence").Value,i);
                    i++;
                }

            }
            //for (long m = 0; m < i ;m++ )
            //  Console.WriteLine(sequencArray[m].ToString());

            Assert.True(t);
        }

        [Test, Description("An empty element representing the device MUST be returned to indicate that the request was valid and no data was found if no satisfying data was found in the specified range.")]
        public void T085_Empty_Device_Must_Be_Returned_If_No_Data_Is_Found_In_Specified_Range()
        {
            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            long nextsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "nextSequence"));
            string devicename = currentresult.Element(MTConnectNameSpace.mtStreams + "Streams").Element(MTConnectNameSpace.mtStreams + "DeviceStream").Attribute("name").Value;
            result = ParseXML.DoSample(devicename+"/sample?from=" + nextsequence.ToString());
            XElement sampleresult = XElement.Parse(result);
            Console.WriteLine(result);

            long lastsequence2 = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "lastSequence"));
            XElement streams = sampleresult.Element(MTConnectNameSpace.mtStreams + "Streams");
            if (!streams.HasElements)
            {

                Console.WriteLine("A empty element representing the device MUST be returned when no data is found in the specified range. \n");
                Assert.True(false);

            }
            else
            {
                XElement devicestream = streams.Element(MTConnectNameSpace.mtStreams + "DeviceStream");
                if (devicestream.HasElements)
                {
                    if (lastsequence2 > nextsequence - 1)
                        Assert.True(true);
                    else
                    {
                        Console.WriteLine("A empty element representing the device MUST be returned when no data is found in the specified range. \n");
                        Assert.True(false);
                    }
                }
                else
                {
                    Assert.True(true);
 
                }
            
            }
          /*  if (lastsequence2 == nextsequence - 1)
            {
                if (!streams.HasElements)
                {
                    Console.WriteLine("A empty element representing the device MUST be returned when no data is found in the specified range. \n");
                    Assert.True(false);
                }
                else
                {
                    XElement devicestream = streams.Element(MTConnectNameSpace.mtStreams + "DeviceStream");
                    if(devicestream.HasElements)
                        Console.WriteLine("A empty element representing the device MUST be returned when no data is found in the specified range. \n");

                    Assert.True(!devicestream.HasElements);
                }
            }
            else
            {
                XElement devicestream = streams.Element(MTConnectNameSpace.mtStreams + "DeviceStream");
                Assert.True(devicestream.HasElements);
            }*/
        }

        [Test, Description("Every device in MTConnect MUST have an Availability dataitem.")]
        public void T098_Every_Device_Must_Have_An_Availability_DataItem()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            
            IEnumerable<XElement> devices = proberesult.Descendants(MTConnectNameSpace.mtDevices + "Device");
            Console.WriteLine("There are totally "+devices.Count().ToString()+" devices represented by the Agent: ");
            bool t2 = true;
            bool temp;
            foreach (XElement device in devices)
            {
                temp = false;
                XElement dataitems = device.Element(MTConnectNameSpace.mtDevices + "DataItems");
                //Console.WriteLine(dataitems.ToString());
                foreach (XElement dataitem in dataitems.Elements())
                {
                    if (dataitem.Attribute("type").Value == "AVAILABILITY")
                        temp = true;
                }
                if (temp)
                { Console.WriteLine(device.Attribute("name").Value + ": There is an Availability dataitem."); }
                else
                { Console.WriteLine(device.Attribute("name").Value + ": There is no Availability dataitem."); }
                t2 = t2 & temp;

            }

            Assert.True(t2);
        }

        [Test, Description("The name attribute of Sample MUST match the name of the DataItem this sample is associated with.")]
        public void T130_Name_Of_Sample_Match_Name_of_DataItem()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);

            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");
            XElement currentresult;
            XElement data;
            bool t1 = true;
            foreach (XElement dataitem in dataitems)
            {
                if (dataitem.Attribute("category").Value == "SAMPLE")
                {
                    Console.WriteLine(dataitem.ToString());
                    result = ParseXML.DoCurrent(@"current?path=//DataItem[@id=""" + dataitem.Attribute("id").Value + @"""]");
                    currentresult = XElement.Parse(result);
                    data = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Samples").First().Elements().First();
                    Console.WriteLine(data.ToString()+"\n");
                    if (data.Attribute("name") != null & dataitem.Attribute("name") != null)
                    {
                        t1 = t1 & (data.Attribute("name").Value.Equals(dataitem.Attribute("name").Value));
                        if (!data.Attribute("name").Value.Equals(dataitem.Attribute("name").Value))
                            Console.WriteLine(@"The name attribute of Sample(id=""" + data.Attribute("dataItemId").Value+@""") doesn't match the name of the associated DataItem. \n");
                        
                    }
                }
            }
            Assert.True(t1);
        }

        [Test, Description("The name attribute of Event MUST match the name of the DataItem this event is associated with.")]
        public void T154_Name_Of_Event_Match_Name_of_DataItem()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);

            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");

            XElement currentresult;
            XElement data;
            bool t1 = true;
            foreach (XElement dataitem in dataitems)
            {
                if (dataitem.Attribute("category").Value == "EVENT")
                {
                    Console.WriteLine(dataitem.ToString());
                    result = ParseXML.DoCurrent(@"current?path=//DataItem[@id=""" + dataitem.Attribute("id").Value + @"""]");
                    currentresult = XElement.Parse(result);
                    // Console.WriteLine(result);
                    data = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Events").First().Elements().First();
                    Console.WriteLine(data.ToString()+"\n");
                    if (data.Attribute("name") != null & dataitem.Attribute("name") != null)
                    {
                        t1 = t1 & (data.Attribute("name").Value.Equals(dataitem.Attribute("name").Value));
                        if (!data.Attribute("name").Value.Equals(dataitem.Attribute("name").Value))
                            Console.WriteLine(@"The name attribute of Sample(id=""" + data.Attribute("dataItemId").Value + @""") doesn't match the name of the associated DataItem. \n");
                    }
                }
            }
            Assert.True(t1);
        }

        [Test, Description("The name of condition MUST match the name of the condition's associated DataItem.")]
        public void T167_Name_Of_Condition_Match_Name_of_DataItem()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);

            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");


            XElement currentresult;
            XElement data;
            bool t1 = true;
            foreach (XElement dataitem in dataitems)
            {
                if (dataitem.Attribute("category").Value == "CONDITION")
                {
                    Console.WriteLine(dataitem.ToString());
                    result = ParseXML.DoCurrent(@"current?path=//DataItem[@id=""" + dataitem.Attribute("id").Value + @"""]");
                    currentresult = XElement.Parse(result);
                    // Console.WriteLine(result);
                    data = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Condition").First().Elements().First();
                    Console.WriteLine(data.ToString()+"\n");
                    if (data.Attribute("name") != null & dataitem.Attribute("name") != null)
                    {
                        t1 = t1 & (data.Attribute("name").Value.Equals(dataitem.Attribute("name").Value));
                        if (!data.Attribute("name").Value.Equals(dataitem.Attribute("name").Value))
                            Console.WriteLine(@"The name attribute of Sample(id=""" + data.Attribute("dataItemId").Value + @""") doesn't match the name of the associated DataItem. \n");
                   
                    }
                }
            }
            Assert.True(t1);
        }

        [Test, Description("The element of event or sample will be the transformation of the dataitem type.")]
        public void T117_DataItem_Type_Transformed_To_Be_Element_Of_Event_Or_Sample()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            //bool t1 = ParseXML.ValidateMTConnectDevices(result);
            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");
            XElement currentresult;
            XElement data;
            bool t2 = true;
            foreach (XElement dataitem in dataitems)
            {
                if (dataitem.Attribute("category").Value != "CONDITION")
                {
                    result = ParseXML.DoCurrent(@"current?path=//DataItem[@id=""" + dataitem.Attribute("id").Value + @"""]");
                    Console.WriteLine(result);
                    currentresult = XElement.Parse(result);
                    data = currentresult.Descendants(MTConnectNameSpace.mtStreams + ParseXML.GetContainer(dataitem)).First().Elements().First();
                    t2 = t2 & (data.Name.LocalName.Equals(ParseXML.getDataElementName(dataitem.Attribute("type").Value)));
                    if (!data.Name.LocalName.Equals(ParseXML.getDataElementName(dataitem.Attribute("type").Value)))
                    {
                        Console.WriteLine(@"\n The data element name for dataitem(id=""" + dataitem.Attribute("id").Value + @""") is not correct. \n");
                    }
                }
            }
            Assert.True(t2);
        }
    }

    [TestFixture, Description("MTConnectAgent_XML")]
    public class S3_MTConnectAgent_XML
    {
        [Test, Description("Data MUST be sent back in valid XML")]
        public void T002_Data_Sent_Back_in_Valid_XML()
        {
            string result = ParseXML.DoProbe("probe");
            bool validProbe = ParseXML.ValidateMTConnectDevices(result);
            result = ParseXML.DoProbe("sample");
            bool validSample = ParseXML.ValidateMTConnectStreams(result);
            result = ParseXML.DoProbe("current");
            bool validCurrent = ParseXML.ValidateMTConnectStreams(result);
            Assert.True(validProbe & validSample & validCurrent);
        }

        [Test, Description("Xml MUST have root of MTConnectDevices or MTConnectStreams or MTConnectError")]
        public void T005_Xml_Root_Must_Be_MTConnectDevices_Or_MTConnectStreams_Or_MTConnectError()
        {
            string result = ParseXML.DoProbe("probe");
            XElement xmlresult = XElement.Parse(result);
            bool t1 = (xmlresult.Name.LocalName == "MTConnectDevices");
            Console.WriteLine(@"Name of root element is """ + xmlresult.Name.LocalName+@""".");
            result = ParseXML.DoProbe("current");
            xmlresult = XElement.Parse(result);
            bool t2 = (xmlresult.Name.LocalName == "MTConnectStreams");
            Console.WriteLine(@"Name of root element is """ + xmlresult.Name.LocalName + @""".");
            result = ParseXML.DoProbe("prob");
            xmlresult = XElement.Parse(result);
            bool t3 = (xmlresult.Name.LocalName == "MTConnectError");
            Console.WriteLine(@"Name of root element is """ + xmlresult.Name.LocalName + @""".");
            Assert.True(t1 & t2 & t3);
        }


    }

    [TestFixture, Description("MTConnectAgent_Probe_Request")]
    public class S4_MTConnectAgent_Probe_Request
    {
        [Test, Description("The MTConnect Agent MUST provide to a probe request with a response of MTConnectDevices")]
        public void T032_Probe_Response_MTConnectDevices()
        {
            string result = ParseXML.DoProbe("probe");
            XElement xmlresult = XElement.Parse(result);
            Console.WriteLine("The root element of response to probe request is " + xmlresult.Name.LocalName);
            Assert.True(xmlresult.Name.LocalName == "MTConnectDevices");

        }
        
        [Test, Description("Agent MUST represent at least one device")]
        public void T003_Agent_Represents_At_Least_One_Device()
        {
            string result = ParseXML.DoProbe("probe");

            XElement xmlresult = XElement.Parse(result);
            Console.WriteLine(xmlresult.ToString());
            XElement devices = xmlresult.Element(MTConnectNameSpace.mtDevices + "Devices");
            int num = devices.Elements().Count();
            Console.WriteLine("\n The Agent represents "+num.ToString()+" devices.");
            Assert.True(num>0);
        }

        

        [Test, Description("A probe request MUST NOT supply any parameters.If any are supplied, they MUST be ignored")]
        public void T034_Probe_Request_Parameters_Ignored()
        {
            string result = ParseXML.DoProbe("probe");
            XElement xmlresult1 = XElement.Parse(result);
           // Console.WriteLine(xmlresult1.ToString());
            result = ParseXML.DoProbe("probe?at=1");
            XElement xmlresult2 = XElement.Parse(result);
            Console.WriteLine(xmlresult2.ToString());
            bool comp = ParseXML.MTConnectDevicesAreEqual(xmlresult1, xmlresult2);
            Assert.True(comp);
        }


        [Test, Description("The probe request MUST support two variations:http://10.0.1.23/mill-1/probe；http://10.0.1.23/probe")]
        public void T035_Probe_Request_Support_DeviceName()
        {
            bool outcome;
            string result = ParseXML.DoProbe("probe");
            XElement xmlresult1 = XElement.Parse(result);
            string devicename = xmlresult1.Element(MTConnectNameSpace.mtDevices + "Devices").Element(MTConnectNameSpace.mtDevices + "Device").Attribute("name").Value;
            XElement device1 = xmlresult1.Element(MTConnectNameSpace.mtDevices + "Devices").Element(MTConnectNameSpace.mtDevices + "Device");
            //Console.WriteLine(device1.ToString());
            result = ParseXML.DoProbe(devicename + "/probe");
            XElement xmlresult2 = XElement.Parse(result);

            XElement device2 = xmlresult2.Element(MTConnectNameSpace.mtDevices + "Devices").Element(MTConnectNameSpace.mtDevices + "Device");
            Console.WriteLine(device2.ToString());
            string devicename2 = device2.Attribute("name").Value;
            bool t1 = (xmlresult2.Element(MTConnectNameSpace.mtDevices + "Devices").Elements().Count() == 1);
            bool t2 = (devicename == devicename2);
           // Console.WriteLine(t1.ToString() + t2.ToString());
            outcome = t1 & t2;

            Assert.True(outcome);
        }

    }

    [TestFixture, Description("MTConnectAgent_Current_Request")]
    public class S5_MTConnectAgent_Current_Request
    {

        [Test, Description("If the path is not given, current request MUST be responded with all data items for the device(s).")]
        public void T050_Current_Responded_With_All_Data_When_Path_Not_Given()
        {

            string result = ParseXML.DoSample("probe");
            XElement proberesult = XElement.Parse(result);
            Stream resultstream1 = ParseXML.convert_string_to_stream(result);
            XmlReader reader1 = XmlReader.Create(resultstream1);
            XmlNameTable nametable = reader1.NameTable;
            XmlNamespaceManager namespacemanager1 = new XmlNamespaceManager(nametable);
            namespacemanager1.AddNamespace("mt", MTConnectNameSpace.mtConnectUriDevices);
            IEnumerable<XElement> dataItems = proberesult.XPathSelectElements(@"//mt:DataItem", namespacemanager1);
            long count1 = dataItems.Count();


            result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);


            IEnumerable<XElement> samples = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            long count = 0;
            bool t = true;
            String id;
            foreach (XElement sampleelement in samples)
            {
                foreach (XElement sampledata in sampleelement.Elements())
                {
                    count++;
                    id = sampledata.Attribute("dataItemId").Value;
                    dataItems = proberesult.XPathSelectElements(@"//mt:DataItem[@id=""" + id + @"""]", namespacemanager1);
                    t = t & (dataItems.Count() == 1);

                }

            }
            foreach (XElement eventelement in events)
            {
                foreach (XElement eventdata in eventelement.Elements())
                {
                    count++;
                    id = eventdata.Attribute("dataItemId").Value;
                    dataItems = proberesult.XPathSelectElements(@"//mt:DataItem[@id=""" + id + @"""]", namespacemanager1);
                    t = t & (dataItems.Count() == 1);

                }

            }
            foreach (XElement conditionelement in condition)
            {
                foreach (XElement conditiondata in conditionelement.Elements())
                {
                    count++;
                    id = conditiondata.Attribute("dataItemId").Value;
                    dataItems = proberesult.XPathSelectElements(@"//mt:DataItem[@id=""" + id + @"""]", namespacemanager1);
                    t = t & (dataItems.Count() == 1);
                }

            }

            Assert.True(count == count1 & t);


        }

        [Test, Description("Current MUST return the nextSequence number for the event or sample directly following the point at which the snapshot was taken.")]
        public void T05152_Current_NextSequence()
        {

            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            IEnumerable<XElement> samples = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            long sequence = 0;

            foreach (XElement sampleelement in samples)
            {
                foreach (XElement sampledata in sampleelement.Elements())
                {

                    if (Int64.Parse(sampledata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(sampledata.Attribute("sequence").Value);
                }

            }
            foreach (XElement eventelement in events)
            {
                foreach (XElement eventdata in eventelement.Elements())
                {
                    if (Int64.Parse(eventdata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(eventdata.Attribute("sequence").Value);
                }

            }
            foreach (XElement conditionelement in condition)
            {
                foreach (XElement conditiondata in conditionelement.Elements())
                {
                    if (Int64.Parse(conditiondata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(conditiondata.Attribute("sequence").Value);
                }

            }
            Console.WriteLine("Greatest sequence in current responding=" + sequence.ToString());

            long nextsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "nextSequence"));
            Assert.True(nextsequence == sequence + 1);
        }

        [Test, Description("The MTConnect Agent MUST accept the following parameter for the current request:path,frequency,at.")]
        public void T055_Current_Request_With_Parameters()
        {

            string result = ParseXML.DoProbe("current");
            XElement currentresult = XElement.Parse(result);
            String lastsequence = ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence");
            result = ParseXML.DoCurrent("current?at=" + lastsequence + @"&path=//DataItem[@category=""SAMPLE""]");
            XElement result2 = XElement.Parse(result);
            Console.WriteLine(result2.ToString());
            bool t1 = (result2.Name.LocalName == "MTConnectStreams");
            //IEnumerable<XElement> conditions = result2.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            //IEnumerable<XElement> events = result2.Descendants(MTConnectNameSpace.mtStreams + "Events");
            //bool t2 = (conditions.Count() == 0 & events.Count() == 0);
            Assert.True(t1);
        }

        [Test, Description("Frequency parameter in current request must not be used with at parameter.")]
        public void T056_Current_Request_Cannot_Use_Both_At_And_Frequency()
        {

            string result = ParseXML.DoCurrent("current");
            XElement currentresult1 = XElement.Parse(result);
            String lastsequence = ParseXML.GetMTConnectStreamsAttribute(currentresult1, "lastSequence");
            result = ParseXML.DoCurrent("current?at=" + lastsequence + @"&frequency=10");
            XElement currentresult2 = XElement.Parse(result);
            Console.WriteLine(result);
            if (currentresult2.Name.LocalName != "MTConnectError")
            {
                Assert.True(false);
            }
            else
            {
                String errorcode = currentresult2.Descendants(MTConnectNameSpace.mtError + "Error").First().Attribute("errorCode").Value;
                Console.WriteLine("ErrorCode=" + errorcode);

                Assert.True(errorcode.Equals("INVALID_REQUEST"));
            }
        }

        [Test, Description("If at parameter is supplied in current request, the most current values on or before the sequence number MUST be provided.")]
        public void T057_Current_Cannot_Get_Sequence_Greater_Than_At()
        {

            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            String lastsequence = ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence");
            result = ParseXML.DoCurrent("current?at=" + lastsequence);
            currentresult = XElement.Parse(result);

            IEnumerable<XElement> samples = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            long sequence = 0;

            foreach (XElement sampleelement in samples)
            {
                foreach (XElement sampledata in sampleelement.Elements())
                {

                    if (Int64.Parse(sampledata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(sampledata.Attribute("sequence").Value);
                }

            }
            foreach (XElement eventelement in events)
            {
                foreach (XElement eventdata in eventelement.Elements())
                {
                    if (Int64.Parse(eventdata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(eventdata.Attribute("sequence").Value);
                }

            }
            foreach (XElement conditionelement in condition)
            {
                foreach (XElement conditiondata in conditionelement.Elements())
                {
                    if (Int64.Parse(conditiondata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(conditiondata.Attribute("sequence").Value);
                }

            }
            Console.WriteLine("The largest sequence in the response is "+sequence.ToString()+".");
            Console.WriteLine("The at parameter in the request is " + lastsequence+ ".");
            Assert.True(sequence == Int64.Parse(lastsequence));
        }

        [Test, Description("'at' must be greater than or equal to the first available sequence number.")]
        public void T059_At_Must_Greater_Or_Equal_FirstSequence()
        {

            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            long firstsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "firstSequence"));
            long at = firstsequence - 1;
            result = ParseXML.DoCurrent("current?at=" + at.ToString());
            currentresult = XElement.Parse(result);
            Console.WriteLine(result);
            if (currentresult.Name.LocalName != "MTConnectError")
            {
                Assert.True(false);
            }
            else
            {
                String errorcode = currentresult.Descendants(MTConnectNameSpace.mtError + "Error").First().Attribute("errorCode").Value;
                Console.WriteLine("ErrorCode=" + errorcode);
                Assert.True(errorcode.Equals("OUT_OF_RANGE"));
            }
        }
    }

    [TestFixture, Description("MTConnectAgent_Sample_Request")]
    public class S6_MTConnectAgent_Sample_Request
    {

        [Test, Description("The reponse to a sample request MUST be a valid MTConnectStreams XML Document")]
        public void T036_Sample_Response_MTConnectStreams()
        {
            string result = ParseXML.DoSample("sample");
            XElement xmlresult = XElement.Parse(result);
            Console.WriteLine("The root of the response xml is " + xmlresult.Name.LocalName + ".");
            Assert.True(xmlresult.Name.LocalName == "MTConnectStreams");
        }

        [Test, Description("All parameters to sample request MUST only be given once and the order of the parameters is not important")]
        public void T037_Sample_Request_Parameters_Order_Not_Important()
        {
            string result = ParseXML.DoProbe("current");
            XElement currentresult = XElement.Parse(result);
            String lastsequence = ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence");
            result = ParseXML.DoSample("sample?from=" + lastsequence + "&count=1");
            XElement sampleresult1 = XElement.Parse(result);
            Console.WriteLine(sampleresult1.ToString());
            result = ParseXML.DoSample("sample?count=1&from=" + lastsequence);
            XElement sampleresult2 = XElement.Parse(result);
            Console.WriteLine(sampleresult2.ToString());
            bool comp = ParseXML.MTConnectStreamsAreEqual(sampleresult1, sampleresult2);
            if (!comp)
                Console.WriteLine("Different order of the parameters gives different xml responses.");
            Assert.True(comp);
        }

      /*  [Test, Description("The MTConnect Agent MUST accept the following parameters for the sample request:path, from, count, frequency")]
        public void T038_Sample_Request_With_Parameters()
        {
            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            String lastsequence = ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence");
            //result = ParseXML.DoSample("sample?from=" + lastsequence + @"&count=1&frequency=1&path=//DataItem[@category=""SAMPLE""]");
            HttpWebRequest cmd1 = (HttpWebRequest)WebRequest.Create(ParseXML.deviceURI + "/" + "sample?from=" + lastsequence + @"&count=1&frequency=1&path=//DataItem[@category=""SAMPLE""]");
            HttpWebResponse resp1 = (HttpWebResponse)cmd1.GetResponse();
            Console.WriteLine("Http Response StatusCode:" + resp1.StatusCode.ToString());
            //XElement sampleresult = XElement.Parse(result);
            //Console.WriteLine(sampleresult.ToString());
            //bool t1 = (sampleresult.Name.LocalName == "MTConnectStreams");
            //IEnumerable<XElement> conditions = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            //IEnumerable<XElement> events = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            //bool t2 = (conditions.Count() == 0 & events.Count() == 0);
            //Assert.True(t1 & t2);
            Assert.True(resp1.StatusCode.ToString() == "OK");
        }*/

        [Test, Description("If the path specifies a component, all data items for that component and any of its sub-components MUST be included.")]
        public void T039_Path_Specify_Component()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            Stream resultstream = ParseXML.convert_string_to_stream(result);
            XmlReader reader = XmlReader.Create(resultstream);
            XmlNameTable nametable = reader.NameTable;
            XmlNamespaceManager namespacemanager = new XmlNamespaceManager(nametable);
            namespacemanager.AddNamespace("mt", MTConnectNameSpace.mtConnectUriDevices);
            IEnumerable<XElement> samples = proberesult.XPathSelectElements(@"//mt:DataItem[@category=""SAMPLE""]", namespacemanager);
            int count = samples.Count();
            String[] dataitemIdArray = new String[count];
            int i = 0;
            foreach (XElement sample in samples)
            {
                dataitemIdArray[i] = sample.Attribute("id").Value;
                i++;

            }

            result = ParseXML.DoCurrent(@"current?path=//DataItem[@category=""SAMPLE""]");
            XElement currentresult = XElement.Parse(result);
            Console.WriteLine(currentresult.ToString());
            IEnumerable<XElement> events = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            bool t1 = (events.Count() == 0 & condition.Count() == 0);
            Console.WriteLine(t1.ToString());


            IEnumerable<XElement> SamplesElements = currentresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            //i = 0;
            bool t2 = true;
            foreach (XElement SamplesElement in SamplesElements)
            {
                foreach (XElement sampledata in SamplesElement.Elements())
                {
                    t2 = t2 & (dataitemIdArray.Contains(sampledata.Attribute("dataItemId").Value));
                    Console.WriteLine(sampledata.ToString());
                    //i++;
                }
            }
            //Console.WriteLine(i.ToString());
            Assert.True(t1 & t2);

        }

        [Test, Description("If the value of from is less than 0 ,an INVALID_REQUEST error MUST be returned.")]
        public void T041_INVALID_REQUEST_If_From_Less_Than_0()
        {

            string result = ParseXML.DoSample("sample?from=-1");
            XElement errorresult = XElement.Parse(result);
            Console.WriteLine(errorresult.ToString());

            if (errorresult.Name.LocalName == "MTConnectError")
            {
                Console.WriteLine("\nThe errorcode is " + ParseXML.getErrorCode(errorresult));
                Assert.True(ParseXML.getErrorCode(errorresult) == "INVALID_REQUEST");
            }
            else
            {
                Assert.True(false);
            }
        }

        [Test, Description("The Agent MUST NOT send back more than count of Events, Condition, and Samples (in aggregate), but fewer Events, Condition, and Samples MAY be returned.")]
        public void T042_No_More_Than_Count()
        {

            string result = ParseXML.DoSample("sample?count=50");
            XElement sampleresult = XElement.Parse(result);
            Console.WriteLine(sampleresult.ToString() + "\n");
            int i = 0;
            IEnumerable<XElement> samples = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            foreach (XElement sampleelement in samples)
            {
                i = i + sampleelement.Elements().Count();

            }
            foreach (XElement eventelement in events)
            {
                i = i + eventelement.Elements().Count();

            }
            foreach (XElement conditionelement in condition)
            {
                i = i + condition.Elements().Count();

            }
            Console.WriteLine("Responded dataitem amount=" + i.ToString() + ".\n");
            Console.WriteLine(@"""Count"" parameter in the request=50." + "\n");
            long firstsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "firstSequence"));
            long lastsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "lastSequence"));
            if (lastsequence - firstsequence + 1 < 50)
                Assert.True(i == lastsequence - firstsequence + 1);
            else
                Assert.True(i == 50);
        }

        [Test, Description("If a valid value of from and no path parameter is provided ,the minimum sequence in the response should be equal to the value of from.")]
        public void T044_VALID_FROM_IS_PROVIDED_WITHOUT_PATH()
        {

            string result = ParseXML.DoSample("current");
            XElement currentresult = XElement.Parse(result);
            long nextsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "nextSequence"));
            result = ParseXML.DoSample("sample?from=" + nextsequence.ToString());
            XElement sampleresult = XElement.Parse(result);
            Console.WriteLine(sampleresult.ToString());
            long dataNum = 0;
            long sequence = nextsequence + 1;
            IEnumerable<XElement> samples = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            foreach (XElement sampleelement in samples)
            {
                foreach (XElement sampledata in sampleelement.Elements())
                {
                    dataNum++;
                    if (Int64.Parse(sampledata.Attribute("sequence").Value) < sequence)
                        sequence = Int64.Parse(sampledata.Attribute("sequence").Value);

                }

            }
            foreach (XElement eventelement in events)
            {
                foreach (XElement eventdata in eventelement.Elements())
                {
                    dataNum++;
                    if (Int64.Parse(eventdata.Attribute("sequence").Value) < sequence)
                        sequence = Int64.Parse(eventdata.Attribute("sequence").Value);

                }

            }
            foreach (XElement conditionelement in condition)
            {
                foreach (XElement conditiondata in conditionelement.Elements())
                {
                    dataNum++;
                    if (Int64.Parse(conditiondata.Attribute("sequence").Value) < sequence)
                        sequence = Int64.Parse(conditiondata.Attribute("sequence").Value);

                }

            }
            bool outcome;
            if (dataNum == 0)
            {
                outcome = (sequence == nextsequence + 1);

            }

            else
            {
                outcome = (sequence == nextsequence);
            }



            Assert.True(outcome);
        }


        [Test, Description("If the value of count is less than 1 , an INVALID_REQUEST error MUST be returned.")]
        public void T043_INVALID_REQUEST_If_Count_Less_Than_1()
        {

            string result = ParseXML.DoSample("sample?count=0");
            XElement errorresult = XElement.Parse(result);
            Console.WriteLine(errorresult.ToString());
            if (errorresult.Name.LocalName == "MTConnectError")
            {
                Console.WriteLine("\nThe errorcode is "+ParseXML.getErrorCode(errorresult));
                Assert.True(ParseXML.getErrorCode(errorresult) == "INVALID_REQUEST");
            }
            else
            {
                Assert.True(false);
            }
        }

        [Test, Description("The nextSequence number in the header MUST be set to the sequence number following the largest sequence number (highest sequence number + 1) of all the Events, Condition, and Samples considered when collecting the results.")]
        public void T045_Set_NextSequence_Correctly()
        {
            string result = ParseXML.DoSample("current");
            XElement currentresult = XElement.Parse(result);
            long nextsequence1 = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "nextSequence"));
            result = ParseXML.DoSample("sample?from=" + nextsequence1.ToString() + "&count=10");
            XElement sampleresult = XElement.Parse(result);
            long nextsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "nextSequence"));
            Console.WriteLine(sampleresult.ToString());

            IEnumerable<XElement> samples = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            long sequence = nextsequence1 - 1;
            long dataNum = 0;
            foreach (XElement sampleelement in samples)
            {
                foreach (XElement sampledata in sampleelement.Elements())
                {
                    dataNum++;
                    if (Int64.Parse(sampledata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(sampledata.Attribute("sequence").Value);

                }

            }
            foreach (XElement eventelement in events)
            {
                foreach (XElement eventdata in eventelement.Elements())
                {
                    dataNum++;
                    if (Int64.Parse(eventdata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(eventdata.Attribute("sequence").Value);

                }

            }
            foreach (XElement conditionelement in condition)
            {
                foreach (XElement conditiondata in conditionelement.Elements())
                {
                    dataNum++;
                    if (Int64.Parse(conditiondata.Attribute("sequence").Value) > sequence)
                        sequence = Int64.Parse(conditiondata.Attribute("sequence").Value);

                }

            }
            bool outcome;
            if (dataNum < 10)
            {
                outcome = (nextsequence == sequence + 1);

            }
            else if (dataNum == 10)
            {
                outcome = (nextsequence == nextsequence1 + 10);
            }
            else
            {
                outcome = false;
            }
            //Console.WriteLine("\nThe largest sequence number in the response="+sequence.ToString()+".\n");
            //long nextsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "nextSequence"));

            Assert.True(outcome);
        }

        [Test, Description("The path MUST default to all components in the device or devices if no device is specified.")]
        public void T046_Path_Default_To_All_Components()
        {
            bool temp;
            bool t = true;
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");
            foreach (XElement dataitem in dataitems)
            {
                String container = ParseXML.GetContainer(dataitem);
                IEnumerable<XElement> categories = currentresult.Descendants(MTConnectNameSpace.mtStreams + container);
                temp = false;
                foreach (XElement category in categories)
                {
                    foreach (XElement data in category.Elements())
                    {
                        if (data.Attribute("dataItemId").Value == dataitem.Attribute("id").Value)
                            temp = true;
                    }
                }
                t = t & temp;
            }

            Assert.True(t);
        }


        [Test, Description("The count MUST default to 100 if it is not specified.")]
        public void T047_Count_Default_To_100()
        {

            string result = ParseXML.DoSample("sample");
            Console.WriteLine(result);
            XElement sampleresult = XElement.Parse(result);
            // result = ParseXML.DoSample("sample?count=100");
            // XElement sampleresult2 = XElement.Parse(result);
            long lastsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "lastSequence"));
            long firstsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "firstSequence"));


            IEnumerable<XElement> samples = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = sampleresult.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            long count = 0;
            foreach (XElement sampleelement in samples)
            {
                count = count + sampleelement.Elements().Count();
            }
            foreach (XElement eventelement in events)
            {
                count = count + eventelement.Elements().Count();
            }
            foreach (XElement conditionelement in condition)
            {
                count = count + conditionelement.Elements().Count();
            }
            Console.WriteLine("The number of dataitems in the response= " + count.ToString() + ".");
            bool t;
            if (lastsequence - firstsequence + 1 < 100)
                t = (count == lastsequence - firstsequence + 1);
            else
                t = (count == 100);
            Assert.True(t);
            //Assert.True(ParseXML.MTConnectStreamsAreEqual(sampleresult,sampleresult2));
        }

        [Test, Description("The from MUST default to 0 and return the first available data.")]
        public void T048_From_Default_To_0()
        {

            string result = ParseXML.DoSample("sample");
            XElement sampleresult1 = XElement.Parse(result);
            //result = ParseXML.DoSample("sample?from=0");
            //XElement sampleresult2 = XElement.Parse(result);
            //bool t2 = ParseXML.MTConnectStreamsAreEqual(sampleresult1,sampleresult2);



            IEnumerable<XElement> samples = sampleresult1.Descendants(MTConnectNameSpace.mtStreams + "Samples");
            IEnumerable<XElement> events = sampleresult1.Descendants(MTConnectNameSpace.mtStreams + "Events");
            IEnumerable<XElement> condition = sampleresult1.Descendants(MTConnectNameSpace.mtStreams + "Condition");
            long sequence = 0;
            foreach (XElement sampleelement in samples)
            {
                foreach (XElement sampledata in sampleelement.Elements())
                {
                    if (sequence == 0)
                        sequence = Int64.Parse(sampledata.Attribute("sequence").Value);
                    else if (Int64.Parse(sampledata.Attribute("sequence").Value) < sequence)
                        sequence = Int64.Parse(sampledata.Attribute("sequence").Value);
                }

            }
            foreach (XElement eventelement in events)
            {
                foreach (XElement eventdata in eventelement.Elements())
                {
                    if (sequence == 0)
                        sequence = Int64.Parse(eventdata.Attribute("sequence").Value);
                    else if (Int64.Parse(eventdata.Attribute("sequence").Value) < sequence)
                        sequence = Int64.Parse(eventdata.Attribute("sequence").Value);

                }

            }
            foreach (XElement conditionelement in condition)
            {
                foreach (XElement conditiondata in conditionelement.Elements())
                {
                    if (sequence == 0)
                        sequence = Int64.Parse(conditiondata.Attribute("sequence").Value);
                    else if (Int64.Parse(conditiondata.Attribute("sequence").Value) < sequence)
                        sequence = Int64.Parse(conditiondata.Attribute("sequence").Value);

                }

            }


            bool t;
            if (sequence == 0)
            {
                t = ParseXML.GetMTConnectStreamsAttribute(sampleresult1, "firstSequence").Equals(ParseXML.GetMTConnectStreamsAttribute(sampleresult1, "nextSequence"));
                Console.WriteLine("No data is available in the Agent now.");
            }
            else
            {
                Console.WriteLine("The minimum sequence in the response is " + sequence.ToString() + ".");
                t = sequence.Equals(Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult1, "firstSequence")));

            }

            Assert.True(t);
        }
    }

    [TestFixture, Description("MTConnectAgent_Error")]
    public class S7_MTConnectAgent_Error
    {
        [Test, Description("The Errors element MUST contain at least one Error element.")]
        public void T0717273_MTConnectError_Test()
        {

            string result = ParseXML.DoSample("sam");
            XElement xmlresult = XElement.Parse(result);
            Console.WriteLine(result);
            if (xmlresult.Name.LocalName != "MTConnectError")
            {
                Assert.True(false);
            }
            else
            {
                bool validMTConnectError = ParseXML.ValidateMTConnectError(result);
                int num = xmlresult.Element(MTConnectNameSpace.mtError + "Errors").Elements().Count();
                Console.WriteLine("The Errors element contain "+num.ToString()+" Error element. \n");

                Assert.True(validMTConnectError & num>=1);
            }
        }



        [Test, Description("If a current request is made for a sequence number prior to the first available sequence number, the agent MUST return a OUT_OF_RANGE error.")]
        public void T081_OUT_OF_RANGE_If_At_Prior_to_FirstSequence()
        {

            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            long firstsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "firstSequence"));
            result = ParseXML.DoCurrent("current?at=" + (firstsequence - 1).ToString());
            currentresult = XElement.Parse(result);
            Console.WriteLine(result);
            if (currentresult.Name.LocalName != "MTConnectError")
            {
                Assert.True(false);
            }
            else
            {
                String errorcode = currentresult.Descendants(MTConnectNameSpace.mtError + "Error").First().Attribute("errorCode").Value;
                Console.WriteLine("\n ErrorCode=" + errorcode+"\n");
                Assert.True(errorcode.Equals("OUT_OF_RANGE"));
                //Assert.True(ParseXML.getErrorCode(currentresult) == "OUT_OF_RANGE");
            }
        }

        [Test, Description("If a current request is made with a sequence number greater than the end of the buffer,the OUT_OF_RANGE error MUST be given.")]
        public void T082_OUT_OF_RANGE_If_At_Greater_than_LastSequence()
        {

            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            long lastsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence"));
            result = ParseXML.DoCurrent("current?at=" + (lastsequence + 200).ToString());
            currentresult = XElement.Parse(result);
            Console.WriteLine(result);

            if (currentresult.Name.LocalName != "MTConnectError")
            {
                if (currentresult.Name.LocalName == "MTConnectStreams")
                {
                    long lastsequence2 = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence"));
                    Assert.True(lastsequence2 >= lastsequence + 200);

                }
                else
                    Assert.True(false);
            }
            else
            {
                String errorcode = currentresult.Descendants(MTConnectNameSpace.mtError + "Error").First().Attribute("errorCode").Value;
                Console.WriteLine("ErrorCode=" + errorcode+"\n");
                Assert.True(errorcode.Equals("OUT_OF_RANGE"));
            }
           
        }

        [Test, Description("If the value for from is larger than the last item’s sequence number + 1, an OUT_OF_RANGE error MUST be returned from the  Agent.")]
        public void T083_OUT_OF_RANGE_If_From_Greater_than_LastSequence_Plus_One()
        {
            string result = ParseXML.DoCurrent("current");
            XElement currentresult = XElement.Parse(result);
            long lastsequence = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(currentresult, "lastSequence"));
            result = ParseXML.DoSample("sample?from=" + (lastsequence + 200).ToString());
            XElement sampleresult = XElement.Parse(result);
            Console.WriteLine(result);
            if (sampleresult.Name.LocalName != "MTConnectError")
            {
                if (sampleresult.Name.LocalName == "MTConnectStreams")
                {
                    long lastsequence2 = Int64.Parse(ParseXML.GetMTConnectStreamsAttribute(sampleresult, "lastSequence"));
                    Assert.True(lastsequence2 >= lastsequence + 200-1);

                }
                else
                    Assert.True(false);
            }
            else
            {
                String errorcode = sampleresult.Descendants(MTConnectNameSpace.mtError + "Error").First().Attribute("errorCode").Value;
                Console.WriteLine("ErrorCode=" + errorcode+"\n");
                Assert.True(errorcode.Equals("OUT_OF_RANGE"));
            }
           
            
        }


    }

    [TestFixture, Description("MTConnectAgent_Devices")]
    public class S8_MTConnectAgent_Devices
    {
        [Test, Description("The id attribute must adhere to the w3c standard ID-type and must be unique within the entire XML document.")]
        public void T099_To_106_Id_Unique_Valid_MTConnectDevices()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            bool t= true;
            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");
            String[] IdArray = new String[dataitems.Count()];
            int i = 0;
            foreach (XElement dataitem in dataitems)
            {
                if (i != 0)
                {
                    if (IdArray.Contains(dataitem.Attribute("id").Value))
                    {
                        t = false;
                        Console.WriteLine("Id value "+dataitem.Attribute("id").Value+" is not unique ."); 
                    }
                }
                IdArray[i] = dataitem.Attribute("id").Value;
                i++;
            }
            Assert.True(t);
        }

        [Test, Description("The data item MUST specify the type of data being collected, the id of the data item, and the category of the item.The dataitem type must be all in capitals with an underscore(_) between words.")]
        public void T113_DataItem_Must_Specify_Type_Id_Category()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            bool t1 = true;
            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");
            foreach (XElement dataitem in dataitems)
            {
                t1=t1&(dataitem.Attributes("category").Count() > 0)&(dataitem.Attributes("id").Count() > 0)&(dataitem.Attributes("type").Count() > 0);
                if (dataitem.Attributes("category").Count() == 0)
                    Console.WriteLine(@"The category is not specified for dataItem(id=""" + dataitem.Attribute("id").Value + @""").");
                if (dataitem.Attributes("id").Count() == 0)
                    Console.WriteLine(@"The id is not specified for dataItem(type=""" + dataitem.Attribute("type").Value + @""").");
                if (dataitem.Attributes("type").Count() == 0)
                    Console.WriteLine(@"The type is not specified for dataItem(id=""" + dataitem.Attribute("id").Value + @""").");
            }
            Assert.True(t1);
        }

        [Test, Description("The units MUST be specified for any data item with category Sample.")]
        public void T114_Units_Must_Be_Specified_For_Sample()
        {
            string result = ParseXML.DoProbe("probe");
            XElement proberesult = XElement.Parse(result);
            bool t1 = true;
            IEnumerable<XElement> dataitems = proberesult.Descendants(MTConnectNameSpace.mtDevices + "DataItem");
            foreach (XElement dataitem in dataitems)
            {
                if (dataitem.Attribute("category").Value == "SAMPLE")
                {
                    t1 = t1 & (dataitem.Attributes("units").Count() > 0);
                    if (dataitem.Attributes("units").Count() == 0)
                        Console.WriteLine(@"The unit is not specified for dataItem(id="""+dataitem.Attribute("id").Value+@""").");
                }

            }
            Assert.True(t1);
        }

        
    }
}

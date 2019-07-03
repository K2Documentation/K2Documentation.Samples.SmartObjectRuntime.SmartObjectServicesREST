using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace K2Learning.Samples.SmartObjectRuntime.SmartObjectServicesREST
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting SmartObject REST call");
            Console.WriteLine("##############################################");
            ProcessSmartObjectResult(GetSmartObjectResult());
            Console.WriteLine("##############################################");
            Console.WriteLine("Completed SmartObject REST call");
            Console.ReadLine();
        }

        private static XDocument GetSmartObjectResult()
        {
            // create the web request to the REST server endpoint for the "Get Process Instance Detail" method of the "Workflow Instance Report" SmartObject
            //your URL will be different and depends on the configuration of Service Endpoints specified int he host server configuraiton file
            //note the space handling - the REST service endpoints use DISPLAY names, not SYSTEM names
            string ListUsersRESTURL = @"http://k2.denallix.com:8888/SmartObjectServices/rest/System/Reports/Workflow%20Instance%20Report/Get%20Process%20Instance%20Detail?$format=XML";
            HttpWebRequest request = WebRequest.Create(ListUsersRESTURL) as HttpWebRequest;
            // Assign Credentials to the HTTP request
            request.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            //prepare a XDocument to hold the XML response
            XDocument responseDocument;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            using (Stream responseStream = response.GetResponseStream())
            using (XmlReader responseReader = XmlReader.Create(responseStream))
            {
                responseDocument = XDocument.Load(responseReader);
            }

            return responseDocument;
        }

        //process and output the results ot the console
        private static void ProcessSmartObjectResult(XDocument responseDocument)
        {
            //construct the LINQ query to select process instance from the list of process instances
            var processInstancesQuery =
               from processInstancesCollection in responseDocument.Elements("ArrayOfWorkflowReportingService_WorkflowInstanceReport")
               from processInstance in processInstancesCollection.Elements("WorkflowReportingService_WorkflowInstanceReport")
               select new
               {
                   folio = (string)processInstance.Element("ProcessInstanceFolio"),
                   processInstanceId = (string)processInstance.Element("ProcessInstanceID"),
               };

            foreach (var processInstance in processInstancesQuery)
            {
                Console.WriteLine("Folio:" + processInstance.folio + " | Process Instance ID:" + processInstance.processInstanceId);
            }
        }
    }
}

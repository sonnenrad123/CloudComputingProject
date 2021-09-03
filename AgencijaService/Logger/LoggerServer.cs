using Common;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    class LoggerServer
    {
        private ServiceHost serviceHost;
        // dodati endpoint sa ovim imenom u ServiceDefinition
        private String externalEndpointName = "InputRequest"; //ime endpointa u properties
        public LoggerServer()
        {
            // uzimamo endpoint procesa koji se izvrsava a da je vrste externalEndpoint name
            RoleInstanceEndpoint inputEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[externalEndpointName];
            //formatiranje endpoint-a uz uzimanje ip adrese i lokacije na kojem se nalazi
            string endpoint = String.Format("net.tcp://{0}/{1}", inputEndPoint.IPEndpoint, externalEndpointName);
            //Pravimo klasican servicehost typeof(gde smo implementirali interfejs)
            serviceHost = new ServiceHost(typeof(ILoggerImplemented));
            NetTcpBinding binding = new NetTcpBinding();
            //dodajemo host
            serviceHost.AddServiceEndpoint(typeof(ILogger), binding, endpoint);
        }
        public void Open()
        {
            try
            {
                serviceHost.Open();
                Trace.TraceInformation(String.Format("Host for {0} endpoint type opened successfully.", externalEndpointName));
            }
            catch (Exception e)
            {
                Trace.TraceInformation(String.Format("Host open error for InputRequest endpoint type. Error  message is: ") + e.Message);
            }
        }
        public void Close()
        {
            try
            {
                serviceHost.Close();
                Trace.TraceInformation(String.Format("Host for {0} endpoint type closed successfully at {1} ", externalEndpointName, DateTime.Now));
            }
            catch (Exception e)
            {
                Trace.TraceInformation(String.Format("Host close error for {0} endpoint type. Error message is: {1}. ", externalEndpointName, e.Message));
            }
        }
    }
}

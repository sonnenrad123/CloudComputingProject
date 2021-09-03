using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerPooling();
        }

        static void LoggerPooling()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<ILogger> factory = new ChannelFactory<ILogger>(binding, new EndpointAddress("net.tcp://localhost:10100/InputRequest"));
            while (true)
            {
                ILogger proxy = factory.CreateChannel();
                string logovi = proxy.IzvuciLogoveIzBloba();

                if (logovi != "" && logovi != null)
                {
                    string[] redovi = logovi.Split('|');
                    foreach(string red in redovi)
                    {
                        Console.WriteLine(red);
                    }
                }
                Thread.Sleep(1000);
            }


        }
    }
}

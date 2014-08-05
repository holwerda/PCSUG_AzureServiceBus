using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DemoRelayClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Your Service Namespace: ");
            string serviceNamespace = Console.ReadLine();
            Console.Write("Your Issuer Name: ");
            string issuerName = Console.ReadLine();
            Console.Write("Your Issuer Secret: ");
            string issuerSecret = Console.ReadLine();

            TransportClientEndpointBehavior relayCredentials = new TransportClientEndpointBehavior();
            relayCredentials.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "CalculatorService");
            ChannelFactory<ICalculatorChannel> channelFactory = new ChannelFactory<ICalculatorChannel>("RelayEndpoint", new EndpointAddress(serviceUri));
            channelFactory.Endpoint.Behaviors.Add(relayCredentials);
            ICalculatorChannel channel = channelFactory.CreateChannel();
            channel.Open();

            IHybridConnectionStatus hybridConnectionStatus = channel.GetProperty<IHybridConnectionStatus>();
            hybridConnectionStatus.ConnectionStateChanged += (o, e) =>
            {
                Console.WriteLine("Upgraded!");
            };

            Console.WriteLine("Press any key to exit");

            DateTime lastTime = DateTime.Now;
            int count = 0;

            while (!Console.KeyAvailable)
            {
                channel.add(1, 3);

                count++;
                if (DateTime.Now - lastTime > TimeSpan.FromMilliseconds(250))
                {
                    lastTime = DateTime.Now;
                    Console.WriteLine("     Sent {0} messages...", count);
                    count = 0;
                }
            }

            channel.Close();
            channelFactory.Close();
        }
    }
}

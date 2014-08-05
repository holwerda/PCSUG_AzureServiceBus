using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoQueueingSender
{
    class Program
    {
        public const string defaultAppSettingValue = "Endpoint=sb://[your namespace].servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[your secret]";

        static void Main(string[] args)
        {

            //Make sure to set value in config for appSetting key: Microsoft.ServiceBus.ConnectionString
            if (ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"] == defaultAppSettingValue)
            {
                Console.WriteLine("You need to set your own service bus endpoint in the config file.");
                Console.WriteLine("Press any key to Exit");
                Console.ReadKey();
                return;
            }


            Console.WriteLine("Enter queue name then press Enter");
            var queueName = Console.ReadLine();  //not validating because we won't enter bad data :)



            //it is a good practice to try and create queue instead of hoping it is there
            CreateQueue(queueName);

            var keyEntered = new ConsoleKeyInfo();

            do
            {
                Console.WriteLine("Enter message then press Enter");
                var message = Console.ReadLine();  //not validating because we won't enter bad data :)

                SendMessage(message, queueName);

                Console.WriteLine("Press 'q' to quit or any other key to get more messages");
                keyEntered = Console.ReadKey();
                Console.WriteLine("\n");
            } while (keyEntered.KeyChar.ToString() != "q");

        }

        private static void SendMessage(string message, string queueName)
        {
            var client = Microsoft.ServiceBus.Messaging.QueueClient.Create(queueName);      //using fully qualified namespace so you can see where it resides

            var brokeredMessage = new Microsoft.ServiceBus.Messaging.BrokeredMessage(message);      //using fully qualified namespace so you can see where it resides

            try
            {
                client.Send(brokeredMessage);
            }
            catch (Microsoft.ServiceBus.Messaging.MessagingException e)     //using fully qualified namespace so you can see where it resides
            {
                if (!e.IsTransient)   //indicates whethere you should retry or not = along the lines of network issues.
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                else
                {
                    //HandleTransientErrors(e);
                }
            }
        }

        private static void CreateQueue(string queueName)
        {

            var manager = Microsoft.ServiceBus.NamespaceManager.Create();       //using fully qualified namespace so you can see where it resides

            if (!manager.QueueExists(queueName))
            {
                manager.CreateQueue(queueName);
            }

        }
    }
}

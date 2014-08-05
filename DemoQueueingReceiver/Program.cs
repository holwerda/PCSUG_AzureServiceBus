using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoQueueingReceiver
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
                GetMessages(queueName);
                Console.WriteLine("Press 'q' to quit or any other key to get more messages");
                keyEntered = Console.ReadKey();
                Console.WriteLine("\n");
            } while (keyEntered.KeyChar.ToString() != "q");


        }

        private static void GetMessages(string queueName)
        {
            Microsoft.ServiceBus.Messaging.BrokeredMessage message = null;

            var receivingClient = Microsoft.ServiceBus.Messaging.QueueClient.Create(queueName);      //using fully qualified namespace so you can see where it resides

            while (true)
            {
                try
                {

                    message = receivingClient.Receive(TimeSpan.FromSeconds(5));
                    if (message != null)
                    {
                        Console.WriteLine(string.Format("Message received: {0}", message.GetBody<string>()));

                        message.Complete();
                    }
                    else
                    {
                        //no more messages in the queue
                        Console.WriteLine("No Messages");

                        break;
                    }
                }
                catch (Microsoft.ServiceBus.Messaging.MessagingException e)
                {
                    if (!e.IsTransient)
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
            receivingClient.Close();
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

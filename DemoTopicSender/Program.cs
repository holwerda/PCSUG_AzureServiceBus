using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTopicSender
{
    class Program
    {
        public const string defaultAppSettingValue = "Endpoint=sb://[your namespace].servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[your secret]";
        
        public const string topicName = "pcsugDemoTopic";
        public const string subscription1Name = "Bucket1";
        public const string subscription2Name = "Bucket2";

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

            //it is a good practice to try and create topic instead of hoping it is there
            CreateTopic();

            var keyEntered = new ConsoleKeyInfo();

            do
            {
                Console.WriteLine("Enter message then press Enter");
                var message = Console.ReadLine();  //not validating because we won't enter bad data :)

                Console.WriteLine("Enter priority 1 or 2 then press Enter");
                var priority = Console.ReadLine();  //not validating because we won't enter bad data :)

                SendMessage(message, priority);

                Console.WriteLine("Press 'q' to quit or any other key to send more messages");
                keyEntered = Console.ReadKey();
                Console.WriteLine("\n");
            } while (keyEntered.KeyChar.ToString() != "q");

        }

        private static void SendMessage(string message, string priority)
        {
            var client = Microsoft.ServiceBus.Messaging.TopicClient.Create(topicName);      //using fully qualified namespace so you can see where it resides

            var brokeredMessage = new Microsoft.ServiceBus.Messaging.BrokeredMessage(message);      //using fully qualified namespace so you can see where it resides
            brokeredMessage.Properties.Add("Priority", Convert.ToInt16(priority));

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

        private static void CreateTopic()
        {

            var manager = Microsoft.ServiceBus.NamespaceManager.Create();       //using fully qualified namespace so you can see where it resides

            if (!manager.TopicExists(topicName))
            {
                manager.CreateTopic(topicName);
                {


                    Microsoft.ServiceBus.Messaging.TopicDescription myTopic = manager.GetTopic(topicName);

                    Console.WriteLine("Creating Subscriptions '{0}' and '{1}'...", subscription1Name, subscription2Name);

                    if (!manager.SubscriptionExists(topicName, subscription1Name))
                    {
                        Microsoft.ServiceBus.Messaging.SubscriptionDescription myAuditSubscription = manager.CreateSubscription(myTopic.Path, subscription1Name);
                    }

                    if (!manager.SubscriptionExists(topicName, subscription2Name))
                    {
                        var subscriptionFilter = new Microsoft.ServiceBus.Messaging.SqlFilter("Priority = 1");

                        Microsoft.ServiceBus.Messaging.SubscriptionDescription myAgentSubscription = manager.CreateSubscription(myTopic.Path, subscription2Name, subscriptionFilter);
                    }

                }
            }
        }
    }
}

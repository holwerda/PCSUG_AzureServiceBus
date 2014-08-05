using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTopicSubscriber
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

            Console.WriteLine("Enter 1 for {0} or 2 for {1}", subscription1Name, subscription2Name);
            var bucketNumber = Convert.ToInt16(Console.ReadKey().KeyChar.ToString());  //not validating because we won't enter bad data :)

            //it is a good practice to try and create queue instead of hoping it is there
            CreateTopic();

            var keyEntered = new ConsoleKeyInfo();                      

            do
            {
                GetMessages(bucketNumber);
                Console.WriteLine("Press 'q' to quit or any other key to get more messages");
                keyEntered = Console.ReadKey();
                Console.WriteLine("\n");
            } while (keyEntered.KeyChar.ToString() != "q");


        }

        private static void GetMessages(int bucketNumber)
        {
            Console.WriteLine("Getting Messages");

            Microsoft.ServiceBus.Messaging.BrokeredMessage message = null;
            //note: ReceiveMode.ReceiveAndDelete has the potential to lose messages as it will delete it on the request when getting new messages.  Use default PeekLock if you want guaranteed delivery.
            Microsoft.ServiceBus.Messaging.SubscriptionClient bucketSubscriptionClient = Microsoft.ServiceBus.Messaging.SubscriptionClient.Create(topicName, String.Format("Bucket{0}", bucketNumber), Microsoft.ServiceBus.Messaging.ReceiveMode.ReceiveAndDelete);
            while (true)
            {
                try
                {
                    message = bucketSubscriptionClient.Receive(TimeSpan.FromSeconds(5));
                    
                    if (message != null)
                    {
                        Console.WriteLine(string.Format("Message: {0}", message.GetBody<string>()));
                        // Further custom message processing could go here...

                    }
                    else
                    {
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
                }
            }

            bucketSubscriptionClient.Close();
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
    

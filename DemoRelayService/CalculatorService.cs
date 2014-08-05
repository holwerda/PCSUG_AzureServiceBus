//---------------------------------------------------------------------------------
// Microsoft (R)  Windows Azure SDK
// Software Development Kit
// 
// Copyright (c) Microsoft Corporation. All rights reserved.  
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
//---------------------------------------------------------------------------------

namespace DemoRelayService
{
    using System;
    using System.ServiceModel;

    [ServiceBehavior(Name = "CalculatorService", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    class CalculatorService : ICalculatorContract
    {
        public void add(int num1, int num2)
        {
            Console.WriteLine("Sum of {0} and {1} is  {2} ", num1, num2, num1 + num2);
        }
    }
}

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

namespace DemoRelayClient
{
    using System;
    using System.ServiceModel;

    [ServiceContract(Name = "ICalculatorContract", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    public interface ICalculatorContract
    {
        [OperationContract(IsOneWay = true)]
        void add(int num1, int num2);
    }

    public interface ICalculatorChannel : ICalculatorContract, IClientChannel { }
}

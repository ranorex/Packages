using System;
using System.Collections.Generic;

namespace Ranorex.NeoLoad
{
    internal interface INeoloadApi
    {
        void ConnectToRuntimeApi(string runtimeUrl, string apiKey);
        void ConnectToDataExchangeApi(string dataExchangeUrl, string apiKey, NeoloadApi.NeoloadContextData ctx);
        void AddVirtualUsers(string population, int ammount);
        void RemoveVirtualUsers(string population, int ammount);
        void SendNavTiming(
            IEnumerable<NavigationTimingWrapper.NavTiming> navtiming,
            string transactionName,
            string url,
            string browser,
            string testCase);
        void StartNeoLoadTest(string scenario, TimeSpan timeout, TimeSpan interval);
        void StopNeoLoadTest(TimeSpan timeout, TimeSpan interval);
    }
}

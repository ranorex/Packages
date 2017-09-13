///////////////////////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the  R A N O R E X  Project.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Neotys.DataExchangeAPI.Client;
using Neotys.DataExchangeAPI.Model;
using Neotys.RuntimeAPI.Client;
using Neotys.RuntimeAPI.Model;
using NtState = global::Neotys.RuntimeAPI.Model.Status;

namespace Ranorex.NeoLoad
{
    internal class NeoloadApi : INeoloadApi
    {
    	public static INeoloadApi Instance { get; private set; }

        public struct NeoloadContextData
        {
            public string hardware;
            public string location;
            public string software;
            public string script;
            public string osFriendlyName;
        }

        private IDataExchangeAPIClient dataExchangeClient;
        private IRuntimeAPIClient runtimeClient;
        private Context context;

        static NeoloadApi()
        {
        	Instance = new NeoloadApi();
        }
        
        private NeoloadApi()
        {

        }

        public void ConnectToRuntimeApi(string runtimeApiUrl, string apiKey)
        {
            this.runtimeClient = RuntimeAPIClientFactory.NewClient(runtimeApiUrl, apiKey);
        }

        public void ConnectToDataExchangeApi(string dataExchangeApiUrl, string apiKey, NeoloadContextData ctx)
        {
            context = CreateContext(ctx);
            this.dataExchangeClient = DataExchangeAPIClientFactory.NewClient(dataExchangeApiUrl, context, apiKey);
        }

        private static Context CreateContext(NeoloadContextData ctx)
        {
            ContextBuilder cb = new ContextBuilder();
            cb.Hardware = ctx.hardware;
            cb.Location = ctx.location;
            cb.Software = ctx.software;
            cb.Script = ctx.script;
            cb.Os = ctx.osFriendlyName;

            return cb.build();
        }

        public void SendNavTiming(
            IEnumerable<NavigationTimingWrapper.NavTiming> navtiming,
            string transactionName,
            string url,
            string browser,
            string testCase)
        {
            CheckDataExchangeIsConnected();
            var entriesToSend = navtiming.Select(nt =>
                BuildEntry(
                    nt,
                    url,
                    BuildArgList(
                         testCase,
                         browser,
                         transactionName,
                         nt.Key,
                         nt.SubPath)))
                .ToList();

            this.dataExchangeClient.AddEntries(entriesToSend);
        }

        public void StartNeoLoadTest(string scenario, TimeSpan timeout, TimeSpan interval)
        {
            this.CheckRuntimeIsConnected();

            var curState = this.runtimeClient.getStatus();
            switch (curState)
            {
                case NtState.NO_PROJECT: throw new InvalidOperationException("Failed to start test because no Project is loaded in NeoLoad.");
                case NtState.TEST_RUNNING: throw new InvalidOperationException("A Neotys test is already running. Cannot start new test run.");
                default:
                    this.WaitForNeoloadState(NtState.READY, timeout, interval);
                    this.runtimeClient.StartTest(new StartTestParamsBuilder(scenario).Build());
                    this.WaitForNeoloadState(NtState.TEST_RUNNING, timeout, interval);
                    break;
            }
        }

        public void StopNeoLoadTest(TimeSpan timeout, TimeSpan interval)
        {
            this.CheckRuntimeIsConnected();

            var curState = this.runtimeClient.getStatus();
            switch (curState)
            {
                case NtState.TEST_STOPPING: Report.Warn("Cannot stop NeoLoad test because it is already stopping."); break;
                case NtState.TEST_RUNNING:
                    this.runtimeClient.StopTest(new StopTestParamsBuilder().Build());
                    this.WaitForNeoloadState(NtState.READY, timeout, interval);
                    break;
                default:
                    var s = this.runtimeClient.getStatus();
                    throw new Exception(string.Format("Cannot stop NeoLoad test because no test is running. The current status of the NeoLoad system is '{0}'.", s));
            }
        }

        private void WaitForNeoloadState(NtState state, TimeSpan timeout, TimeSpan interval)
        {
            if (!RetryUntil(() => this.runtimeClient.getStatus() == state, timeout, interval))
            {
                var curState = this.runtimeClient.getStatus();
                throw new Exception(string.Format("Failed to wait for NeoLoad state '{0}' as it was not reached within the given timeout of '{1}' with a check interval of '{2}'. The last retrieved status was '{3}'.", state, timeout, interval, curState));
            }
        }

        private static bool RetryUntil(Func<bool> check, TimeSpan timeout, TimeSpan interval)
        {
            for (var start = System.DateTime.UtcNow; System.DateTime.UtcNow < start + timeout; /* No increment */)
            {
            	Thread.Sleep(interval);
                if (check())
                {
                    return true;
                }
            }

            return false;
        }

        public void AddVirtualUsers(string population, int ammount)
        {
            this.CheckRuntimeIsConnected();

            if (runtimeClient.getStatus() != NtState.TEST_RUNNING)
            {
                throw new Exception("Cannot add/remove virtual users when no NeoLoad test is running.");
            }

            this.runtimeClient.AddVirtualUsers(new AddVirtualUsersParamsBuilder(population, ammount).Build());
        }

        public void RemoveVirtualUsers(string population, int ammount)
        {
            this.CheckRuntimeIsConnected();

            if (runtimeClient.getStatus() != NtState.TEST_RUNNING)
            {
                throw new Exception("Cannot add/remove virtual users when no NeoLoad test is running.");
            }

            this.runtimeClient.StopVirtualUsers(new StopVirtualUsersParamsBuilder(population, ammount).Build());
        }

        private void CheckRuntimeIsConnected()
        {
            if (this.runtimeClient == null)
            {
                throw new InvalidOperationException(string.Format(
            		"Not connected to NeoLoad runtime API. NeoLoad actions cannot be used until a connection to a " + 
            		"NeoLoad server was established. Please add a '{0}' module to your test suite that is executed " + 
            		"before any NeoLoad action is invoked.", "ConnectToRuntimeApi"));
            }
        }

        private void CheckDataExchangeIsConnected()
        {
            this.CheckRuntimeIsConnected();
            if (this.dataExchangeClient == null)
            {
                throw new InvalidOperationException(string.Format("Not connected to NeoLoad data exchange API. This action requires such a connection. Please add a '{0}' module to your test suite that is executed before this action.", "ConnectDataExchangeApi"));
            }
        }

        private static Entry BuildEntry(
            NavigationTimingWrapper.NavTiming timing,
            string URL,
            IList<string> pathArgumentList)
        {
            EntryBuilder eb = new EntryBuilder(pathArgumentList);
            eb.Unit = "Milliseconds";
            eb.Value = timing.Value;
            eb.Url = URL;

            return eb.Build();
        }

        private static List<string> BuildArgList(
            string testCase,
            string browser,
            string transaction,
            string param,
            IEnumerable<string> subPath)
        {
            List<string> path = CreateCommonRootPath(testCase, browser, transaction);
            path.AddRange(subPath);
            path.Add(param);

            return path;
        }

        private static List<string> CreateCommonRootPath(string testCase, string browser, string transaction)
        {
            return new List<string>()
            {
                testCase,
                transaction,
            };
        }
    }
}

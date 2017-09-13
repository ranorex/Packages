///////////////////////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the  R A N O R E X  Project.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Management;
using Ranorex.Core.Testing;

namespace Ranorex.NeoLoad
{
    /// <summary>
    /// Establishes a connection with the NeoLoad data exchange API when run.
    /// </summary>
    /// <remarks>
    /// A connection to the data exchange API can only be established after a connection to the runtime API
    /// was established.
    /// </remarks>
    [TestModule("E9E07FEF-4B9A-4B90-8C96-716398AE1FE2", ModuleType.UserCode)]
    public sealed class ConnectDataExchangeApi : ITestModule
    {
        internal static INeoloadApi api = NeoloadApi.Instance;

        /// <summary>
        /// The URI to the data exchange API
        /// </summary>
        [TestVariable("BE94795A-B55A-4259-A35A-BD824282231A")]
        public string DataExchangeApiUri { get; set; }

        /// <summary>
        /// The API key for the API. Needs to be set according to the NeoLoad server settings.
        /// </summary>
        [TestVariable("95467160-1711-4BD2-8843-980EDF15E6D1")]
        public string ApiKey { get; set; }

        /// <summary>
        /// Location string. This string will show up on the server and help you identify your test scenario.
        /// </summary>
        [TestVariable("4AE03C84-5409-49F1-B2DB-367E3DB763D4")]
        public string Location { get; set; }

        /// <summary>
        /// Hardware string. This string will show up on the server and help you identify your test scenario.
        /// </summary>
        [TestVariable("97D77805-2808-4236-81FB-6A52F1503615")]
        public string Hardware { get; set; }


        /// <summary>
        /// Software string. This string will show up on the server and help you identify your test scenario.
        /// </summary>
        [TestVariable("673C217F-E8B4-484F-BF21-301E3A16C23B")]
        public string Software { get; set; }
                
        public ConnectDataExchangeApi()
        {
        	this.Hardware = "Example Hardware";
        	this.Software = "Example Software";
        }
        
        public void Run()
        {
            if (string.IsNullOrWhiteSpace(DataExchangeApiUri))
            {
                throw new InvalidOperationException("No runtime API URI was provided. Cannon connect to NeoLoad server.");
            }

            if (string.IsNullOrWhiteSpace(this.ApiKey))
            {
                throw new InvalidOperationException("No NeoLoad API Key provided.");
            }
            
            string tsName = "<No test suite available>";
            if(TestSuite.Current != null) 
            {
            	tsName = TestSuite.Current.Name;
            }

            var ctx = new NeoloadApi.NeoloadContextData()
            {
                hardware = this.Hardware,
                software = this.Software,
                location = this.Location,
                script = tsName,
                osFriendlyName = GetOSFriendlyName(),
            };

            api.ConnectToDataExchangeApi(this.DataExchangeApiUri, this.ApiKey, ctx);
        }

        private static string GetOSFriendlyName()
        {
        	return Host.Local.OSEdition;
        }
    }
}

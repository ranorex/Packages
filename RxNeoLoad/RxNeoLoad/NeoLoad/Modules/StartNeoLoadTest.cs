///////////////////////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the  R A N O R E X  Project.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using Ranorex.Core.Testing;

namespace Ranorex.NeoLoad
{
    /// <summary>
    /// Starts a NeoLoad test
    /// </summary>
    /// <remarks>
    /// A NeoLoad test can only be started, when a connection to the runtime and data exchange API was
    /// established prior.
    /// </remarks>
    [TestModule("1702CBFB-DA15-4C42-B6C4-6FCBA8DAE96F", ModuleType.UserCode, 1)]
    public class StartNeoLoadTest : ITestModule
    {
        // For testing, make it mockable
        internal static INeoloadApi api = NeoloadApi.Instance;
        
        /// <summary>
        /// Check interval for operations that can timeout in the format 'hh:mm:ss'.
        /// This interval needs to be smaller than the timeout.
        /// </summary>
        [TestVariable("29B0EB4D-D82E-4DDA-8ABB-355734B346D2")]
        public string Interval { get; set; }
        /// <summary>
        /// Timeout for the connect operation in the format 'hh:mm:ss'.
        /// </summary>
        [TestVariable("7E6AAB81-CA2F-4084-A2B2-C0B8792122A1")]
        public string Timeout { get; set; }

        /// <summary>
        /// The name of the scenario. Use this info tho identify the collected data on the NeoLoad server system.
        /// </summary>
        [TestVariable("7242C624-2E81-447E-A148-533E4BB082BE")]
        public string Scenario { get; set; }

        public StartNeoLoadTest()
        {
        	this.Interval = "00:00:10";
        	this.Timeout = "00:01:00";
        }
        
        void ITestModule.Run()
        {
            if (string.IsNullOrWhiteSpace(this.Scenario))
            {
                throw new InvalidOperationException("No scenario name available. Please set a valid NeoLoad scenario name.");
            }


            try
            {
                const string fmt = @"hh\:mm\:ss";
                var timeout = TimeSpan.ParseExact(this.Timeout, fmt, CultureInfo.InvariantCulture);
                var interval = TimeSpan.ParseExact(this.Interval, fmt, CultureInfo.InvariantCulture);

                if (timeout < interval)
                {
                    throw new ArgumentException(string.Format("The given timeout of '{0}' is smaller than the interval with a value of '{1}', but interval has to be smaller than timeout.",
                        timeout.ToString(fmt), interval.ToString(fmt)));
                }

                api.StartNeoLoadTest(this.Scenario, timeout, interval);
            }
            catch (FormatException ex)
            {
                throw new Exception("'Timeout' or 'Interval' was specified with invalid format. Please use the format 'hh:mm:ss' e.g. '00:01:10' for one minute and ten seconds." + ex);
            }
        }
    }
}

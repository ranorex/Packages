///////////////////////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the  R A N O R E X  Project.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Ranorex.Core.Testing;

namespace Ranorex.NeoLoad
{
    /// <summary>
    /// Establishes a connection with the NeoLoad runtime API when run.
    /// </summary>
    [TestModule("0E3400B0-D8D5-406D-A350-A9978FAB51AB", ModuleType.UserCode, 1)]
    public class ConnectToRuntimeApi : ITestModule
    {
        // For testing, make it mockable
        internal static INeoloadApi api = NeoloadApi.Instance;

        /// <summary>
        /// The URI to the data runtime API
        /// </summary>
        [TestVariable("F000412E-4C82-4363-92C3-0D0B8A291B0F")]
        public string RuntimeApiUri { get; set; }

        /// <summary>
        /// The API key for the API. Needs to be set according to the NeoLoad server settings.
        /// </summary
        [TestVariable("DE7E55A7-F558-4861-8738-C6D6C2D9EE37")]
        public string ApiKey { get; set; }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            // check variable content
            if (string.IsNullOrWhiteSpace(this.RuntimeApiUri))
            {
                throw new InvalidOperationException("No runtime API URL provided. Cannot connect to NeoLoad server provided.");
            }

            if (string.IsNullOrWhiteSpace(this.ApiKey))
            {
                throw new InvalidOperationException("No NeoLoad API Key provided.");
            }

            api.ConnectToRuntimeApi(this.RuntimeApiUri, this.ApiKey);
        }
    }
}

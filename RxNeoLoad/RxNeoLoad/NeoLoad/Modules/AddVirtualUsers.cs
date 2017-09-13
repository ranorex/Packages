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
    /// Adds virtual users to a NeoLoad test
    /// </summary>
    [TestModule("A23B027C-8602-48D8-87F1-1A5CAC12FA52", ModuleType.UserCode, 1)]
    public class AddVirtualUsers : ITestModule
    {
        // For testing, make it mockable
        internal static INeoloadApi api = NeoloadApi.Instance;

        /// <summary>
        /// Name of the population the virtual users should be added to.
        /// </summary>
        [TestVariable("7C18803A-1D5E-48B3-8078-85E01F412489")]
        public string Population { get; set; }

        /// <summary>
        /// Number of virtual users that should get added.
        /// </summary>
        [TestVariable("48097C49-672A-4651-A1CB-D2D0D516916D")]
        public int Amount { get; set; }

        public AddVirtualUsers()
        {
        	this.Amount = 1;
        }
        
        void ITestModule.Run()
        {
            if (string.IsNullOrWhiteSpace(this.Population))
            {
                throw new InvalidOperationException("No population set where the virtual users should be get added/removed. Please set a valid population.");
            }

            if (this.Amount <= 0)
            {
                throw new ArgumentOutOfRangeException("Amount");
            }

            api.AddVirtualUsers(this.Population, Amount);
        }
    }
}

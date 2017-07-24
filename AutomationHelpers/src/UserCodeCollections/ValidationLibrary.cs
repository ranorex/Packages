//
// Copyright © 2017 Ranorex All rights reserved
//

using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of useful validation helper methods.
    /// </summary>
    [UserCodeCollection]
    public static class ValidationLibrary
    {
        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <param name="value1">First value to compare</param>
        /// <param name="value2">Second value to compare</param>
        [UserCodeMethod]
        public static void CompareValues(string value1, string value2)
        {
            Validate.AreEqual(value1, value2);
        }
    }
}

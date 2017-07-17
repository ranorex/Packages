// TODO: Add Header

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    [UserCodeCollection]
    class StringLibrary
    {
        /// <summary>
        /// Concatenates two strings and returns the new string.
        /// </summary>
        /// <param name="value1">First string</param>
        /// <param name="value2">Second string</param>
        /// <returns>Concatenated string</returns>
        [UserCodeMethod]
        public static string ConcatStrings(string value1, string value2)
        {
            return String.Concat(value1, value2);
        }
    }
}

//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of string helper methods.
    /// </summary>
    [UserCodeCollection]
    public static class StringLibrary
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

        /// <summary>
        /// Creates and returns a random string. If a string shorter than 35 characters is used it may not be unique.
        /// </summary>
        /// <param name="length">Expected length of the return value (default length is 35)</param>
        /// <returns>Random string</returns>
        [UserCodeMethod]
        public static string GetRandomString(string length)
        {
            string returnValue = "";

            if (length == null || length.Equals(""))
            {
                length = "35";
            }

            if (Int32.Parse(length) > 35)
            {
                returnValue = getRandomString((Int32.Parse(length) - 36).ToString());
                return returnValue + Guid.NewGuid().ToString();
            }
            else
            {
                returnValue = Guid.NewGuid().ToString().Substring(0, Int32.Parse(length));
            }

            return returnValue;
        }
    }
}

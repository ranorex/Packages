// This file is part of the  R A N O R E X  Project. | http://www.ranorex.com

using System;
using System.Text;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception exception)
        {
            return GetFullMessage(exception, " ");
        }

        public static string GetFullMessage(this Exception exception, string delimiter)
        {
            Utils.CheckArgumentNotNull(delimiter, "delimiter");

            var sb = new StringBuilder(exception.Message);

            var currentException = exception.InnerException;
            while (currentException != null)
            {
                sb.Append(delimiter);
                sb.Append(currentException.Message);
                currentException = currentException.InnerException;
            }

            return sb.ToString();
        }
    }
}



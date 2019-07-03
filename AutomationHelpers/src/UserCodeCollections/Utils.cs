// This file is part of the  R A N O R E X  Project. | http://www.ranorex.com

using System;
using Ranorex.Core;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    public static class Utils
    {
        public static void CheckArgumentNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ReportException(Exception exception, string source)
        {
            CheckArgumentNotNull(exception, "exception");
            CheckArgumentNotNull(source, "source");

            Report.Log(
                ReportLevel.Error,
                source,
                exception.GetFullMessage(),
                new SimpleReportMetadata("stacktrace", exception.StackTrace));
        }
    }
}

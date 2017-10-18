//
// Copyright © 2017 Ranorex All rights reserved
//

using System.Collections.Generic;
using Ranorex;
using Ranorex.Core;

namespace RanorexAutomationHelpers.Test
{
    internal sealed class TestReportLogger : IReportLogger
    {
        public bool PreFilterMessages { get; private set; }

        internal string LastLogMessage { get; set; }

        public void End()
        {
            // Method intentionally left empty.
        }

        public void LogData(ReportLevel level, string category, string message, object data, IDictionary<string, string> metaInfos)
        {
            this.LastLogMessage = message;
        }

        public void LogText(ReportLevel level, string category, string message, bool escape, IDictionary<string, string> metaInfos)
        {
            this.LastLogMessage = message;
        }

        public void Start()
        {
            // Method intentionally left empty.
        }
    }
}



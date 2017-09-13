///////////////////////////////////////////////////////////////////////////////////////////////////
//
// This file is part of the  R A N O R E X  Project.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace Ranorex.NeoLoad
{
    /// <summary>
    /// Wrapper class, extracting navigation timing values from a website.
    /// Ranorex "ExecuteScript" is used to inject JS code, retrieving the values.
    /// Further information about navigation timing can be found here:
    /// https://www.w3.org/TR/navigation-timing/
    ///
    /// Note:
    /// Upgrade to navigation timing level 2 to be done in future:
    /// https://www.w3.org/TR/navigation-timing-2/
    /// cbreit - 26.01.2016: still,in draft mode and not supported by all browsers
    /// </summary>
    internal static class NavigationTimingWrapper
    {
        internal class NavTiming
        {
            private static readonly List<string> NoSubPath = new List<string>();
            public string Key { get; private set; }
            public long Value { get; private set; }
            public IEnumerable<string> SubPath { get; private set; }

            public NavTiming(string key, long value)
                : this(key, value, NoSubPath)
            {
            }

            public NavTiming(string key, long value, IEnumerable<string> subPath)
            {
                this.Key = key;
                this.Value = value;
                this.SubPath = subPath ?? NoSubPath;
            }
        }

        private static readonly List<string> AdvancedSubPath = new List<string>() { "Advanced" };

        private static readonly List<KeyMapping> KeyMappings = new List<KeyMapping>()
        {
            new KeyMapping("redirect") { ResultKey = "Redirect", SubPath = AdvancedSubPath },
            new KeyMapping("unloadEvent") { ResultKey = "Unload", SubPath = AdvancedSubPath },
            new KeyMapping("domainLookup") { ResultKey = "DNS", SubPath = AdvancedSubPath },
            new KeyMapping("connect") { ResultKey = "TCP", SubPath = AdvancedSubPath },
            new KeyMapping("responseStart", "requestStart", "Request") { SubPath = AdvancedSubPath },
            new KeyMapping("response") { ResultKey = "Response", SubPath = AdvancedSubPath },
            new KeyMapping("domLoading", "domComplete", "Processing") { SubPath = AdvancedSubPath },
            new KeyMapping("loadEvent") { ResultKey = "onLoad", SubPath = AdvancedSubPath },
            new KeyMapping("navigationStart", "loadEventEnd", "User Experience"),
        };

        private class KeyMapping
        {
            public string StartKey { get; private set; }
            public string EndKey { get; private set; }
            public string ResultKey { get; set; }

            public IList<string> SubPath { get; set; }

            public KeyMapping(string key)
                : this(key + "Start", key + "End", key)
            {
                this.ResultKey = key;
            }

            public KeyMapping(string startKey, string endKey, string resultKey)
            {
                this.StartKey = startKey;
                this.EndKey = endKey;
                this.ResultKey = resultKey;
            }
        }

        public static IEnumerable<NavTiming> ReadTimingValuesFromPage(WebDocument doc)
        {

            string supported = doc.ExecuteScript("return (window.performance != null);").Trim();

            if (!supported.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                throw (new InvalidOperationException("Reading timing values from browser failed because it is not supported."));
            }

            var jsonString = doc.ExecuteScript("return JSON.stringify(performance.timing);");
            var timings = JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonString);

            return KeyMappings.Select(km => CalculateTiming(timings, km));
        }

        private static NavTiming CalculateTiming(Dictionary<string, long> src, KeyMapping mapping)
        {
            Debug.Assert(src[mapping.StartKey] <= src[mapping.EndKey]);
            return new NavTiming(mapping.ResultKey, src[mapping.EndKey] - src[mapping.StartKey], mapping.SubPath);
        }
    };
}

//
// Copyright © 2017 Ranorex All rights reserved
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of useful general helper methods.
    /// </summary>
    [UserCodeCollection]
    public static class SystemLibrary
    {
        private static readonly Dictionary<string, System.DateTime> timers = new Dictionary<string, System.DateTime>();

        /// <summary>
        /// Kills a process.
        /// </summary>
        /// <param name="processname">Name of the process to kill</param>
        [UserCodeMethod]
        public static void KillProcess(string processname)
        {
            Process[] processes = Process.GetProcessesByName(processname);
            if (processes.Length == 0)
            {
                Report.Error(string.Format("Process '{0}' not found.", processname));
            }

            foreach (Process p in processes)
            {
                try
                {
                    p.Kill();
                    Report.Info("Process killed: " + p.ProcessName);
                }
                catch (Exception ex)
                {
                    Report.Error(ex.Message);
                }
            }
        }

        /// <summary>
        /// Starts a new timer.
        /// </summary>
        /// <param name="timerName">Name of the timer</param>
        [UserCodeMethod]
        public static void StartTimer(string timerName)
        {
            if (timers.ContainsKey(timerName))
            {
                throw new ArgumentException(string.Format("Timer with name '{0}' already exists", timerName));
            }

            timers.Add(timerName, System.DateTime.Now);
            Report.Log(ReportLevel.Info, "Timer", "Started: '" + timerName + "'");
        }

        /// <summary>
        /// Stops a timer.
        /// </summary>
        /// <param name="timerName">Name of the timer to stop</param>
        [UserCodeMethod]
        public static TimeSpan StopTimer(string timerName)
        {
            if (timers.ContainsKey(timerName))
            {
                System.DateTime end = System.DateTime.Now;
                System.DateTime start = timers[timerName];
                TimeSpan duration = end - start;

                timers.Remove(timerName);

                Report.Log(
                    level: ReportLevel.Info,
                    category: "Timer",
                    message: string.Format("Stopped: '{0}' (duration: {1} seconds)", timerName, duration.TotalMilliseconds / 1000));

                return duration;
            }

            throw new Exception(string.Format("Timer '{0}' does not exist.", timerName));
        }
    }
}
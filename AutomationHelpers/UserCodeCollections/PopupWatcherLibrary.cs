//
// Copyright © 2017 Ranorex All rights reserved
//

using System;
using System.Collections.Generic;
using System.Linq;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace Ranorex.AutomationHelpers.UserCodeCollections
{
    /// <summary>
    /// A collection of popup watcher helper methods.
    /// </summary>
    [UserCodeCollection]
    public sealed class PopupWatcherLibrary
    {
        private static readonly Dictionary<string, PopupWatcher> watchers = new Dictionary<string, PopupWatcher>();

        /// <summary>
        /// Waits for a popup window to appear and clicks an element to close the window.
        /// </summary>
        /// <param name="findElement">Element to wait for</param>
        /// <param name="clickElement">Element to click after the popup appears</param>
        /// <exception cref="ArgumentException">If the watcher with given parameters is already running</exception>
        /// <returns>Reference to a newly created <see cref="PopupWatcher"/></returns>
        [UserCodeMethod]
        public static PopupWatcher CreatePopupWatcher(RepoItemInfo findElement, RepoItemInfo clickElement)
        {
            var key = findElement.GetMetaInfos()["id"] + clickElement.GetMetaInfos()["id"];

            if (watchers.ContainsKey(key))
            {
                throw new ArgumentException("Popup watcher with given parameters already exists.");
            }

            var watcher = new PopupWatcher();
            watcher.WatchAndClick(findElement, clickElement);
            watcher.Start();
            watchers.Add(key, watcher);
            Report.Info("Popup watcher started.");
            return watcher;
        }

        /// <summary>
        /// Remove an active popup watcher.
        /// </summary>
        /// <param name="findElement">Element to wait for</param>
        /// <param name="clickElement">Element to click after the popup appears</param>
        [UserCodeMethod]
        public static void RemovePopupWatcher(RepoItemInfo findElement, RepoItemInfo clickElement)
        {
            var key = findElement.GetMetaInfos()["id"] + clickElement.GetMetaInfos()["id"];
            PopupWatcher watcher = null;
            if (watchers.TryGetValue(key, out watcher))
            {
                watcher.Clear();
                watcher.Stop();
                Report.Info("Popup watcher stopped.");
                watchers.Remove(key);
            }
            else
            {
                Report.Warn("The popup watcher you tried to remove does not exist.");
            }
        }

        /// <summary>
        /// Stops a popup watcher.
        /// </summary>
        /// <param name="watcher">The popup watcher to stop</param>
        [UserCodeMethod]
        public static void StopPopupWatcher(PopupWatcher watcher)
        {
            watcher.Stop();
            Report.Info("Popup watcher stopped.");
        }
    }
}
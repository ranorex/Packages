// TODO: Add Header

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
    public class PopupWatcherLibrary
    {
    	private static readonly Dictionary<string, PopupWatcher> watchers = new Dictionary<string, PopupWatcher>();

        /// <summary>
        /// Waits for a popup window to appear and clicks an element to close the window.
        /// </summary>
        /// <param name="findElement">Element to be waited for.</param>
        /// <param name="clickElement">Elment which should be clicked after the popup appeard.</param>
        /// <exception cref="ArgumentException"> if the watcher with given parameters is already running </exception>
        /// <returns>Refference to a newly created <see cref="PopupWatcher"/>></returns>
        [UserCodeMethod]
        public PopupWatcher CreatePopupWatcher(RepoItemInfo findElement, RepoItemInfo clickElement)
        {
            var key = findElement.GetMetaInfos()["id"] + clickElement.GetMetaInfos()["id"];

            if (watchers.ContainsKey(key))
            {
                throw new ArgumentException("Watcher with given parameters aleardy exists.");
            }

            var watcher = new PopupWatcher();
            watcher.WatchAndClick(findElement, clickElement);
            watcher.Start();
            watchers.Add(key, watcher);
            Report.Info("Popup watcher started.");
            return watcher;
        }

        /// <summary>
        /// Remove an active watcher.
        /// </summary>
        /// <param name="findElement">Element to be waited for.</param>
        /// <param name="clickElement">Elment which should be clicked after the popup appeard.</param>
        [UserCodeMethod]
        public void RemovePopupWatcher(RepoItemInfo findElement, RepoItemInfo clickElement)
        {
            var key = findElement.GetMetaInfos()["id"] + clickElement.GetMetaInfos()["id"];

            if (watchers.TryGetValue(key, out var watcher))
            {
                watcher.Clear();
                watcher.Stop();
                watchers.Remove(key);
            }
            else
            {
                Report.Warn("The watcher you have tried to remove does not exist.");
            }
        }

        /// <summary>
        /// Stops the popup watcher.
        /// </summary>
        /// <param name="watcher">A watcher to be stopped.</param>
        [UserCodeMethod]
        public static void StopPopupWatcher(PopupWatcher watcher)
        {
            watcher.Stop();
            Report.Info("Popup watcher stopped.");
        }
    }
}
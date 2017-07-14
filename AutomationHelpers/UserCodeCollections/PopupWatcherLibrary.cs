// TODO: Add Header

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
        /// <summary>
        /// Waits for a popup window to appear and clicks an element to close the window.
        /// </summary>
        /// <param name="findElement">Element to be waited for.</param>
        /// <param name="clickElement">Elment which should be clicked after the popup appeard.</param>
        /// <returns>Refference to a newly created <see cref="PopupWatcher"/>></returns>
        [UserCodeMethod]
        public PopupWatcher CreatePopupWatcher(RepoItemInfo findElement, RepoItemInfo clickElement)
        {
            var watcher = new PopupWatcher();
            watcher.WatchAndClick(findElement, clickElement);
            watcher.Start();
            Report.Info("Popup watcher started.");
            return watcher;
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
//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using NUnit.Framework;
using NSubstitute;
using Ranorex;
using Ranorex.AutomationHelpers.UserCodeCollections;
using Ranorex.Core;
using Ranorex.Core.Repository;

namespace RanorexAutomationHelpers.Test
{
    [TestFixture]
    public sealed class PopupWatcherLibraryTests
    {
        [Test]
        public void StartPopupWatcherTest_Single_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null);
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            var watcher = PopupWatcherLibrary.StartPopupWatcher(repoItemInfo, repoItemInfo);

            //Assert
            Report.DetachLogger(logger);
            Assert.IsNotNull(watcher);
            Assert.AreEqual("Popup watcher started.", logger.LastLogMessage);
        }

        [Test]
        public void StartPopupWatcherTest_Twice_ThrowsException()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            try
            {
                PopupWatcherLibrary.StartPopupWatcher(repoItemInfo, repoItemInfo);
                PopupWatcherLibrary.StartPopupWatcher(repoItemInfo, repoItemInfo);
                //Assert
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Report.DetachLogger(logger);
                Assert.AreEqual("Popup watcher with given parameters already exists.", ex.Message);
            }
        }

        [Test]
        public void StopPopupWatcherTest_Single_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var watcher = PopupWatcherLibrary.StartPopupWatcher(repoItemInfo, repoItemInfo);

            //Act
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo, repoItemInfo);

            //Assert
            Report.DetachLogger(logger);
            Assert.IsNotNull(watcher);
            Assert.AreEqual("Popup watcher stopped.", logger.LastLogMessage);
        }

        [Test]
        public void StopPopupWatcherTest_StopWithoutStart_ReportsFail()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo, repoItemInfo);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("The popup watcher you tried to remove does not exist.", logger.LastLogMessage);
        }

        [Test]
        public void StopAllPopupWatcherTest_StopWithoutStart_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo1 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var repoItemInfo2 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo1, repoItemInfo1);
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo2, repoItemInfo2);

            //Act
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo1, repoItemInfo1);
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo2, repoItemInfo2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Popup watcher stopped.", logger.LastLogMessage);
        }

        [Test]
        public void StopAllPopupWatcherTest_RemovesAllEntries_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo1 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var repoItemInfo2 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo1, repoItemInfo1);
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo2, repoItemInfo2);

            //Act
            PopupWatcherLibrary.StopAllPopupWatchers();

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Popup watcher stopped.", logger.LastLogMessage);
            Assert.AreEqual(0, PopupWatcherLibrary.Watchers.Count);
        }
    }
}

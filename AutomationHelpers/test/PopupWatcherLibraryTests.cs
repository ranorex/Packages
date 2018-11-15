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
        private TestReportLogger logger;

        [SetUp]
        public void Init()
        {
            logger = new TestReportLogger();
            Report.AttachLogger(logger);
        }

        [TearDown]
        public void Dispose()
        {
            Report.DetachLogger(logger);
        }

        [Test]
        public void StartPopupWatcherTest_Single_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null);

            //Act
            var watcher = PopupWatcherLibrary.StartPopupWatcher(repoItemInfo, repoItemInfo);

            //Assert
            Assert.IsNotNull(watcher);
            Assert.AreEqual("Popup watcher started.", logger.LastLogMessage);
        }

        [Test]
        public void StartPopupWatcherTest_Twice_ThrowsException()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());

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
                Assert.AreEqual("Popup watcher with given parameters already exists.", ex.Message);
            }
        }

        [Test]
        public void StartPopupWatcherTest_WithoutParameters_ThrowsException()
        {
            //Assert
            Assert.Throws<ArgumentNullException>(() => PopupWatcherLibrary.StartPopupWatcher(null, null));
        }

        [Test]
        public void StopPopupWatcherTest_Single_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var watcher = PopupWatcherLibrary.StartPopupWatcher(repoItemInfo, repoItemInfo);

            //Act
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo, repoItemInfo);

            //Assert
            Assert.IsNotNull(watcher);
            Assert.AreEqual("Popup watcher stopped.", logger.LastLogMessage);
        }

        [Test]
        public void StopPopupWatcherTest_StopWithoutStart_ReportsFail()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());

            //Act
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo, repoItemInfo);

            //Assert
            Assert.AreEqual("The popup watcher you tried to remove does not exist.", logger.LastLogMessage);
        }

        [Test]
        public void StopAllPopupWatcherTest_StopWithoutStart_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo1 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var repoItemInfo2 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo1, repoItemInfo1);
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo2, repoItemInfo2);

            //Act
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo1, repoItemInfo1);
            PopupWatcherLibrary.StopPopupWatcher(repoItemInfo2, repoItemInfo2);

            //Assert
            Assert.AreEqual("Popup watcher stopped.", logger.LastLogMessage);
        }

        [Test]
        public void StopAllPopupWatcherTest_RemovesAllEntries_Success()
        {
            //Arrange
            var parentFolder = Substitute.For<RepoGenBaseFolder>("Form1", "/notExistent", null, Duration.Zero, true);
            var repoItemInfo1 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            var repoItemInfo2 = new RepoItemInfo(parentFolder, "self", RxPath.Parse(string.Empty), Duration.Zero, null, Guid.NewGuid().ToString());
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo1, repoItemInfo1);
            PopupWatcherLibrary.StartPopupWatcher(repoItemInfo2, repoItemInfo2);

            //Act
            PopupWatcherLibrary.StopAllPopupWatchers();

            //Assert
            Assert.AreEqual("Popup watcher stopped.", logger.LastLogMessage);
            Assert.AreEqual(0, PopupWatcherLibrary.Watchers.Count);
        }
    }
}

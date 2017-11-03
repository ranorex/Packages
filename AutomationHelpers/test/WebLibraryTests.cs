//
// Copyright © 2017 Ranorex All rights reserved
//

using NUnit.Framework;
using Ranorex;
using Ranorex.AutomationHelpers.UserCodeCollections;
using System.IO;

namespace RanorexAutomationHelpers.Test
{
    [TestFixture]
    public sealed class WebLibraryTests
    {
        [Test]
        public void WebLibraryTest_NoUri_Fail()
        {
            //Arrange
            string address = null;
            string fileName = null;
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            WebLibrary.DownloadFile(address, fileName);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual($"Downloading a file from: {address} failed for the following reason: Value cannot be null.\r\nParameter name: {nameof(address)}", logger.LastLogMessage);
        }

        [Test]
        public void WebLibraryTest_NoLocalPath_Fail()
        {
            //Arrange
            string address = "https://www.ranorex.com/release-notes.html";
            string fileName = null;
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            WebLibrary.DownloadFile(address, fileName);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual($"Downloading a file from: {address} failed for the following reason: Value cannot be null.\r\nParameter name: {nameof(fileName)}", logger.LastLogMessage);
        }

        [Test]
        public void WebLibraryTest_Simple_Success()
        {
            //Arrange
            string address = "https://www.ranorex.com/release-notes.html";
            string fileName = Path.Combine(System.Environment.CurrentDirectory, "release-notes.html");
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            WebLibrary.DownloadFile(address, fileName);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual($"File successfully downloaded to {fileName}", logger.LastLogMessage);
        }
    }
}

//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.IO;
using NUnit.Framework;
using Ranorex;
using Ranorex.AutomationHelpers.UserCodeCollections;

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
            Assert.AreEqual(string.Format("Downloading a file from: {0} failed for the following reason:\r\nValue cannot be null.\r\nParameter name: address", address), logger.LastLogMessage);
        }

        [Test]
        public void WebLibraryTest_IncorrectUri_Fail()
        {
            //Arrange
            string address = "ranorex.com";
            string fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

            //Act and Assert
            Assert.Throws<InvalidOperationException>(
                () => WebLibrary.DownloadFile(address, fileName));
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
            Assert.AreEqual(string.Format("Downloading a file from: {0} failed for the following reason:\r\nValue cannot be null.\r\nParameter name: fileName", address), logger.LastLogMessage);
        }

        [Test]
        public void WebLibraryTest_DownloadHtmlToCurrentDirWithFilePath_Success()
        {
            //Arrange
            string address = "https://www.ranorex.com/release-notes.html";
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "release-notes.html");
            File.Delete(fileName);
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            WebLibrary.DownloadFile(address, fileName);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(string.Format("File successfully downloaded to {0}", fileName), logger.LastLogMessage);
            Assert.IsTrue(File.Exists(fileName));
            File.Delete(fileName);
        }

        [Test]
        public void WebLibraryTest_DownloadHtmlToCurrentDirWithDirPath_Success()
        {
            //Arrange
            string address = "https://www.ranorex.com/release-notes.html";
            string fileDirPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            string fileName = Path.Combine(fileDirPath, "release-notes.html");
            File.Delete(fileName);
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            WebLibrary.DownloadFile(address, fileDirPath);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(string.Format("File successfully downloaded to {0}", fileName), logger.LastLogMessage);
            Assert.IsTrue(File.Exists(fileName));
            File.Delete(fileName);
        }

        [Test]
        public void WebLibraryTest_DownloadExeToSystemDir_Fail()
        {
            //Arrange
            string address = "https://www.ranorex.com/download/Ranorex-7.2.0.exe";
            string fileName = Path.Combine(Environment.SystemDirectory, "Ranorex-7.2.0.exe");
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            WebLibrary.DownloadFile(address, fileName);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(string.Format("Downloading a file from: {0} failed for the following reason:\r\nAn exception occurred during a WebClient request.\r\nAccess to the path '{1}' is denied.", address, fileName), logger.LastLogMessage);
        }
		
		[Test]
        public void WebLibraryTest_ResponseStatusCode_404()
        {
        	var logger = new TestReportLogger();
        	
        	string statusCode = WebLibrary.GetHttpStatusCode("https://httpstat.us/404");
        	
        	Assert.AreEqual("404", statusCode, logger.LastLogMessage);
        }

        [Test]
        public void WebLibraryTest_ResponseStatusCode_200()
        {
            var logger = new TestReportLogger();

            string statusCode = WebLibrary.GetHttpStatusCode("https://httpstat.us/200");

            Assert.AreEqual("200", statusCode, logger.LastLogMessage);
        }

        [Test]
        public void WebLibraryTest_ResponseStatusCode_500()
        {
            var logger = new TestReportLogger();

            string statusCode = WebLibrary.GetHttpStatusCode("https://httpstat.us/501");

            Assert.AreEqual("NotImplemented", statusCode, logger.LastLogMessage);
        }
    }
}

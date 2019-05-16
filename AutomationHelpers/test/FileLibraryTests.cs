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
    public sealed class FileLibraryTests
    {
        private static readonly string testFilesDirPath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");

        [Test]
        public void WriteToFile_WriteTextToFile_FileIsCrated()
        {
            //Arrange
            var fileNamePrefix = "testFile";
            var fileExtension = "txt";
            var textToWriteToFile = "Text for test writing to file.";
            var strTimestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = fileNamePrefix + "_" + strTimestamp + "." + fileExtension;

            //Act
            FileLibrary.WriteToFile(textToWriteToFile, fileNamePrefix, fileExtension);

            //Assert
            Assert.IsTrue(File.Exists(fileName));
            File.Delete(fileName);
        }

        [Test]
        public void WriteToFile_WriteTextToFile_FileContainsGivenText()
        {
            //Arrange
            var fileNamePrefix = "testFile";
            var fileExtension = "txt";
            var textToWriteToFile = "Text for test writing to file.";
            var strTimestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = fileNamePrefix + "_" + strTimestamp + "." + fileExtension;

            //Act
            FileLibrary.WriteToFile(textToWriteToFile, fileNamePrefix, fileExtension);

            //Assert
            Assert.AreEqual(textToWriteToFile, File.ReadAllText(fileName));
            File.Delete(fileName);
        }

        [Test]
        public void AppendStringToExistingFile_AppendTextWithNewLine_TextIsAppended()
        {
            //Arrange
            var textToWriteToFile = "Text for test writing to file.";
            var textToAppendToExistingFile = "Appended text.";
            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, textToWriteToFile);

            //Act
            FileLibrary.AppendStringToExistingFile(textToAppendToExistingFile, fileName, true);

            //Assert
            Assert.AreEqual(
                string.Format("{0}{1}{2}", textToWriteToFile, Environment.NewLine, textToAppendToExistingFile),
                File.ReadAllText(fileName));
            File.Delete(fileName);
        }

        [Test]
        public void AppendStringToExistingFile_AppendTextWithoutNewLine_TextIsAppended()
        {
            //Arrange
            var textToWriteToFile = "Text for test writing to file.";
            var textToAppendToExistingFile = "Appended text.";
            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, textToWriteToFile);

            //Act
            FileLibrary.AppendStringToExistingFile(textToAppendToExistingFile, fileName, false);

            //Assert
            Assert.AreEqual(
                string.Format("{0}{1}", textToWriteToFile, textToAppendToExistingFile),
                File.ReadAllText(fileName));
        }

        [Test]
        public void CheckFilesExist_SearchForNonexistentFiles_ThrowsException()
        {
            //Act
            TestDelegate td = () => FileLibrary.CheckFilesExist(testFilesDirPath, "SomeFile.txt", 1, 1);

            //Assert
            Assert.Throws(typeof(ValidationException), td, "Objects are not equal (actual='0', expected='1').");
        }

        [Test]
        public void CheckFilesExist_SearchForExistingFiles_DoNotThrowsException()
        {
            //Act
            TestDelegate td = () => FileLibrary.CheckFilesExist(testFilesDirPath, "TextFile*", 5, 1);

            //Assert
            Assert.DoesNotThrow(td);
        }

        [Test]
        public void DeleteFiles_DeleteNonexistentFiles_ReportFilesNotFound()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var pattern = "SomeFile.txt";

            //Act
            FileLibrary.DeleteFiles(testFilesDirPath, pattern);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("No files have been found in '{0}' with the pattern '{1}'.", testFilesDirPath, pattern),
                logger.LastLogMessage);
        }

        [Test]
        public void DeleteFiles_DeleteFile_ReportFileDeleted()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var fileName = Path.Combine(testFilesDirPath, "SomeFile.txt");
            File.WriteAllText(fileName, "Some text");
            var pattern = "SomeFile.txt";

            //Act
            FileLibrary.DeleteFiles(testFilesDirPath, pattern);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("File has been deleted: {0}", fileName),
                logger.LastLogMessage);
        }

        [Test]
        public void WaitForFile_WaitForNonexistentFile_ReportFileNotFound()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var pattern = "SomeFile.txt";

            //Act
            FileLibrary.WaitForFile(testFilesDirPath, pattern, 500, 100);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("File with pattern '{0}' wasn't found in directory '{1}'.", pattern, testFilesDirPath),
                logger.LastLogMessage);
        }

        [Test]
        public void WaitForFile_WaitForExistingFile_ReportFileFound()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var pattern = "TextFile*";

            //Act
            FileLibrary.WaitForFile(testFilesDirPath, pattern, 500, 100);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("File with pattern '{0}' was found in directory '{1}'.", pattern, testFilesDirPath),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesBinaryEqual_CompareNonexistentFile_ReportFileNotExist()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "BinaryFile1kB11.file");
            var filePath2 = Path.Combine(testFilesDirPath, "BinaryFile2kB.file");

            //Act
            FileLibrary.ValidateFilesBinaryEqual(filePath1, filePath2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("The file '{0}' does not exist.", filePath1),
                logger.LastLogMessage);
        }


        [Test]
        public void ValidateFilesBinaryEqual_CompareFilesWithDifferentSize_ReportFilesNotEqual()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "BinaryFile1kB1.file");
            var filePath2 = Path.Combine(testFilesDirPath, "BinaryFile2kB.file");

            //Act
            FileLibrary.ValidateFilesBinaryEqual(filePath1, filePath2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Files '{0}' and '{1}' are not equal because they differ in size.", filePath1, filePath2),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesBinaryEqual_CompareDifferentFiles_ReportFilesNotEqual()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "BinaryFile1kB1.file");
            var filePath2 = Path.Combine(testFilesDirPath, "BinaryFile1kB2.file");

            //Act
            FileLibrary.ValidateFilesBinaryEqual(filePath1, filePath2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Files '{0}' and '{1}' are not equal.", filePath1, filePath2),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesBinaryEqual_CompareEqualFiles_ReportFilesAreEqual()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "BinaryFile1kB1.file");
            var filePath2 = Path.Combine(testFilesDirPath, "BinaryFile1kB1.file");

            //Act
            FileLibrary.ValidateFilesBinaryEqual(filePath1, filePath2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Files '{0}' and '{1}' are equal.", filePath1, filePath2),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesTextEqual_CompareNonexistentFile_ReportFileNotExist()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "TextFile11.txt");
            var filePath2 = Path.Combine(testFilesDirPath, "TextFile2.txt");

            //Act
            FileLibrary.ValidateFilesTextEqual(filePath1, filePath2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("The file '{0}' does not exist.", filePath1),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesTextEqual_CompareDifferentFiles_ReportFilesNotEqual()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "TextFile1.txt");
            var filePath2 = Path.Combine(testFilesDirPath, "TextFile2.txt");

            //Act
            FileLibrary.ValidateFilesTextEqual(filePath1, filePath2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Files '{0}' and '{1}' are not equal.", filePath1, filePath2),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesTextEqual_CompareDifferentFiles_ReportFilesEqual()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "TextFile1.txt");
            var filePath2 = Path.Combine(testFilesDirPath, "TextFile1.txt");

            //Act
            FileLibrary.ValidateFilesTextEqual(filePath1, filePath2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Files '{0}' and '{1}' are equal.", filePath1, filePath2),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesTextEqual_CompareFilesWithDifferentEOLOnly_ReportFilesNotEqual()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "TextFileUnixEOL.txt");
            var filePath2 = Path.Combine(testFilesDirPath, "TextFileWinEOL.txt");

            //Act
            FileLibrary.ValidateFilesTextEqual(filePath1, filePath2, false);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Files '{0}' and '{1}' are not equal.", filePath1, filePath2),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFilesTextEqual_CompareFilesWithDifferentEOLOnlyNormalizeEOL_ReportFilesEqual()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath1 = Path.Combine(testFilesDirPath, "TextFileUnixEOL.txt");
            var filePath2 = Path.Combine(testFilesDirPath, "TextFileWinEOL.txt");

            //Act
            FileLibrary.ValidateFilesTextEqual(filePath1, filePath2, true);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Files '{0}' and '{1}' are equal.", filePath1, filePath2),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFileContainsText_SearchTextInNonexistentFile_ReportFileNotExist()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath = Path.Combine(testFilesDirPath, "TextFileForSearch1.txt");
            var textToSearchFor = "Maecenas tristique consequat est, et condimentum";

            //Act
            FileLibrary.ValidateFileContainsText(filePath, textToSearchFor);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("The file '{0}' does not exist.", filePath),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFileContainsText_SearchText_ReportTextNotFound()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath = Path.Combine(testFilesDirPath, "TextFileForSearch.txt");
            var textToSearchFor = "Maecenas123 tristique consequat est, et condimentum";

            //Act
            FileLibrary.ValidateFileContainsText(filePath, textToSearchFor);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Text '{0}' was not found in file '{1}'.", textToSearchFor, filePath),
                logger.LastLogMessage);
        }

        [Test]
        public void ValidateFileContainsText_SearchText_ReportTextFound()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            var filePath = Path.Combine(testFilesDirPath, "TextFileForSearch.txt");
            var textToSearchFor = "Maecenas tristique consequat est, et condimentum";

            //Act
            FileLibrary.ValidateFileContainsText(filePath, textToSearchFor);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual(
                string.Format("Text '{0}' was found on line 7: 'Maecenas tristique consequat est, et condimentum lacus bibendum non. Nam semper malesuada risus ut interdum.'.", textToSearchFor),
                logger.LastLogMessage);
        }

        /*
         [Test]
         public void Method_Condition_Result()
         {
             //Arrange

             //Act

             //Assert
         }
         */
    }
}
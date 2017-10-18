//
// Copyright © 2017 Ranorex All rights reserved
//

using NUnit.Framework;
using Ranorex.AutomationHelpers.UserCodeCollections;
using RanorexAutomationHelpers.Test;

namespace Ranorex.AutomationHelpers.Tests
{
    [TestFixture]
    public sealed class ValidationLibraryTests
    {
        [Test]
        public void CompareValuesTest_EqualStrings_Success()
        {
            //Arrange
            var value1 = "value1";
            var value2 = "value1";
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            ValidationLibrary.CompareValues(value1, value2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Objects are equal (actual='value1', expected='value1').", logger.LastLogMessage);
        }

        [Test]
        public void CompareValuesTest_NotEqualStrings_ThrowsException()
        {
            //Arrange
            var value1 = "value1";
            var value2 = "value2";
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            try
            {
                ValidationLibrary.CompareValues(value1, value2);
                //Assert
                Assert.Fail();
            }
            catch (ValidationException ex)
            {
                Report.DetachLogger(logger);
                Assert.AreEqual("Objects are not equal (actual='value1', expected='value2').", logger.LastLogMessage);
                Assert.AreEqual("Objects are not equal (actual='value1', expected='value2').", ex.Message);
            }
        }

        [Test]
        public void CompareValuesTest_EmptyStrings_Success()
        {
            //Arrange
            var value1 = string.Empty;
            var value2 = string.Empty;
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            ValidationLibrary.CompareValues(value1, value2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Objects are equal (actual='', expected='').", logger.LastLogMessage);
        }

        [Test]
        public void CompareValuesTest_NullStrings_Success()
        {
            //Arrange
            string value1 = null;
            string value2 = null;
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            ValidationLibrary.CompareValues(value1, value2);

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Objects are equal (actual='(null)', expected='(null)').", logger.LastLogMessage);
        }
    }
}
//
// Copyright © 2017 Ranorex All rights reserved
//

using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ranorex;
using Ranorex.AutomationHelpers.UserCodeCollections;

namespace RanorexAutomationHelpers.Test
{
    [TestClass]
    public sealed class SystemLibraryTests
    {
        [TestMethod()]
        public void StartTimerTest_Single_Success()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            SystemLibrary.StartTimer("testTimer");

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Started: 'testTimer'", logger.LastLogMessage);
        }

        [TestMethod()]
        public void StartTimerTest_Multiple_Success()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            SystemLibrary.StartTimer("testTimer1");
            SystemLibrary.StartTimer("testTimer2");
            SystemLibrary.StartTimer("testTimer3");

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Started: 'testTimer3'", logger.LastLogMessage);
        }


        [TestMethod()]
        public void StartTimerTest_StartedSameTimerTwice_ThrowsException()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            try
            {
                SystemLibrary.StartTimer("testTimer");
                SystemLibrary.StartTimer("testTimer");
                //Assert
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Report.DetachLogger(logger);
                Assert.AreEqual("Started: 'testTimer'", logger.LastLogMessage);
                Assert.AreEqual("Timer with name 'testTimer' already exists", ex.Message);
            }
        }

        [TestMethod()]
        public void StopTimerTest_Single_Success()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            SystemLibrary.StartTimer("testTimer");

            //Act
            Thread.Sleep(1);
            var time = SystemLibrary.StopTimer("testTimer");

            //Assert
            Assert.IsTrue(time.TotalMilliseconds > 0);
            Report.DetachLogger(logger);
            StringAssert.Contains(logger.LastLogMessage, "Stopped: 'testTimer' (duration: ");
        }

        [TestMethod()]
        public void StopTimerTest_Multiple_Success()
        {
            //Arrange
            SystemLibrary.StartTimer("testTimer1");
            SystemLibrary.StartTimer("testTimer2");
            SystemLibrary.StartTimer("testTimer3");

            //Act
            Thread.Sleep(1);
            var time1 = SystemLibrary.StopTimer("testTimer1");
            var time2 = SystemLibrary.StopTimer("testTimer2");
            var time3 = SystemLibrary.StopTimer("testTimer3");

            //Assert
            Assert.IsTrue(time1.TotalMilliseconds > 0);
            Assert.IsTrue(time2.TotalMilliseconds > 0);
            Assert.IsTrue(time3.TotalMilliseconds > 0);
        }


        [TestMethod()]
        public void StopTimerTest_StoppedWithoutStart_Fail()
        {
            //Arrange
            //Act
            try
            {
                SystemLibrary.StopTimer("testTimer");
                //Assert
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Timer 'testTimer' does not exist.", ex.Message);
            }
        }


        [TestMethod()]
        public void StopTimerTest_StoppedSameTimerTwice_Fail()
        {
            //Arrange
            SystemLibrary.StartTimer("testTimer");

            //Act
            try
            {
                SystemLibrary.StopTimer("testTimer");
                SystemLibrary.StopTimer("testTimer");
                //Assert
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Timer 'testTimer' does not exist.", ex.Message);
            }
        }
    }
}

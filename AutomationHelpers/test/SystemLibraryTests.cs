//
// Copyright © 2018 Ranorex All rights reserved
//

using System;
using System.Threading;
using NUnit.Framework;
using Ranorex;
using Ranorex.AutomationHelpers.UserCodeCollections;

namespace RanorexAutomationHelpers.Test
{
    [TestFixture]
    public sealed class SystemLibraryTests
    {
        [Test]
        public void StartTimerTest_Single_Success()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            SystemLibrary.StartTimer("testTimerStart");

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Started: 'testTimerStart'", logger.LastLogMessage);
        }

        [Test]
        public void StartTimerTest_Multiple_Success()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            SystemLibrary.StartTimer("testTimerStart1");
            SystemLibrary.StartTimer("testTimerStart2");
            SystemLibrary.StartTimer("testTimerStart3");

            //Assert
            Report.DetachLogger(logger);
            Assert.AreEqual("Started: 'testTimerStart3'", logger.LastLogMessage);
        }


        [Test]
        public void StartTimerTest_StartedSameTimerTwice_ThrowsException()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);

            //Act
            try
            {
                SystemLibrary.StartTimer("testTimerStartTwice");
                SystemLibrary.StartTimer("testTimerStartTwice");
                //Assert
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Report.DetachLogger(logger);
                Assert.AreEqual("Started: 'testTimerStartTwice'", logger.LastLogMessage);
                Assert.AreEqual("Timer with name 'testTimerStartTwice' already exists", ex.Message);
            }
        }

        [Test]
        public void StopTimerTest_Single_Success()
        {
            //Arrange
            var logger = new TestReportLogger();
            Report.AttachLogger(logger);
            SystemLibrary.StartTimer("testTimerStartAndStop");

            //Act
            Thread.Sleep(1);
            var time = SystemLibrary.StopTimer("testTimerStartAndStop");

            //Assert
            Assert.IsTrue(time.TotalMilliseconds > 0);
            Report.DetachLogger(logger);
            StringAssert.Contains("Stopped: 'testTimerStartAndStop' (duration: ", logger.LastLogMessage);
        }

        [Test]
        public void StopTimerTest_Multiple_Success()
        {
            //Arrange
            SystemLibrary.StartTimer("testTimerStartAndStop1");
            SystemLibrary.StartTimer("testTimerStartAndStop2");
            SystemLibrary.StartTimer("testTimerStartAndStop3");

            //Act
            Thread.Sleep(1);
            var time1 = SystemLibrary.StopTimer("testTimerStartAndStop1");
            var time2 = SystemLibrary.StopTimer("testTimerStartAndStop2");
            var time3 = SystemLibrary.StopTimer("testTimerStartAndStop3");

            //Assert
            Assert.IsTrue(time1.TotalMilliseconds > 0);
            Assert.IsTrue(time2.TotalMilliseconds > 0);
            Assert.IsTrue(time3.TotalMilliseconds > 0);
        }


        [Test]
        public void StopTimerTest_StoppedWithoutStart_Fail()
        {
            //Arrange
            //Act
            try
            {
                SystemLibrary.StopTimer("testTimerNoStart");
                //Assert
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Timer 'testTimerNoStart' does not exist.", ex.Message);
            }
        }


        [Test]
        public void StopTimerTest_StoppedSameTimerTwice_Fail()
        {
            //Arrange
            SystemLibrary.StartTimer("testTimerStopTwice");

            //Act
            try
            {
                SystemLibrary.StopTimer("testTimerStopTwice");
                SystemLibrary.StopTimer("testTimerStopTwice");
                //Assert
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Timer 'testTimerStopTwice' does not exist.", ex.Message);
            }
        }
    }
}

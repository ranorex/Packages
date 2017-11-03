//
// Copyright © 2017 Ranorex All rights reserved
//

using NUnit.Framework;
using Ranorex.AutomationHelpers.UserCodeCollections;

namespace RanorexAutomationHelpers.Test
{
	[TestFixture]
	public sealed class StringLibraryTests
	{
		[Test]
		public void ConcatStringsTest_SomeStrings_Success()
		{
			//Arrange
			var value1 = "value1";
			var value2 = "value2";

			//Act
			var actual = StringLibrary.ConcatStrings(value1, value2);

			//Assert
			Assert.AreEqual("value1value2", actual);
		}

		[Test]
		public void ConcatStringsTest_EmptyStrings_Success()
		{
			//Arrange
			var value1 = string.Empty;
			var value2 = string.Empty;

			//Act
			var actual = StringLibrary.ConcatStrings(value1, value2);

			//Assert
			Assert.AreEqual(string.Empty, actual);
		}

		[Test]
		public void ConcatStringsTest_NullStrings_Success()
		{
			//Arrange
			string value1 = null;
			string value2 = null;

			//Act
			var actual = StringLibrary.ConcatStrings(value1, value2);

			//Assert
			Assert.AreEqual(string.Empty, actual);
		}
	}
}
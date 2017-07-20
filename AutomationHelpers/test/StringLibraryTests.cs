//
// Copyright © 2017 Ranorex All rights reserved
//

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ranorex.AutomationHelpers.UserCodeCollections;

namespace Ranorex.AutomationHelpers.Tests
{
	[TestClass()]
	public sealed class StringLibraryTests
	{
		[TestMethod()]
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

		[TestMethod()]
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

		[TestMethod()]
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
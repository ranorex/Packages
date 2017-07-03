using Ranorex;
using Ranorex.Core.Reporting;
using Ranorex.Core.Testing;
using System;

namespace Ranorex.AutomationHelpers
{
	/// <summary>
	/// Allows conversion of Ranorex report files to PDF
	/// </summary>
	[TestModule("FFA0759D-37D2-4ABB-89A7-411F0FCF2DFE", ModuleType.UserCode, 1), UserCodeCollection]
	public class Ranorex_Helper_ReportToPdf : ITestModule
	{
		//Variables
		string PDFName;
		string xml;
		string details;
		static bool registered = false;
		static System.IO.DirectoryInfo zippedReportFileDirectoryInfo;
		
		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		public Ranorex_Helper_ReportToPdf()
		{
			//Init variables
			this.PDFName = "";
			
			this.xml =  "";
			
			//Possible values: none | failed | all
			this.details = "all";
		}
		
		/// <summary>
		/// Performs the playback of actions in this module.
		/// </summary>
		/// <remarks>You should not call this method directly, instead pass the module
		/// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
		/// that will in turn invoke this method.</remarks>
		void ITestModule.Run()
		{
			//Delegate must be registered only once
			if(!registered)
			{
				//PDF will be generated at the very end of the testsuite
				TestSuite.TestSuiteCompleted += delegate {
					
					//Specify report name if not already set
					if(String.IsNullOrEmpty(this.PDFName))
					{
						this.PDFName = CreatePDFName();
					}
					
					//Necessary to end the testreport in order to update the duration
					TestReport.EndTestModule();
					
					//Comment out if ConvertReportToPDF() is called directly
					try
					{
						Report.LogHtml(ReportLevel.Success,"PDFReport", "Successfully created PDF: <a href='" + ConvertReportToPDF(PDFName, xml, details) + "' target='_blank'>Open PDF</a>");
					}
					catch (Exception e)
					{
						Console.BackgroundColor = ConsoleColor.Black;
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("ReportToPDF: " + e.Message);
						Console.ResetColor();
						Console.WriteLine("Press any key to continue...");
						Console.ReadKey();
					}
					
					//Delete *.rxzlog if not enabled within test suite settings
					Cleanup();
					
					//Update error value
					UpdateError();
				};
				
				registered = true;
			}
		}
		
		public static string ConvertReportToPDF (string PDFName, string xml, string details)
		{
			var zippedReportFileDirectory = CreateTempReportFileDirectory();
			var reportFileDirectory = TestReport.ReportEnvironment.ReportFileDirectory;
			var name = TestReport.ReportEnvironment.ReportName;
			
			var input = String.Format(@"{0}\{1}.rxzlog", zippedReportFileDirectory, name);
			var PDFReportFilePath = String.Format(@"{0}\{1}", reportFileDirectory, CheckExtension(PDFName));
			
			TestReport.SaveReport();
			Report.Zip(TestReport.ReportEnvironment, zippedReportFileDirectory, name);

			Ranorex.PDF.Creator.CreatePDF(input, PDFReportFilePath, xml, details);
			return PDFReportFilePath;
		}

		private static string CheckExtension(string PDFName)
		{
			var split = PDFName.Split('.');
			
			for(int i =0; i <split.Length; i++)
			{
				if(split[i].Contains("pdf") && i == split.Length -1 && split.Length > 1)
				{
					return PDFName;
				}
			}
			
			return PDFName + ".pdf";
		}
		
		private static string CreateTempReportFileDirectory()
		{
			//Create new temp directory for zipped report
			try 
			{
				zippedReportFileDirectoryInfo = System.IO.Directory.CreateDirectory(String.Format(@"{0}\temp", TestReport.ReportEnvironment.ReportFileDirectory));
				return zippedReportFileDirectoryInfo.FullName;					
			} 
			catch (Exception ex) 
			{
				throw new Exception("Failed to create temp folder: " + ex.Message);
			}	
		}
		
		private void Cleanup()
		{
			try 
			{
				zippedReportFileDirectoryInfo.Delete(true);	
			}
			catch (Exception ex) 
			{
				throw new Exception("Failed to recursively delete zipped report file directory: " + ex.Message);
			}
		}
		
		private string CreatePDFName()
		{
			//Report Status is not part of the ReportName at this stage of the test
			var name = TestReport.ReportEnvironment.ReportName;

			//Get status from TestSuite
			var testsuite = (TestSuite) TestSuite.Current;
			
			if( testsuite.ReportSettings.ReportFormatString.Contains("%X"))
			{
				name = name += "_" + GetTestSuiteStatus();
			}

			return name;
		}
		
		private void UpdateError()
		{
			var testsuite = (TestSuite) TestSuite.Current;
			
			if(GetTestSuiteStatus().Contains("Failed"))
			{
				Report.Failure("Rethrow Exception within PDF Module (Necessary for correct error value)");
			}
		}
		
		private string GetTestSuiteStatus()
		{
			string status = "";
			
			var rootChildren = ActivityStack.Instance.RootActivity.Children;
			
			if(rootChildren.Count > 1)
			{
				Console.WriteLine("Multiple TestSuiteActivites, status taken from first entry");
			}
			
			var testSuiteAct = rootChildren[0] as TestSuiteActivity;
			
			if(testSuiteAct != null)
			{
				status = testSuiteAct.Status.ToString();
			}
			
			return status;
		}
	}
}

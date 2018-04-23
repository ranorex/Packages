using Ranorex.Core;
using Ranorex.Core.Testing;

namespace Ranorex.Eyes
{
    /// <summary>
    /// Module to initialize Applitools Eyes.
    /// </summary>
    [TestModule("AC04A95D-ED97-4A3E-B39A-DA00D4F95A0C", ModuleType.UserCode, 1)]
    public class InitializeEyes : ITestModule
    {
        [TestVariable("a88a0537-03c9-4224-88bc-abcd10d396ad")]
        public string EyesApiKey { get; set; }

        [TestVariable("ba71e7cd-b6ef-491a-94b1-9493a0c33c09")]
        public string AppName { get; set; }

        [TestVariable("62220F07-EA02-4629-AF7D-999867F8DE42")]
        public string ServerURL { get; set; }

        [TestVariable("c64dad18-55cc-48fe-96a8-0df9084f52ae")]
        public string EyesBatchID { get; set; }

        [TestVariable("0551bfca-74ea-4fdf-b7de-ec5003239ed5")]
        public string ViewPortWidth { get; set; }

        [TestVariable("474e2225-adc4-4807-82d6-dffcecca508a")]
        public string ViewPortHeight { get; set; }

        [TestVariable("680F6759-0B46-4F09-8F83-9CCAF1384F1E")]
        public string MatchLevel { get; set; }

        public InitializeEyes()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            if (Configuration.Current.Tools.UseUiaLauncher)
            {
                Report.Failure("Ranorex.Eyes is not supporting UiaLauncher, please disabled it in Settings/Advanced and run this test again.");
            }

            int portHeight, portWidth;
            int.TryParse(ViewPortHeight, out portHeight);
            int.TryParse(ViewPortWidth, out portWidth);

            if (string.IsNullOrEmpty(AppName))
            {
                if (TestSuite.Current != null)
                {
                    AppName = TestSuite.Current.Name;
                }
            }

            EyesWrapper.Initialize(
                EyesApiKey, AppName, ServerURL, EyesBatchID, portWidth, portHeight, MatchLevel);
        }
    }
}

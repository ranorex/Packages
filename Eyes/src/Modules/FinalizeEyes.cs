using Ranorex.Core.Testing;

namespace Ranorex.Eyes
{
    /// <summary>
    /// Module to finalize Applitools Eyes.
    /// </summary>
    [TestModule("8F446117-AC3D-47FA-BB16-222138411086", ModuleType.UserCode, 1)]
    public class FinalizeEyes : ITestModule
    {
        public FinalizeEyes()
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
            Report.Info("Applitools: Closing Eyes.");
            EyesWrapper.CloseTest();
        }
    }
}

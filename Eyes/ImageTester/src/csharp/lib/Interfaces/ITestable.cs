using System;
using Applitools.Images;

namespace Applitools.ImageTester.Interfaces
{
    public interface ITestable : IDisposable
    {
        void Run(Eyes eyes);
        string Name();
    }
}
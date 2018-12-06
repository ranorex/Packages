using System;
using System.IO;
using Applitools.Images;
using Applitools.ImageTester.Interfaces;

namespace Applitools.ImageTester.TestObjects
{
    public abstract class TestUnit : ITestable
    {
        protected readonly FileStream file;
        protected string name;

        protected TestUnit(FileStream file)
        {
            this.file = file;
            if (file != null && !file.CanRead)
            {
                throw new InvalidOperationException(string.Format("Unreadable path/file '{0}', might be a permission issue!", file.Name));
            }
        }

        protected TestUnit(string name)
        {
            file = null;
            this.name = name;
        }

        public virtual string Name()
        {
            return file == null ? name : file.Name;
        }

        public virtual FileStream GetFile()
        {
            return file;
        }

        public abstract void Run(Eyes eyes);

        public abstract void Dispose();
    }
}
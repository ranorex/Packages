using System;
using System.Collections.Generic;
using System.IO;
using Applitools.Images;

namespace Applitools.ImageTester.TestObjects
{
    public class Batch : TestUnit
    {
        private BatchInfo batch;
        private readonly IList<Test> tests = new List<Test>();

        public Batch(FileStream file) : base(file)
        {
        }

        public Batch(BatchInfo batch) : base(batch.Name)
        {
            this.batch = batch;
        }

        public override void Run(Eyes eyes)
        {
            batch = batch == null ? new BatchInfo(Name()) : batch;
            eyes.Batch = batch;
            Console.WriteLine("Batch: {0}", Name());
            foreach (var test in tests)
            {
                try
                {
                    test.Run(eyes);
                }
                finally
                {
                    test.Dispose();
                }
            }

            eyes.Batch = null;
        }

        public virtual void AddTest(Test test)
        {
            tests.Add(test);
        }

        public override void Dispose()
        {
            if (tests == null)
            {
                return;
            }

            foreach (var test in tests)
            {
                test.Dispose();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Applitools.Images;

namespace Applitools.ImageTester.TestObjects
{
    public class Suite : TestUnit
    {
        private IList<Batch> batches;
        private Test test_ = null;

        public Suite(FileStream file) : base(file)
        {
            batches = new List<Batch>();
        }

        public override void Run(Eyes eyes)
        {
            if (!batches.Any() || test_ == null)
            {
                Console.WriteLine("Nothing to test!");
                return;
            }

            foreach (Batch batch in batches)
            {
                try
                {
                    batch.Run(eyes);
                }
                finally
                {
                    batch.Dispose();
                }
            }

            Console.WriteLine("> No batch <");
            test_.Run(eyes);
        }

        public virtual void AddBatch(Batch batch)
        {
            batches.Add(batch);
        }

        public virtual void PortBatchesTo(Suite destination)
        {
            destination.batches = destination.batches.Concat(batches).ToList();
            batches.Clear();
        }

        public virtual void PortTestTo(Batch destination)
        {
            destination.AddTest(test_);
            test_ = null;
        }

        public virtual bool HasOrphanTest()
        {
            return test_ != null;
        }

        public virtual void SetTest(Test test)
        {
            if (test_ != null)
            {
                throw new ArgumentException("test is not null as expected!");
            }

            test_ = test;
        }

        public override void Dispose()
        {
            test_?.Dispose();
        }
    }
}
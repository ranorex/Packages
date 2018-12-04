using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Applitools.Images;
using Applitools.ImageTester.Interfaces;

namespace Applitools.ImageTester.TestObjects
{
    public class Test : TestUnit
    {
        protected readonly string appname;
        protected Size viewportSize;
        private readonly IList<ITestable> steps;

        public Test(FileStream file, string appname) : this(file, appname, Size.Empty)
        {
        }

        public Test(FileStream file, string appname, Size viewportSize) : base(file)
        {
            steps = new List<ITestable>();
            this.appname = appname;
            this.viewportSize = viewportSize;
        }

        public override void Run(Eyes eyes)
        {
            eyes.Open(appname, Name(), viewportSize);
            foreach (ITestable step in steps)
            {
                try
                {
                    step.Run(eyes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Error in Step {0}: \\n {1} \\n This step will be skipped!", step.Name(), e.Message));
                    Console.WriteLine(e.StackTrace);
                }
            }

            try
            {
                TestResults result = eyes.Close(false);
                PrintTestResults(result);
                HandleResultsDownload(result);
            }
            catch (EyesException e)
            {
                Console.WriteLine(string.Format("Error closing test {0} \\nPath: {1} \\nReason: {2}", Name(), file.Name, e.Message));
                Console.WriteLine("Aborting...");
                try
                {
                    eyes.AbortIfNotClosed();
                    Console.WriteLine("Aborted!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error while aborting: {0}", ex.Message));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected virtual void HandleResultsDownload(TestResults results)
        {
        }

        public virtual void AddStep(ITestable step)
        {
            steps.Add(step);
        }

        public virtual void AddSteps(IList<ITestable> steps)
        {
            foreach (var step in steps)
            {
                AddStep(step);
            }
        }

        protected virtual void PrintTestResults(TestResults result)
        {
            string res = "Empty";
            if (result.Steps > 0)
            {
                if (result.IsNew)
                {
                    res = "New";
                }
                else if (result.IsPassed)
                {
                    res = "Passed";
                }
                else
                {
                    res = "Failed";
                }
            }

            Console.WriteLine(string.Format("\\t[{0}] - {1}", res, Name()));
            if (!result.IsPassed && !result.IsNew)
            {
                Console.WriteLine(string.Format("\\tResult url: {0}", result.Url));
            }
        }

        protected static List<int> ParsePagesToList(string input)
        {
            if (input == null)
            {
                return new List<int>();
            }

            var pagesToInclude = new List<int>();
            var inputPages = input.Split(',');
            for (int i = 0; i < inputPages.Length; i++)
            {
                if (inputPages[i].Contains("-"))
                {
                    int left = int.Parse(inputPages[i].Split('-')[0]);
                    int right = int.Parse(inputPages[i].Split('-')[1]);
                    if (left <= right)
                    {
                        for (int j = left; j <= right; j++)
                        {
                            pagesToInclude.Add(j);
                        }
                    }
                    else
                    {
                        for (int j = left; j >= right; j--)
                        {
                            pagesToInclude.Add(j);
                        }
                    }
                }
                else
                {
                    pagesToInclude.Add(int.Parse(inputPages[i]));
                }
            }

            return pagesToInclude;
        }

        public override void Dispose()
        {
            if (steps == null)
                return;
            foreach (ITestable step in steps)
            {
                step?.Dispose();
            }
        }
    }
}
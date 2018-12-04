using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Applitools.ImageTester.Interfaces;
using Applitools.ImageTester.TestObjects;

namespace Applitools.ImageTester
{
    public class SuiteBuilder
    {
        private readonly string rootFolder;
        private readonly string appname;
        private readonly Size viewport;
        private float pdfdpi_ = 300F;
        private string pdfPassword_;
        private string pages_;

        public virtual string GetPages()
        {
            return pages_;
        }

        public virtual void SetPages(string steps)
        {
            this.pages_ = steps;
        }

        public SuiteBuilder(string rootFolder, string appname, Size viewport)
        {
            this.rootFolder = rootFolder;
            this.appname = appname;
            this.viewport = viewport;
        }

        public virtual ITestable Build()
        {
            return Build(rootFolder, appname, viewport);
        }

        public virtual void SetDpi(float dpi)
        {
            this.pdfdpi_ = dpi;
        }

        private ITestable Build(string curr, string appname, Size viewport)
        {
            string jenkinsJobName = Environment.GetEnvironmentVariable("JOB_NAME");
            string jenkinsApplitoolsBatchId = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_ID");
            Batch jenkinsBatch = null;
            if ((jenkinsJobName != null) && (jenkinsApplitoolsBatchId != null))
            {
                BatchInfo batch = new BatchInfo(jenkinsJobName);
                batch.Id = jenkinsApplitoolsBatchId;
                jenkinsBatch = new Batch(batch);
            }

            ITestable unit = Build(curr, appname, viewport, jenkinsBatch);
            if (unit is ImageStep)
            {
                ImageStep step = (ImageStep)unit;
                Test test = new Test(step.GetFile(), appname, this.viewport);
                test.AddStep(step);
                unit = test;
            }

            if (unit is Test && jenkinsBatch != null)
            {
                jenkinsBatch.AddTest((Test)unit);
                return jenkinsBatch;
            }
            else
            {
                return unit;
            }
        }

        private ITestable Build(string curr, string appname, Size viewport, Batch flatBatch)
        {
            if (appname == null)
            {
                appname = "ImageTester";
            }

            var attr = File.GetAttributes(curr);
            if (!attr.HasFlag(FileAttributes.Directory))
            {
                var fileStream = File.OpenRead(curr);
                if (PDFTest.Supports(fileStream))
                {
                    PDFTest pdftest = new PDFTest(fileStream, this.appname, pdfdpi_);
                    pdftest.SetPdfPassword(pdfPassword_);
                    pdftest.SetPages(pages_);
                    return pdftest;
                }

                //TODO: Implement
                //if (PostscriptTest.Supports(fileStream))
                //{
                //    PostscriptTest postScriptest = new PostscriptTest(fileStream, appname);
                //    return postScriptest;
                //}

                return ImageStep.Supports(fileStream) ? new ImageStep(fileStream) : null;
            }

            Test currTest = null;
            Batch currBatch = flatBatch;
            Suite currSuite = null;
            var supportedExtensions = PDFTest.SupportedExtensionsWithSeparator
                .Concat(ImageStep.SupportedExtensionsWithSeparator);
            var files = supportedExtensions
                .AsParallel()
                .SelectMany(p => Directory.EnumerateFiles(curr, "*" + p, SearchOption.AllDirectories))
                .OrderBy(f => f);
            foreach (var file in files)
            {
                var fileStream = File.OpenRead(file);
                ITestable unit = Build(file, appname, viewport, flatBatch);
                if (unit is ImageStep)
                {
                    if (currTest == null)
                        currTest = new Test(fileStream, appname, viewport);
                    ImageStep step = (ImageStep)unit;
                    currTest.AddStep(step);
                }
                else if (unit is Test)
                {
                    if (currBatch == null)
                        currBatch = new Batch(fileStream);
                    currBatch.AddTest((Test)unit);
                }
                else if (flatBatch == null)
                    if (unit is Batch)
                    {
                        if (currSuite == null)
                            currSuite = new Suite(fileStream);
                        currSuite.AddBatch((Batch)unit);
                    }
                    else if (unit is Suite)
                    {
                        Suite suite = (Suite)unit;
                        if (currSuite == null)
                            currSuite = new Suite(fileStream);
                        suite.PortBatchesTo(currSuite);
                        if (suite.HasOrphanTest())
                        {
                            if (currBatch == null)
                                currBatch = new Batch(fileStream);
                            suite.PortTestTo(currBatch);
                        }
                    }
                    else
                    {
                    }
            }

            if (flatBatch == null)
            {
                if (currTest == null && currBatch == null && currSuite == null)
                    return null;
                if (currTest != null && currBatch == null && currSuite == null)
                    return currTest;
                if (currTest == null && currBatch != null && currSuite == null)
                    return currBatch;
                if (currTest == null && currBatch == null && currSuite != null)
                    return currSuite;
                if (currSuite == null)
                    currSuite = new Suite(File.OpenRead(curr));
                if (currBatch != null)
                    currSuite.AddBatch(currBatch);
                if (currTest != null)
                    currSuite.SetTest(currTest);
                return currSuite;
            }
            else if (currTest != null)
                return currTest;
            return currBatch;
        }

        public virtual string GetPdfPassword()
        {
            return pdfPassword_;
        }

        public virtual void SetPdfPassword(string pdfPassword)
        {
            this.pdfPassword_ = pdfPassword;
        }
    }
}
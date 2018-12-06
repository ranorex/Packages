using System;
using System.Drawing;
using System.IO;
using Applitools.Images;
using Applitools.ImageTester;
using CommandLine;

namespace Applitools.ImageTester
{
    public static class ImageTester
    {
        internal static readonly log4net.LogManager mgr = null; // explicit ref

        public static void Main(string[] args)
        {
            Options options = null;
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(opts => options = opts);
            if (options == null)
            {
                return;
            }

            try
            {
                var eyes = new Eyes();

                // API key
                eyes.ApiKey = options.ApiKey;
                // Applitools Server url
                if (options.ServerUrl != null)
                {
                    eyes.ServerUrl = options.ServerUrl.ToString();
                }
                // Match level
                eyes.MatchLevel = options.MatchLevel;
                //TODO: Proxy
                //TODO: Branch name
                //TODO: Parent branch
                //TODO: Baseline name
                //TODO: Log file
                //TODO: host os
                //TODO: host app
                //TODO: Set failed tests
                // Viewport size
                var viewPortSize = new Size(1920, 1080);
                var vs = options.ViewPortSize?.Split('x');
                if (vs?.Length == 2)
                {
                    viewPortSize = new Size(int.Parse(vs[0]), int.Parse(vs[1]));
                }
                // Folder path
                var root = Path.GetFullPath(options.Folder);

                var builder = new SuiteBuilder(root, options.AppName, viewPortSize);
                //TODO: DPI
                //TODO: Determine Pages to include
                //TODO: Read PDF Password

                //TODO: if utils_enabled builder.setEyesUtilitiesConfig

                var suite = builder.Build();
                if (suite == null)
                {
                    Console.WriteLine("Nothing to test!");
                    return;
                }

                suite.Run(eyes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private sealed class Options
        {
            [Option('k', Required = true, HelpText = "Applitools api key")]
            public string ApiKey { get; set; }

            [Option('a', Required = false, HelpText = "Set own application name, default: ImageTester", Default = "ImageTester")]
            public string AppName { get; set; }

            [Option('f', Required = false, HelpText = "Set the root folder to start the analysis, default is the current directory", Default = ".")]
            public string Folder { get; set; }

            //TODO: Implement proxy

            [Option('s', Required = false, HelpText = "Set Applitools server url")]
            public Uri ServerUrl { get; set; }

            [Option("ml", Required = false, HelpText = "Set match level to one of [%s], default = Strict", Default = MatchLevel.Strict)]
            public MatchLevel MatchLevel { get; set; }

            //TODO: Implement branch, parentBranch, baseline

            [Option("vs", Required = false, HelpText = "Declare viewport size identifier <width>x<height> ie. 1000x600, if not set,default will be the first image in every test")]
            public string ViewPortSize { get; set; }

            //TODO: Implement logFile, autoSave, hostOs, hostApp, dpi, selectedPages, PDFPassword
            //TODO: Implement extra features are available case: viewKey, outFolder, getDiffs, getImages, getGifs
        }
    }
}
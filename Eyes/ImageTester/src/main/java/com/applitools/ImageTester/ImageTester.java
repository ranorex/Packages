package com.applitools.ImageTester;

import com.applitools.ImageTester.Interfaces.ITestable;
import com.applitools.eyes.*;
import com.applitools.eyes.images.Eyes;
import org.apache.commons.cli.*;
import org.apache.log4j.*;

import java.io.File;
import java.io.IOException;
import java.io.PrintStream;
import java.net.URI;
import java.net.URISyntaxException;

public class ImageTester {
    private static final String cur_ver = "0.2.7"; //TODO find more suitable place and logic
    private static final String eyes_utils = "EyesUtilities.jar";

    private static boolean eyes_utils_enabled = false;

    public static void main(String[] args) {
        PrintStream out = System.out;
        eyes_utils_enabled = new File(eyes_utils).exists();
        CommandLineParser parser = new DefaultParser();
        Options options = getOptions();

        // This part disables log4j warnings
        org.apache.log4j.Logger.getRootLogger().setLevel(Level.OFF);

        try {
            CommandLine cmd = parser.parse(options, args);
            Eyes eyes = new Eyes() {
                @Override
                public String getBaseAgentId() {
                    return String.format("ImageTester/%s [%s]", cur_ver, super.getBaseAgentId());
                }
            };

            //API key
            eyes.setApiKey(cmd.getOptionValue("k"));
            // Applitools Server url
            if (cmd.hasOption("s")) eyes.setServerUrl(new URI(cmd.getOptionValue("s")));
            // Match level
            if (cmd.hasOption("ml")) eyes.setMatchLevel(Utils.parseEnum(MatchLevel.class, cmd.getOptionValue("ml")));
            // Proxy
            if (cmd.hasOption("p")) {
                String[] proxyops = cmd.getOptionValues("p");
                if (proxyops.length == 1)
                    eyes.setProxy(new ProxySettings(proxyops[0]));
                else if (proxyops.length == 3) {
                    eyes.setProxy(new ProxySettings(proxyops[0], proxyops[1], proxyops[2]));
                } else
                    throw new ParseException("Proxy setting are invalid");
            }

            // Branch name
            if (cmd.hasOption("br")) eyes.setBranchName(cmd.getOptionValue("br"));
            // Parent branch
            if (cmd.hasOption("pb")) eyes.setParentBranchName(cmd.getOptionValue("pb"));
            // Baseline name
            if (cmd.hasOption("bn")) eyes.setBaselineEnvName(cmd.getOptionValue("bn"));
            // Parent branch
            if (cmd.hasOption("pb") && !cmd.hasOption("br"))
                throw new ParseException("Parent Branches (pb) should be combined with branches (br).");
            // Log file
            if (cmd.hasOption("lf")) eyes.setLogHandler(new FileLogger(cmd.getOptionValue("lf"), true, true));
            //host os
            if (cmd.hasOption("os")) eyes.setHostOS(cmd.getOptionValue("os"));
            //host app
            if (cmd.hasOption("ap")) eyes.setHostApp(cmd.getOptionValue("ap"));
            // Set failed tests
            eyes.setSaveFailedTests(cmd.hasOption("as"));
            // Viewport size
            RectangleSize viewport = null;
            if (cmd.hasOption("vs")) {
                String[] dims = cmd.getOptionValue("vs").split("x");
                if (dims.length != 2)
                    throw new ParseException("invalid viewport-size, make sure the call is -vs <width>x<height>");
                viewport = new RectangleSize(Integer.parseInt(dims[0]), Integer.parseInt(dims[1]));
            }
            // Folder path
            File root = new File(cmd.getOptionValue("f", "."));
            root = new File(root.getCanonicalPath());

            SuiteBuilder builder = new SuiteBuilder(root, cmd.getOptionValue("a", "ImageTester"), viewport);

            //DPI
            builder.setDpi(Float.valueOf(cmd.getOptionValue("dpi", "300")));

            // Determine Pages to include
            if (cmd.hasOption("sp")) builder.setPages(cmd.getOptionValue("sp"));

            // Read PDF Password
            if (cmd.hasOption("pp")) builder.setPdfPassword(cmd.getOptionValue("pp"));



            if (eyes_utils_enabled) builder.setEyesUtilitiesConfig(new EyesUtilitiesConfig(cmd));


            ITestable suite = builder.build();
            if (suite == null) {
                System.out.printf("Nothing to test!\n");
                System.exit(0);
            }

            suite.run(eyes);
        } catch (ParseException e) {
            out.println(e.getMessage());
            HelpFormatter formatter = new HelpFormatter();
            formatter.printHelp("ImageTester -k <api-key> [options]", options);
        } catch (URISyntaxException e) {
            e.printStackTrace();
            System.exit(-1);
        } catch (IOException e) {
            e.printStackTrace();
            System.exit(-1);
        }
    }

    private static Options getOptions() {
        Options options = new Options();

        options.addOption(Option.builder("k")
                .longOpt("apiKey")
                .desc("Applitools api key")
                .required()
                .hasArg().argName("apikey")
                .build());

        options.addOption(Option.builder("a")
                .longOpt("AppName")
                .desc("Set own application name, default: ImageTester")
                .hasArg()
                .argName("name")
                .build()
        );

        options.addOption(Option.builder("f")
                .longOpt("folder")
                .desc("Set the root folder to start the analysis, default: \\.")
                .hasArg()
                .argName("path")
                .build()
        );

        options.addOption(Option.builder("p")
                .longOpt("proxy")
                .desc("Set proxy address")
                .numberOfArgs(3)
                .optionalArg(true)
                .valueSeparator(';') //; and not : to avoid split of the http: part.
                .argName("url [;user;password]")
                .build()
        );

        options.addOption(Option.builder("s")
                .longOpt("server")
                .desc("Set Applitools server url")
                .hasArg()
                .argName("url")
                .build()
        );

        options.addOption(Option.builder("ml")
                .longOpt("matchLevel")
                .desc(String.format("Set match level to one of [%s], default = Strict", Utils.getEnumValues(MatchLevels.class)))
                .hasArg()
                .argName("level")
                .build());

        options.addOption(Option.builder("br")
                .longOpt("branch")
                .desc("Set branch name")
                .hasArg()
                .argName("name")
                .build());

        options.addOption(Option.builder("pb")
                .longOpt("parentBranch")
                .desc("Set parent branch name, optional when working with branches")
                .hasArg()
                .argName("name")
                .build());

        options.addOption(Option.builder("bn")
                .longOpt("baseline")
                .desc("Set baseline name")
                .hasArg()
                .argName("name")
                .build());

        options.addOption(Option.builder("vs")
                .longOpt("viewportsize")
                .desc("Declare viewport size identifier <width>x<height> ie. 1000x600, if not set,default will be the first image in every test")
                .hasArg()
                .argName("size")
                .build());

        options.addOption(Option.builder("lf")
                .longOpt("logFile")
                .desc("Specify Applitools log-file")
                .hasArg()
                .argName("file")
                .build());

        options.addOption(Option.builder("as")
                .longOpt("autoSave")
                .desc("Automatically save failed tests. Waring, might save buggy baselines without human inspection. ")
                .hasArg(false)
                .build());

        options.addOption(Option.builder("os")
                .longOpt("hostOs")
                .desc("Set OS identifier for the screens under test")
                .hasArg()
                .argName("os")
                .build());

        options.addOption(Option.builder("ap")
                .longOpt("hostApp")
                .desc("Set Host-app identifier for the screens under test")
                .hasArg()
                .argName("app")
                .build());
        options.addOption(Option.builder("di")
                .longOpt("dpi")
                .desc("PDF conversion dots per inch parameter default value 300")
                .hasArg()
                .argName("Dpi")
                .build());
        options.addOption(Option.builder("sp")
                .longOpt("selectedPages")
                .desc("PDF pages to select")
                .hasArg()
                .argName("Pages")
                .build());
        options.addOption(Option.builder("pp")
                .longOpt("PDFPassword")
                .desc("PDF Password")
                .hasArg()
                .argName("Password")
                .build());


        if (eyes_utils_enabled) {
            System.out.printf("%s is integrated, extra features are available. \n", eyes_utils);
            options.addOption(Option.builder("vk")
                    .longOpt("viewKey")
                    .desc("Specify enterprise view-key for additional api functions")
                    .hasArg()
                    .argName("key")
                    .build()
            );

            options.addOption(Option.builder("of")
                    .longOpt("outFolder")
                    .hasArg()
                    .desc("Specify the output target folder for the images results")
                    .argName("folder")
                    .build()
            );

            options.addOption(Option.builder("gd")
                    .longOpt("getDiffs")
                    .desc("Download diffs")
                    .hasArg(false)
                    .build()
            );

            options.addOption(Option.builder("gi")
                    .longOpt("getImages")
                    .desc("Download baseline and actual images")
                    .hasArg(false)
                    .build()
            );

            options.addOption(Option.builder("gg")
                    .longOpt("getGifs")
                    .desc("Download animated gif of the results")
                    .hasArg(false)
                    .build()
            );
        }
        return options;
    }
}

package com.applitools.ImageTester.TestObjects;

import com.applitools.Commands.AnimatedDiffs;
import com.applitools.Commands.DownloadDiffs;
import com.applitools.Commands.DownloadImages;
import com.applitools.ImageTester.EyesUtilitiesConfig;
import com.applitools.ImageTester.Interfaces.IDisposable;
import com.applitools.ImageTester.Interfaces.ITestable;
import com.applitools.eyes.EyesException;
import com.applitools.eyes.RectangleSize;
import com.applitools.eyes.TestResults;
import com.applitools.eyes.images.Eyes;

import java.io.File;
import java.util.*;

public class Test extends TestUnit {

    protected final String appname_;
    protected RectangleSize viewportSize_;
    private Queue<ITestable> steps_;
    private EyesUtilitiesConfig eyesUtilitiesConfig_;


    public Test(File file, String appname) {
        this(file, appname, null);
    }

    public Test(File file, String appname, RectangleSize viewportSize) {
        super(file);
        steps_ = new LinkedList<ITestable>();
        appname_ = appname;
        viewportSize_ = viewportSize;
    }

    public void run(Eyes eyes) {
        eyes.open(appname_, name(), viewportSize_);
        for (ITestable step : steps_) {
            try {
                step.run(eyes);
                //Disposing steps without regions immediately
                if (step instanceof IDisposable)
                    if (!(step instanceof ImageStep && ((ImageStep) step).hasRegionFile()))
                        ((IDisposable) step).dispose();
            } catch (Throwable e) {
                System.out.printf("Error in Step %s: \n %s \n This step will be skipped!", step.name(), e.getMessage());
                e.printStackTrace();
            }
        }
        try {
            TestResults result = eyes.close(false);
            printTestResults(result);
            handleResultsDownload(result);
        } catch (
                EyesException e)

        {
            System.out.printf("Error closing test %s \nPath: %s \nReason: %s \n",
                    name(),
                    file_.getAbsolutePath(),
                    e.getMessage());

            System.out.println("Aborting...");
            try {
                eyes.abortIfNotClosed();
                System.out.printf("Aborted!");
            } catch (Throwable ex) {
                System.out.printf("Error while aborting: %s", ex.getMessage());
                System.out.println("I don't have any idea what just happened.");
                System.out.println("Please try reaching our support at support@applitools.com");
            }
        } catch (
                Exception e)

        {
            System.out.println("Oops, something went wrong!");
            System.out.print(e);
            e.printStackTrace();
        }

    }

    protected void handleResultsDownload(TestResults results) throws Exception {
        if (eyesUtilitiesConfig_ == null) return;
        if (eyesUtilitiesConfig_.getDownloadDiffs() || eyesUtilitiesConfig_.getGetGifs() || eyesUtilitiesConfig_.getGetImages()) {
            if (eyesUtilitiesConfig_.getViewKey() == null) throw new RuntimeException("The view-key cannot be null");
            if (eyesUtilitiesConfig_.getDownloadDiffs() && !results.isNew() && !results.isPassed())
                new DownloadDiffs(results.getUrl(), eyesUtilitiesConfig_.getDestinationFolder(), eyesUtilitiesConfig_.getViewKey()).run();
            if (eyesUtilitiesConfig_.getGetGifs() && !results.isNew() && !results.isPassed())
                new AnimatedDiffs(results.getUrl(), eyesUtilitiesConfig_.getDestinationFolder(), eyesUtilitiesConfig_.getViewKey()).run();
            if (eyesUtilitiesConfig_.getGetImages())
                new DownloadImages(results.getUrl(), eyesUtilitiesConfig_.getDestinationFolder(), eyesUtilitiesConfig_.getViewKey(), false, false).run();
        }
    }

    public void addStep(ImageStep step) {
        steps_.add(step);
    }

    public void addSteps(Collection<ITestable> steps) {
        steps_.addAll(steps);
    }

    protected void printTestResults(TestResults result) {
        String res = result.getSteps() > 0 ? (result.isNew() ? "New" : (result.isPassed() ? "Passed" : "Failed")) : "Empty";
        System.out.printf("\t[%s] - %s", res, name());
        if (!result.isPassed() && !result.isNew())
            System.out.printf("\tResult url: %s", result.getUrl());
        System.out.println();

    }

    public void setEyesUtilitiesConfig(EyesUtilitiesConfig eyesUtilitiesConfig) {
        eyesUtilitiesConfig_ = eyesUtilitiesConfig;
    }

    protected static List<Integer> parsePagesToList(String input) {
        if (input == null) return null;
        ArrayList<Integer> pagesToInclude = new ArrayList<Integer>();
        String[] inputPages = input.split(",");
        for (int i = 0; i < inputPages.length; i++) {
            if (inputPages[i].contains("-")) {
                int left = Integer.valueOf(inputPages[i].split("-")[0]);
                int right = Integer.valueOf(inputPages[i].split("-")[1]);
                if (left <= right) {
                    for (int j = left; j <= right; j++) {
                        pagesToInclude.add(j);
                    }
                } else {
                    for (int j = left; j >= right; j--) {
                        pagesToInclude.add(j);
                    }
                }
            } else {
                pagesToInclude.add(Integer.valueOf(inputPages[i]));
            }
        }
        return pagesToInclude;

    }

    public void dispose() {
        if (steps_ == null) return;
        for (ITestable step : steps_) {
            if (step instanceof IDisposable) ((IDisposable) step).dispose();
        }
    }
}

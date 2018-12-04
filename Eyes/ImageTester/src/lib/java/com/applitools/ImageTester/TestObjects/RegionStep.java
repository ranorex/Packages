package com.applitools.ImageTester.TestObjects;

import com.applitools.ImageTester.Interfaces.ITestable;
import com.applitools.eyes.Region;
import com.applitools.eyes.images.Eyes;
import com.opencsv.CSVReader;
import org.apache.commons.io.FilenameUtils;
import org.apache.commons.lang3.StringUtils;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;


public class RegionStep implements ITestable {
    private static final String REGION_FILE_TMPL = "%s.regions";
    private static final String STEP_FORMAT = "%s-%s";

    private final ImageStep step_;
    private final Region region_;
    private final String tag_;

    private RegionStep(ImageStep step, Region region, String tag) {
        step_ = step;
        region_ = region;
        tag_ = tag;
    }

    public void run(Eyes eyes) throws IOException {
        eyes.checkRegion(step_.getImage(), region_, tag_);
    }

    public String name() {
        return String.format("Image: %s, region: %s", step_.name(), region_.toString());
    }

    public static boolean supports(ImageStep step) {
        return regionsFile(step.file_).exists();
    }

    public static Collection<ITestable> getSteps(ImageStep step) throws IOException {
        int stepno = 1;
        List<ITestable> regions = new ArrayList<ITestable>();
        File regionsFile = regionsFile(step.file_);
        CSVReader reader = getReader(regionsFile);
        List<String[]> lines = reader.readAll();
        int l, t, w, h;
        for (String[] line : lines) {
            if (StringUtils.isEmpty(line[0])) {
                regions.add(new ImageStep(step.file_));
                continue;
            } else if (line.length != 4) throw new IOException(
                    "Invalid csv formatting for file " + regionsFile.getName() + " Should be left,top,width,height");

            l = Integer.parseInt(line[0]);
            t = Integer.parseInt(line[1]);
            w = Integer.parseInt(line[2]);
            h = Integer.parseInt(line[3]);
            String stepName = String.format(STEP_FORMAT, step.name(), stepno);
            regions.add(new RegionStep(step, new Region(l, t, w, h), stepName));
            ++stepno;
        }
        return regions;
    }

    private static CSVReader getReader(File regionfile) {
        try {
            return new CSVReader(new FileReader(regionfile));
        } catch (FileNotFoundException e) {
            e.printStackTrace();
            throw new RuntimeException("This is a bug! Region file should be checked for existense");
        }
    }

    private static File regionsFile(File imageFile) {
        String regionFilePath = String.format(REGION_FILE_TMPL, FilenameUtils.removeExtension(imageFile.getName()));
        regionFilePath = FilenameUtils.concat(imageFile.getParent(), regionFilePath);
        return new File(regionFilePath);
    }
}

package com.applitools.ImageTester.TestObjects;

import com.applitools.ImageTester.Interfaces.ITestable;
import com.applitools.ImageTester.Patterns;
import com.applitools.eyes.images.Eyes;

import javax.imageio.ImageIO;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.util.Collection;
import java.util.regex.Pattern;

public class ImageStep extends TestUnit {
    private static final Pattern pattern = Patterns.IMAGE;
    private BufferedImage img_;

    public ImageStep(File file) {
        super(file);
    }

    public void run(Eyes eyes) throws IOException {
        try {
            eyes.checkImage(getImage(), name());
        } catch (IOException e) {
            System.out.printf("Failed to process image file: %s \n Reason: %s \n",
                    file_,
                    e.getMessage());
            throw e;
        }
    }

    public BufferedImage getImage() throws IOException {
        if (img_ == null) {
            img_ = ImageIO.read(file_);
        }
        return img_;
    }

    @Override
    public String name() {
        return super.name().replaceAll(Patterns.IMAGE_EXT, "");
    }

    public static boolean supports(File file) {
        return pattern.matcher(file.getName()).matches();
    }

    public boolean hasRegionFile() {
        return RegionStep.supports(this);
    }

    public Collection<ITestable> getRegions() throws IOException {
        return RegionStep.getSteps(this);
    }

    public void dispose() {
        if (img_ != null) img_ = null;
    }
}

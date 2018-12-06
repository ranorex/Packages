package com.applitools.ImageTester;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.ParseException;

/**
 * Created by liranbarokas on 08/05/2017.
 */
public class EyesUtilitiesConfig {

    private String viewKey_;
    private String destinationFolder_;
    private Boolean downloadDiffs_ = false;
    private Boolean getImages_ = false;
    private Boolean getGifs_ = false;


    private EyesUtilitiesConfig(String viewKey, String destinationFolder, Boolean downloadDiffs, Boolean getImages, Boolean getGifs) {
        viewKey_ = viewKey;
        destinationFolder_ = destinationFolder;
        downloadDiffs_ = downloadDiffs;
        getImages_ = getImages;
        getGifs_ = getGifs;
    }

    public EyesUtilitiesConfig(CommandLine cmd) throws ParseException {
        if (cmd.hasOption("gd") || cmd.hasOption("gi") || cmd.hasOption("gg")) {
            if (!cmd.hasOption("vk"))
                throw new ParseException("gd|gi|gg must be called with enterprise view-key (vk)");
            viewKey_ = cmd.getOptionValue("vk");
            destinationFolder_ = cmd.getOptionValue("of", "./");
            downloadDiffs_ = cmd.hasOption("gd");
            getImages_ = cmd.hasOption("gi");
            getGifs_ = cmd.hasOption("gg");
        }
    }


    public String getViewKey() {
        return viewKey_;
    }

    public void setViewKey(String viewKey) {
        this.viewKey_ = viewKey;
    }

    public String getDestinationFolder() {
        return destinationFolder_;
    }

    public void setDestinationFolder(String destinationFolder) {
        this.destinationFolder_ = destinationFolder;
    }

    public Boolean getDownloadDiffs() {
        return downloadDiffs_;
    }

    public void setDownloadDiffs(Boolean downloadDiffs) {
        this.downloadDiffs_ = downloadDiffs;
    }

    public Boolean getGetImages() {
        return getImages_;
    }

    public void setGetImages(Boolean getImages) {
        this.getImages_ = getImages;
    }

    public Boolean getGetGifs() {
        return getGifs_;
    }

    public void setGetGifs(Boolean getGifs) {
        this.getGifs_ = getGifs;
    }


}

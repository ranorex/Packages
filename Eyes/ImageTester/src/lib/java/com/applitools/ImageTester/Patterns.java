package com.applitools.ImageTester;

import java.util.regex.Pattern;

/**
 * Created by yanir on 01/09/2016.
 */
public abstract class Patterns {
    public static final String IMAGE_EXT = "(\\.(?i)(jpg|png|gif|bmp))$";
    private static final String IMAGE_PATTERN = "(.+)" + IMAGE_EXT;
    public static final Pattern IMAGE = Pattern.compile(IMAGE_PATTERN);
    private static final String PDF_EXT = "(?i)(\\.Pdf)$";
    private static final String PDF_PATTERN = "(.+)" + PDF_EXT;
    public static final Pattern PDF = Pattern.compile(PDF_PATTERN);
}

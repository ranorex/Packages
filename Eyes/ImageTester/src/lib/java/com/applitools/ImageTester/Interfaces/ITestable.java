package com.applitools.ImageTester.Interfaces;


import com.applitools.eyes.images.Eyes;

import java.io.IOException;

public interface ITestable {
    void run(Eyes eyes) throws IOException;

    String name();
}

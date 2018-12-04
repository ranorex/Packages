package com.applitools.ImageTester.TestObjects;

import com.applitools.ImageTester.Interfaces.IDisposable;
import com.applitools.ImageTester.Interfaces.ITestable;

import java.io.File;

public abstract class TestUnit implements ITestable, IDisposable {
    protected final File file_;
    protected String name_;

    protected TestUnit(File file) {
        file_ = file;
        if (file != null && !file.canRead()) {
            throw new RuntimeException(String.format("Unreadable path/file %s, might be a permission issue!", file.getAbsolutePath()));
        }
    }

    protected TestUnit(String name) {
        file_ = null;
        name_ = name;
    }

    public String name() {
        return file_ == null ? name_ : file_.getName();
    }

    public File getFile() {
        return file_;
    }

}

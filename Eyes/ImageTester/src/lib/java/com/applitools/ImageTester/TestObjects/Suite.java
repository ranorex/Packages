package com.applitools.ImageTester.TestObjects;


import com.applitools.eyes.images.Eyes;

import java.io.File;
import java.io.IOException;
import java.util.LinkedList;
import java.util.Queue;

public class Suite extends TestUnit {
    private Queue<Batch> batches_;
    private Test test_ = null;

    public Suite(File file) {
        super(file);
        batches_ = new LinkedList<Batch>();
    }

    public void run(Eyes eyes) throws IOException {
        if (batches_.isEmpty() && test_ == null) {
            System.out.printf("Nothing to test!\n");
            return;
        }

        for (Batch batch : batches_) {
            try {
                batch.run(eyes);
            }finally {
                batch.dispose();
            }
        }

        if (test_ != null) {
            System.out.printf("> No batch <\n");
            test_.run(eyes);
        }
    }

    public void addBatch(Batch batch) {
        batches_.add(batch);
    }

    public void portBatchesTo(Suite destination) {
        destination.batches_.addAll(batches_);
        batches_.clear();
    }

    public void portTestTo(Batch destination) {
        destination.addTest(test_);
        test_ = null;
    }

    public boolean hasOrphanTest() {
        return test_ != null;
    }

    public void setTest(Test test) {
        if (test_ != null) throw new RuntimeException("test is not null as expected!");
        test_ = test;
    }

    public void dispose() {
        if (test_ != null) test_.dispose();
    }
}

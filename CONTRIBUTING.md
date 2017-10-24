# How to contribute

We believe that software test automation makes the world a better place and this is your chance to help by improving some of the NuGet packages we use in our beloved Ranorex product! For this here are the guidelines we'd like you to follow:

#### Table Of Contents



[Contribute to code](#contribute-to-code)
  * [Fork this repository](#fork-this-repository)
  * [Clone this repository](#clone-this-repository)
  * [Write code](#write-code)
  * [Write/run tests](#write/run-tests)
  * [Commit your changes](#commit-your-changes)
  * [Sync your fork](#sync-your-fork)
  * [Update your branch](#update-your-branch)

[Styleguides](#styleguides)
  * [Coding guideline](#coding-guideline)
  * [Commit message guideline](#commit-message-guideline)

[I just have an idea for a feature!!](#i-just-have-an-idea-for-a-feature!!)

## Contribute to Code

### Fork this repository

Forking a repository allows you to freely experiment with changes without affecting the original project:

1. Navigate to the [ranorex/Packages](https://github.com/ranorex/Packages) repository.
2. In the top-right corner of the page, click Fork.

### Clone this repository

To be able to contribute code, you first need to clone this repository:
    
    $ git clone https://github.com/YOUR-USERNAME/Packages.git

and create a new branch for your changes:

    $ cd packages
    $ git checkout -b new_branch_name

### Write code

Modify the code in your branch now like you want, but we hope that you are planning to submit your change back to this repository, so please keep a few things in mind: [Coding Guideline](#coding-guideline)

### Write/run tests

Please make sure to provide unit tests for your new functionality, if it is applicable. Run the appropriate unit tests, include tests that fail without your new code and pass with it.

### Commit your changes

When you're happy with the code on your computer, you need to commit the changes to Git:

    $ git commit -a

This should fire up your editor to write a commit message. When you have finished, save and close to continue.

A well-formatted and descriptive commit message is very helpful to others for understanding why the change was made, so please take the time to write it.

A good commit message looks like this: [Commit Message Guideline](#commit-message-guideline)

### Sync your fork

Make sure your fork is up to date by typing

    $ git remote -v

You'll see the current configured remote repository for your fork.

    origin  https://github.com/YOUR-USERNAME/Packages.git (fetch)
    origin  https://github.com/YOUR-USERNAME/Packages.git (push)

For the first time the ranorex/Packages remote repository is missing, you have to add it to your local repository on your local machine by typing:

    $ git remote add upstream https://github.com/ranorex/Packages.git

Verify that ranorex/Packages remote is added as 'upstream' by again typing:

    $ git remote -v

Now you can fetch the branches and their respective commits from the upstream repository:

    $ git fetch upstream

If there are changes on upstream/master you can merge them into your local master branch. This brings your fork's master branch into sync with the upstream repository, without losing your local changes:

    $ git checkout master
    $ git merge upstream/master
    $ git push origin master

### Update your branch

It's pretty likely that other changes to master have happened while you were working. Go get them:

    $ git pull --rebase

Now reapply your patch on top of the latest changes:

    $ git checkout my_new_branch
    $ git rebase master
    $ git push origin my_new_branch

### Submitting changes

Please send a [GitHub Pull Request](https://github.com/ranorex/Packages/pull/new/master) with a description of your changes (read more about [pull requests](http://help.github.com/pull-requests/)). Please follow our coding conventions (below) and make sure all of your commits are there and are atomic (one feature per commit).
When ok, press "Send pull request", we will be notified about your submission and provide you with feedback.

## Styleguides

### Coding guideline

+ Use Spaces instead of Tabs
+ Indentation: 4 spaces
+ Stay under a total width of 120 characters
+ Line Endings: Windows `\r\n`
+ Trim trailing whitespaces
+ Start brackets on separate lines
+ Always make brackets
+ Separate expression with a single space for if's, for's,...
+ Follow the conventions in the source you see used already.

These are guidelines - please use your best judgment in using them. If violations of these rules are detected during code reviews we will contact you to make necessary changes to your code so that a potential merge can be accepted.

### Commit message guideline

1. Header **[required]**
    + One Line
    + 72 characters max
    + May start with a 'Helpers: '
    + Imperative form, present tense
    + No period at end
2. Details block **[optional]**
    + Follows header separated by newline
    + Wrapped at 72 characters
    + Focus on: Why, Side effects, consequences...
    + May replicate parts of linked work item Details
    + Can consist of multiple paragraphs separated by single blank line
3. Reference section **[optional]**
    + Separated from previous section via blank line
    + Reference issues and pull requests

## I just have an idea for a feature!!

Fill the gap between your needs and our development focus using [Ranorex User voice](https://www.ranorex.com/uservoice.html).

# How to contribute

We believe that software test automation makes the world a better place and this is your chance to help by improving some of the NuGet packages we use in Ranorex Studio! Below you'll find useful instructions and guidelines. Follow them to make contributing as smooth as possible.

#### Table Of Contents

[I have an idea for a feature!](#i-have-an-idea-for-a-feature!)

[Contributing to code](#contributing-to-code)
  * [Fork this repository](#fork-this-repository)
  * [Clone this repository](#clone-this-repository)
  * [Write code](#write-code)
  * [Run tests](#run-tests)
  * [Commit your changes](#commit-your-changes)
  * [Sync your fork](#sync-your-fork)
  * [Update your branch](#update-your-branch)

[Style guides](#styleguides)
  * [Coding style guide](#coding-style-guide)
  * [Commit message style guide](#commit-message-style-guide)

## I have an idea for a feature!

Fill the gap between your needs and our development focus by telling us about your ideo on [Ranorex UserVoice](https://www.ranorex.com/uservoice.html).

## Contributing to code

### Fork this repository

Fork the repository to freely experiment with changes without affecting the original project.

1. Navigate to the [ranorex/Packages](https://github.com/ranorex/Packages) repository.
2. In the top-right corner of the page, click Fork.

### Clone this repository

To be able to contribute code, you first need to clone this repository:
    
    $ git clone https://github.com/YOUR-USERNAME/Packages.git

Then create a new branch for your changes:

    $ cd packages
    $ git checkout -b new_branch_name

### Write code

Now modify the code in your branch as you desire. If you plan on submitting your changes back to this repository, which we hope you do, please make sure to follow the [Coding style guide](#coding-style-guide)

### Run tests

Please make sure to run the appropriate unit tests. Also include tests that fail without your new code, and pass with it.

### Commit your changes

When you're happy with the code on your computer, commit the changes to Git:

    $ git commit -a

This should fire up your editor to write a commit message. A well-formatted and descriptive commit message really helps others understand why you made your changes, so please take the time to write it. Simply follow our [Commit message style guide](#commit-message-style-guide).

When you're done, save and close to continue.

### Sync your fork

Make sure your fork is up to date:

    $ git remote -v

You'll see the currently configured remote repository for your fork.

    origin  https://github.com/YOUR-USERNAME/Packages.git (fetch)
    origin  https://github.com/YOUR-USERNAME/Packages.git (push)

Initially, the ranorex/Packages remote repository will be missing. You have to add it to your local repository on your local machine:

    $ git remote add upstream https://github.com/ranorex/Packages.git

Verify that it was added as 'upstream':

    $ git remote -v

Now you can fetch the branches and their respective commits from the upstream repository:

    $ git fetch upstream

If there are changes on upstream/master you can merge them into your local master branch. This syncs your fork's master branch with the upstream repository while keeping your local changes:

    $ git checkout master
    $ git merge upstream/master
    $ git push origin master

### Update your branch

It's pretty likely that other changes to master were made while you were working on your code. Go get them:

    $ git pull --rebase

Now reapply your patch on top of the latest changes:

    $ git checkout my_new_branch
    $ git rebase master
    $ git push origin my_new_branch

### Submit your changes

Please send a [GitHub Pull Request](https://github.com/ranorex/Packages/pull/new/integration) *to the integration branch* with a description of your changes (read more about [pull requests](https://help.github.com/articles/creating-a-pull-request-from-a-fork/)). Please follow our coding style guide (#coding-styleguide) and make sure all of your commits are there and atomic (one feature per commit).
Once everything's correct, press "Send pull request". We will then be notified about your submission and provide you with feedback.

## Style guides

### Coding style guide

+ Use spaces instead of tabs
+ Indentation: 4 spaces
+ 120 characters max per line
+ Line endings: Windows `\r\n`
+ Trim trailing whitespaces
+ Always use brackets
+ Place brackets on separate lines
+ Separate expressions with a single space for if, for,...
+ Follow the conventions already present in the source

If we find your code doesn't follow these guidelines during a code review, we will contact you so you can make the necessary changes and a potential merge can be accepted.

### Commit message style guide

1. Header **[required]**
    + One line
    + 72 characters max
    + May start with 'Helpers: '
    + Imperative form, present tense
    + No period at the end
2. Details block **[optional]**
    + Separated from header by a blank line
    + Wrapped at 72 characters
    + Focus on: why, side effects, consequences...
    + May repeat parts of linked work item details
    + May contain multiple paragraphs, each separated by a blank line
3. Reference section **[optional]**
    + Separated from previous section by a blank line
    + Reference issues and pull requests
4. 

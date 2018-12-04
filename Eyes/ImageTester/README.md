# Image Tester [ ![Download](https://api.bintray.com/packages/applitoolseyes/generic/ImageTester/images/download.svg) ](https://bintray.com/applitoolseyes/generic/download_file?file_path=ImageTester.jar)
ImageTester is a Cli tool to perform visual tests on images or PDF files. 

If you don't have your Applitools account yet, 
please [sign up first]("https://applitools.com/sign-up/") 
and get your Applitools api-key that will be used next to execute the tests.

The tool can be invoked on a single file or a complex folder structure with mixed content.
Once provided a complex folder structure the tool recursively scans the structure and determines on each level what should be the batch-name, 
the test-name and the tag values relatively to the target files.  
For example, given the following folder structure:
```
+- Folder A  
|  +- Folder B
|  |  +- Screenshot1.png
|  |  +- Screenshot2.png
|  +- Folder C
|  |  +- Screenshot3.png
|  |  +- Screenshot4.png
|  |  +- Screenshot5.png
```
The following tests will be generated in Applitools:
```
Batch name - A:
    Test name - B > steps: Screenshot1, Screenshot2
    Test name - C > steps: Screenshot3, Screenshot4, Screenshot5
```

The parameters for Applitools test will be derived from the file and the folder structure according to the following table:

|            | Single image file                           | Multi-image file (PDF)            | 
|------------|---------------------------------------------|-----------------------------------|
| Step tag   | The filename                                | Step index                        |
| Test name  | Parent folder-name if applicable*           | The filename                      |
| Batch name | 2nd level parent folfer-name if applicable* | Parent folder-name if applicable* |

\* When no sufficient levels in the specified structure to derive all test parameters, the values will be taken from the child value.
For example, for the following structure:
```
+- Folder A
|  +- Screenshot1.png
|  +- Screenshot2.png

```
The following test will be generated in Applitools:
```
Batch name - A:
    Test name - A > steps: Screenshot1, Screenshot2
```
Note that the batch name was derived from the test name as there is no additional 
folder level that can be used as batch name.

## Execution
The tool build in java and requires minimal set of parametersm the minimal command will look as follow:

>java -jar ImageTester.jar -k [api-key]

\* In the minimal set of parameters will assume that the search folder is the execution folder.

+ Required parameters:
    + `-k [api-key]` - Applitools api key
+ Optional paramaeters:
    + `-f [path]` - A path to target folder or file
    + `-a [app-name]` - Set custom application name; default = ImageTester
    + `-p [http://proxy{;user;pass}]` - Set proxy and optional username + password
    + `-s [server]` - Set Applitools server url
    + `-ml [match-level]` - Set the comparison level, one from Strict/Content/Layout; Default = Strict
    + `-br [branch]` - Set the branch
    + `-pb [parent-branch]` - Set the parent branch
    + `-bn [baseline]` - Set custom baseline name
    + `-vs [WidthxHeight]` - Set the viewport size identifier
    + `-lf [log-file]` - Set log fine name to enable logging
    + `-as` - Set automatic save on failures
    + `-os [osname]` - Set custom os
    + `-ap [app name]` - Set browser or equivalent application name  
    ######For PDFs only
    + `-di [dpi]` - Set the quality of the conversion on PDF files
    + `-sp [pages]` - Comma separated page numbers to include in PDF testing (ie: 1,2,5,7); Default all included
    + `-pp [password]` - The password if the PDF files protected
 
## Enterprise features in combination with [Eyes Utilities](https://github.com/yanirta/EyesUtilities)
By placing the Eyes-Utilities jar into the same folder as the ImageTester, new enterprise api features
made possible by providing an enterprise read-key.

>java -jar ImageTester.jar -k [api-key] -vk [view-key] [options]
+ Required parameters:
    + `-k [api-key]` - Applitools api key
    + `-vk [view-key]` - Applitools enterprise view-key
+ Selective flags - Required one or more
    + `-gd` - Get diff images of the failed steps
    + `-gi` - Get images of the failed steps
    + `-gd` - Get animated gifs of the failed steps

# Supporting regions
To validate only a specific regions of a particular image the folder must contain .regions file with the same name as the image.
ie. If my image file is 'step1.png' then the regions file should be 'step1.regions'.
Internally the format should be 4 columns csv in the following order: left,top,width,height

Here is an example of what can be step1.regions:
```
0,0,100,200
500,100,240,123
```
In order to include a capture of the entire image too, add an empty line as part of the regions list.

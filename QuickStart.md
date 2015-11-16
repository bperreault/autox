_A quick tutorial of this software._

# Quick Start #
After installation and configuration properly finished, then you can start the Controller and see the controller screen.
https://autox.googlecode.com/git/dotnet/Docs/Controller.PNG
  * Tip: Double click on the tree items, it will expand its contents.
  * Tip: Try right click on these items, see what will happen.

## Get the UI Objects ##
Click from Menu **Automation** -> **Get UI Objects**, it will start a browser, and capture some objects from the page. The results will be saved to an xml file. Then right click on one folder of **UI Object Pool**, choose **New GUI Item**, choose the xml file you just got. Then the UI Objects are put on the tree.

![https://autox.googlecode.com/git/dotnet/Docs/GetUIObjects.png](https://autox.googlecode.com/git/dotnet/Docs/GetUIObjects.png)

  * Tip: What kinds of objects will be capture? Check in **File** -> **Settings**, item **CSSType** Define the types of UI Objects that we are interested in.
  * Tip: To make your tree tidy, create some folders, give them some meaningful name. Drag the Objects to proper folder.

## Prepare Testing Data ##
Right click on one folder of **Data**, choose **New Data**, an **XML Element Editor** will show. Click **New Attribute** Button, give your new data a name, then in **XML Element Editor**, give it a new value. These data will be used as user data in automation test cases or test suites.
  * Tip: Automation will try to get data's value at runtime. So update a data will affect all the scripts that use it. Don't want this behaviour? OK, at Test screen, use **Default Data Value**, don't use data.

## Create Test Screen ##
Right click on project tree, choose **New Screen**; then drag UI Object from **UI Object Pool**, drop into it. Click on the button right to **Test Steps**, you can define the actions on the UI Object, define the data/default data they will require, and adjust the steps order.
https://autox.googlecode.com/git/dotnet/Docs/TestScreen.PNG
  * Tip: This is a template of screen: when it is called by test suite or test case, in fact, they have a copy of this test screen. Change the test screen will not affect the existed test suites and test cases.
  * Tip: This is basic unit of automation: every time the agent ask for next step, it will get all steps of one screen. So put the action will cause leaving current screen to the last step.

## Create Test Suite ##
Right click on project tree, choose **New Suite**; then drag the test screen you just created into it, and drag the data into it. A test suite is ready to serve! It will run the test screen with certain data. You can update its properties: Give it a better description, it will be useful while you analyse the result; Maturity has 3 options: backyard means it is just created, need debugging, cannot be used in formal automation test; Playground means it can work, but sometimes may failed, need to analyse the reason: AUT changed or found a bug; Stadium means it is most stable and important test, can be integrated into CI.

![https://autox.googlecode.com/git/dotnet/Docs/TestSuite.png](https://autox.googlecode.com/git/dotnet/Docs/TestSuite.png)

  * Tip: Click from **File** -> **Settings**, update item **Maturity** to **Playground;Stadium**; then go to tab **Monitor**, in project tree, all the **Backyard** test suite gone. Just prevent immature scripts running in formal environment to waste the resources. A further idea is that we can set up another server, only **Stadium** scripts can running on it, this stand for integration test; and **Playground** stand for functional test.

## Debug the Test Suite ##
Choose a test suite, then click from menu **Automation** -> **Debug Test**. It will start a browser, try your script on your local machine.
  * Tip: How to change to your favourite browser? Click from **File** -> **Settings**, item **BrowserType** define the default browser.
  * Tip: Above the root items of every tabs, there is a text box. Hard to see it? yes, I intend to make them almost invisible, just in case somebody does not like them. They are filter box, input a word then press Enter, it will filter the tree/table, only the items that contain the word will be showed on the tree/table.

## Check the Result ##
Click on tab "Result", expand the result tree, locate your test result. The failed step's "Link" column will contain a link of screen snapshot at the failure point.

![https://autox.googlecode.com/git/dotnet/Docs/Result.png](https://autox.googlecode.com/git/dotnet/Docs/Result.png)

  * Tip: On the result tree, choose a result, right click it, choose **Export**, will export the html format result out to your local disk. Sometimes, this one can provide more information for overview.

Now you can change the **DefaultURL** to point to your AUT, start your automation tasks.
  * _Just that? Of course no, you will find how to use real time monitor, automation agents and how to organize automation test in another doc._
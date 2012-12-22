package autox;

import autox.actions.ActionFactory;
import autox.communication.Communication;
import autox.config.Configuration;
import autox.log.Log;
import org.apache.commons.io.FileUtils;
import org.apache.commons.lang.exception.ExceptionUtils;

import java.io.*;
import java.nio.file.Files;

public class Start {

    /**
     * @param args
     */
    public static void main(String[] args) {
        // if run.mode = test , start command line mode to test
        String mode = Configuration.getInstance().get("run.mode", "run");
        Log.debug("Current Mode is:"+mode);
        Log.debug("Current Path is:"+System.getProperty("user.dir"));
        if (mode.equalsIgnoreCase("test")) {
            test();
        } else {

            while (true) {
                register();
                readRunReturnOneCommand();
            }
        }


    }

    private static void test() {
        while (true) {
            int choice = showMenu();
            switch (choice) {
                case 0:
                    return;
                case 1:
                    runXMLTestFile();
                    break;
                case 2:
                    register();
                    break;
                case 3:
                    readRunReturnOneCommand();
                    break;
                case 101:
                    testSetEnv();
                    break;
                case 102:
                    testWait();
                    break;
                case 103:
                    testStart();
                    break;
                case 104:
                    testClose();
                    break;
                case 105:
                    testCheck();
                    break;
                case 106:
                    testClick();
                    break;
                case 107:
                    testCommand();
                    break;
                case 108:
                    testEnter();
                    break;
                case 109:
                    testExisted();
                    break;
                case 110:
                    testUnknown();
                    break;
                case 111:
                    testGetValue();
                    break;
                case 113:
                    testNotExisted();
                    break;
                case 114:
                    testVerifyValue();
                    break;
                case 115:
                    testLogin();
                    break;
                default:
                    Write("\nPlease input a valid choice\n");
                    break;
            }
        }
    }

    private static void readRunReturnOneCommand() {
        //TODO add some error handling make it stronger
        String command = Communication.readCommand();
        String result = ActionFactory.handle(command);
        Communication.setResult(result);
    }

    private static void register() {
        Communication.register();
    }

    private static void testVerifyValue() {
        testXMLFile("./autox/testData/testVerifyValue.xml");
    }

    private static void testNotExisted() {
        testXMLFile("./autox/testData/testNotExisted.xml");
    }

    private static void testGetValue() {
        testXMLFile("./autox/testData/testGetValue.xml");
    }

    private static void testExisted() {
        testXMLFile("./autox/testData/testExisted.xml");
    }

    private static void testCommand() {
        testXMLFile("./autox/testData/testCommand.xml");
    }

    private static void testCheck() {
        testXMLFile("./autox/testData/testCheck.xml");
    }

    private static int showMenu() {
        Write("\n\nSimple Tests for AutoX Client");
        Write("0. Exit\t\t1. Run XML Test\t\t2. Get All Objects");
        Write("101. Test Set Env\t\t102. Test Wait\t\t103. Test Start\t\t104. Test Close\t\t105. Test Check");
        Write("106. Test Click\t\t107. Test Command\t\t108. Test Enter\t\t109. Test Exist\t\t110. Test Unknown");
        Write("111. Test GetValue\t\t112. Test TBD\t\t113. Test NotExisted\t\t114. Test VerifyValue\t\t115. Test Login");
        Write("Please input your choice:");
        try {
            return Integer.parseInt(ReadInput());
        } catch (Exception e) {

            Write(e.getMessage() + "\n" + ExceptionUtils.getStackTrace(e));
        }
        return -1;
    }

    private static void testClick() {
        testXMLFile("./autox/testData/testClick.xml");
    }

    private static void testEnter() {
        testXMLFile("./autox/testData/testEnter.xml");
    }

    private static void testLogin() {
        testXMLFile("./autox/testData/Login.xml");
    }

    private static void testClose() {
        testXMLFile("./autox/testData/testClose.xml");
    }

    private static void testStart() {
        testXMLFile("./autox/testData/testStart.xml");
    }

    private static void testUnknown() {
        testXMLFile("./autox/testData/SomeUnknown.xml");
    }

    private static void testWait() {
        testXMLFile("./autox/testData/Wait.xml");
    }

    private static void testSetEnv() {
        testXMLFile("./autox/testData/SetEnv.xml");
    }

    private static void runXMLTestFile() {
        String fileName = ReadInput("Please input the full name of XML Test file:");
        testXMLFile(fileName);
    }

    private static void testXMLFile(String fileName) {
        try {
            String xmlString = FileUtils.readFileToString(new File(fileName));
            String result  = ActionFactory.handle(xmlString);
            Write(result);
            System.in.read();

        } catch (Exception e) {
            System.out.println(e.getMessage()+"\n"+ ExceptionUtils.getFullStackTrace(e));
        }
    }



    private static String ReadInput(String message) {
        Write("\n" + message);
        return ReadInput();
    }

    private static String ReadInput() {
        try {
            BufferedReader br = new BufferedReader(new InputStreamReader(System.in));
            return br.readLine();
        } catch (Exception e) {

            Write(e.getMessage() + "\n" + ExceptionUtils.getStackTrace(e));
        }
        return "-1";
    }

    private static void Write(String message) {
        System.out.println(message);

    }

}

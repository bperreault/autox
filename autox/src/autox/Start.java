package autox;

import autox.actions.ActionFactory;
import autox.communication.Communication;
import autox.config.Configuration;
import org.apache.commons.lang.exception.ExceptionUtils;
import org.jdom.Element;

import java.io.BufferedReader;
import java.io.InputStreamReader;

public class Start {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
        // if run.mode = test , start command line mode to test
        String mode = Configuration.getInstance().get("run.mode","run");
        if(mode.equalsIgnoreCase("test")){
            test();
        }   else{
            // TODO read the configuration, connect to autox server, read command and run command
            while (true){

                String commandInfo = Communication.readCommand();
                String resultInfo = ActionFactory.handle(commandInfo);
                Communication.setResult(resultInfo);
            }
        }


	}

    private static void test() {
        while (true){
            int choice = showMenu();
            switch (choice) {
                case 0:
                    return;
                default:
                    Write("\nPlease input a valid choice\n");
                    break;
            }
        }
    }

    private static int showMenu() {
        Write("\n\nSimple Tests for AutoX Client\n");
        Write("0. Exit");

        Write("\n");
        Write("Please input your choice:");
        try {

            return Integer.parseInt(ReadInput());
        } catch (Exception e) {

            Write(e.getMessage() + "\n" + ExceptionUtils.getStackTrace(e));
        }
        return -1;
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

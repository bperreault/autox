package autox;

import autox.actions.ActionFactory;
import autox.communication.Communication;
import autox.config.Configuration;
import org.jdom.Element;

public class Start {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
        // if run.mode = test , start command line mode to test
        String mode = Configuration.getInstance().get("run.mode","run");
        if(mode.equalsIgnoreCase("test")){

        }   else{
            // TODO read the configuration, connect to autox server, read command and run command
            while (true){

                String commandInfo = Communication.readCommand();
                String resultInfo = ActionFactory.handle(commandInfo);
                Communication.setResult(resultInfo);
            }
        }


	}

}

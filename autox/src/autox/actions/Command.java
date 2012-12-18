package autox.actions;

import autox.log.Log;
import org.apache.commons.lang.StringUtils;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 * Command string sample:
 * <Step Action="autox.actions.Command" Script="(batch/script file or script)" />
 */
public class Command extends Action {
    @Override
    public Object findTestObject() {
        return null;
    }

    @Override
    protected void handle(Object testObject) {
        String data = getOriginal().getAttributeValue("Script");
        if (StringUtils.isEmpty(data)) {
            result.Error("command is empty!");
        } else {
            callCommand(data);
        }
    }

    private void callCommand(String cmd) {
        Log.info("Cmd: " + cmd);
        Process p;
        try {
            p = Runtime.getRuntime().exec(cmd);
            BufferedReader stdInput = new BufferedReader(new
                    InputStreamReader(p.getInputStream()));

            BufferedReader stdError = new BufferedReader(new
                    InputStreamReader(p.getErrorStream()));

            String s;

            String stdInputString = "", stdErrorString = "";
            while ((s = stdInput.readLine()) != null) {
                stdInputString += s;
            }

            while ((s = stdError.readLine()) != null) {
                stdErrorString += s;
            }

            Log.info("Std Output: " + stdInputString);
            Log.info("Std Error: " + stdErrorString);
        } catch (IOException e) {
            Log.error(e.getMessage(), e);
            result.Error(e.getMessage());
        }
    }
}

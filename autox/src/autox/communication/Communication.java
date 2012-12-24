package autox.communication;

import autox.actions.Result;
import autox.config.Configuration;
import autox.log.Log;
import org.apache.commons.lang.StringUtils;
import org.tempuri.Service;

public class Communication {
    public static final String AUTOMATION_SERVICE_NAME = "CRM_Auto/";
    public static final String CENTER_SERVER_IP = Configuration.getInstance().get("center.service.ip", "localhost");


    public static String callWebService(String parameter, String methodName) {
        Service service = new Service();
        return service.getServiceSoap().command(readCommand());


        //return Result.failed("Call Web Service Failed, please check the parameters you sent:\n" + parameter);
    }

    public static String readCommand() {
        String idPart = "", instanceID;
        instanceID = Configuration.getInstance().get("instance.id", "");
        if (StringUtils.isNotEmpty(instanceID)) {
            idPart = " InstanceId=\"" + instanceID + "\" ";
        }
        String parameter = "<Command Name=\"RequestCommand\" " + idPart + " ComputerName=\""
                + Configuration.getInstance().get("computer_name", "noNameComputer") + "\" />";

        String ret = null;
        ret = callWebService(parameter, "Command");
        return ret;
    }

    public static void setResult(String resultInfo) {
        //TODO need to think carefully.
    }

    public static void register() {
        String parameter = "<Command Name='Register' >\n" +
                "        <Computer Name=\""
                + Configuration.getInstance().get("computer.name", "noNameComputer") + "\"  OS='Windows XP Professional' Browser='org.openqa.selenium.ie.InternetExplorerDriver' DB='SQL Server 2005' VMType='VI_SERVER' VM_Host='vmserver2-qa' UserName='root' Password='Welcome1' VM_File='...vmx' WindowsUser='vmserver2-qa\\qatest' WindowsPassword='Passw0rd' />\n" +
                "</Command>";//TODO update this line please.
        long wait_interval = 1000;
        while (true) {
            String ret = null;
            ret = callWebService(parameter, "Command");
            if (Result.fromString(ret).isSuccess())
                break;
            wait_interval += wait_interval;
            if (wait_interval > 60000)
                wait_interval = 1000;
            try {
                Thread.sleep(wait_interval);
            } catch (InterruptedException e) {
                Log.warn("We are unlucky, the thread.wait throw out exception!");
            }
        }

    }
}

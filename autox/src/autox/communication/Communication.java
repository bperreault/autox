package autox.communication;

import autox.actions.Result;
import autox.config.Configuration;
import autox.log.Log;
import org.apache.axis.client.Call;
import org.apache.axis.client.Service;
import org.apache.axis.encoding.XMLType;
import org.apache.commons.lang.StringUtils;

import javax.xml.namespace.QName;
import javax.xml.rpc.ParameterMode;
import javax.xml.rpc.ServiceException;
import java.net.MalformedURLException;
import java.rmi.RemoteException;


public class Communication {
    public static final String AUTOMATION_SERVICE_NAME = "CRM_Auto/";
    public static final String CENTER_SERVER_IP = Configuration.getInstance().get("center.service.ip", "localhost");
    public static final String PARAMETER_NAME = "xmlFormatCommand";
    public static final String NAMESPACE_AUTOMATION = "http://tempuri.org/Automation";
    private static final String NAME_SPACE_URI = "http://tempuri.org/";
    static String endpoint = "http://" + CENTER_SERVER_IP + "/"
            + AUTOMATION_SERVICE_NAME + "AutomationService.asmx";
    //public class Communication implements CSProcess {
    volatile boolean Stop = false;

    // private static Communication Instance = new Communication();
    //
    // private Communication() {
    //
    // }
    //
    // public static Communication GetInstance() {
    //
    // return Instance;
    // }

    //@Override
//    public void run() {
//        String instanceID = null;
//        while (!Stop) {
//            String idPart = " ";
//            if(StringUtils.isNotEmpty(instanceID)){
//                idPart = " InstanceId=\""+instanceID+"\" ";
//            }
//            String parameter = "<Command Name=\"RequestCommand\" "+idPart+ " ComputerName=\""
//                    + Configuration.getInstance().get("computer_name", "noNameComputer") + "\" />";
//            endpoint = "http://" + CENTER_SERVER_IP + "/"
//                    + AUTOMATION_SERVICE_NAME + "AutomationService.asmx";
//
//            try {
//                String ret = callWebService(parameter, "Command");
//
//                Element element = new XML(ret).getRoot();
//                String result_String = element.getAttributeValue("Result");
//                if (result_String.equalsIgnoreCase("Success")) {
//
//
//                    Element commandNode = element.getChild("Command");
//                    if (commandNode != null) {
//
//                        //setresult here, and Wait can ignore setresult
//                        if (commandNode.getAttributeValue("Action").indexOf("Wait") > 0 && null == commandNode.getAttributeValue("InstanceId")) {
//                            Thread.sleep(1000*15);
//                            continue;
//                        }
//                        Log.GetInstance().Info("We receive from server:\n"+ret);
//                        String result = ActionFactory.createAction(XML.toString(commandNode));
//
//                        instanceID = commandNode.getAttributeValue("InstanceId");
//
//                        Element setResultNode = new XML("<Command Name='SetResult' />").getRoot();
//                        if (!StringUtils.isEmpty(instanceID))
//                            setResultNode.setAttribute("InstanceId", instanceID);
//                        setResultNode.addContent(result);
//
//                        String message =  XML.toString(setResultNode);
//                        Log.GetInstance().Info("return message to server:\n"+message);
//                        String retOfSetResult = callWebService(XML.toString(setResultNode), "Command");
//
//                        Log.GetInstance().Info("The return of SetResult, for debug use:\n"+retOfSetResult);
//
//                    }
//                } else {
//                    Log.GetInstance().Error("We receive an wrong command:\n" + ret);
//                    String reason = element.getAttributeValue("Reason");
//                    if (!reason.isEmpty()) {
//                        if (reason.trim().contains("RE-Register"))
//                            return;
//                    }
//                }
//                System.gc();
//
//            } catch (Exception e) {
//                Log.GetInstance().Fatal(e.getMessage());
//                try {
//                    Thread.sleep(1000*15);
//                } catch (InterruptedException e1) {
//                    Log.GetInstance().Debug("sleep error, can be ignored.");
//                }
//            }
//
//        }
//
//    }

    public static String callWebService(String parameter, String methodName) {
        Service service = new Service();

        Call call = null;
        try {
            call = (Call) service.createCall();
            call.setTargetEndpointAddress(new java.net.URL(endpoint));
            call.setOperationName(new QName(NAME_SPACE_URI, "Command"));
            //IMPORTANT: use QName here, or it will fail.
            call.addParameter(new QName(NAME_SPACE_URI, PARAMETER_NAME), XMLType.XSD_STRING, ParameterMode.IN);
            call.setReturnType(XMLType.XSD_STRING);
            call.setUseSOAPAction(true);
            call.setSOAPActionURI("http://tempuri.org/Command");

            return (String) call.invoke(new Object[]{parameter.trim()});
        } catch (ServiceException e) {
            e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
        } catch (RemoteException e) {
            e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
        } catch (MalformedURLException e) {
            e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
        }
        return Result.failed("Call Web Service Failed, please check the parameters you sent:\n" + parameter);
    }

    public static String readCommand() {
        String idPart = "", instanceID;
        instanceID = Configuration.getInstance().get("instance.id", "");
        if (StringUtils.isNotEmpty(instanceID)) {
            idPart = " InstanceId=\"" + instanceID + "\" ";
        }
        String parameter = "<Command Name=\"RequestCommand\" " + idPart + " ComputerName=\""
                + Configuration.getInstance().get("computer_name", "noNameComputer") + "\" />";
        endpoint = "http://" + CENTER_SERVER_IP + "/"
                + AUTOMATION_SERVICE_NAME + "AutomationService.asmx";
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

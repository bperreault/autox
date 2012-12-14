package autox.communication;

import org.apache.axis.client.Call;
import org.apache.axis.client.Service;
import org.apache.axis.encoding.XMLType;
import org.apache.commons.lang.StringUtils;
import org.jdom.Element;

import autox.config.Configuration;


import javax.xml.namespace.QName;
import javax.xml.rpc.ParameterMode;
import javax.xml.rpc.ServiceException;
import java.net.MalformedURLException;
import java.rmi.RemoteException;


public class Communication {
    private static final String NAME_SPACE_URI = "http://tempuri.org/";

	//public class Communication implements CSProcess {
    volatile boolean Stop = false;

    public static final String AUTOMATION_SERVICE_NAME = "CRM_Auto/";
    public static final String CENTER_SERVER_IP = Configuration.getInstance().get("center_service_ip", "localhost");
    public static final String PARAMETER_NAME = "xmlFormatCommand";
    public static final String NAMESPACE_AUTOMATION = "http://tempuri.org/Automation";
    String endpoint = "http://" + CENTER_SERVER_IP + "/"
            + AUTOMATION_SERVICE_NAME + "AutomationService.asmx";

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

    public String callWebService(String parameter, String methodName)
            throws RemoteException, MalformedURLException, ServiceException {
        Service service = new Service();

        Call call = (Call) service.createCall();
        call.setTargetEndpointAddress(new java.net.URL(endpoint));
        call.setOperationName(new QName(NAME_SPACE_URI, "Command"));
        //IMPORTANT: use QName here, or it will fail.
        call.addParameter(new QName(NAME_SPACE_URI, PARAMETER_NAME), XMLType.XSD_STRING, ParameterMode.IN);
        call.setReturnType(XMLType.XSD_STRING);
        call.setUseSOAPAction(true);
        call.setSOAPActionURI("http://tempuri.org/Command");

        return (String) call.invoke(new Object[]{parameter.trim()});
    }
}

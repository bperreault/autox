<?php
require_once 'log4php/Logger.php';
require_once 'XML/Util.php';

Logger::configure('config.xml');
ini_set('soap.wsdl_cache_enabled', "0");
class WebTest extends PHPUnit_Extensions_Selenium2TestCase
{
    private $log;
    private $config;
    private $clientId;
    private $registered;

    public function testMainProcessFake()
    {
        //$xml = $this->fakeReadCommand('C:\Users\jien\Documents\autox\dotnet\AutoX.PHP.Client\Commands.xml');
        //$log = Logger::getLogger('autox.log');
        $xml = $this->fakeReadCommand('/Users/jien.huang/Documents/Commands.xml');
        //var_dump($xml);
        $mainId = strval($xml->attributes()->_id);
        $this->log->debug($mainId);
        $items = $xml->xpath("Step");
        foreach ($items as $item) {
            $this->doTest($item);
            //$this->log->debug($item);
        }


    }

    public function testMainProcess(){
         while(1){
 			//read command from center
 			$command = $this->requestCommandFromHost();
			if(empty($xml_command)){
                sleep(6);
                continue;
            }

             $instanceId = $this->getInstanceId($command);
             if(empty($instanceId)){
                 sleep(6);
                 continue;
             }
             //TODO prepare the return result here
             $result = new SimpleXMLElement("<Result />");
             $result->addAttribute("Created", date("Y-m-d H:i:s"));
 			//analasyze the xml, choose a action to run
             $items = $xml->xpath("Step");
             foreach ($items as $item) {
                 $this->log->debug($item);
                 $ret = $this->doTest($item);
                 $this->xml_appendChild($result,$ret);
             }
             $result->addAttribute("Created", date("Y-m-d H:i:s"));
             $this->sendResultToHost($result);
 		}
    }

    private function fakeReadCommand($xmlFile)
    {
        $xml_str = file_get_contents($xmlFile);
        $xml = new SimpleXMLElement($xml_str);
        return $xml;
    }

    private function doTest($cmd)
    {
        //get action name, if it is wait 17 sec, then wait, then return null
        $this->log->debug($cmd);

        $actionName = $this->getActionName($cmd);
        $data = $this->getData($cmd);
        $this->log->debug("Action:" . $actionName);

        if ($actionName == "Wait" && $data==null) {
            sleep(17);
            return;
        }
        $ret = "<StepResult Action='" + $actionName + "' Result='Error' Reason='Client does not support this action' />";
        switch ($actionName) {
            case "Check":
                //check a checkbox or radio
                break;
            case "Click":
                $this->click($cmd);
                break;
            case "Close":
                $this->close($cmd);
                break;
            case "Command":
                $this->command($data);
                break;
            case "Enter":
                $this->enter($cmd, $data);
                break;
            case "SetEnv":
                $this->setEnv($cmd);
                break;
            case "Start":
                $this->start($cmd);
                break;
            case "Wait":
                wait($cmd);
                break;
            case "GetValue":
                //
                break;
            case "Existed":
                //
                break;
            case "NotExisted":
                //
                break;
            case "VerifyValue":
                //
                break;
            case "VerifyTable":
                //
                break;
            default:
                //this is a command we don't support, do nothing here, the default ret is for this case.
                return $ret;

        }

        return $ret;
    }

    function guid(){
        if($this->clientId!=null)
            return $this->clientId;
//        if (function_exists('com_create_guid')){
//            return strval(com_create_guid());
//        }else{
            mt_srand((double)microtime()*10000);//optional for php 4.2.0 and up.
            $charid = strtoupper(md5(uniqid(rand(), true)));
            $hyphen = chr(45);// "-"
            $uuid = substr($charid, 0, 8).$hyphen
                .substr($charid, 8, 4).$hyphen
                .substr($charid,12, 4).$hyphen
                .substr($charid,16, 4).$hyphen
                .substr($charid,20,12);
            return strval($uuid);
//        }
    }
    //register
    private function getRegisterString(){
        $computerName = getHostName();
        $ipAddress = gethostbyname($computerName);
        $version = PHP_OS;
        $cmd = $this->initXCommand("Register","ComputerName",$computerName,"IPAddress",$ipAddress,"Version",$version,"_id",$this->guid());
        return $cmd->asXML();
    }

    private function getRequestCommandString(){
        $computerName = getHostName();
        $cmd = $this->initXCommand("RequestCommand","ComputerName",$computerName,"_id",$this->clientId);
        return $cmd->asXML();
    }

    private function getSetResultString($result){
        $cmd = $this->initXCommand("SetResult","ClientId",$this->clientId);
        $this->xml_appendChild($cmd,$result);
        return $cmd->asXML();
    }

    function xml_appendChild(SimpleXMLElement $to, SimpleXMLElement $from) {
        $toDom = dom_import_simplexml($to);
        $fromDom = dom_import_simplexml($from);
        $toDom->appendChild($toDom->ownerDocument->importNode($fromDom, true));
    }

    private function initXCommand(){
        $numargs = func_num_args();
        $action = func_get_arg(0);
        $cmd = new SimpleXMLElement("<Command />");
        $cmd->addAttribute("Action",$action);
        $arg_list = func_get_args();
        for($i=1;$i<$numargs;$i++){
            $key = $arg_list[$i];
            $i++;
            $value = $arg_list[$i];
            $cmd->addAttribute($key,$value);
        }

        return $cmd;
    }

    //read command from center, return an xml

    private function getActionName($cmd)
    {
        return strval($cmd["Action"]);
    }

    //run the command, return an xml format string

    private function getData($cmd)
    {
        return strval($cmd["Data"]);
    }

    private function getInstanceId($cmd){
        return strval($cmd["InstanceId"]);
    }

    private function click($xmlElement)
    {
        $xpath = $this->getUIObject($xmlElement);
        $this->byXPath($xpath)->click();
    }

    private function getUIObject($cmd)
    {
        $uiobject = $cmd->xpath("UIObject");
        $xpath = $uiobject[0]["XPath"];
        return strval($xpath);
    }

    //get action name

    private function close($xmlElement)
    {
        $this->closeBrowser();
    }

    private function command($cmd)
    {
        exec($cmd);
    }

    //send result back to center, xml format string

    private function enter($xmlElement, $data)
    {
        $xpath = $this->getUIObject($xmlElement);
        $this->byXPath($xpath)->click();
        $this->keys($data);
    }

    private function setEnv($xmlElement)
    {
        foreach ($xmlElement->attributes() as $key => $value) {
            $this->config[$key] = strval($value);
        }
        $this->setBrowser($this->config['BrowserType']);
        $this->setBrowserUrl($this->config['DefaultURL']);
        $this->prepareSession();
        $this->url($this->config['DefaultURL']);
    }

    //----------------actions-----------------

    private function start($xmlElement)
    {
        $this->setBrowser($this->config['BrowserType']);
        $this->setBrowserUrl($this->config['DefaultURL']);
        $this->prepareSession();
        $this->url($this->config['DefaultURL']);
    }

    protected function setUp()
    {
        //read config file
        //start selenium server???

        $this->log = Logger::getLogger('autox.log');
        $this->config = parse_ini_file("autox.ini");
        $this->setBrowser($this->config['BrowserType']);
        $this->setBrowserUrl($this->config['DefaultURL']);
        var_dump($this->getRegisterString());
    }

    protected function tearDown()
    {
        //$this->closeBrowser();
    }

    private function readCommand($cmd)
    {
        try {
            $soap = new SoapClient($this->config["EndPoint"]);
            $param["xmlFormatCommand"] = $cmd;
            $ret = $soap->__Call("Command", array($param));
            //           var_dump($ret);
            $xml_str = $ret->CommandResult;
            return new SimpleXMLElement($xml_str);
        } catch (Exception $e) {
            echo print_r($e->getMessage(), true);
            $this->log->error($e->getMessage());
        }
        return null;
    }

    private function registerToHost(){
        $cmd = $this->getRegisterString();

        while(1){
            try{
                $result = $this->readCommand($cmd);
                if($result!=null)
                    return;
            }catch(Exception $e){
                $this->log->warn("Register failed, due to: "+$e->getMessage());
                sleep(17);
            }
        }
    }

    private function requestCommandFromHost(){
        $cmd = $this->getRequestCommandString();
        try{
            $command = $this->readCommand($cmd);
            if($command!=null)
                return $command;
        }catch(Exception $e){
            $this->log->error("Request command failed, due to: "+$e->getMessage());
            sleep(6);
        }
        return null;
    }
    private function sendResultToHost($ret)
    {
        $cmd = $this->getSetResultString($ret);
        $this->readCommand($cmd);
    }

    private function snapshot()
    {
        //TODO read the related config, do screen shot when required; below function return base64 string
        return $this->screenshot();
    }

    private function existed($xmlElement)
    {
        $xpath = $this->getUIObject($xmlElement);
        //TODO how to set result????
    }

    private function wait($xmlElement)
    {
        $time = 17;
        try {
            $data = $xmlElement['Data'];
            $time = intval($data);
        } catch (Exception $e) {
        }
        sleep($time);
    }


    //------------actions---------------------
}

?>
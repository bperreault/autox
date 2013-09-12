<?php
require_once 'log4php/Logger.php';
require_once 'XML/Util.php';
require_once 'Communicate.php';

Logger::configure('config.xml');
ini_set('soap.wsdl_cache_enabled', "0");
date_default_timezone_set('UTC');
class WebTest extends PHPUnit_Extensions_Selenium2TestCase
{
    private $log;
    private $config;
    private $clientId;
    private $communicate;

    protected function setUp()
    {
        $this->log = Logger::getLogger('default');
        $this->log->debug("In setUp now...");
        //read config file

        $this->config = parse_ini_file("autox.ini");
        $this->log->debug("After parse ini...");
        $this->log->debug($this->config);
        $this->setBrowser($this->config['BrowserType']);
        $this->setBrowserUrl($this->config['DefaultURL']);

        //read uuid, if not existed, create one then save it to ini


        try{
            $this->clientId = $this->config['ClientId'];
            $this->log->debug("After get client id...");
        }catch(Exception $ex){
            $this->log->debug('First Time use this client. \n');
            $this->clientId = null;
        }
        $this->log->debug($this->clientId);
        if(empty($this->clientId)){
            $this->clientId = $this->getGuid();
            $this->append_ini_file("autox.ini","ClientId",$this->clientId);
        }
        $this->log->debug($this->clientId);
        $this->communicate = new Communicate();
    }

    private function append_ini_file($path,$key,$value){
        $content = "\n" . $key . "=" . $value;
        $this->log->debug("In append ini now...");
        if (!$handle = fopen($path, 'a')) {
            return;
        }
        $this->log->debug("After open ini...");
        if (!fwrite($handle, $content)) {
            return;
        }
        $this->log->debug("After write to ini...");
        fclose($handle);
    }

    public function testMainProcess(){

        while(true){
            if(!$this->communicate->isFake())
                $this->registerToHost();
//            $this->closeWindow();
//            exec("taskkill firefox");
//            sleep(10);
//            $this->setBrowser($this->config['BrowserType']);
//            $this->setBrowserUrl($this->config['DefaultURL']);
//            $session = $this->prepareSession();
//            //var_dump($session);
//            $this->log->debug($this->config['DefaultURL']);
//
//            $this->url($this->config['DefaultURL']);
            $this->requestReturnCycle();

            sleep(17);
        }
    }

    protected function tearDown()
    {
        //$this->closeWindow();
    }


    private function doTest($cmd)
    {
        //get action name, if it is wait 17 sec, then wait, then return null
        $this->log->debug($cmd);

        $actionName = $this->getActionName($cmd);
        $data = $this->getData($cmd);
        $uiObjectName = $this->getUIObjectName($cmd);
        $this->log->debug("Action:" . $actionName);

        $start = time();
        $stepResult = new SimpleXMLElement("<StepResult Action='" . $actionName . "' _id='" . $this->getGuid() ."' Data='" . $data ."' UIObject='" . $uiObjectName . "' StartTime='" . date("Y-m-d H:i:s", $start) . "' />");

        if ($actionName == "Wait" && $data==null) {
            sleep(17);
            $this->setSuccessResult($stepResult);
            $this->setDurationToResult($stepResult, $start);
            return $stepResult;
        }


        switch ($actionName) {
            case "Check":
                $this->check($cmd, $stepResult);
                break;
            case "Click":
                $this->clickOnUIObject($cmd, $stepResult);
                break;
            case "Close":
                $this->closeBrowser($cmd, $stepResult);
                break;
            case "Command":
                $this->command($data, $stepResult);
                break;
            case "Enter":
                $this->enter($cmd, $data, $stepResult);
                break;
            case "SetEnv":
                $this->setEnv($cmd, $stepResult);
                break;
            case "Start":
                $this->start($cmd, $stepResult);
                break;
            case "Wait":
                $this->wait($cmd, $stepResult);
                break;
            case "GetValue":
                $this->getValue($cmd, $stepResult);
                break;
            case "Existed":
                $this->existed($cmd,$stepResult);
                break;
            case "NotExisted":
                $this->notExisted($cmd,$stepResult);
                break;
            case "VerifyValue":
                $this->verifyValue($cmd,$stepResult);
                break;
            case "VerifyTable":
                $this->verifyTable($cmd,$stepResult);
                break;
            default:
                //this is a command we don't support, do nothing here, the default ret is for this case.
                $stepResult->addAttribute("Reason","Client does not support this action" . $actionName);
                break;

        }
        $this->setDurationToResult($stepResult, $start);
//        var_dump($stepResult);
        return $stepResult;
    }

    /**
     * @return string
     */
    private function getGuid()
    {
        mt_srand((double)microtime() * 10000); //optional for php 4.2.0 and up.
        $charid = strtoupper(md5(uniqid(rand(), true)));
        $hyphen = chr(45); // "-"
        $uuid = substr($charid, 0, 8) . $hyphen
            . substr($charid, 8, 4) . $hyphen
            . substr($charid, 12, 4) . $hyphen
            . substr($charid, 16, 4) . $hyphen
            . substr($charid, 20, 12);
        return strval($uuid);
    }

    private function getClientId(){
        if(empty($this->clientId))
            $this->clientId = $this->getGuid();
        return $this->clientId;
    }
    //register
    private function getRegisterString(){
        $computerName = getHostName();
        $ipAddress = gethostbyname($computerName);
        $version = PHP_OS;
        $cmd = $this->initXCommand("Register","ComputerName",$computerName,"IPAddress",$ipAddress,"Version",$version,"_id",$this->getClientId(),"DefaultURL",$this->config['DefaultURL']);
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

    private function xml_appendChild(SimpleXMLElement $to, SimpleXMLElement $from) {
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

    private function getUIObjectName($cmd){
        return strval($cmd["UIObject"]);
    }

    private function getInstanceId($cmd){
        return strval($cmd["InstanceId"]);
    }


    private function getUIObject($cmd)
    {
        $uiobject = $cmd->xpath("UIObject");
        $xpath = $uiobject[0]["XPath"];
        return strval($xpath);
    }

    private function setSuccessResult($stepResult){
        $stepResult->addAttribute("Result","Success");
    }

    private function setFailedResult($stepResult,$reason){
        $stepResult->addAttribute("Result","Error");
        if(!empty($reason)){
            $stepResult->addAttribute("Reason",$reason);
        }
    }

//----------------actions-----------------
    private function clickOnUIObject($xmlElement, $stepResult)
    {
        $uiObject = $this->findUIObject($xmlElement);
        if(empty($uiObject))
        {
            $this->setFailedResult($stepResult,"Cannot find expected UI Object");
            return;
        }
        $uiObject->click();
        $this->setSuccessResult($stepResult);
    }



    private function closeBrowser($xmlElement, $stepResult)
    {
        $this->setBrowserUrl("http://127.0.0.1:4444/wd/hub/static/resource/hub.html");
        $session = $this->prepareSession();
        $session->cookie()->clear();
        $this->url("http://127.0.0.1:4444/wd/hub/static/resource/hub.html");
        $this->setSuccessResult($stepResult);
    }

    private function command($cmd, $stepResult)
    {
        exec($cmd);
        $this->setSuccessResult($stepResult);
    }

    //send result back to center, xml format string

    private function enter($xmlElement, $data, $stepResult)
    {
        $uiObject = $this->findUIObject($xmlElement);
        if(empty($uiObject))
        {
            $this->setFailedResult($stepResult,"Cannot find expected UI Object");
            return;
        }
        $uiObject->click();
        $this->keys($data);
        $this->setSuccessResult($stepResult);
    }

    private function setEnv($xmlElement, $stepResult)
    {
        foreach ($xmlElement->attributes() as $key => $value) {
            $this->config[$key] = strval($value);
        }
        $this->setBrowser($this->config['BrowserType']);
        $this->setBrowserUrl($this->config['DefaultURL']);
        $this->prepareSession();
        $this->log->debug($this->config['DefaultURL']);

        $this->url($this->config['DefaultURL']);
        $this->setSuccessResult($stepResult);
    }

    private function start($xmlElement, $stepResult)
    {
        $this->setBrowser($this->config['BrowserType']);
        $this->setBrowserUrl($this->config['DefaultURL']);
        $this->prepareSession();
        $this->url($this->config['DefaultURL']);
        $this->setSuccessResult($stepResult);
    }

    private function existed($xmlElement, $stepResult)
    {
        $uiObject = $this->findUIObject($xmlElement);
        if(empty($uiObject))
        {
            $this->setFailedResult($stepResult,"Cannot find expected UI Object");
            return;
        }
        $this->setSuccessResult($stepResult);
    }

    private function notExisted($xmlElement, $stepResult)
    {
        $uiObject = $this->findUIObject($xmlElement);
        if(!empty($uiObject))
        {
            $this->setFailedResult($stepResult,"find unexpected UI Object");
            return;
        }
        $this->setSuccessResult($stepResult);
    }

    private function check($xmlElement, $stepResult)
    {
        $uiObject = $this->findUIObject($xmlElement);
        if(empty($uiObject))
        {
            $this->setFailedResult($stepResult,"find unexpected UI Object");
            return;
        }
        $data = $this->getData($xmlElement);
        if(empty($data)){
            $uiObject->click();
            $this->setSuccessResult($stepResult);
            return;
        }
        $toCheck = $data=="True"||$data=="true";
        $checkStatus = !empty($uiObject->attribute("checked"));
        if($toCheck && !$checkStatus || !toCheck && $checkStatus)
            $uiObject->click();
        $this->setSuccessResult($stepResult);
    }

    private function getValue($xmlElement, $stepResult)
    {
        $uiObject = $this->findUIObject($xmlElement);
        if(empty($uiObject))
        {
            $this->setFailedResult($stepResult,"find unexpected UI Object");
            return;
        }
        $data = $this->getData($xmlElement);
        if(empty($data)){
            $this->setFailedResult($stepResult,"No expected data?");
            return;
        }
        $items = explode("=>",$data);
        if(empty($items[1])){
            $this->setFailedResult($stepResult,"The correct format for this action is 'attribute=>variable', e.g.: value=>currentValue");
            return;
        }
        $elementValue = $uiObject->value($items[0]);
        $stepResult->addAttribute($items[1],$elementValue);
        $this->setSuccessResult($stepResult);
    }

    private function verifyValue($xmlElement, $stepResult)
    {
        $uiObject = $this->findUIObject($xmlElement);
        if(empty($uiObject))
        {
            $this->setFailedResult($stepResult,"find unexpected UI Object");
            return;
        }
        $data = $this->getData($xmlElement);
        if(empty($data)){
            $this->setFailedResult($stepResult,"No expected data?");
            return;
        }
        $items = explode("=>",$data);
        if(empty($items[1])){
            $tag = $uiObject->name();
            $type = $uiObject->attribute("type");
            if($tag=="select"){
                $target = $uiObject->byXPath("//option[@value='" . $data . "' and @selected]");
                if(empty($target)){
                    $this->setFailedResult($stepResult,"value not matched");
                    return;
                }

            }
            if($tag=="radio"||$tag=="checkbox"){
                $target = $uiObject->attribute("checked");
                if(empty($target)){
                    $this->setFailedResult($stepResult,"value not matched");
                    return;
                }
            }
            $target = $uiObject->attribute("value");
            if($target!=$data){
                $this->setFailedResult($stepResult,"value not matched");
                return;
            }
            $this->setSuccessResult($stepResult);
            return;
        }
        $elementValue = $uiObject->attribute($items[0]);
        if($elementValue==$items[1])
            $this->setSuccessResult($stepResult);
        else
            $this->setFailedResult($stepResult,"Expected[". $item[1] . "]<>Actual[" . $elementValue ."]");
    }

    private function verifyTable($xmlElement, $stepResult)
    {
        $this->log->debug("in verifyTable now ...");
        $uiObject = $this->findUIObject($xmlElement);
        if(empty($uiObject))
        {
            $this->setFailedResult($stepResult,"find unexpected UI Object");
            return;
        }
        $data = $this->getData($xmlElement);
        if(empty($data)){
            $this->setFailedResult($stepResult,"No expected data?");
            return;
        }
        $this->log->debug("Data->". $data);
        $items = explode("|",$data);
        $rows = $uiObject->elements($this->using('css selector')->value('tr'));
        foreach($rows as $row){
            if($this->verifyRow($row,$items))
            {
                var_dump($row);
                $this->log->debug("Found the row!");
                $this->setSuccessResult($stepResult);
                return;
            }
        }
        $this->setFailedResult($stepResult,"No expected row");
    }

    private function verifyRow($row,$items){
        foreach($items as $item){
            try{
                $this->log->debug("//*[contains(text(),'" . $item . "')]");
                $found = $row->byXPath("//*[contains(text(),'" . $item . "')]");

                if(empty($found))
                    return false;
            }catch(Exception $ex){
                $this->log->debug($ex->getMessage());
                return false;
            }

        }
        return true;
    }

    private function wait($xmlElement, $stepResult)
    {
        $time = 17;
        try {
            $data = $xmlElement['Data'];
            $time = intval($data);
        } catch (Exception $e) {
        }
        sleep($time);
        $this->setSuccessResult($stepResult);
    }
//------------actions---------------------

    private function snapshot($stepResult)
    {
        //TODO read the related config, do screen shot when required; below function return base64 string
        if(strval($stepResult["Result"])!="Success")
            $stepResult->addAttribute("Link", $this->screenshot());
    }



    private function registerToHost(){
        $cmd = $this->getRegisterString();

        while(1){
            try{
                $result = $this->communicate->readCommand($cmd);
                if($result!=null)
                    return;
            }catch(Exception $e){
                $this->log->warn("Register failed, due to: " . $e->getMessage());
                sleep(17);
            }
        }
    }

    private function requestCommandFromHost(){
        $cmd = $this->getRequestCommandString();
        try{
            $command = $this->communicate->readCommand($cmd);
            $this->log->debug("Recieve command:\n" . $command->asXML());
            if(!empty($command))
                return $command;
        }catch(Exception $e){
            $this->log->error("Request command failed, due to: " . $e->getMessage());
            sleep(6);
        }
        return null;
    }
    private function sendResultToHost($ret)
    {
        $cmd = $this->getSetResultString($ret);
        $this->communicate->readCommand($cmd);
    }


    private function requestReturnCycle()
    {
        $this->log->debug("In requestReturnCycle now ...");
        while (true) {
            //read command from center

            $command = $this->requestCommandFromHost();

            if (empty($command)) {
                sleep(6);
                continue;
            }

            $instanceId = $this->getInstanceId($command);
            if (empty($instanceId)) {
                sleep(6);
                continue;
            }
            //TODO prepare the return result here
            $result = new SimpleXMLElement("<Result />");
            $result->addAttribute("Created", date("Y-m-d H:i:s", time()));
            //$result->addAttribute("_id",$instanceId);
            $result->addAttribute("InstanceId",$instanceId);
            //analasyze the xml, choose a action to run
            $items = $command->xpath("Step");
            foreach ($items as $item) {
                //$this->log->debug($item);
                $ret = $this->doTest($item);
                $this->xml_appendChild($result, $ret);
            }
            $result->addAttribute("Updated", date("Y-m-d H:i:s", time()));
            $this->sendResultToHost($result);
            sleep(1);
        }
    }

    /**
     * @param $stepResult
     * @param $start
     */
    private function setDurationToResult($stepResult, $start)
    {
        $this->snapshot($stepResult);
        $end = time();
        $duration = $end - $start;

        $stepResult->addAttribute("EndTime", date("Y-m-d H:i:s", $end));
        $stepResult->addAttribute("Duration", date("H:i:s", $duration));
        $stepResult->addAttribute("_type", "Result");
    }

    /**
     * @param $xmlElement
     * @return mixed
     */
    private function findUIObject($xmlElement)
    {
        $xpath = $this->getUIObject($xmlElement);
        $uiObject = $this->byXPath($xpath);
        return $uiObject;
    }


}
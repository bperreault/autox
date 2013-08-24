<?php
require_once 'log4php/Logger.php';
require_once 'XML/Util.php';

Logger::configure('config.xml');
ini_set('soap.wsdl_cache_enabled', "0");
class WebTest extends PHPUnit_Extensions_Selenium2TestCase
{
	private $log;
	
    protected function setUp()
    {		
		$this->log = Logger::getLogger('autox.log');
		$this->setBrowser('firefox');
		$this->setBrowserUrl('http://demouat.com/');
	}
	
	protected function tearDown(){
		//$this->closeBrowser();
	}
 
    public function testMainProcess()
    {
		$cmd = $this->fakeReadCommand('C:\Users\jien\Documents\autox\dotnet\AutoX.PHP.Client\Commands.xml');
		//var_dump($cmd);
		$this->log->debug($cmd['_id']);
		$items = $cmd->xpath('*/Step');
		foreach($items as $item){
			doTest($item);
		}
		// while(1){
// 			//read command from center
// 			$command = readCommand();
//			$xml_command = convertCommand($command)
// 			if(empty($xml_command))
// 				continue;
// 			//analasyze the xml, choose a action to run
// 			$result = doTest($xml_command);
// 			if(empty($result))
// 				continue;
// 			//form a return result
// 			//send result back
// 			sendResult($result);
// 		}
		        
    }
	private function fakeReadCommand($xmlFile){
		$xml_str = file_get_contents($xmlFile);
		$xml = new SimpleXMLElement($xml_str);
		return $xml;
	}	
		
	//read command from center, return an xml
	private function readCommand($cmd){
		try{
			$soap = new SoapClient("http://localhost:8081/AutoX.Web/Service.asmx?wsdl");
			                $param["xmlFormatCommand"] = $cmd;
			                $ret = $soap->__Call("Command",array($param));
			//           var_dump($ret);
			$xml_str = $ret->CommandResult;                
			return new SimpleXMLElement($xml_str);				
		}catch(Exception $e){
			echo print_r($e->getMessage(),true);
		}		
		return null;
	}
	
	//run the command, return an xml format string
	private function doTest($cmd){
		//get action name, if it is wait 17 sec, then wait, then return null
		$actionName = $cmd['Action'];
		$this->log->debug($actionName);
		if($actionName=="Wait"){
			//wait
			return;
		}
		$ret = "<StepResult Action='" + $actionName + "' Result='Error' Reason='Client does not support this action' />";
		switch($actionName){
			case "Check":
			//check a checkbox or radio
				break;
			case "Click":
			//click something
				break;
			case "Close":
			//
				break;
			case "Command":
			//
				break;
			case "Enter":
			//
				break;
			case "SetEnv":
			//
				break;
			case "Start":
			//
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
			case "":
			//
				break;
			case "":
			//
				break;
			default:
			//this is a command we don't support, do nothing here, the default ret is for this case.
				
		}
		return $ret;
	}
	
	//get action name
	private function getActionName($cmd){
		
	}
	//send result back to center, xml format string
	private function sendResult($ret){
		
	}
	
	private function convertCommand($string_command){
		
	}
 
	private function wait($xmlelement){
		$time = 17;
		try{
			$data = $xmlelement['Data'];
			$time = intval($data);
		}catch(Exception $e){
		}
		sleep($time);		
	}
}
?>
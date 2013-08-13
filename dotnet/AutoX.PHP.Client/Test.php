<?php
require_once 'log4php/Logger.php';
require_once 'XML/Util.php';

Logger::configure('config.xml');
global $log;
ini_set('soap.wsdl_cache_enabled', "0");
class WebTest extends PHPUnit_Extensions_Selenium2TestCase
{
	
		
    protected function setUp()
    {
		//read config file 
		//start selenium server???
		//$log.configure('config.xml');
		//$log.getLogger('autox.log');
		$log = Logger::getLogger('autox.log');
		$this->setBrowser('firefox');
		$this->setBrowserUrl('http://demouat.com/');
		
    }
	
	protected function tearDown(){
		//$this->closeBrowser();
	}
 
    public function testMainProcess()
    {
		$log = Logger::getLogger('autox.log');
		$log->debug($this->readCommand());
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
	
		
	//read command from center, return an xml
	private function readCommand(){
		$soap = new SoapClient("http://localhost:8081/AutoX.Web/Service.asmx?wsdl");
 		$param["input"] = "Test String";
 		$ret = $soap->__Call("Hello",array($param));
		var_dump($ret);
 		echo $ret->HelloResult;
		
		return null;
	}
	
	//run the command, return an xml format string
	private function doTest($cmd){
		//get action name, if it is wait 17 sec, then wait, then return null
		$actionName = getActionName($cmd);
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
			//
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
 
}
?>
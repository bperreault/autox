<?php
require_once 'log4php/Logger.php';
require_once 'XML/Util.php';

Logger::configure('config.xml');
ini_set('soap.wsdl_cache_enabled', "0");
date_default_timezone_set('UTC');

class Communicate {

    private $log;
    private $config;
    private $fake;

    function __construct()
    {
        $this->log = Logger::getLogger('communication');
        $this->config = parse_ini_file("autox.ini");
        try{
            $this->fake = $this->config['Fake'];
        }catch(Exception $ex){
            //this means the fake file is not existed
            $this->fake = "";
        }
    }

    public function isFake(){
        return !empty($this->fake);
    }

    public function readCommand($cmd)
    {
        $this->log->debug($cmd);

        if($this->isFake())
            return $this->fakeReadCommand();

        try {
            $soap = new SoapClient($this->config["EndPoint"]);
            $param["xmlFormatCommand"] = $cmd;
            $ret = $soap->__Call("Command", array($param));
            $xml_str = $ret->CommandResult;
            $this->log->debug($xml_str);
            return new SimpleXMLElement($xml_str);
        } catch (Exception $e) {
            echo print_r($e->getMessage(), true);
            $this->log->error($e->getMessage());
        }
        return null;
    }

    private function fakeReadCommand()
    {
        $xml_str = file_get_contents($this->fake);
        $xml = new SimpleXMLElement($xml_str);
        return $xml;
    }
}
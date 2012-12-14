package autox.log;


import org.apache.commons.io.FileUtils;
import org.apache.log4j.BasicConfigurator;
import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

import java.io.File;


public class LoggerInstance {

	private static LoggerInstance instance = new LoggerInstance();
	private Logger log = Logger.getLogger("");
	private LoggerInstance(){
        File properties = new File("./log4j.properties");
        if(properties.exists()){
            PropertyConfigurator.configure("log4j.properties");
            System.out.println(properties.getAbsolutePath()+" is not existed, please check!");
        }
        else
            BasicConfigurator.configure();

	}
	
	public static LoggerInstance getInstance(){
		return instance;
	}
	
	public Logger get(){
		return log;
	}
}


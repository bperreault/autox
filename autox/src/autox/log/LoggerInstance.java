package autox.log;


import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;



public class LoggerInstance {

	private static LoggerInstance instance = new LoggerInstance();
	private Logger log = Logger.getLogger("");
	private LoggerInstance(){
		PropertyConfigurator.configure("log4j.properties");
	}
	
	public static LoggerInstance getInstance(){
		return instance;
	}
	
	public Logger get(){
		return log;
	}
}


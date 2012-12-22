package autox.log;


import autox.config.Configuration;
import org.apache.log4j.BasicConfigurator;
import org.apache.log4j.Level;
import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;


public class LoggerInstance {

	private static LoggerInstance instance = new LoggerInstance();
	private Logger log;
	private LoggerInstance(){
        File logPath = new File("./logs");
        if(!logPath.exists()){
            logPath.mkdir();
        }
        log = Logger.getLogger("");
        String env = System.getProperty("user.dir")+"/log4j.properties";
        File properties = new File(env);
        if(!properties.exists()||properties.length()<64){
            try {
                properties.createNewFile();
                FileWriter fileWriter = new FileWriter(properties);
                fileWriter.write("log4j.rootLogger=DEBUG,ATDailyFileOut,ATStandOut\n" +
                        "\n" +
                        "log4j.logger.Sage.Automation.Utils.Log = FileOut,ATStandOut\n" +
                        "\n" +
                        "log4j.appender.ATStandOut = org.apache.log4j.ConsoleAppender\n" +
                        "log4j.appender.ATStandOut.Target = System.out\n" +
                        "log4j.appender.ATStandOut.Threshold = DEBUG\n" +
                        "log4j.appender.ATStandOut.layout = org.apache.log4j.PatternLayout\n" +
                        "log4j.appender.ATStandOut.layout.ConversionPattern =  [%d{yyyy-MM-dd HH:mm:ss,SSS}]%L %m%n\n" +
                        "\n" +
                        "log4j.appender.ATDailyFileOut=org.apache.log4j.DailyRollingFileAppender\n" +
                        "log4j.appender.ATDailyFileOut.file=./logs/AT_Daily.Log\n" +
                        "log4j.appender.ATDailyFileOut.Append = true\n" +
                        "log4j.appender.ATDailyFileOut.Threshold = DEBUG\n" +
                        "log4j.appender.ATDailyFileOut.layout=org.apache.log4j.PatternLayout\n" +
                        "log4j.appender.ATDailyFileOut.layout.ConversionPattern=[%d{yyyy-MM-dd HH:mm:ss,SSS}]%L    %m%n");
                fileWriter.close();

            } catch (IOException e) {
                Log.warn("properties file copy failed!",e);
                BasicConfigurator.configure();
                return;
            }
        }
        PropertyConfigurator.configure(env);

	}
	
	public static LoggerInstance getInstance(){
		return instance;
	}
	
	public Logger get(){
        log.setLevel(Level.toLevel(Configuration.getInstance().get("log.level", "DEBUG")));
		return log;
	}
}


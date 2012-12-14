package autox.log;

public class Log {
	public static void debug(Object message){
		LoggerInstance.getInstance().get().debug(message);
	}
	public static void debug(Object message,Throwable t){
		LoggerInstance.getInstance().get().debug(message,t);
	}
	
	public static void info(Object message){
		LoggerInstance.getInstance().get().info(message);
	}
	public static void info(Object message,Throwable t){
		LoggerInstance.getInstance().get().info(message,t);
	}
	
	public static void warn(Object message){
		LoggerInstance.getInstance().get().warn(message);
	}
	public static void warn(Object message,Throwable t){
		LoggerInstance.getInstance().get().warn(message,t);
	}
	
	public static void error(Object message){
		LoggerInstance.getInstance().get().error(message);
	}
	public static void error(Object message,Throwable t){
		LoggerInstance.getInstance().get().error(message,t);
	}
	
	public static void fatal(Object message){
		LoggerInstance.getInstance().get().fatal(message);
	}
	public static void fatal(Object message,Throwable t){
		LoggerInstance.getInstance().get().fatal(message,t);
	}
}

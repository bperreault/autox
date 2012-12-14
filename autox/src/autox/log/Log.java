package autox.log;

public class Log {
	public static void debug(Object message){
		LoggerInstance.getInstance().get().debug(getPositionInformation(new Throwable())+message);
	}
	public static void debug(Object message,Throwable t){
		LoggerInstance.getInstance().get().debug(getPositionInformation(t)+message,t);
	}
	
	public static void info(Object message){
		LoggerInstance.getInstance().get().info(getPositionInformation(new Throwable())+message);
	}
	public static void info(Object message,Throwable t){
		LoggerInstance.getInstance().get().info(getPositionInformation(t)+message,t);
	}
	
	public static void warn(Object message){
		LoggerInstance.getInstance().get().warn(getPositionInformation(new Throwable())+message);
	}
	public static void warn(Object message,Throwable t){
		LoggerInstance.getInstance().get().warn(getPositionInformation(t)+message,t);
	}
	
	public static void error(Object message){
		LoggerInstance.getInstance().get().error(getPositionInformation(new Throwable())+message);
	}
	public static void error(Object message,Throwable t){
		LoggerInstance.getInstance().get().error(getPositionInformation(t)+message,t);
	}
	
	public static void fatal(Object message){
		LoggerInstance.getInstance().get().fatal(getPositionInformation(new Throwable())+message);
	}
	public static void fatal(Object message,Throwable t){
		LoggerInstance.getInstance().get().fatal(getPositionInformation(t)+message,t);
	}

    private static String getPositionInformation(Throwable t){
        return " at " + t.getStackTrace()[1].getClassName() +
                "." + t.getStackTrace()[1].getMethodName() +
                "("+t.getStackTrace()[1].getFileName()+":"+ t.getStackTrace()[1].getLineNumber()+")\n";
    }
}

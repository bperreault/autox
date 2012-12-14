package autox.config;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.Properties;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Configuration {
	private static Properties props = new Properties();
	private static Configuration Instance = new Configuration();

	private Configuration() {
		String env = ".\\config.properties";
		try {
			props.load(new FileInputStream(new File(env)));
			setHostProperty();
		} catch (FileNotFoundException e) {
			Logger.getAnonymousLogger().log(Level.SEVERE, e.getMessage());
		} catch (IOException e) {
			Logger.getAnonymousLogger().log(Level.SEVERE, e.getMessage());
		}

	}

	private void setHostProperty() {
		InetAddress address;
		try {
			address = InetAddress.getLocalHost();
			String hostName = address.getHostName();
			if (hostName != null && !hostName.isEmpty()) {
				props.setProperty("computer.name", hostName);
			}
		} catch (UnknownHostException e) {
			Logger.getAnonymousLogger().log(Level.SEVERE, e.getMessage());

		}

	}

	public static Configuration getInstance() {
		return Instance;
	}

	public String get(String key) {
		return props.getProperty(key);
	}

	public String get(String key, String defaultValue) {
		return props.getProperty(key, defaultValue);
	}

	public void setClientProperties() {
		props.setProperty("os.name", System.getProperty("os.name"));
		props.setProperty("os.version", System.getProperty("os.version"));

	}

}

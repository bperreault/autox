package autox.config;

import autox.log.Log;

import java.io.*;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.Date;
import java.util.Properties;

public class Configuration {


    private Configuration() {

        try {
            props.load(new FileInputStream(new File(env)));
            setHostProperty();
            setClientProperties();

        } catch (FileNotFoundException e) {
            Log.fatal(e.getMessage(), e);
        } catch (IOException e) {
            Log.fatal(e.getMessage(), e);
        }

    }

    public void set(String key, String value) {
        props.setProperty(key, value);
        try {
            props.store(new FileWriter(env), new Date().toString());
        } catch (IOException e) {
            Log.fatal(e.getMessage(), e);
        }
    }

    private void setHostProperty() {
        InetAddress address;
        try {
            address = InetAddress.getLocalHost();
            String hostName = address.getHostName();
            if (hostName != null && !hostName.isEmpty()) {
                set("computer.name", hostName);
            }
        } catch (UnknownHostException e) {
            Log.fatal(e.getMessage(), e);
        }

    }

    public static Configuration getInstance() {
        return Instance;
    }

//    public String get(String key) {
//        return props.getProperty(key);
//    }

    public String get(String key, String defaultValue) {
        return props.getProperty(key, defaultValue);
    }

    private void setClientProperties() {
        set("os.name", System.getProperty("os.name"));
        set("os.version", System.getProperty("os.version"));

    }

    private static Properties props = new Properties();
    private static Configuration Instance = new Configuration();
    private String env = System.getProperty("user.dir") + "/autox/config.properties";
}

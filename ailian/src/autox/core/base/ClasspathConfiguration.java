package autox.core.base;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

import org.slf4j.LoggerFactory;

import com.dhemery.configuring.Configuration;
import com.dhemery.configuring.ConfigurationException;
import com.dhemery.configuring.MapBackedConfiguration;
import com.dhemery.configuring.TrimmingConfiguration;

/**
 * A {@link Configuration} that loads its properties from resources.
 */
public class ClasspathConfiguration extends TrimmingConfiguration {

	
    /**
     * Create a {@code Configuration} by loading properties from the named resources.
     * This class loads resources in the order they are listed.
     * If a property appears in multiple resources,
     * the configuration retains the last value loaded.
     * <p>The resource names must be absolute names.
     * That is, they must start with <tt>/</tt>.</p>
     * @param resourceNames the names of the resources that define properties
     */
    public ClasspathConfiguration(String... resourceNames) {
        super(configurationFromResources(resourceNames));
    }

    private static Configuration configurationFromResources(String... resourceNames) {
        Properties properties = new Properties();
        mergePropertiesFromResources(properties, resourceNames);
        return new MapBackedConfiguration(properties);
    }

    private static void mergePropertiesFromResources(Properties properties, String... resourceNames) {
        for(String resourceName : resourceNames) {
            mergePropertiesFromResource(properties, resourceName);
        }
    }

    private static void mergePropertiesFromResource(Properties properties, String resourceName) {
        mergePropertiesFromStream(properties, streamForResource(resourceName));
    }

    private static void mergePropertiesFromStream(Properties properties, InputStream stream) {
        try {
            properties.load(stream);
            stream.close();
        } catch (IOException cause) {
            throw new ConfigurationException("IO Exception while reading properties from " + stream, cause);
        }
    }

    private static InputStream streamForResource(String name) {
        InputStream stream = ClasspathConfiguration.class.getResourceAsStream(name);
        if(stream == null) complain(name);
        return stream;
    }

	private static void complain(String name) {
		String explanation = new StringBuilder()
				.append("Please add property file ").append(name)
				.append(" to the classpath\n").append("Classpath was: ")
				.append(System.getProperty("java.class.path")).toString();
		LoggerFactory.getLogger(ClasspathConfiguration.class).warn(explanation);
	}
}

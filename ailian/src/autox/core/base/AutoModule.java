package autox.core.base;

import javax.inject.Singleton;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.dhemery.configuring.Configuration;
import com.dhemery.core.Lazily;
import com.dhemery.core.Lazy;
import com.dhemery.core.Supplied;
import com.dhemery.core.Supplier;
import com.google.inject.AbstractModule;
import com.google.inject.Provides;
import com.google.inject.name.Names;


public class AutoModule extends AbstractModule {

	protected final Logger log = LoggerFactory.getLogger(getClass());
	private static final String[] PROPERTY_RESOURCE_NAMES = { "/default.properties","/auto.properties.default", "/my.properties"  };
	private final Lazy<Configuration> configuration;
	
	public AutoModule(){
		configuration = Lazily.get(configurationSupplier());
	}
	
	public AutoModule(Configuration config){
		configuration = Lazily.get(Supplied.instance(config));
	}
	
	private static Supplier<Configuration> configurationSupplier() {
		return new Supplier<Configuration>() {
			@Override
			public Configuration get() {
				Configuration configuration = new ClasspathConfiguration(PROPERTY_RESOURCE_NAMES);
				return configuration;
			}
		};
	}
	
	@Provides @Singleton
	private Configuration configuration() {
		return configuration.get();
	}

	@Override
	protected void configure() {
		try{
			
			Names.bindProperties(binder(), configuration.get().asMap());
		}catch(Throwable cause){
			addError(cause);
		}		
	}
}


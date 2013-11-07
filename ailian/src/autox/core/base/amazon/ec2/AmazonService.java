package autox.core.base.amazon.ec2;

import java.util.ArrayList;
import java.util.List;

import org.apache.jcs.JCS;
import org.apache.jcs.access.exception.CacheException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import autox.core.base.ClasspathConfiguration;

import com.amazonaws.auth.AWSCredentials;
import com.amazonaws.auth.BasicAWSCredentials;
import com.amazonaws.services.ec2.AmazonEC2Client;
import com.amazonaws.services.ec2.model.CreateImageRequest;
import com.amazonaws.services.ec2.model.DeregisterImageRequest;
import com.amazonaws.services.ec2.model.DescribeImagesRequest;
import com.amazonaws.services.ec2.model.DescribeImagesResult;
import com.amazonaws.services.ec2.model.DescribeInstancesResult;
import com.amazonaws.services.ec2.model.Image;
import com.amazonaws.services.ec2.model.Instance;
import com.amazonaws.services.ec2.model.InstanceType;
import com.amazonaws.services.ec2.model.RebootInstancesRequest;
import com.amazonaws.services.ec2.model.Reservation;
import com.amazonaws.services.ec2.model.RunInstancesRequest;
import com.amazonaws.services.ec2.model.StartInstancesRequest;
import com.amazonaws.services.ec2.model.StopInstancesRequest;
import com.amazonaws.services.ec2.model.TerminateInstancesRequest;
import com.dhemery.configuring.Configuration;

public class AmazonService {

	protected static final Logger log = LoggerFactory.getLogger(AmazonService.class);
	private static AmazonEC2Client amazonClient;
	Configuration configuration = new ClasspathConfiguration();
	private static AmazonService instance = new AmazonService();
	private static JCS amazon;

	private static Runnable t;

	private AmazonService() {
		AWSCredentials credentials = new BasicAWSCredentials(
				configuration.requiredOption("amazon.access.key"),
				configuration.requiredOption("amazon.secret.key"));
		amazonClient = new AmazonEC2Client(credentials);
		amazonClient.setEndpoint(configuration
				.requiredOption("amazon.endpoint"));

	}

	private static void putCache() throws CacheException {
		if (amazon == null)
			amazon = JCS.getInstance("Instances");
		synchronized (AmazonService.class) {
			amazon.clear();
			DescribeInstancesResult result = amazonClient.describeInstances();
			for (Reservation r : result.getReservations()) {
				for (Instance i : r.getInstances()) {
					amazon.putInGroup(i.getInstanceId(), "Instances", i);
				}
			}

			DescribeImagesRequest ir = new DescribeImagesRequest()
					.withOwners("self");
			DescribeImagesResult images = amazonClient.describeImages(ir);
			for (Image r : images.getImages()) {
				amazon.putInGroup(r.getImageId(), "AMIs", r);
			}
			log.debug("put cache finished");
		}
	}

	public static AmazonService getInstance() {
		// start a thread, update the JCS every x minutes

		if (t == null) {
			t = new Runnable() {
				public void run() {
					while(! Thread.interrupted())
					try {
						putCache();
						Thread.sleep(1000 * 30);
					} catch (CacheException e) {
						e.printStackTrace();
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
			};
			new Thread(t).start();
		}
		return instance;
	}
	
	public void close(){
		Thread.currentThread().interrupt();
	}

	public void createECAccordingAMI(String ami) {
		amazonClient.runInstances(new RunInstancesRequest()
				.withImageId(ami)
				.withMinCount(1)
				.withMaxCount(1)
				.withInstanceType(
						InstanceType.fromValue(configuration
								.requiredOption("amazon.instance.type"))));
	}

	public String toString() {
		String ret = "\n";
		while (true) {
			if (amazon == null)
				try {
					amazon = JCS.getInstance("Instances");
				} catch (CacheException e) {

					e.printStackTrace();
					continue;
				}
			synchronized (AmazonService.class) {
				for (Object key : amazon.getGroupKeys("Instances")) {
					Instance i = (Instance) amazon.getFromGroup(key, "Instances");
					if(i==null)
						continue;
					ret += (i.getInstanceId() + " " + i.getImageId() + " "
							+ i.getImageId() + " " + i.getArchitecture() + " "
							+ i.getPrivateDnsName() + " " + i.getState()
							.getName()) + "\n";
				}
				for (Object key : amazon.getGroupKeys("AMIs")) {
					Image r = (Image) amazon.getFromGroup(key, "AMIs");;
					if(r==null)
						continue;
					ret += (r.getImageId() + " " + r.getDescription() + " " + r
							.getState()) + "\n";
				}
			}
			if (ret.length() > 10)
				break;
			else {
				try {
					Thread.sleep(1000 * 5);
				} catch (InterruptedException e) {

					e.printStackTrace();
				}
				continue;
			}
		}

		return ret;
	}

	public List<Instance> getInstances() {
		List<Instance> list = new ArrayList<Instance>();
		
		if (amazon == null)
			try {
				amazon = JCS.getInstance("Instances");
			} catch (CacheException e) {

				e.printStackTrace();
				return list;
			}
		synchronized (AmazonService.class) {
			for (Object key : amazon.getGroupKeys("Instances")) {
				Instance i = (Instance) amazon.getFromGroup(key, "Instances");
				if(i==null)
					continue;
				list.add(i);
			}
			
		}
		return list;
	}

	public List<Image> getImages() {
		List<Image> list = new ArrayList<Image>();
		if (amazon == null)
			try {
				amazon = JCS.getInstance("Instances");
			} catch (CacheException e) {

				e.printStackTrace();
				return list;
			}
		synchronized (AmazonService.class) {			
			for (Object key : amazon.getGroupKeys("AMIs")) {
				Image r = (Image) amazon.getFromGroup(key, "AMIs");;
				if(r==null)
					continue;
				list.add(r);
			}
		}

		return list;
	}

	public void startEC2(String... ec2id) {
		StartInstancesRequest startInstances = new StartInstancesRequest();
		amazonClient.startInstances(startInstances.withInstanceIds(ec2id));
	}

	public void stopEC2(String... ec2id) {
		StopInstancesRequest stopInstancesRequest = new StopInstancesRequest();
		amazonClient.stopInstances(stopInstancesRequest.withInstanceIds(ec2id));
	}

	public void saveEC2ToAMI(String name, String description, String ec2id) {
		CreateImageRequest createImageRequest = new CreateImageRequest();
		amazonClient.createImage(createImageRequest.withInstanceId(ec2id)
				.withDescription(description).withName(name));
	}

	public void deleteAMI(String ami) {
		DeregisterImageRequest request = new DeregisterImageRequest(ami);
		amazonClient.deregisterImage(request);
	}

	public void terminateEC2(String... ec2id) {
		TerminateInstancesRequest terminateInstancesRequest = new TerminateInstancesRequest();
		amazonClient.terminateInstances(terminateInstancesRequest
				.withInstanceIds(ec2id));
	}

	public void rebootEC2(String... ec2id) {
		amazonClient.rebootInstances(new RebootInstancesRequest()
				.withInstanceIds(ec2id));
	}

}

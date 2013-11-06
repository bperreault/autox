package autox.core.base.amazon.ec2;

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

	protected final Logger log = LoggerFactory.getLogger(getClass());
	private AmazonEC2Client amazonClient;
	Configuration configuration = new ClasspathConfiguration();
	private static AmazonService instance = new AmazonService();
	
	private AmazonService(){
		AWSCredentials credentials = new BasicAWSCredentials(configuration.requiredOption("amazon.access.key"),configuration.requiredOption("amazon.secret.key"));
		amazonClient = new AmazonEC2Client(credentials);
		amazonClient.setEndpoint(configuration.requiredOption("amazon.endpoint"));
	}
	
	public static AmazonService getInstance(){
		return instance;
	}
	
	public void createECAccordingAMI(String ami) {
		amazonClient.runInstances(new RunInstancesRequest().withImageId(ami).withMinCount(1).withMaxCount(1).withInstanceType(InstanceType.fromValue(configuration.requiredOption("amazon.instance.type"))));
	}

	public String toString(){
		String ret = "\n";
		DescribeInstancesResult result = amazonClient.describeInstances();
		for(Reservation r : result.getReservations()){
			for(Instance i : r.getInstances()){
				ret+=(i.getInstanceId()+" " + i.getImageId()+" "+i.getImageId()+" "+i.getArchitecture()+" "+i.getPrivateDnsName()+" "+i.getState().getName())+"\n";
			}
		}
		DescribeImagesRequest ir = new DescribeImagesRequest().withOwners("self");
		DescribeImagesResult images = amazonClient.describeImages(ir);
		for(Image r : images.getImages()){
			ret+=(r.getImageId()+" "+r.getDescription()+" "+r.getState())+"\n";
		}
		return ret;
	}
	
	public void startEC2(String... ec2id){		
		StartInstancesRequest startInstances = new StartInstancesRequest();
		amazonClient.startInstances(startInstances.withInstanceIds(ec2id));				
	}

	public void stopEC2(String ...ec2id) {
		StopInstancesRequest stopInstancesRequest = new StopInstancesRequest();
		amazonClient.stopInstances(stopInstancesRequest.withInstanceIds(ec2id));
	}

	public void saveEC2ToAMI(String name, String description, String ec2id) {
		CreateImageRequest createImageRequest = new CreateImageRequest();
		amazonClient.createImage(createImageRequest.withInstanceId(ec2id).withDescription(description).withName(name));
	}

	public void deleteAMI(String ami) {
		DeregisterImageRequest request = new DeregisterImageRequest(ami);
		amazonClient.deregisterImage(request);		
	}

	public void terminateEC2(String...ec2id) {
		TerminateInstancesRequest terminateInstancesRequest = new TerminateInstancesRequest();
		amazonClient.terminateInstances(terminateInstancesRequest.withInstanceIds(ec2id));		
	}
	
	public void rebootEC2(String ...ec2id){
		amazonClient.rebootInstances(new RebootInstancesRequest().withInstanceIds(ec2id));	
	}
	
}

package autox.core.base.amazon.ec2;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.amazonaws.services.ec2.model.Image;
import com.amazonaws.services.ec2.model.Instance;

public class Amazon2Jsp {
	
	public String getOneRow_Html(Object... args){
		String format = "<tr>\n";
		for(int i =0;i<args.length;i++){
			format+="<td>%s</td>\n";
		}
		format+="</tr>\n";
		return String.format(format, args);
	}
	
	private String getOneAMIRow(Image image){
		return getOneRow_Html(image.getImageId(),image.getDescription(),image.getName(),image.getState());
	}
	
	public String getAMIRows(){
		String ret = "";
		for(Image image :AmazonService.getInstance().getImages()){
			ret+= getOneRow_Html(image);
		}
		return ret;
	}
	
	/**
	 * @param args
	 */
//	public static void main(String[] args) {
//		
//		String ami = "ami-a3c123d4";
//		String ec2id = "i-0c899f40";
//		String amiName = "new1Name";
//		Logger log = LoggerFactory.getLogger("main");
//		//connect to amazon(handled by AmazonService
//		log.debug(AmazonService.getInstance().toString());
//		//create new EC with AMI
//		AmazonService.getInstance().createECAccordingAMI(ami);
//		sleep(60);
//		log.debug(AmazonService.getInstance().toString());
//		//stop the EC
//		AmazonService.getInstance().stopEC2(ec2id);
//		sleep(30);
//		log.debug(AmazonService.getInstance().toString());
//		//save EC to new AMI
//		AmazonService.getInstance().saveEC2ToAMI(amiName,"Description: new AMI",ec2id);
//		sleep(30);
//		log.debug(AmazonService.getInstance().toString());
//		//delete AMI
//		AmazonService.getInstance().deleteAMI(ami);
//		sleep(30);
//		log.debug(AmazonService.getInstance().toString());
//		//terminate EC
//		AmazonService.getInstance().terminateEC2(ec2id);
//		sleep(60);
//		log.debug(AmazonService.getInstance().toString());
//		AmazonService.getInstance().close();
//	}

	private static void sleep(int i) {
		try {
			Thread.sleep(1000*i);
		} catch (InterruptedException e) {
			
			e.printStackTrace();
		}
		
	}

}

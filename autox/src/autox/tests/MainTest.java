package autox.tests;

import java.security.KeyPairGenerator;
import java.security.KeyPair;
import java.security.PublicKey;
import java.security.PrivateKey;
import javax.crypto.Cipher;

import org.apache.axis.encoding.Base64;
import org.apache.james.mime4j.field.datetime.DateTime;
import org.python.modules.time;

import autox.log.Log;


public class MainTest {

	private static byte[] encrypt(byte[] inpBytes, PublicKey key,
		      String xform) throws Exception {
		    Cipher cipher = Cipher.getInstance(xform);
		    cipher.init(Cipher.ENCRYPT_MODE, key);
		    return cipher.doFinal(inpBytes);
		  }
		  private static byte[] decrypt(byte[] inpBytes, PrivateKey key,
		      String xform) throws Exception{
		    Cipher cipher = Cipher.getInstance(xform);
		    cipher.init(Cipher.DECRYPT_MODE, key);
		    return cipher.doFinal(inpBytes);
		  }

		  public static void main(String[] unused) throws Exception {
		    String xform = "RSA";
		    
		    // Generate a key-pair
		    Log.debug("Before generate key pair");
		    
		    KeyPairGenerator kpg = KeyPairGenerator.getInstance("RSA");
		    kpg.initialize(512); // 512 is the keysize.
		    KeyPair kp = kpg.generateKeyPair();
		    PublicKey pubk = kp.getPublic();
		    PrivateKey prvk = kp.getPrivate();

		    System.out.println("publick key:"+pubk.toString());
		    System.out.println("private key:"+prvk.toString());
		    
		    byte[] dataBytes =
		        "Jien Huang is a good father and husband.".getBytes();
Log.debug("before encrypt");
		    byte[] encBytes = encrypt(dataBytes, pubk, xform);
		    Log.debug("before decrypt");
		    byte[] decBytes = decrypt(encBytes, prvk, xform);

		    boolean expected = java.util.Arrays.equals(dataBytes, decBytes);
		    System.out.println(Base64.encode(encBytes));
		    System.out.println("Test " + (expected ? "SUCCEEDED!" : "FAILED!"));
		  }

}

package autox.tests;

import autox.log.Log;
import org.apache.axis.encoding.Base64;

import javax.crypto.Cipher;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.PrivateKey;
import java.security.PublicKey;


public class MainTest {

    private static byte[] encrypt(byte[] inpBytes, PublicKey key,
                                  String xForm) throws Exception {
        Cipher cipher = Cipher.getInstance(xForm);
        cipher.init(Cipher.ENCRYPT_MODE, key);
        return cipher.doFinal(inpBytes);
    }

    private static byte[] decrypt(byte[] inpBytes, PrivateKey key,
                                  String xForm) throws Exception {
        Cipher cipher = Cipher.getInstance(xForm);
        cipher.init(Cipher.DECRYPT_MODE, key);
        return cipher.doFinal(inpBytes);
    }

    public static void main(String[] unused) throws Exception {
        String xForm = "RSA";

        // Generate a key-pair
        Log.debug("Before generate key pair");

        KeyPairGenerator kpg = KeyPairGenerator.getInstance(xForm);
        kpg.initialize(512); // 512 is the key size.
        KeyPair kp = kpg.generateKeyPair();
        PublicKey publicKey = kp.getPublic();
        PrivateKey privateKey = kp.getPrivate();

        System.out.println("public key:" + publicKey.toString());
        System.out.println("private key:" + privateKey.toString());

        byte[] dataBytes =
                "Jien Huang is a good father and husband.".getBytes();
        Log.debug("before encrypt");
        byte[] encBytes = encrypt(dataBytes, publicKey, xForm);
        Log.debug("before decrypt");
        byte[] decBytes = decrypt(encBytes, privateKey, xForm);
        Log.debug("end of decrypt");
        boolean expected = java.util.Arrays.equals(dataBytes, decBytes);
        System.out.println(Base64.encode(encBytes));
        System.out.println("Test " + (expected ? "SUCCEEDED!" : "FAILED!"));
    }

}

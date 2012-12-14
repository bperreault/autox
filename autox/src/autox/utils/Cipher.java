package autox.utils;

import autox.config.Configuration;
import autox.log.Log;
import org.apache.axis.encoding.Base64;
import sun.misc.BASE64Decoder;

import java.security.*;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.PKCS8EncodedKeySpec;
import java.security.spec.X509EncodedKeySpec;

/**
 *
 * User: jien.huang
 * Date: 12/14/12
 * Time: 2:38 PM
 *
 */
public class Cipher {
    public static final String ALGORITHM = "RSA";

    public static byte[] encrypt(byte[] inpBytes, PublicKey key,
                                  String xForm) throws Exception {
        javax.crypto.Cipher cipher = javax.crypto.Cipher.getInstance(xForm);
        cipher.init(javax.crypto.Cipher.ENCRYPT_MODE, key);
        return cipher.doFinal(inpBytes);
    }

    public static byte[] encrypt(byte[] inpBytes, PrivateKey key,
                                 String xForm) throws Exception {
        javax.crypto.Cipher cipher = javax.crypto.Cipher.getInstance(xForm);
        cipher.init(javax.crypto.Cipher.ENCRYPT_MODE, key);
        return cipher.doFinal(inpBytes);
    }

    public static byte[] decrypt(byte[] inpBytes, PrivateKey key,
                                  String xForm) throws Exception {
        javax.crypto.Cipher cipher = javax.crypto.Cipher.getInstance(xForm);
        cipher.init(javax.crypto.Cipher.DECRYPT_MODE, key);
        return cipher.doFinal(inpBytes);
    }

    public static byte[] decrypt(byte[] inpBytes, PublicKey key,
                                 String xForm) throws Exception {
        javax.crypto.Cipher cipher = javax.crypto.Cipher.getInstance(xForm);

        cipher.init(javax.crypto.Cipher.DECRYPT_MODE, key);
        return cipher.doFinal(inpBytes);
    }

    public static String getPublicKeyString(PublicKey key){
        X509EncodedKeySpec x509EncodedKeySpec = new X509EncodedKeySpec(key.getEncoded());
        return Base64.encode(x509EncodedKeySpec.getEncoded());

    }

    public static String getPrivateKeyString(PrivateKey key){
        PKCS8EncodedKeySpec pkcs8EncodedKeySpec = new PKCS8EncodedKeySpec(key.getEncoded());
        return Base64.encode(pkcs8EncodedKeySpec.getEncoded());
    }

    public static PublicKey getPublicKeyFromString(String key)  {
        try {
            KeyFactory keyFactory = KeyFactory.getInstance(ALGORITHM);
            X509EncodedKeySpec x509EncodedKeySpec = new X509EncodedKeySpec(Base64.decode(key));
            return keyFactory.generatePublic(x509EncodedKeySpec);
        } catch (NoSuchAlgorithmException e) {
            return null;
        } catch (InvalidKeySpecException e) {
            return null;
        }

    }

    public static PrivateKey getPrivateKeyFromString(String key){
        try {
            KeyFactory keyFactory = KeyFactory.getInstance(ALGORITHM);
            PKCS8EncodedKeySpec pkcs8EncodedKeySpec = new PKCS8EncodedKeySpec(Base64.decode(key));
            return keyFactory.generatePrivate(pkcs8EncodedKeySpec);
        } catch (NoSuchAlgorithmException e) {
            return null;
        } catch (InvalidKeySpecException e) {
            return null;
        }
    }

    public static String encrypt(String data,String publicKey) throws Exception {
        return Base64.encode(encrypt(data.getBytes(), getPublicKeyFromString(publicKey), ALGORITHM));
    }

    public static String encrypt(String data) throws Exception {
        return encrypt(data, Configuration.getInstance().get("key.public",""));
    }

    public static String decrypt(String data,String publicKey) throws Exception {
        return Base64.encode(decrypt(Base64.decode(data),getPublicKeyFromString(publicKey),ALGORITHM));
    }

    public static String decrypt(String data) throws Exception {
        return decrypt(data,Configuration.getInstance().get("key.public",""));
    }

    public static String getFromBASE64(String s) {
        if (s == null) return null;
        BASE64Decoder decoder = new BASE64Decoder();
        try {
            byte[] b = decoder.decodeBuffer(s);
            return new String(b);
        } catch (Exception e) {
            return null;
        }

    }

    public static class Keys {
        private String publicKeyString;
        private String privateKeyString;

        public String getPublicKeyString() {
            return publicKeyString;
        }

        public String getPrivateKeyString() {
            return privateKeyString;
        }

        public Keys generateKeyPair() throws NoSuchAlgorithmException {
            KeyPairGenerator kpg = KeyPairGenerator.getInstance(ALGORITHM);
            kpg.initialize(512); // 512 is the key size.
            KeyPair kp = kpg.generateKeyPair();
            PublicKey publicKey = kp.getPublic();
            PrivateKey privateKey = kp.getPrivate();

            publicKeyString = Cipher.getPublicKeyString(publicKey);
            Log.info("public key string:" + publicKeyString);
            privateKeyString = Cipher.getPrivateKeyString(privateKey);
            Log.info("private key string:" + privateKeyString);
            return this;
        }
    }
}

using System;
using Socket.Newtonsoft.Json;
using Socket.Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class StringUtils
{
	public static Dictionary<string, string> getPayload() { return new Dictionary<string, string>(); } 

    public static float convertToFloat(string getString) { return float.Parse(getString); }
    public static int convertToInt(string getString) { return int.Parse(getString); }
    public static string convertFloatToString(float getString) { return Convert.ToString(getString); }
    public static string convertIntToString(int getString) { return Convert.ToString(getString); }

    public static string convertPayloadToJson(Dictionary<string, string> getPayload) { return JsonConvert.SerializeObject(getPayload); }

    public static Dictionary<string, string> parsePayload(string getString) { return JsonConvert.DeserializeObject<Dictionary<string, string>>(getString); }

    public static Vector3 getVectorFromJson(Dictionary<string, string> getPayload, string getString)    
    {
        return new Vector3(
            float.Parse(getPayload["x"+getString]), 
            float.Parse(getPayload["y"+getString]), 
            float.Parse(getPayload["z"+getString])
            );
    }

    public static Quaternion getQuaternionFromJson(Dictionary<string, string> getPayload, string getString)
    {
        return Quaternion.Euler(
            float.Parse(getPayload["x" + getString]),
            float.Parse(getPayload["y" + getString]),
            float.Parse(getPayload["z" + getString])
            );
    }


    public static Dictionary<string, string> getPositionAndRotation(Vector3 getPosition, Vector3 getRotation)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["xPos"] = Convert.ToString(getPosition.x);
        payload["yPos"] = Convert.ToString(getPosition.y);
        payload["zPos"] = Convert.ToString(getPosition.z);
        payload["xRot"] = Convert.ToString(getRotation.x);
        payload["yRot"] = Convert.ToString(getRotation.y);
        payload["zRot"] = Convert.ToString(getRotation.z);
        return payload;
    }

    public static Dictionary<string, string> getPositionAndRotation(Vector3 getPosition, Quaternion getRotation)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["xPos"] = Convert.ToString(getPosition.x);
        payload["yPos"] = Convert.ToString(getPosition.y);
        payload["zPos"] = Convert.ToString(getPosition.z);
        payload["xRot"] = Convert.ToString(getRotation.x);
        payload["yRot"] = Convert.ToString(getRotation.y);
        payload["zRot"] = Convert.ToString(getRotation.z);

        return payload;
    }

    public static string printDictionary(Dictionary<string, string> getDict)
    {
        string getString = "";
        foreach (KeyValuePair<string, string> getpay in getDict)
        {
            getString += getpay.Key + " = " + getpay.Value + "\n";
        }
        return getString;
    }

    public static string printDictionary(Dictionary<string, int> getDict)
    {
        string getString = "";
        foreach (KeyValuePair<string, int> getpay in getDict)
        {
            getString += getpay.Key + " = " + getpay.Value + "\n";
        }
        return getString;
    }

    public static string randomStringGen(int length)
    {
        string str = "";
        string context = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        for (int i = 0; i < length; i++)
        {

            str += context[UnityEngine.Random.Range(0, context.Length)];
        }

        return str;
    }


    public static class SimpleAESEncryption
    {

        private static string IV = "jGzmNyX04bZJfZJhP23yCQ==";
        /// <summary>
        /// A class containing AES-encrypted text, plus the IV value required to decrypt it (with the correct password)
        /// </summary>
        public struct AESEncryptedText
        {
            public string IV;
            public string EncryptedText;
        }

        /// <summary>
        /// Encrypts a given text string with a password
        /// </summary>
        /// <param name="plainText">The text to encrypt</param>
        /// <param name="password">The password which will be required to decrypt it</param>
        /// <returns>An AESEncryptedText object containing the encrypted string and the IV value required to decrypt it.</returns>
        public static AESEncryptedText Encrypt(string plainText, string password)
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.Key = ConvertToKeyBytes(aes, password);

                var textBytes = Encoding.UTF8.GetBytes(plainText);

                var aesEncryptor = aes.CreateEncryptor();
                var encryptedBytes = aesEncryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);


                return new AESEncryptedText
                {

                    IV = Convert.ToBase64String(aes.IV),
                    EncryptedText = Convert.ToBase64String(encryptedBytes)
                };
            }
        }

        //This is poor practice to use the same IV key but oh well for simplicit sake
        public static AESEncryptedText Encrypt(string plainText, string in_IV, string password)
        {
            using (var aes = Aes.Create())
            {
                aes.IV = Convert.FromBase64String(IV);
                aes.Key = ConvertToKeyBytes(aes, password);

                var textBytes = Encoding.UTF8.GetBytes(plainText);

                var aesEncryptor = aes.CreateEncryptor();
                var encryptedBytes = aesEncryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);


                return new AESEncryptedText
                {

                    IV = Convert.ToBase64String(aes.IV),
                    EncryptedText = Convert.ToBase64String(encryptedBytes)
                };
            }
        }
        /// <summary>
        /// Decrypts an AESEncryptedText with a password
        /// </summary>
        /// <param name="encryptedText">The AESEncryptedText object to decrypt</param>
        /// <param name="password">The password to use when decrypting</param>
        /// <returns>The original plainText string.</returns>
        public static string Decrypt(AESEncryptedText encryptedText, string password)
        {
            return Decrypt(encryptedText.EncryptedText, encryptedText.IV, password);
        }

        /// <summary>
        /// Decrypts an encrypted string with an IV value password
        /// </summary>
        /// <param name="encryptedText">The encrypted string to be decrypted</param>
        /// <param name="iv">The IV value which was generated when the text was encrypted</param>
        /// <param name="password">The password to use when decrypting</param>
        /// <returns>The original plainText string.</returns>
        public static string Decrypt(string encryptedText, string iv, string password)
        {
            using (Aes aes = Aes.Create())
            {
                var ivBytes = Convert.FromBase64String(iv);
                var encryptedTextBytes = Convert.FromBase64String(encryptedText);

                var decryptor = aes.CreateDecryptor(ConvertToKeyBytes(aes, password), ivBytes);
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedTextBytes, 0, encryptedTextBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        // Ensure the AES key byte-array is the right size - AES will reject it otherwise
        private static byte[] ConvertToKeyBytes(SymmetricAlgorithm algorithm, string password)
        {
            algorithm.GenerateKey();

            var keyBytes = Encoding.UTF8.GetBytes(password);
            var validKeySize = algorithm.Key.Length;

            if (keyBytes.Length != validKeySize)
            {
                var newKeyBytes = new byte[validKeySize];
                Array.Copy(keyBytes, newKeyBytes, Math.Min(keyBytes.Length, newKeyBytes.Length));
                keyBytes = newKeyBytes;
            }

            return keyBytes;
        }
    }

}
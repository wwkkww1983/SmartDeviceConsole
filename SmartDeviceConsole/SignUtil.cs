using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceConsole
{
    class SignUtil
    {
        public static string sign(Dictionary<string, string> signParams, string deviceSecret)
        {
            string[] sortedKeys = signParams.Keys.ToArray<string>();
            Array.Sort(sortedKeys);

            StringBuilder canonicalizedQueryString = new StringBuilder();
            foreach(string key in sortedKeys){
                if ("sign".Equals(key))
                {
                    continue;
                }

                string value = "";
                signParams.TryGetValue(key, out value);

                canonicalizedQueryString.Append(key).Append(value);
            }

            return hmacsha1Encrypt(canonicalizedQueryString.ToString(), deviceSecret);
        }

        public static string hmacsha1Encrypt(string EncryptText, string EncryptKey)
        {
            string message;
            string key;
            message = EncryptText;
            key = EncryptKey;
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);
            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);

            return ByteToString(hashmessage);

        }

        public static string ByteToString(byte[] bytes)

        {
            string hexString = string.Empty;

            if (bytes != null)

            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)

                {

                    strB.Append(bytes[i].ToString("X2"));

                }

                hexString = strB.ToString();

            }
            return hexString;

        }
    }
}
